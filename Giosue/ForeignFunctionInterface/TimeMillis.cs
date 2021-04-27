﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giosue.ForeignFunctionInterface
{
    class TimeMillis : IGiosueCallable
    {
        private static readonly DateTime UnixEpoch = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        
        public int Arity => 0;

        public TimeMillis()
        {

        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            unchecked
            {
                return (int)(DateTime.Now - UnixEpoch).TotalMilliseconds;
            }
        }
    }
}