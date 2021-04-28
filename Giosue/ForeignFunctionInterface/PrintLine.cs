﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giosue.ForeignFunctionInterface
{
    class PrintLine : GiosueCallable
    {
        public const string Name = "ScriveLina";

        public int Arity => 1;

        public PrintLine()
        {

        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            Console.WriteLine(arguments[0]);
            return null;
        }
    }
}
