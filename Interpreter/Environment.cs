using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeCrafters.Interpreter.Exceptions;

namespace CodeCrafters.Interpreter;

internal class Environment
{
    private readonly Dictionary<string, object?> _values = [];
    //private readonly Environment _globals = new();
    private readonly Environment? _enclosing = null;

    public Environment()
    {
        //_globals.Define("clock", new ILoxCallable() {
        //    public int Arity() { return 0; }
        //    public object Call(Interpreter interpreter, List<object> arguments)
        //    {
        //        return (double)System.currentTimeMillis() / 1000.0;
        //    }
        //    public override string ToString() { return "<native fn>"; }
        //});
    }
    public Environment(Environment enclosing) : this()
    {
        _enclosing = enclosing;
    }

    public object? Get(Token name)
    {
        if (_values.TryGetValue(name.Lexeme, out var result))
        {
            return result;
        }

        if (_enclosing is not null)
        {
            return _enclosing.Get(name);
        }

        throw new RuntimeException(name, "Undefined variable '" + name.Lexeme + "'.");
    }

    public void Assign(Token name, object? value)
    {
        if (_values.ContainsKey(name.Lexeme))
        {
            _values[name.Lexeme] = value;
            //_values.put(name.Lexeme, value);
            return;
        }

        if (_enclosing is not null)
        {
            _enclosing?.Assign(name, value);
            return;
        }

        throw new RuntimeException(name, "Undefined variable '" + name.Lexeme + "'.");
    }

    public void Define(string name, object? value)
    {
        if (!_values.TryAdd(name, value))
        {
            _values[name] = value;
        }
    }
}

