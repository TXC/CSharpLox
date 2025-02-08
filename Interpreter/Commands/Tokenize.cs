using CodeCrafters.Interpreter.Attributes;

// ReSharper disable IdentifierTypo

namespace CodeCrafters.Interpreter.Commands;

[Command("tokenize")]
public class Tokenize : ICommand
{
    public Tokenize() { }

    public int Execute(params string[] args)
    {
        var source = args[1];
        if (!File.Exists(source))
        {
            throw new FileNotFoundException("File not found", source);
        }
        var content = File.ReadAllText(source).Trim();

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

        return scanner.IsValid ? 0 : 65;
    }
}
