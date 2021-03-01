using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceManager.Exceptions
{

    public class TokenTooLongException : Exception
    {
        public string Token { get; } = null;

        #region Common constructors

        public TokenTooLongException()
        {
        }

        public TokenTooLongException(string message) : base(message)
        {
        }

        public TokenTooLongException(string message, Exception inner) : base(message, inner)
        {
        }

        #endregion Common constructors

        public TokenTooLongException(string token, string message) : this(message)
        {
            Token = token;
        }

        public TokenTooLongException(string token, string message, Exception inner) : this(message, inner)
        {
            Token = token;
        }
    }
}
