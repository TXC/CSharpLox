namespace CodeCrafters.Interpreter.Exceptions;

internal class TokenException : LoxException
{
    public char Character { get; set; }
    public int Line { get; set; }

    public TokenException() : base()
    {
    }

    public TokenException(string message, char character, int line) : base(message)
    {
        Character = character;
        Line = line;
    }

    public TokenException(string message, int line) : base(message)
    {
        Line = line;
    }

    public TokenException(string message, char character, int line, Exception? innerException)
        : base(message, innerException)
    {
        Character = character;
        Line = line;
    }
    public TokenException(string message, int line, Exception? innerException)
        : base(message, innerException)
    {
        Line = line;
    }
}

