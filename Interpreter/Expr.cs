using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCrafters.Interpreter;

public abstract class Expr {
  public interface IVisitor<out T> {
    T VisitBinaryExpr(Binary expr);
    T VisitGroupingExpr(Grouping expr);
    T VisitLiteralExpr(Literal expr);
    T VisitUnaryExpr(Unary expr);
  }
  public class Binary(Expr Left, Token Operator, Expr Right) : Expr
  {
    public Expr Left { get; set; } = Left;
    public Token Operator { get; set; } = Operator;
    public Expr Right { get; set; } = Right;
    public override T Accept<T>(IVisitor<T> visitor) {
      return visitor.VisitBinaryExpr(this);
    }
  }

  public class Grouping(Expr Expression) : Expr
  {
    public Expr Expression { get; set; } = Expression;
    public override T Accept<T>(IVisitor<T> visitor) {
      return visitor.VisitGroupingExpr(this);
    }
  }

  public class Literal(object? Value) : Expr
  {
    public object? Value { get; set; } = Value;
    public override T Accept<T>(IVisitor<T> visitor) {
      return visitor.VisitLiteralExpr(this);
    }
  }

  public class Unary(Token Operator, Expr Right) : Expr
  {
    public Token Operator { get; set; } = Operator;
    public Expr Right { get; set; } = Right;
    public override T Accept<T>(IVisitor<T> visitor) {
      return visitor.VisitUnaryExpr(this);
    }
  }

  public abstract T Accept<T>(IVisitor<T> visitor);
}
