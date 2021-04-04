using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giosue.Exceptions
{
    public enum GiosueExceptionCategory : int
    {
        AllOK = 0,
        Unknown = 1,
        Scanner = 10,
        Parser = 20,
        Interpreter = 30,
    }

    public abstract class GiosueException<T> : Exception where T : Enum
    {
        /// <summary>
        /// The category of the exception.
        /// </summary>
        public GiosueExceptionCategory Category { get; protected init; }

        /// <summary>
        /// The subtype of the exception that occurred.
        /// </summary>
        public abstract T ExceptionType { get; }


        //protected GiosueException() : base() { }

        protected GiosueException(string message) : base(message) { }

        //protected GiosueException(string message, Exception inner) : base(message, inner) { }

        //protected GiosueException(GiosueExceptionCategory category, T exceptionType) : this() { }

        //protected GiosueException(GiosueExceptionCategory category, T exceptionType, string message) : this(message) { }

        //protected GiosueException(GiosueExceptionCategory category, T exceptionType, string message, Exception inner) : this(message, inner) { }
    }
}
