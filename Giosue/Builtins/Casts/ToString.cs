﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giosue.Builtins.Casts
{
    class ToString : IGiosueCallable
    {
        public const string Name = "TrasformaInStringa";

        public int Arity => 1;

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            return arguments[0].ToString();
        }
    }
}
