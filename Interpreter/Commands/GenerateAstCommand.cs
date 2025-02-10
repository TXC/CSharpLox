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
            "Grouping : Expr Expression",
            "Literal  : object? Value",
            "Unary    : Token Operator, Expr Right",
            "Variable : Token Name",
        ]);

        Generator.Define(outputDir, "Stmt", "void", [
            "Block      : List<Stmt> Statements",
            "Expression : Expr Value",
            "Print      : Expr Value",
            "Var        : Token Name, Expr Initializer",
        ]);
    }

}
