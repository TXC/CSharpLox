// ReSharper disable IdentifierTypo

namespace CodeCrafters.Interpreter.Tokens;

public class Token(TokenType? tokenType, string lexeme, object? literal, int line)
{
    public TokenType? Type { get; set; } = tokenType;
    public string Lexeme { get; set; } = lexeme;
    public object? Literal { get; set; } = literal;
    public int Line { get; set; } = line;

    public Token(TokenType? tokenType, string lexeme) : this(tokenType, lexeme, null, 0)
    {
    }

    public Token(TokenType? tokenType) : this(tokenType, string.Empty, null, 0)
    {
    }

    public Token Clone()
        => new Token(Type, Lexeme, Literal, Line);

    public override string ToString()
    {
        if (Type is null && Literal is null)
        {
            return $"[line {Line}] Error: {Lexeme}";
        }

        if (Type is null)
        {
            return $"[line {Line}] Error: {Lexeme}: {Literal}";
        }

        return $"{Type} {Lexeme} {Literal ?? "null"}";
    }
}
