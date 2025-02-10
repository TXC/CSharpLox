using System.Globalization;
using System.Linq;
using System.Text;

namespace CodeCrafters.Interpreter.Ast;

internal class Printer : Expr.IVisitor<string>, Stmt.IVisitor
{
    private readonly StringBuilder _sb = new StringBuilder();
    public string Print(List<Stmt> statements)
    {
        foreach (var stmt in statements)
        {
            stmt.Accept(this);
        }

        return _sb.ToString();
    }

    public string Print(Expr expr) => expr.Accept(this);

    public string VisitAssignExpr(Expr.Assign expr) => expr.Accept(this);

    public string VisitBinaryExpr(Expr.Binary expr)
        => Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);

    public void VisitBlockStmt(Stmt.Block stmt)
    {
        foreach (var st in stmt.Statements)
        {
            st.Accept(this);
        }
    }

    public void VisitExpressionStmt(Stmt.Expression stmt)
    {
        var res = stmt.Value.Accept(this);
        _sb.Append(res);
    }
    

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

    public void VisitPrintStmt(Stmt.Print stmt)
    {
        var res = stmt.Value.Accept(this);
        _sb.Append(res);
    }

    public string VisitUnaryExpr(Expr.Unary expr)
        => Parenthesize(expr.Operator.Lexeme, expr.Right);

    public string VisitVariableExpr(Expr.Variable expr)
        => Parenthesize(expr.Name.Lexeme, expr);

    public void VisitVarStmt(Stmt.Var stmt)
    {
        var res = stmt.Initializer.Accept(this);
        _sb.Append(res);
    }

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


