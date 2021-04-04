using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giosue.Exceptions
{
    public enum InterpreterExceptionType : int
    {
        AllOK = 0,
        Unknown = 1,
        MismatchedTypes
    }

    public class InterpreterException : GiosueException<InterpreterExceptionType>
    {
        public override InterpreterExceptionType ExceptionType { get; }

        //public InterpreterException() { }
        //public InterpreterException(string message) : base(message) { }
        //public InterpreterException(string message, Exception inner) : base(message, inner) { }
        public InterpreterException(InterpreterExceptionType exceptionType, string message) : base(message)
        {
            Category = GiosueExceptionCategory.Interpreter;
            ExceptionType = exceptionType;
        }
    }
}

