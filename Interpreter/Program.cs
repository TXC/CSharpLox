using CodeCrafters.Interpreter.Attributes;
using CodeCrafters.Interpreter.Commands;
using Microsoft.Extensions.Logging;
// ReSharper disable IdentifierTypo

namespace CodeCrafters.Interpreter;

public class Program
{
    public static void Main(string[] args)
    {
        using var factory = LoggerFactory.Create(builder => builder.AddConsole());
        var log = factory.CreateLogger("Program");

        try
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("Usage: ./your_program.sh <command> [arguments]");
                System.Environment.Exit(1);
            }

            var types = AttributeHelper.GetAttributeTypes<ICommand, CommandAttribute>(x => x.Command == args[0]);

            if (types.Count > 1)
            {
                log.LogError("Ambiguous command: {argument}", args[0]);
                System.Environment.Exit(1);
            }
            else if (types.Count == 0)
            {
                log.LogError("Unknown command: {argument}", args[0]);
                System.Environment.Exit(1);
            }

            var type = types.Single();
            var classInstance = (ICommand?)Activator.CreateInstance(type, null) ?? throw new NullReferenceException($"Unknown instance {type.Name}");
            var methodInfo = type.GetMethod("Execute");
            var result = methodInfo?.Invoke(classInstance, [args]);

            Lox.CheckAndHaltOnError();
            System.Environment.Exit(0);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "{message}", ex.Message);
        }
    }
}
