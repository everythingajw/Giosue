using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giosue.ReturnCodes
{
    public enum GiosueReturnCode : int
    {
        AllOK = 0,
        UnknownException = 1,
        FileNotFound = 2,
        UnexpectedCharacter = 3,
        UnterminatedString = 4,
        MismatchedTypes = 5,
        TokenTooLong = 6
    }
}
