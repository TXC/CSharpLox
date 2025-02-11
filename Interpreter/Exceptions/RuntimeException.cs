namespace CodeCrafters.Interpreter.Exceptions;

internal class RuntimeException : Exception
{
    public Token? Token { get; set; }

    public RuntimeException(Token? token) : base()
    {
        Token = token;
    }

    public RuntimeException(Token? token, string? message) : base(message)
    {
        Token = token;
    }

    public RuntimeException(Token? token, string? message, Exception? innerException) : base(message, innerException)
    {
        Token = token;
    }
}
