using CodeCrafters.Interpreter.Attributes;
// ReSharper disable IdentifierTypo

namespace CodeCrafters.Interpreter.Commands;

[Command("evaluate")]
public class EvaluateCommand : BaseCommand, ICommand
{
    public void Execute(params string[] args)
    {
        var content = ParseFile(args);
        var scanner = new Scanner(content);
        var tokens = scanner.ScanTokens();

        var parser = new Parser(tokens);
        var expression = parser.Parse();
        var interpreter = new Interpreter();

        Lox.CheckAndHaltOnError();

        interpreter.Interpret(expression);

        Lox.CheckAndHaltOnError();

        //Console.WriteLine(new AstPrinter().Print(expression));
    }
}

