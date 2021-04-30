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
