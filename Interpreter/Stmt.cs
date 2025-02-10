using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// ReSharper disable IdentifierTypo

namespace CodeCrafters.Interpreter;

internal abstract class Stmt {
  public interface IVisitor {
    void VisitBlockStmt(Block stmt);
    void VisitExpressionStmt(Expression stmt);
    void VisitPrintStmt(Print stmt);
    void VisitVarStmt(Var stmt);
  }
  public class Block(List<Stmt> statements) : Stmt
  {
    public List<Stmt> Statements { get; set; } = statements;
    public override void Accept(IVisitor visitor) => visitor.VisitBlockStmt(this);
  }

  public class Expression(Expr value) : Stmt
  {
    public Expr Value { get; set; } = value;
    public override void Accept(IVisitor visitor) => visitor.VisitExpressionStmt(this);
  }

  public class Print(Expr value) : Stmt
  {
    public Expr Value { get; set; } = value;
    public override void Accept(IVisitor visitor) => visitor.VisitPrintStmt(this);
  }

  public class Var(Token name, Expr initializer) : Stmt
  {
    public Token Name { get; set; } = name;
    public Expr Initializer { get; set; } = initializer;
    public override void Accept(IVisitor visitor) => visitor.VisitVarStmt(this);
  }

  public abstract void Accept(IVisitor visitor);
}
