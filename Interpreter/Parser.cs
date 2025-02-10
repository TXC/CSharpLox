using CodeCrafters.Interpreter.Exceptions;
// ReSharper disable IdentifierTypo

namespace CodeCrafters.Interpreter;

internal class Parser(List<Token> tokens)
{
    private int _current = 0;

    public List<Stmt> Parse()
    {
        try
        {
            var statements = new List<Stmt>();
            while (!IsAtEnd())
            {
                var stmt = Declaration();
                if (stmt is null)
                {
                    Lox.Report(_current, "", "");
                    continue;
                }
                statements.Add(stmt);
            }

            return statements;
        }
        catch (RuntimeException)
        {
            throw;
            //return null;
        }
    }

    private Expr Expression() => Assignment();

    private Stmt? Declaration()
    {
        try
        {
            if (Match(TokenType.VAR))
            {
                return VarDeclaration();
            }

            return Statement();
        }
        catch (ParseError error)
        {
            Synchronize();
            return null;
        }
    }

    private Stmt Statement()
    {
        if (Match(TokenType.PRINT))
        {
            return PrintStatement();
        }

        if (Match(TokenType.LEFT_BRACE))
        {
            return new Stmt.Block(Block());
        }

        return ExpressionStatement();
    }

    private Stmt PrintStatement()
    {
        var value = Expression();
        Consume(TokenType.SEMICOLON, "Expect ';' after value.");
        return new Stmt.Print(value);
    }

    private Stmt? VarDeclaration()
    {
        Token name = Consume(TokenType.IDENTIFIER, "Expect variable name.");

        Expr initializer = null;
        if (Match(TokenType.EQUAL))
        {
            initializer = Expression();
        }

        Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
        return new Stmt.Var(name, initializer);
    }

    private Stmt ExpressionStatement()
    {
        Expr value = Expression();
        Consume(TokenType.SEMICOLON, "Expect ';' after value.");
        return new Stmt.Expression(value);
    }

    private List<Stmt> Block()
    {
        var statements = new List<Stmt>();

        while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd())
        {
            var block = Declaration();
            if (block is null)
            {
                throw new ParseError(tokens[_current]);
            }
            statements.Add(block);
        }

        Consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
        return statements;
    }

    private Expr Assignment()
    {
        var expr = Equality();

        if (Match(TokenType.EQUAL))
        {
            var equals = Previous();
            var value = Assignment();

            if (expr is Expr.Variable e) {
                return new Expr.Assign(e.Name, value);
            }

            Lox.Error(equals, "Invalid assignment target.");
        }

        return expr;
    }

    private Expr Equality()
    {
        var expr = Comparison();

        while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
        {
            var _operator = Previous();
            var right = Comparison();
            expr = new Expr.Binary(expr, _operator, right);
        }

        return expr;
    }

    private Expr Comparison()
    {
        var expr = Term();

        while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
        {
            var _operator = Previous();
            var right = Term();
            expr = new Expr.Binary(expr, _operator, right);
        }

        return expr;
    }

    private Expr Term()
    {
        var expr = Factor();

        while (Match(TokenType.MINUS, TokenType.PLUS))
        {
            var _operator = Previous();
            var right = Factor();
            expr = new Expr.Binary(expr, _operator, right);
        }

        return expr;
    }

    private Expr Factor()
    {
        var expr = Unary();

        while (Match(TokenType.SLASH, TokenType.STAR))
        {
            var _operator = Previous();
            var right = Unary();
            expr = new Expr.Binary(expr, _operator, right);
        }

        return expr;
    }

    private Expr Unary()
    {
        if (Match(TokenType.BANG, TokenType.MINUS))
        {
            var _operator = Previous();
            var right = Unary();
            return new Expr.Unary(_operator, right);
        }

        return Primary();
    }

    private Expr Primary()
    {
        if (Match(TokenType.FALSE))
        {
            return new Expr.Literal(false);
        }

        if (Match(TokenType.TRUE))
        {
            return new Expr.Literal(true);
        }

        if (Match(TokenType.NIL))
        {
            return new Expr.Literal(null);
        }

        if (Match(TokenType.NUMBER, TokenType.STRING))
        {
            return new Expr.Literal(Previous().Literal);
        }

        if (Match(TokenType.IDENTIFIER))
        {
            return new Expr.Variable(Previous());
        }

        if (Match(TokenType.LEFT_PAREN))
        {
            var expr = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
            return new Expr.Grouping(expr);
        }

        throw Error(Peek(), "Expect expression.");
    }

    private bool Match(params TokenType[] types)
    {
        foreach (var type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }

        return false;
    }

    private Token Consume(TokenType type, string message)
    {
        if (Check(type))
        {
            return Advance();
        }

        throw Error(Peek(), message);
    }

    private bool Check(TokenType type)
    {
        if (IsAtEnd())
        {
            return false;
        }
        return Peek().Type == type;
    }

    private Token Advance()
    {
        if (!IsAtEnd())
        {
            _current++;
        }
        return Previous();
    }

    private bool IsAtEnd() => Peek().Type == TokenType.EOF;

    private Token Peek() => tokens[_current];

    private Token Previous() => tokens[_current - 1];

    private static ParseError Error(Token token, string message)
    {
        Lox.Error(token, message);
        return new ParseError(token, message);
    }

    // ReSharper disable once UnusedMember.Local
    private void Synchronize()
    {
        Advance();

        while (!IsAtEnd())
        {
            if (Previous().Type == TokenType.SEMICOLON)
            {
                return;
            }

            switch (Peek().Type)
            {
                case TokenType.CLASS:
                case TokenType.FUN:
                case TokenType.VAR:
                case TokenType.FOR:
                case TokenType.IF:
                case TokenType.WHILE:
                case TokenType.PRINT:
                case TokenType.RETURN:
                    return;
            }

            Advance();
        }
    }
}
