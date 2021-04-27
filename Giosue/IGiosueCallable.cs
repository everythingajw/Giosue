using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giosue
{
    /// <summary>
    /// 
    /// </summary>
    interface IGiosueCallable
    {
        /// <summary>
        /// The number of arguments the <see cref="IGiosueCallable"/> has.
        /// </summary>
        int Arity { get; }

        object Call(Interpreter interpreter, List<object> arguments);
    }
}
