using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// ReSharper disable IdentifierTypo

namespace CodeCrafters.Interpreter;

internal abstract class Expr {
  public interface IVisitor<out T> {
    T VisitAssignExpr(Assign expr);
    T VisitBinaryExpr(Binary expr);
    T VisitGroupingExpr(Grouping expr);
    T VisitLiteralExpr(Literal expr);
    T VisitLogicalExpr(Logical expr);
    T VisitUnaryExpr(Unary expr);
    T VisitVariableExpr(Variable expr);
  }
  public class Assign(Token name, Expr value) : Expr
  {
    public Token Name { get; set; } = name;
    public Expr Value { get; set; } = value;
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitAssignExpr(this);
  }

  public class Binary(Expr left, Token @operator, Expr right) : Expr
  {
    public Expr Left { get; set; } = left;
    public Token Operator { get; set; } = @operator;
    public Expr Right { get; set; } = right;
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitBinaryExpr(this);
  }

  public class Grouping(Expr expression) : Expr
  {
    public Expr Expression { get; set; } = expression;
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitGroupingExpr(this);
  }

  public class Literal(object? value) : Expr
  {
    public object? Value { get; set; } = value;
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitLiteralExpr(this);
  }

  public class Logical(Expr left, Token @operator, Expr right) : Expr
  {
    public Expr Left { get; set; } = left;
    public Token Operator { get; set; } = @operator;
    public Expr Right { get; set; } = right;
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitLogicalExpr(this);
  }

  public class Unary(Token @operator, Expr right) : Expr
  {
    public Token Operator { get; set; } = @operator;
    public Expr Right { get; set; } = right;
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitUnaryExpr(this);
  }

  public class Variable(Token name) : Expr
  {
    public Token Name { get; set; } = name;
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitVariableExpr(this);
  }

  public abstract T Accept<T>(IVisitor<T> visitor);
}
