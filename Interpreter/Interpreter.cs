using System;
using System.Globalization;
using System.Text;
using CodeCrafters.Interpreter.Exceptions;
using CodeCrafters.Interpreter.Interfaces;
using CodeCrafters.Interpreter.Native;
// ReSharper disable IdentifierTypo

namespace CodeCrafters.Interpreter;

internal class Interpreter : Expr.IVisitor<object?>, Stmt.IVisitor
{
    private readonly StringBuilder _sb = new();
    private Environment _environment;
    private readonly Environment _globals = new();

    public Interpreter()
    {
        _environment = _globals;
        _globals.Define("clock", Clock.Impl);
    }

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

        if (left is null || right is null)
        {
            throw new RuntimeException(expr.Operator, "Operands must not be nil.");
        }

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
                return left switch
                {
                    double a when right is double b => a + b,
                    string c when right is string d => c + d,
                    _ => throw new RuntimeException(expr.Operator, "Operands must be two numbers or two strings.")
                };

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

    public object? VisitCallExpr(Expr.Call expr)
    {
        var callee = Evaluate(expr.Callee);

        var arguments = new List<object?>();
        foreach (var argument in expr.Arguments)
        {
            arguments.Add(Evaluate(argument));
        }

        if (callee is not ILoxCallable function) {
            throw new RuntimeException(expr.Paren, "Can only call functions and classes.");
        }
        if (arguments.Count != function.Arity)
        {
            throw new RuntimeException(expr.Paren, "Expected " + function.Arity + " arguments but got " + arguments.Count + ".");
        }
        return function.Call(this, arguments);
    }

    public object? VisitGroupingExpr(Expr.Grouping expr)
        => Evaluate(expr.Expression);

    public object? VisitLogicalExpr(Expr.Logical expr)
    {
        var left = Evaluate(expr.Left);

        if (expr.Operator.Type == TokenType.OR) {
            if (IsTruthy(left))
            {
                return left;
            }
        }
        else
        {
            if (!IsTruthy(left))
            {
                return left;
            }
        }

        return Evaluate(expr.Right);
    }

    public object? VisitLiteralExpr(Expr.Literal expr)
        => expr?.Value;

    public object? VisitUnaryExpr(Expr.Unary expr)
    {
        var right = Evaluate(expr.Right);

        switch (expr.Operator.Type) {
            case TokenType.BANG:
                return !IsTruthy(right);
            case TokenType.MINUS:
                if (right is null)
                {
                    return null;
                }
                CheckNumberOperand(expr.Operator, right);
                return -(double)right;
        }

        // Unreachable.
        return null;
    }

    public object? VisitVariableExpr(Expr.Variable expr)
        => _environment.Get(expr.Name);

    private object? Evaluate(Expr? expr)
        => expr?.Accept(this);

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

    public void VisitFunctionStmt(Stmt.Function stmt)
    {
        var function = new LoxFunction(stmt, _environment);
        _environment.Define(stmt.Name.Lexeme, function);
    }

    public void VisitIfStmt(Stmt.If stmt)
    {
        if (IsTruthy(Evaluate(stmt.Condition)))
        {
            Execute(stmt.ThenBranch);
        }
        else if (stmt.ElseBranch != null)
        {
            Execute(stmt.ElseBranch);
        }
    }

    public void VisitPrintStmt(Stmt.Print stmt)
    {
        var value = Evaluate(stmt.Value);
        _sb.Append(Stringify(value) + "\n");
    }

    public void VisitReturnStmt(Stmt.Return stmt)
    {
        object? value = null;
        if (stmt.Value != null) value = Evaluate(stmt.Value);

        throw new Return(value);
    }

    public void VisitVarStmt(Stmt.Var stmt)
    {
        object? value = null;
        if (stmt.Initializer is not null)
        {
            value = Evaluate(stmt.Initializer);
        }

        _environment.Define(stmt.Name.Lexeme, value);
    }

    public void VisitWhileStmt(Stmt.While stmt)
    {
        while (IsTruthy(Evaluate(stmt.Condition)))
        {
            Execute(stmt.Body);
        }
    }

    public object? VisitAssignExpr(Expr.Assign expr)
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
