namespace CodeCrafters.Interpreter.Exceptions;

internal class ParseError : RuntimeException
{
    public ParseError(Token token) : base(token)
    {
    }

    public ParseError(Token token, string? message) : base(token, message)
    {
    }

    public ParseError(Token token, string? message, Exception? innerException) : base(token, message, innerException)
    {
    }
}
