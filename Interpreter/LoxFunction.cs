using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CodeCrafters.Interpreter.Exceptions;
using CodeCrafters.Interpreter.Interfaces;
// ReSharper disable IdentifierTypo

namespace CodeCrafters.Interpreter;

internal class LoxFunction(Stmt.Function declaration, Environment closure) : ILoxCallable
{
    public int Arity => declaration.Parameters.Count;

    public object? Call(Interpreter interpreter, IEnumerable<object?> arguments)
    {
        //var environment = new Environment(interpreter.globals);
        var environment = new Environment(closure);
        for (var i = 0; i < declaration.Parameters.Count; i++)
        {
            environment.Define(declaration.Parameters[i].Lexeme, arguments.ElementAt(i));
        }

        try
        {
            interpreter.ExecuteBlock(declaration.Body, environment);
        }
        catch (Return returnValue)
        {
            return returnValue.Value;
        }
        return null;
    }

    public override string ToString() => "<fn " + declaration.Name.Lexeme + ">";
}
