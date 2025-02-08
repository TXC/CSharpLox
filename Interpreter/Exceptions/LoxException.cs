namespace CodeCrafters.Interpreter.Exceptions;

internal class LoxException : Exception
{
    public int Code { get; set; }

    public LoxException(int? code) : base()
    {
        Code = code ?? 1;
    }

    public LoxException(int? code, string? message) : base(message)
    {
        Code = code ?? 1;
    }

    public LoxException(int? code, string? message, Exception? innerException) : base(message, innerException)
    {
        Code = code ?? 1;
    }
}
