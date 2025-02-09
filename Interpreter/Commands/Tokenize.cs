using CodeCrafters.Interpreter.Attributes;

// ReSharper disable IdentifierTypo

namespace CodeCrafters.Interpreter.Commands;

[Command("tokenize")]
public class Tokenize : BaseCommand, ICommand
{
    public Tokenize() { }

    public void Execute(params string[] args)
    {
        var content = ParseFile(args);

        var scanner = new Scanner(content);
        var tokens = scanner.ScanTokens();

        foreach (var token in tokens)
        {
            if (token.Type is null)
            {
                Console.Error.WriteLine(token);
            }
            else
            {
                Console.WriteLine(token);
            }
        }

        if (Lox.HadError)
        {
            Environment.Exit(65);
        }
    }
}
