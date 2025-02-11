using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCrafters.Interpreter.Exceptions
{
    internal class Break : LoxException
    {
        public Break() : base(null)
        {
        }

        public Break(string? message) : base(null, message)
        {
        }

        public Break(string? message, Exception? innerException) : base(null, message, innerException)
        {
        }
    }
}
