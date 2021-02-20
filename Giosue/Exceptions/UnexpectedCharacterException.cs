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

        public char UnexpectedCharacter { get; } = default;

        #region Common constructors

        public UnexpectedCharacterException() : base()
        {

        }

        public UnexpectedCharacterException(string message) : base(message)
        {

        }
        
        public UnexpectedCharacterException(string message, Exception innerException) : base(message, innerException)
        {

        }

        #endregion Common constructors

        public UnexpectedCharacterException(int line, char unexpectedCharacter, string message) : base(message)
        {
            Line = line;
            UnexpectedCharacter = unexpectedCharacter;
        }

        public UnexpectedCharacterException(int line, char unexpectedCharacter, string message, Exception innerException) : base(message, innerException)
        {
            Line = line;
            UnexpectedCharacter = unexpectedCharacter;
        }
    }
}
