﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giosue.Builtins.Casts
{
    class ToInt : IGiosueCallable
    {
        public const string Name = "TrasformaInIntero";

        public int Arity => throw new NotImplementedException();

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            throw new NotImplementedException();
        }
    }
}
