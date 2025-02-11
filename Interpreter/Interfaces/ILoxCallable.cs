using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable IdentifierTypo

namespace CodeCrafters.Interpreter.Interfaces;

internal interface ILoxCallable
{
    int Arity { get; }

    object? Call(Interpreter interpreter, IEnumerable<object?> arguments);
}
