using System.Globalization;
using CodeCrafters.Interpreter.Exceptions;
using Microsoft.Extensions.Logging;

// ReSharper disable IdentifierTypo

namespace CodeCrafters.Interpreter;
public class Scanner
{
    //private readonly string _source = source;
    private readonly string _source;
    private List<Token> _tokens = [];
    private int _start = 0;
    private int _current = 0;
    private int _line = 1;
    private readonly ILogger _logger;
    private readonly Dictionary<string, TokenType> _keywords = new() {
        {"and",    TokenType.AND },
        {"class",  TokenType.CLASS},
        {"else",   TokenType.ELSE},
        {"false",  TokenType.FALSE},
        {"for",    TokenType.FOR},
        {"fun",    TokenType.FUN},
        {"if",     TokenType.IF},
        {"nil",    TokenType.NIL},
        {"or",     TokenType.OR},
        {"print",  TokenType.PRINT},
        {"return", TokenType.RETURN},
        {"super",  TokenType.SUPER},
        {"this",   TokenType.THIS},
        {"true",   TokenType.TRUE},
        {"var",    TokenType.VAR},
        {"while",  TokenType.WHILE},
    };

    public bool IsValid { get; private set; }

    public Scanner(string source)
    {
        using var factory = LoggerFactory.Create(builder => builder.AddConsole());
        _logger = factory.CreateLogger("Scanner");
        _source = source;
    }

    public List<Token> ScanTokens()
    {
        IsValid = true;
        while (!IsAtEnd())
        {
            _start = _current;
            // We are at the beginning of the next lexeme.
            ScanToken();
        }

        _tokens.Add(new Token(TokenType.EOF, "", null, _line));
        return _tokens;
    }

    private void ScanToken()
    {
        var c = Advance();

        try
        {
            var tokentype = c switch
            {
                '(' => TokenType.LEFT_PAREN,
                ')' => TokenType.RIGHT_PAREN,
                '{' => TokenType.LEFT_BRACE,
                '}' => TokenType.RIGHT_BRACE,
                ',' => TokenType.COMMA,
                '.' => TokenType.DOT,
                '-' => TokenType.MINUS,
                '+' => TokenType.PLUS,
                ';' => TokenType.SEMICOLON,
                '*' => TokenType.STAR,
                '!' => Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG,
                '=' => Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL,
                '<' => Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS,
                '>' => Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER,
                // A comment goes until the end of the line.
                '/' => Match('/') ? ReadUntilEol() : TokenType.SLASH,
                ' ' or '\n' or '\r' or '\t' => TokenType.None,
                '"' => AddString(),
                _ => IsDigit(c) ? AddNumber() : IsAlpha(c) ? AddIdentifier() : AddInvalidToken("Unexpected character", c)
            };

            if (tokentype != TokenType.None)
            {
                AddToken(tokentype);
            }
        }
        catch (LoxException ex)
        {
            _logger.LogError("Caught error: {error}", ex.Message);
            return;
        }

        if (c == '\n')
        {
            _line++;
        }
    }

    private TokenType ReadUntilEol()
    {
        while (Peek() != '\n' && !IsAtEnd())
        {
            Advance();
        }

        return TokenType.None;
    }

    private TokenType AddString()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n')
            {
                _line++;
            }
            Advance();
        }

        if (IsAtEnd())
        {
            return AddInvalidToken("Unterminated string.", null);
        }

        // The closing ".
        Advance();

        // Trim the surrounding quotes.
        var value = _source[(_start + 1)..(_current - 1)];
        AddToken(TokenType.STRING, value);
        return TokenType.None;
    }

    private TokenType AddNumber()
    {
        while (IsDigit(Peek()))
        {
            Advance();
        }

        // Look for a fractional part.
        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            // Consume the "."
            Advance();

            while (IsDigit(Peek()))
            {
                Advance();
            }
        }

        var value = _source[_start.._current];
        if (double.TryParse(value, CultureInfo.InvariantCulture, out var z))
        {
            AddToken(TokenType.NUMBER, z);
        }
        else
        {
            return AddInvalidToken("Parse error.", value);
        }

        return TokenType.None;
    }

    private TokenType AddIdentifier()
    {
        while (IsAlphaNumeric(Peek()))
        {
            Advance();
        }
        var text = _source[_start.._current];
        if (!_keywords.TryGetValue(text, out var type))
        {
            type = TokenType.IDENTIFIER;
        }

        AddToken(type);
        return TokenType.None;
    }

    private bool IsAtEnd() => _current >= _source.Length;
    private bool IsDigit(char c) => c >= '0' && c <= '9';
    private bool IsAlpha(char c) => c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c == '_';
    private bool IsAlphaNumeric(char c) => IsAlpha(c) || IsDigit(c);

    private char Advance() => _source[_current++];

    private void AddToken(TokenType type) => AddToken(type, null);

    private void AddToken(TokenType type, object? literal)
        => _tokens.Add(new Token(type, _source[_start.._current], literal, _line, _start, _current));

    private TokenType AddInvalidToken(string message, object? literal)
    {
        IsValid = false;
        _tokens.Add(new Token(null, message, literal, _line, _start, _current));
        return TokenType.None;
    }

    private bool Match(char expected)
    {
        if (IsAtEnd())
        {
            return false;
        }

        if (_source[_current] != expected)
        {
            return false;
        }

        _current++;
        return true;
    }

    private char Peek() => IsAtEnd() ? '\0' : _source[_current];

    private char PeekNext() => _current + 1 >= _source.Length ? '\0' : _source[_current + 1];
}
