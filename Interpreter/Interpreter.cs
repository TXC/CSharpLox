using System.Globalization;
using System.Text;
using CodeCrafters.Interpreter.Exceptions;
// ReSharper disable IdentifierTypo

namespace CodeCrafters.Interpreter;

internal class Interpreter : Expr.IVisitor<object?>, Stmt.IVisitor
{
    private readonly StringBuilder _sb = new StringBuilder();
    private Environment _environment = new Environment();

    public string Run(List<Stmt> statements)
    {
        try
        {
            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        catch (RuntimeException error)
        {
            Lox.RuntimeError(error);
        }
        return _sb.ToString();
    }

    public string Run(Expr expression)
    {
        try
        {
            var value = Evaluate(expression);
            return Stringify(value);
        }
        catch (RuntimeException error)
        {
            Lox.RuntimeError(error);
        }

        return "";
    }

    public void Interpret(Expr expression)
        => Console.WriteLine(Run(expression));

    public void Interpret(List<Stmt> statements)
        => Console.WriteLine(Run(statements));

    public object? VisitBinaryExpr(Expr.Binary expr)
    {
        var left = Evaluate(expr.Left);
        var right = Evaluate(expr.Right);

        switch (expr.Operator.Type) {
            case TokenType.GREATER:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left > (double)right;
            case TokenType.GREATER_EQUAL:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left >= (double)right;
            case TokenType.LESS:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left < (double)right;
            case TokenType.LESS_EQUAL:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left <= (double)right;
            case TokenType.MINUS:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left - (double)right;
            case TokenType.PLUS:
                if (left is double a && right is double b) {
                    return a + b;
                }

                if (left is string c && right is string d) {
                    return c + d;
                }
                throw new RuntimeException(expr.Operator, "Operands must be two numbers or two strings.");

            case TokenType.SLASH:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left / (double)right;
            case TokenType.STAR:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left * (double)right;
            case TokenType.BANG_EQUAL:
                return !IsEqual(left, right);
            case TokenType.EQUAL_EQUAL:
                return IsEqual(left, right);
        }

        // Unreachable.
        return null;
    }

    public object? VisitGroupingExpr(Expr.Grouping expr)
        => Evaluate(expr.Expression);

    public object? VisitLiteralExpr(Expr.Literal expr)
        => expr?.Value;

    public object? VisitUnaryExpr(Expr.Unary expr)
    {
        var right = Evaluate(expr.Right);

        switch (expr.Operator.Type) {
            case TokenType.BANG:
                return !IsTruthy(right);
            case TokenType.MINUS:
                CheckNumberOperand(expr.Operator, right);
                return -(double)right;
        }

        // Unreachable.
        return null;
    }

    public object? VisitVariableExpr(Expr.Variable expr)
        => _environment.Get(expr.Name);

    private object? Evaluate(Expr expr)
        => expr.Accept(this);

    private void Execute(Stmt stmt)
        => stmt.Accept(this);

    public void ExecuteBlock(List<Stmt> statements, Environment environment)
    {
        var previous = _environment;
        try
        {
            _environment = environment;

            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        finally
        {
            _environment = previous;
        }
    }

    public void VisitBlockStmt(Stmt.Block stmt)
        => ExecuteBlock(stmt.Statements, new Environment(_environment));

    public void VisitExpressionStmt(Stmt.Expression stmt)
        => Evaluate(stmt.Value);

    public void VisitPrintStmt(Stmt.Print stmt)
    {
        var value = Evaluate(stmt.Value);
        _sb.Append(Stringify(value) + "\n");
    }

    public void VisitVarStmt(Stmt.Var stmt)
    {
        object value = null;
        if (stmt.Initializer is null)
        {
            value = Evaluate(stmt.Initializer);
        }

        _environment.Define(stmt.Name.Lexeme, value);
    }

    public object VisitAssignExpr(Expr.Assign expr)
    {
        var value = Evaluate(expr.Value);
        _environment.Assign(expr.Name, value);
        return value;
    }

    private static bool IsTruthy(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (obj is bool b)
        {
            return b;
        }
        return true;
    }

    private static bool IsEqual(object? a, object? b)
    {
        if (a is null && b is null)
        {
            return true;
        }

        if (a is null)
        {
            return false;
        }

        return a.Equals(b);
    }

    private static void CheckNumberOperand(Token token, object operand)
    {
        if (operand is double)
        {
            return;
        }
        throw new RuntimeException(token, "Operand must be a number.");
    }

    private static void CheckNumberOperands(Token token, object left, object right)
    {
        if (left is double && right is double)
        {
            return;
        }
        throw new RuntimeException(token, "Operands must be a numbers.");
    }

    private static string Stringify(object? obj)
    {
        if (obj is null)
        {
            return "nil";
        }

        if (obj is bool)
        {
            return $"{obj}".ToLower();
        }

        if (obj is double a) {
            var text = a.ToString(CultureInfo.InvariantCulture);
            if (text.EndsWith(".0"))
            {
                text = text[..^2];
            }
            return text;
        }

        return obj.ToString() ?? "";
    }
}
