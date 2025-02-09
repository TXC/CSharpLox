using CodeCrafters.Interpreter.Attributes;

namespace CodeCrafters.Interpreter.Commands;

[Command("evaluate")]
public class Evaluate : BaseCommand, ICommand
{
    public void Execute(params string[] args)
    {
        var content = ParseFile(args);
        var scanner = new Scanner(content);
        var tokens = scanner.ScanTokens();

        var parser = new Parser(tokens);
        var expression = parser.Parse();
        var interpreter = new Interpreter();

        if (expression is null || Lox.HadError)
        {
            Environment.Exit(65);
        }

        interpreter.Interpret(expression);
        //Console.WriteLine(new AstPrinter().Print(expression));
    }
}

