// ReSharper disable IdentifierTypo

using System.Globalization;

namespace CodeCrafters.Interpreter.Tokens;

public class Token(TokenType? tokenType, string lexeme, object? literal, int line = 0, int startIndex = 0, int endIndex = 0)
{
    public TokenType? Type { get; set; } = tokenType;
    public string Lexeme { get; set; } = lexeme;
    public object? Literal { get; set; } = literal;
    public int StartIndex { get; set; } = startIndex;
    public int EndIndex { get; set; } = endIndex;
    public int Line { get; set; } = line;

    public Token(TokenType? tokenType, string lexeme) : this(tokenType, lexeme, null)
    {
    }

    public Token(TokenType? tokenType) : this(tokenType, string.Empty, null)
    {
    }

    public Token Clone() => new Token(Type, Lexeme, Literal, Line, StartIndex, EndIndex);

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

        if (Type is TokenType.NUMBER)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "{0} {1} {2:0.0##}", Type, Lexeme, Literal);
        }

        return $"{Type} {Lexeme} {Literal ?? "null"}";
    }
}
