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
        ScannerException = 3,
        ParserException = 4,
        InterpreterException = 5,
    }
}
