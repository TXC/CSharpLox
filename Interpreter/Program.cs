using System.Reflection;
using System.Security.Cryptography.X509Certificates;
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
            if (args.Length < 2)
            {
                Console.Error.WriteLine("Usage: ./your_program.sh tokenize <filename>");
                Environment.Exit(1);
            }

            var result = AttributeHelper.GetAttributeTypes<ICommand, CommandAttribute>(x => x.Command == args[0]);

            if (result.Count > 1)
            {
                log.LogError("Ambiguous command: {argument}", args[0]);
                Environment.Exit(1);
            }
            else if (result.Count == 0)
            {
                log.LogError("Unknown command: {argument}", args[0]);
                Environment.Exit(1);
            }

            var type = result.Single();
            var classInstance = (ICommand?)Activator.CreateInstance(type, null) ?? throw new NullReferenceException($"Unknown instance {type.Name}");
            var methodInfo = type.GetMethod("Execute");
            methodInfo?.Invoke(classInstance, [args]);
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
        }
    }
}
