using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giosue.Exceptions
{
    public enum ScannerExceptionType : int
    {
        AllOK = 0,
        Unknown = 1
        UnexpectedCharacter,
        UnterminatedString
    }

    public class ScannerException : GiosueException<ScannerExceptionType>
    {
        public override GiosueExceptionCategory Category { get; }

        public override ScannerExceptionType ExceptionType { get; }

        //public ScannerException() { }
        //public ScannerException(string message) : base(message) { }
        //public ScannerException(string message, Exception inner) : base(message, inner) { }
        public ScannerException(GiosueExceptionCategory category, ParserExceptionType exceptionType, string message) : base(message)
        {
            Category = category;
            ExceptionType = exceptionType;
        }
    }
}
