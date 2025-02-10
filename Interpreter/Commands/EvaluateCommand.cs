using CodeCrafters.Interpreter.Attributes;
// ReSharper disable IdentifierTypo

namespace CodeCrafters.Interpreter.Commands;

[Command("evaluate")]
public class EvaluateCommand : BaseCommand, ICommand
{
    public void Execute(params string[] args)
    {
        var content = ParseFile(args);
        if (content[..^1] != ";")
        {
            content += ";";
        }
        Lox.Run(content);
    }
}

