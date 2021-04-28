﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giosue.Builtins.ForeignFunctionInterface
{
    class TimeMillis : IGiosueCallable
    {
        public const string Name = "TempoMillis";

        private static readonly DateTime UnixEpoch = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        
        public int Arity => 0;

        public TimeMillis()
        {

        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            unchecked
            {
                return (double)(DateTime.Now - UnixEpoch).TotalMilliseconds;
            }
        }
    }
}