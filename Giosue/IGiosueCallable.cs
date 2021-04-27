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
        object Call(Interpreter interpreter, List<object> arguments);
    }
}
