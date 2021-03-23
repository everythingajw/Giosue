using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giosue.Exceptions
{
    public class MismatchedTypeException : Exception
    {
        public List<Type> ExpectedTypes { get; }
        public Type ActualType { get; }

        public MismatchedTypeException() { }
        public MismatchedTypeException(string message) : base(message) { }
        public MismatchedTypeException(string message, Exception inner) : base(message, inner) { }

        public MismatchedTypeException(Type expectedType, Type actualType, string message = null, Exception inner = null) : this(new List<Type>() { expectedType }, actualType, message, inner)
        {
        }

        public MismatchedTypeException(List<Type> expectedTypes, Type actualType, string message = null, Exception inner = null) : this(message, inner)
        {
            ExpectedTypes = expectedTypes;
            ActualType = actualType;
        }
    }
}
