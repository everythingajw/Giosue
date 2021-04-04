using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giosue.Exceptions
{
    public enum ParserExceptionType
    {

    }
    public class ParserException : GiosueException<ParserExceptionType>
    {
        public override GiosueExceptionCategory Category => throw new NotImplementedException();

        public override ParserExceptionType ExceptionType => throw new NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        public ParserException() { }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public ParserException(string message) : base(message) { }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public ParserException(string message, Exception inner) : base(message, inner) { }
    }
}
