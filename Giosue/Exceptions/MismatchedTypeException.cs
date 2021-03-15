using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giosue.Exceptions
{
    public class MismatchedTypeException : Exception
    {
        public Type ActualLeftType { get; }
        public List<Type> ExpectedLeftTypes { get; }
        public Type ActualRightType { get; }
        public List<Type> ExpectedRightTypes { get; }

        public MismatchedTypeException() { }
        public MismatchedTypeException(string message) : base(message) { }
        public MismatchedTypeException(string message, Exception inner) : base(message, inner) { }

        public MismatchedTypeException(Type actualLeftType, List<Type> expectedLeftTypes, Type actualRightType, List<Type> expectedRightTypes, string message = null, Exception inner = null) : this(message, inner)
        {
            ActualLeftType = actualLeftType;
            ExpectedLeftTypes = expectedLeftTypes;
            ActualRightType = actualRightType;
            ExpectedRightTypes = expectedRightTypes;
        }
    }
}
