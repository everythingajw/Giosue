using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giosue.Exceptions
{
    public enum EnvironmentExceptionType
    {
        AllOK = 0,

    }
    
    public class EnvironmentException : GiosueException<EnvironmentExceptionType>
    {
        public override EnvironmentExceptionType ExceptionType { get; }

        public EnvironmentException() : base(default) { }
        public EnvironmentException(string message) : base(message) { }
        public EnvironmentException(EnvironmentExceptionType environmentExceptionType, string message = default) : this(message)
        {
            ExceptionType = environmentExceptionType;
        }
    }
}
