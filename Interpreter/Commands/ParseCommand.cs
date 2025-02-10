using CodeCrafters.Interpreter.Attributes;

// ReSharper disable IdentifierTypo

namespace CodeCrafters.Interpreter.Commands;

[Command("parse")]
public class ParseCommand : BaseCommand, ICommand
{
    public void Execute(params string[] args)
    {
        var content = ParseFile(args);
        if (content[..^1] != ";")
        {
            content += ";";
        }

        var scanner = new Scanner(content);
        var tokens = scanner.ScanTokens();

        var parser = new Parser(tokens);
        var expressions = parser.Parse();

        //Lox.CheckAndHaltOnError();

        Console.WriteLine(new Ast.Printer().Print(expressions));
    }
}
