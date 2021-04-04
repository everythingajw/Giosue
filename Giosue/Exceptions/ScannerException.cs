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
        Unknown = 1,
        UnexpectedCharacter = 2,
        UnterminatedString = 3,
        MalformedNumericLiteral = 4,
    }

    public class ScannerException : GiosueException<ScannerExceptionType>
    {
        public override ScannerExceptionType ExceptionType { get; }

        public int Line { get; }

        //public ScannerException() { }
        //public ScannerException(string message) : base(message) { }
        //public ScannerException(string message, Exception inner) : base(message, inner) { }
        public ScannerException(ScannerExceptionType exceptionType, int line, string message) : base(message)
        {
            Category = GiosueExceptionCategory.Scanner;
            ExceptionType = exceptionType;
            Line = line;
        }
    }
}
