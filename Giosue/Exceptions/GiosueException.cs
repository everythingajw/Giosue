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
    public enum GiosueExceptionCategory : int
    {
        AllOK = 0,
        Unknown = 1,
        Scanner = 10,
        Parser = 20,
        Interpreter = 30,
    }

    public abstract class GiosueException<T> : Exception where T : Enum
    {
        /// <summary>
        /// The category of the exception.
        /// </summary>
        public GiosueExceptionCategory Category { get; protected init; }

        /// <summary>
        /// The subtype of the exception that occurred.
        /// </summary>
        public abstract T ExceptionType { get; }


        //protected GiosueException() : base() { }

        protected GiosueException(string message) : base(message) { }

        //protected GiosueException(string message, Exception inner) : base(message, inner) { }

        //protected GiosueException(GiosueExceptionCategory category, T exceptionType) : this() { }

        //protected GiosueException(GiosueExceptionCategory category, T exceptionType, string message) : this(message) { }

        //protected GiosueException(GiosueExceptionCategory category, T exceptionType, string message, Exception inner) : this(message, inner) { }
    }
}
