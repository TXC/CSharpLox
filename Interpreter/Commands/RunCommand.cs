using CodeCrafters.Interpreter.Attributes;
// ReSharper disable IdentifierTypo

namespace CodeCrafters.Interpreter.Commands;

[Command("run")]
public class RunCommand : BaseCommand, ICommand
{
    public void Execute(params string[] args)
    {
        var size = args.Count();
        var source = args[size - 1];
        if (!File.Exists(source))
        {
            throw new FileNotFoundException("File not found", source);
        }

        Lox.RunFile(source);
    }
}
