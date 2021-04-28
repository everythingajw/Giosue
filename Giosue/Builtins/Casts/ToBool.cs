using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giosue.Builtins.Casts
{
    class ToBool : IGiosueCallable
    {
        public const string Name = "TrasformaInBool";

        public int Arity => 1;

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            return (bool)arguments[0];
        }
    }
}
