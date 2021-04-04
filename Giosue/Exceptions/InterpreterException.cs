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
        public override GiosueExceptionCategory Category => throw new NotImplementedException();

        public override InterpreterExceptionType ExceptionType => throw new NotImplementedException();

        public InterpreterException() { }
        public InterpreterException(string message) : base(message) { }
        public InterpreterException(string message, Exception inner) : base(message, inner) { }
    }
}

