using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giosue.Exceptions
{
    public enum ParserExceptionType : int
    {
        AllOK = 0,
        Unknown = 1,
    }
    public class ParserException : GiosueException<ParserExceptionType>
    {
        public override ParserExceptionType ExceptionType { get; }

        //public ParserException() : this(default, default, default, default) { }

        //public ParserException(string message) : this(default, default, message, default) { }

        //public ParserException(string message, Exception inner) : this(default, default, message, inner) { }

        //public ParserException(GiosueExceptionCategory category, ParserExceptionType exceptionType) : this(category, exceptionType, default, default) { }

        public ParserException(ParserExceptionType exceptionType, string message) : base(message)
        {
            Category = GiosueExceptionCategory.Parser;
            ExceptionType = exceptionType;
        }

        //protected ParserException(GiosueExceptionCategory category, ParserExceptionType exceptionType, string message, Exception inner) : base(message, inner) { }
    }
}
