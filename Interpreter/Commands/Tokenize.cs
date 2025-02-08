using CodeCrafters.Interpreter.Attributes;

// ReSharper disable IdentifierTypo

namespace CodeCrafters.Interpreter.Commands;

[Command("tokenize")]
public class Tokenize : ICommand
{
    public Tokenize() { }

    public void Execute(params string[] args)
    {
        var filename = args[1];
        var scanner = new Scanner(Path.Combine("Data", filename));
        var tokens = scanner.ScanTokens();

        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
    }
}
