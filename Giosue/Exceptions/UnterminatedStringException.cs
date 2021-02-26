using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giosue.Exceptions
{
    public class UnterminatedStringException : Exception
    {
        public int Line { get; } = 0;

        #region Common constructors

        public UnterminatedStringException() : base()
        {

        }

        public UnterminatedStringException(string message) : base(message)
        {

        }

        public UnterminatedStringException(string message, Exception innerException) : base(message, innerException)
        {

        }

        #endregion Common constructors
    
        public UnterminatedStringException(int line, string message) : this(line, message, null)
        {
            Line = line;
        }

        public UnterminatedStringException(int line, string message, Exception innerException) : this(message, innerException)
        {
            Line = line;
        }
    }
}
