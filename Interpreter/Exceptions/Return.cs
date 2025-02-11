using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCrafters.Interpreter.Exceptions;

internal class Return : RuntimeException
{
    public object? Value = null;

    public Return(object? value) : base(null)
    {
        Value = value;
    }

    public Return(object? value, string? message) : base(null, message)
    {
        Value = value;
    }

    public Return(object? value, string? message, Exception? innerException) : base(null, message, innerException)
    {
        Value = value;
    }
}

