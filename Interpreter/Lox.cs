using CodeCrafters.Interpreter.Exceptions;
// ReSharper disable IdentifierTypo

namespace CodeCrafters.Interpreter;

internal static class Lox
{
    public static bool HadError = false;
    public static bool HadRuntimeError = false;

    public static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine("[line " + line + "] Error" + where + ": " + message);
        HadError = true;
    }

    public static void Error(int line, string message)
        => Report(line, "", message);

    public static bool CheckError()
    {
        return Lox.HadError || Lox.HadRuntimeError;
    }

    public static void CheckAndHaltOnError()
    {
        if (Lox.HadError)
        {
            System.Environment.Exit(65);
        }
        if (Lox.HadRuntimeError)
        {
            System.Environment.Exit(70);
        }
    }

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
}
