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
    void VisitFunctionStmt(Function stmt);
    void VisitIfStmt(If stmt);
    void VisitPrintStmt(Print stmt);
    void VisitReturnStmt(Return stmt);
    void VisitVarStmt(Var stmt);
    void VisitWhileStmt(While stmt);
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

  public class Function(Token name, List<Token> parameters, List<Stmt> body) : Stmt
  {
    public Token Name { get; set; } = name;
    public List<Token> Parameters { get; set; } = parameters;
    public List<Stmt> Body { get; set; } = body;
    public override void Accept(IVisitor visitor) => visitor.VisitFunctionStmt(this);
  }

  public class If(Expr condition, Stmt thenbranch, Stmt? elsebranch) : Stmt
  {
    public Expr Condition { get; set; } = condition;
    public Stmt ThenBranch { get; set; } = thenbranch;
    public Stmt? ElseBranch { get; set; } = elsebranch;
    public override void Accept(IVisitor visitor) => visitor.VisitIfStmt(this);
  }

  public class Print(Expr value) : Stmt
  {
    public Expr Value { get; set; } = value;
    public override void Accept(IVisitor visitor) => visitor.VisitPrintStmt(this);
  }

  public class Return(Token keyword, Expr? value) : Stmt
  {
    public Token Keyword { get; set; } = keyword;
    public Expr? Value { get; set; } = value;
    public override void Accept(IVisitor visitor) => visitor.VisitReturnStmt(this);
  }

  public class Var(Token name, Expr? initializer) : Stmt
  {
    public Token Name { get; set; } = name;
    public Expr? Initializer { get; set; } = initializer;
    public override void Accept(IVisitor visitor) => visitor.VisitVarStmt(this);
  }

  public class While(Expr condition, Stmt body) : Stmt
  {
    public Expr Condition { get; set; } = condition;
    public Stmt Body { get; set; } = body;
    public override void Accept(IVisitor visitor) => visitor.VisitWhileStmt(this);
  }

  public abstract void Accept(IVisitor visitor);
}
