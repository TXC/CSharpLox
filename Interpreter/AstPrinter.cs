using System.Globalization;
using System.Text;
using static CodeCrafters.Interpreter.Expr;

namespace CodeCrafters.Interpreter;

internal class AstPrinter : Expr.IVisitor<string>
{
    public string Print(Expr expr) => expr.Accept(this);

    public string VisitBinaryExpr(Expr.Binary expr)
        => Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);

    public string VisitGroupingExpr(Expr.Grouping expr)
        => Parenthesize("group", expr.Expression);

    public string VisitLiteralExpr(Expr.Literal expr)
    {
        return expr.Value switch
        {
            null => "nil",
            bool => $"{expr.Value}".ToLower(),
            double d => string.Format(CultureInfo.InvariantCulture, "{0:0.0##}", d),
            _ => $"{expr.Value}"
        };
    }

    public string VisitUnaryExpr(Expr.Unary expr)
        => Parenthesize(expr.Operator.Lexeme, expr.Right);

    private string Parenthesize(string name, params Expr[] expressions)
    {
        var builder = new StringBuilder();

        builder.Append('(');
        builder.Append(name);
        foreach (var expr in expressions)
        {
            builder.Append(' ');
            var ex = expr.Accept(this);
            builder.Append(ex);
        }
        builder.Append(')');

        return builder.ToString();
    }
}

