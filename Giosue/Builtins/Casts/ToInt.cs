using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giosue.Builtins.Casts
{
    class ToInt : IGiosueCallable
    {
        public const string Name = "TrasformaInIntero";

        public int Arity => 1;

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            return (double)arguments[0];
        }
    }
}
