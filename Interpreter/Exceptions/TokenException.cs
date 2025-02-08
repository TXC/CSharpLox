namespace CodeCrafters.Interpreter.Exceptions;

internal class TokenException : LoxException
{
    public int Line { get; set; }

    public TokenException() : base()
    {
    }

    public TokenException(string message, int line) : base(message)
    {
        Line = line;
    }

    public TokenException(string message, int line, Exception? innerException) : base(message, innerException)
    {
        Line = line;
    }
}

