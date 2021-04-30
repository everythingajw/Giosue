// Giosue language interpreter
// The interpreter for the Giosue programming language.
// Copyright (C) 2021  Anthony Webster
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giosue.Exceptions
{
    public enum ParserExceptionType : int
    {
        AllOK = 0,
        Unknown = 1,
    }
    public class ParserException : GiosueException<ParserExceptionType>
    {
        public override ParserExceptionType ExceptionType { get; }

        public Token ErroneousToken { get; }

        //public ParserException() : this(default, default, default, default) { }

        //public ParserException(string message) : this(default, default, message, default) { }

        //public ParserException(string message, Exception inner) : this(default, default, message, inner) { }

        //public ParserException(GiosueExceptionCategory category, ParserExceptionType exceptionType) : this(category, exceptionType, default, default) { }

        public ParserException(ParserExceptionType exceptionType, Token erroneousToken, string message) : base(message)
        {
            Category = GiosueExceptionCategory.Parser;
            ErroneousToken = erroneousToken;
            ExceptionType = exceptionType;
        }

        //protected ParserException(GiosueExceptionCategory category, ParserExceptionType exceptionType, string message, Exception inner) : base(message, inner) { }
    }
}
