namespace CodeCrafters.Interpreter.Exceptions;

internal class LoxException : Exception
{
    public LoxException() : base()
    {
    }

    public LoxException(string? message) : base(message)
    {
    }

    public LoxException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
