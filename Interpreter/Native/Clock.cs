using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeCrafters.Interpreter.Interfaces;

namespace CodeCrafters.Interpreter.Native
{
    internal class Clock : ILoxCallable
    {
        public int Arity => 0;

        public object Call(Interpreter interpreter, IEnumerable<object?> arguments)
        {
            return (double)DateTime.Now.Second;
        }

        public override string ToString() => "<native fn>";

        public static readonly Clock Impl = new Clock();

        private Clock() { }
    }
}
