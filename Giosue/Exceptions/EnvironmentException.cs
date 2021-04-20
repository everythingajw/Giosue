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
        Unknown = 1,
        UndefinedVariable = 2,
        VariableAlreadyDefined = 3,
        VariableNameIsReservedKeyword = 4
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
