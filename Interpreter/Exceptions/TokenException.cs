namespace CodeCrafters.Interpreter.Exceptions;

internal class TokenException : LoxException
{
    public char Character { get; set; }
    public int Line { get; set; }

    private const int DefaultCode = 65;

    public TokenException() : base(DefaultCode)
    {
    }

    public TokenException(string message, char character, int line)
        : base(DefaultCode, message)
    {
        Character = character;
        Line = line;
    }

    public TokenException(string message, int line)
        : base(DefaultCode, message)
    {
        Line = line;
    }

    public TokenException(string message, char character, int line, Exception? innerException)
        : base(DefaultCode, message, innerException)
    {
        Character = character;
        Line = line;
    }
    public TokenException(string message, int line, Exception? innerException)
        : base(DefaultCode, message, innerException)
    {
        Line = line;
    }
}

