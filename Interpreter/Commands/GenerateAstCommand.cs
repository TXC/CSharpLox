using CodeCrafters.Interpreter.Attributes;
using CodeCrafters.Interpreter.Ast;
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

namespace CodeCrafters.Interpreter.Commands;

[Command("generate_ast")]
public class GenerateAstCommand : BaseCommand, ICommand
{
    public void Execute(params string[] args)
    {
        var workingDirectory = System.Environment.CurrentDirectory;
        var projectDirectory = Directory.GetParent(workingDirectory)?.Parent?.Parent?.FullName ?? throw new NullReferenceException("Unable to locate project directory");

        string outputDir;
        if (args.Length == 2)
        {
            outputDir = args[1];
            //Console.WriteLine("Usage: generate_ast <output directory>");
            //Environment.Exit(64);
        }
        else
        {
            outputDir = projectDirectory;
        }

        Console.WriteLine($"Writing files to: {outputDir}");
        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        Generator.Define(outputDir, "Expr", "T", [
            "Assign   : Token Name, Expr Value",
            "Binary   : Expr Left, Token Operator, Expr Right",
            "Call     : Expr Callee, Token Paren, List<Expr?> Arguments",
            "Grouping : Expr Expression",
            "Literal  : object? Value",
            "Logical  : Expr Left, Token Operator, Expr Right",
            "Unary    : Token Operator, Expr Right",
            "Variable : Token Name",
        ]);

        Generator.Define(outputDir, "Stmt", "void", [
            "Block      : List<Stmt> Statements",
            "Expression : Expr Value",
            "Function   : Token Name, List<Token> Parameters," +
                        " List<Stmt> Body",
            "If         : Expr Condition, Stmt ThenBranch," +
                        " Stmt? ElseBranch",
            "Print      : Expr Value",
            "Return     : Token Keyword, Expr? Value",
            "Var        : Token Name, Expr? Initializer",
            "While      : Expr Condition, Stmt Body"
        ]);
    }

}
