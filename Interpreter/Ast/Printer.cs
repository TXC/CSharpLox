using System.Globalization;
using System.Linq;
using System.Text;

namespace CodeCrafters.Interpreter.Ast;

internal class Printer : Expr.IVisitor<string>, Stmt.IVisitor
{
    private readonly StringBuilder _sb = new();
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

    public string VisitCallExpr(Expr.Call expr)
        => Parenthesize(expr.Paren.Lexeme, [..expr.Arguments]);

    public void VisitBlockStmt(Stmt.Block stmt)
    {
        var res = Parenthesize("if", null, [..stmt.Statements]);
        _sb.Append(res);
    }

    public void VisitExpressionStmt(Stmt.Expression stmt)
    {
        var res = stmt.Value.Accept(this);
        _sb.Append(res);
    }

    public void VisitFunctionStmt(Stmt.Function stmt)
    {
        var res = Parenthesize("<fn " + stmt.Name.Lexeme + ">", null, [.. stmt.Body]);
        _sb.Append(res);
    }

    public void VisitIfStmt(Stmt.If stmt)
    {
        var res = Parenthesize("if", stmt.Condition, stmt.ThenBranch, stmt.ElseBranch);
        _sb.Append(res);
    }

    public string VisitGroupingExpr(Expr.Grouping expr)
        => Parenthesize("group", expr.Expression);

    public string VisitLogicalExpr(Expr.Logical expr)
        => Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);

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
        var res = Parenthesize("print", stmt.Value);
        //var res = stmt.Value.Accept(this);
        _sb.Append(res);
    }

    public void VisitReturnStmt(Stmt.Return stmt)
    {
        throw new NotImplementedException();
    }

    public string VisitUnaryExpr(Expr.Unary expr)
        => Parenthesize(expr.Operator.Lexeme, expr.Right);

    public string VisitVariableExpr(Expr.Variable expr)
        => Parenthesize(expr.Name.Lexeme, expr);

    public void VisitVarStmt(Stmt.Var stmt)
    {
        var res = Parenthesize(stmt.Name.Lexeme, stmt.Initializer);
        _sb.Append(res);
    }

    public void VisitWhileStmt(Stmt.While stmt)
    {
        var res = Parenthesize("if", stmt.Condition, stmt.Body);
        _sb.Append(res);
    }

    private string Parenthesize(string name, params Expr?[] expressions)
    {
        var builder = new StringBuilder();

        builder.Append('(');
        builder.Append(name);
        foreach (var expr in expressions)
        {
            builder.Append(' ');
            var ex = expr?.Accept(this);
            builder.Append(ex);
        }
        builder.Append(')');

        return builder.ToString();
    }

    private string Parenthesize(string name, Expr? expression = null, params Stmt?[] statements)
    {
        var builder = new StringBuilder();

        builder.Append('(');
        builder.Append(name);
        builder.Append(' ');
        if (expression is not null)
        {
            //builder.Append('(');
            var expr = expression.Accept(this);
            builder.Append(expr);
            //builder.Append(')');
        }

        foreach (var stmt in statements)
        {
            builder.Append('{');
            stmt?.Accept(this);
            builder.Append('}');
        }
        builder.Append(')');

        return builder.ToString();
    }
}


