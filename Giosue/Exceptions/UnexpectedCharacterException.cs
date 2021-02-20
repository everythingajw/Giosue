using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giosue.Exceptions
{
    public class UnexpectedCharacterException : Exception
    {
        public int Line { get; } = 0;

        public UnexpectedCharacterException() : base()
        {

        }

        public UnexpectedCharacterException(string message) : base(message)
        {

        }

        public UnexpectedCharacterException(int line, string message) : this(message)
        {
            Line = line;
        }

        public UnexpectedCharacterException(string message, Exception innerException) : base(message, innerException)
        {

        }

        public UnexpectedCharacterException(int line, string message, Exception innerException) : this(message, innerException)
        {
            Line = line;
        }
    }
}
