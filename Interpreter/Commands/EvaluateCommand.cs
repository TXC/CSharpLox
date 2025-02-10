using CodeCrafters.Interpreter.Attributes;
// ReSharper disable IdentifierTypo

namespace CodeCrafters.Interpreter.Commands;

[Command("evaluate")]
public class EvaluateCommand : BaseCommand, ICommand
{
    public void Execute(params string[] args)
    {
        var content = ParseFile(args);
        content = Lox.AddPrintInput(content);
        Lox.Run(content);

        //if (Lox.HadError)
        //{
        //    System.Environment.Exit(65);
        //}
        if (Lox.HadRuntimeError)
        {
            System.Environment.Exit(70);
        }
    }
}
