using System.IO;
using CodeCrafters.Interpreter.Attributes;

namespace CodeCrafters.Interpreter.Commands;

[Command("generate_ast")]
public class GenerateAst : BaseCommand, ICommand
{
    public void Execute(params string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: generate_ast <output directory>");
            Environment.Exit(64);
        }

        var outputDir = args[1];

        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        DefineAst(outputDir, "Expr", [
            "Binary   : Expr Left, Token Operator, Expr Right",
            "Grouping : Expr Expression",
            "Literal  : object? Value",
            "Unary    : Token Operator, Expr Right"
        ]);
    }

    private static void DefineAst(string outputDir, string baseName, string[] types)
    {
        var path = outputDir + "/" + baseName + ".cs";
        var writer = new StreamWriter(path);

        writer.WriteLine("using System;");
        writer.WriteLine("using System.Collections.Generic;");
        writer.WriteLine("using System.Linq;");
        writer.WriteLine("using System.Text;");
        writer.WriteLine("using CodeCrafters.Interpreter.Tokens;");
        writer.WriteLine();
        writer.WriteLine("namespace CodeCrafters.Interpreter;");
        writer.WriteLine();
        writer.WriteLine("internal abstract class " + baseName + " {");

        DefineVisitor(writer, baseName, types);

        // The AST classes.
        foreach (var type in types)
        {
            var splitType = type.Split(':');
            var className = splitType[0].Trim();
            var fields = splitType[1].Trim();
            DefineType(writer, baseName, className, fields);
        }

        // The base accept() method.
        //writer.WriteLine();
        writer.WriteLine("  public abstract T Accept<T>(IVisitor<T> visitor);");

        writer.WriteLine("}");

        writer.Close();
    }

    private static void DefineType(StreamWriter writer, string baseName, string className, string fieldList)
    {
        var fields = fieldList.Split(", ");

        writer.WriteLine("  public class " + className + "(" +
                     string.Join(", ", fields) + ") : " + baseName);
        writer.WriteLine("  {");

        // Fields.
        //writer.WriteLine();
        foreach (var field in fields)
        {
            var split = field.Split(" ");
            //var fieldType = split[0];
            var fieldName = split[1];
            writer.WriteLine("    public " + field + " { get; set; } = " + fieldName + ";");
        }

        //// Constructor.
        //writer.WriteLine("    public " + className + "(" + fieldList + ")");
        //writer.WriteLine("    {");

        //// Store parameters in fields.
        //foreach (var field in fields)
        //{
        //    writer.WriteLine("      this." + name + " = " + name + ";");
        //}

        // Visitor pattern.
        //writer.WriteLine();
        //writer.WriteLine("    ");
        writer.WriteLine("    public override T Accept<T>(IVisitor<T> visitor) {");
        writer.WriteLine("      return visitor.Visit" +  className + baseName + "(this);");
        writer.WriteLine("    }");

        writer.WriteLine("  }");
        writer.WriteLine();
    }

    private static void DefineVisitor(StreamWriter writer, string baseName, string[] types)
    {
        writer.WriteLine("  public interface IVisitor<out T> {");

        foreach (var type in types)
        {
            var typeName = type.Split(":")[0].Trim();
            writer.WriteLine("    T Visit" + typeName + baseName + "(" +
                             typeName + " " + baseName.ToLower() + ");");
        }

        writer.WriteLine("  }");
    }
}
