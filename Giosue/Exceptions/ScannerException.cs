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
        public ScannerException() { }
        public ScannerException(string message) : base(message) { }
        public ScannerException(string message, Exception inner) : base(message, inner) { }
    }
}
