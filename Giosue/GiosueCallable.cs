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
    abstract class GiosueCallable
    {
        public static virtual string Name { get; }

        /// <summary>
        /// The number of arguments the <see cref="GiosueCallable"/> has.
        /// </summary>
        public abstract int Arity { get; }

        public abstract object Call(Interpreter interpreter, List<object> arguments);
    }
}
