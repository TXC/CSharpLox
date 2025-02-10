using System.Globalization;
using CodeCrafters.Interpreter.Exceptions;

namespace CodeCrafters.Interpreter;

public class Interpreter : Expr.IVisitor<object?>
{
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
    {
        Console.WriteLine(Run(expression));
    }

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

    private object Evaluate(Expr expr)
        => expr.Accept(this);

    private bool IsTruthy(object? obj)
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

    private bool IsEqual(object? a, object? b)
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

    private void CheckNumberOperand(Token token, object operand)
    {
        if (operand is double)
        {
            return;
        }
        throw new RuntimeException(token, "Operand must be a number.");
    }

    private void CheckNumberOperands(Token token, object left, object right)
    {
        if (left is double && right is double)
        {
            return;
        }
        throw new RuntimeException(token, "Operands must be a numbers.");
    }

    private string Stringify(object? obj)
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

        return obj.ToString();
    }
}
