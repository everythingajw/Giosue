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
using System.Text;

namespace Giosue
{
    /// <summary>
    /// Represents the type of a token.
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// The current character is not a token and can be disregarded.
        /// </summary>
        None,

        #region Single character tokens

        /// <summary>
        /// The character <c>(</c>.
        /// </summary>
        LeftParenthesis,

        /// <summary>
        /// The character <c>)</c>.
        /// </summary>
        RightParenthesis,

        /// <summary>
        /// The character <c>{</c>.
        /// </summary>
        LeftBrace,

        /// <summary>
        /// The character <c>}</c>.
        /// </summary>
        RightBrace,

        /// <summary>
        /// The character <c>,</c>.
        /// </summary>
        Comma,

        /// <summary>
        /// The character <c>.</c>.
        /// </summary>
        Dot,

        /// <summary>
        /// The character <c>-</c>.
        /// </summary>
        Minus,

        /// <summary>
        /// The character <c>+</c>.
        /// </summary>
        Plus,

        /// <summary>
        /// The character <c>;</c>.
        /// </summary>
        Semicolon,

        /// <summary>
        /// The character <c>/</c>.
        /// </summary>
        Slash,

        /// <summary>
        /// The character <c>*</c>.
        /// </summary>
        Star,

        /// <summary>
        /// The character <c>@</c>.
        /// </summary>
        At,

        #endregion Single character tokens

        #region Logical and bitwise operators

        /// <summary>
        /// The character <c>!</c>.
        /// </summary>
        Bang,

        /// <summary>
        /// The character <c>!=</c>.
        /// </summary>
        BangEqual,

        /// <summary>
        /// The character <c>=</c>.
        /// </summary>
        Equal,

        /// <summary>
        /// The character <c>==</c>.
        /// </summary>
        EqualEqual,

        /// <summary>
        /// The character <c>&gt;</c>.
        /// </summary>
        Greater,

        /// <summary>
        /// The character <c>&gt;=</c>.
        /// </summary>
        GreaterEqual,

        /// <summary>
        /// The character <c>&lt;</c>.
        /// </summary>
        Less,

        /// <summary>
        /// The character <c>&lt;=</c>.
        /// </summary>
        LessEqual,

        /// <summary>
        /// The character <c>&amp;</c>.
        /// </summary>
        /// <remarks>
        /// This is a bitwise operator, not a logical operator.
        /// </remarks>
        And,

        /// <summary>
        /// The character <c>&amp;&amp;</c>.
        /// </summary>
        /// <remarks>
        /// This is a logical operator, not a bitwise operator.
        /// </remarks>
        AndAnd,

        /// <summary>
        /// The character <c>|</c>.
        /// </summary>
        /// <remarks>
        /// This is a bitwise operator, not a logical operator.
        /// </remarks>
        Pipe,

        /// <summary>
        /// The character <c>||</c>.
        /// </summary>
        /// <remarks>
        /// This is a logical operator, not a bitwise operator.
        /// </remarks>
        PipePipe,

        /// <summary>
        /// The character <c>^</c>.
        /// </summary>
        /// <remarks>
        /// This is a bitwise operator, not a logical operator.
        /// </remarks>
        Caret,

        /// <summary>
        /// The character <c>^^</c>.
        /// </summary>
        /// <remarks>
        /// This is a logical operator, not a bitwise operator.
        /// </remarks>
        CaretCaret,

        #endregion Logical and bitwise operators

        #region Literals

        /// <summary>
        /// An identifier literal, such as <c>myString</c>.
        /// </summary>
        Identifier,

        /// <summary>
        /// A string literal, such as <c>"this string"</c>.
        /// </summary>
        String,

        /// <summary>
        /// An integer literal, such as <c>314</c>.
        /// </summary>
        Integer,

        /// <summary>
        /// A floating-point literal, such as <c>1.618</c>.
        /// </summary>
        Float,

        #endregion Literals

        #region Keywords

        /// <summary>
        /// The keyword <c>var</c>.
        /// </summary>
        Var,

        /// <summary>
        /// The keyword <c>fun</c>.
        /// </summary>
        Fun,

        /// <summary>
        /// The keyword <c>oppure</c>.
        /// </summary>
        /// <remarks>
        /// The equivalent of <c>else</c>.
        /// </remarks>
        Oppure,

        /// <summary>
        /// The keyword <c>falso</c>.
        /// </summary>
        /// <remarks>
        /// The equivalent of <c>false</c>.
        /// </remarks>
        Falso,

        /// <summary>
        /// The keyword <c>se</c>.
        /// </summary>
        /// <remarks>
        /// The equivalent of <c>if</c>.
        /// </remarks>
        Se,

        /// <summary>
        /// The keyword <c>oppure</c>.
        /// </summary>
        /// <remarks>
        /// The equivalent of <c>null</c>.
        /// </remarks>
        Niente,

        /// <summary>
        /// The keyword <c>vero</c>.
        /// </summary>
        /// <remarks>
        /// The equivalent of <c>true</c>.
        /// </remarks>
        Vero,

        /// <summary>
        /// The keyword <c>mentre</c>.
        /// </summary>
        /// <remarks>
        /// The equivalent of <c>while</c>.
        /// </remarks>
        Mentre,


        /// <summary>
        /// The end-of-file character.
        /// </summary>
        EOF

        #endregion Keywords
    }
}
