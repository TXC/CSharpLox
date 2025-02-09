using CodeCrafters.Interpreter.Attributes;

// ReSharper disable IdentifierTypo

namespace CodeCrafters.Interpreter.Commands;

[Command("parse")]
public class Parse : BaseCommand, ICommand
{
    public void Execute(params string[] args)
    {
        var content = ParseFile(args);
        var scanner = new Scanner(content);
        var tokens = scanner.ScanTokens();

        var parser = new Parser(tokens);
        var expressions = parser.Parse();

        Console.WriteLine(new AstPrinter().Print(expressions));
    }

}
