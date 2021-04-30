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

namespace SourceManager
{
    public class StringSource : Source
    {
        /// <summary>
        /// The source string for the tokens.
        /// </summary>
        private string Source { get; } = null;

        /// <summary>
        /// The starting index of the current token.
        /// </summary>
        protected override int TokenStartIndex { get; set; } = 0;

        /// <summary>
        /// The index of the current character.
        /// </summary>
        protected override int CurrentCharacterIndex { get; set; } = 0;

        /// <inheritdoc/>
        public override bool IsAtEnd => CurrentCharacterIndex >= Source.Length;

        /// <inheritdoc/>
        public override string CurrentToken 
        {
            get
            {
                if (_currentToken == null)
                {
                    _currentToken = Source[TokenStartIndex..CurrentCharacterIndex];
                }

                return _currentToken;
            } 
        }

        /// <summary>
        /// Creates a new <see cref="StringSource"/>.
        /// </summary>
        /// <param name="source">The <see cref="string"/> that provides a source for the <see cref="StringSource"/>.</param>
        public StringSource(string source) : base()
        {
            // The source string for a {nameof(StringSource)} cannot be null.
            Source = source ?? throw new ArgumentNullException(nameof(source), $"La stringa per un {nameof(StringSource)} non può essere nulla.");
        }

        /// <inheritdoc/>
        public override bool Advance(out char consumed)
        {
            consumed = default;
            _currentToken = null;

            if (IsAtEnd)
            {
                return false;
            }

            consumed = Source[CurrentCharacterIndex++];
            return true;
        }

        /// <inheritdoc/>
        public override bool AdvanceIfMatches(char c, out char consumed)
        {
            consumed = default;
            
            if (IsAtEnd)
            {
                return false;
            }
            if (Peek(out var current))
            {
                if (current != c)
                {
                    return false;
                }
            }

            // This is safe because we already checked if 
            // we reached the end of the source.
            Advance(out consumed);
            
            return true;
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
            // TODO: What should be cleaned up here?
        }

        /// <inheritdoc/>
        public override bool Peek(out char current)
        {
            current = default;

            if (IsAtEnd)
            {
                return false;
            }

            current = Source[CurrentCharacterIndex];
            return true;
        }

        /// <inheritdoc/>
        public override bool PeekNext(out char next)
        {
            next = default;

            if (CurrentCharacterIndex + 1 >= Source.Length)
            {
                return false;
            }

            next = Source[CurrentCharacterIndex + 1];
            return true;
        }
    }
}
