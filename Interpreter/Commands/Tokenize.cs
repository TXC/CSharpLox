using CodeCrafters.Interpreter.Attributes;

// ReSharper disable IdentifierTypo

namespace CodeCrafters.Interpreter.Commands;

[Command("tokenize")]
public class Tokenize : ICommand
{
    public Tokenize() { }

    public void Execute(params string[] args)
    {
        var scanner = new Scanner(args[1]);
        var tokens = scanner.ScanTokens();

        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
    }
}
