using System.Text.RegularExpressions;
using CodeCrafters.Interpreter.Exceptions;
// ReSharper disable IdentifierTypo

namespace CodeCrafters.Interpreter;

internal static class Lox
{
    public static bool HadError = false;
    public static bool HadRuntimeError = false;
    private static readonly Interpreter Interpreter = new Interpreter();

    public static void Run(string source)
    {
        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();
        var parser = new Parser(tokens);
        //var expression = parser.Parse();
        var statements = parser.Parse();
        
        // Stop if there was a syntax error.
        if (HadError)
        {
            return;
        }

        Console.Error.WriteLine("Running Following:");
        Console.Error.WriteLine(source);

        //Interpreter.Interpret(expression);
        Interpreter.Interpret(statements);
        //var result = Interpreter.Run(statements);
        //Console.WriteLine(result);
    }

    public static void RunFile(string path)
    {
        var source = File.ReadAllText(path).TrimEnd();
        Run(source);

        // Indicate an error in the exit code.
        if (HadError)
        {
            System.Environment.Exit(65);
        }
        if (HadRuntimeError)
        {
            System.Environment.Exit(70);
        }
    }

    public static void RunPrompt()
    {
        using var memory = new MemoryStream();
        using var reader = new StreamReader(memory);
        Console.SetIn(reader);
        for (;;) { 
            Console.Write("> ");
            var line = Console.ReadLine();
            //var line = reader.readLine();
            if (line == null)
            {
                break;
            }
            Run(line);
            HadError = false;
        }
    }

    public static bool CheckError()
    {
        Console.Error.WriteLine($"HadError = {HadError}, HadRuntimeError = {HadRuntimeError}");
        return Lox.HadError || Lox.HadRuntimeError;
    }

    public static void CheckAndHaltOnError()
    {
        Console.Error.WriteLine($"HadError = {HadError}, HadRuntimeError = {HadRuntimeError}");
        if (Lox.HadRuntimeError)
        {
            System.Environment.Exit(70);
        }
        if (Lox.HadError)
        {
            System.Environment.Exit(65);
        }
    }

    public static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine("[line " + line + "] Error" + where + ": " + message);
        HadError = true;
    }

    public static void Error(int line, string message)
        => Report(line, "", message);

    public static void Error(Token token, string message)
    {
        if (token.Type == TokenType.EOF)
        {
            Report(token.Line, " at end", message);
        }
        else
        {
            Report(token.Line, " at '" + token.Lexeme + "'", message);
        }
    }

    public static void RuntimeError(RuntimeException e)
    {
        Console.Error.WriteLine(e.Message);
        Console.Error.WriteLine("[line " + e.Token.Line + "]");
        HadRuntimeError = true;
    }

    public static string AddPrintInput(string input)
    {
        var lines = Regex.Split(input, @"[\n|\n\r|\r]");
        //var lines = input.Split("\n");
        for (var i = 0; i < lines.Length; i++)
        {
            lines[i] = InputAffix(lines[i]);
        }
        return string.Join("\n", lines);
    }

    private static string InputAffix(string input)
    {
        if (input.StartsWith("//"))
        {
            return input;
        }
        if (input.Length < 5 || (input.Length >= 5 && input[..5] != "print"))
        {
            input = "print " + input;
        }
        if (input[..^1] != ";")
        {
            input += ";";
        }

        return input;
    }
}
