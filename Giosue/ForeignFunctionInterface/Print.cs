using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giosue.ForeignFunctionInterface
{
    class Print : GiosueCallable
    {
        public int Arity => 1;

        public Print()
        {

        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            // TODO: Throw exception if invalid arguments
            Console.Write(arguments[0]);
            return null;
        }
    }
}
