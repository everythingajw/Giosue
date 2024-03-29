﻿// Giosue language interpreter
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
using System.IO;
using System.Linq;
using System.Text;
using SourceManager.Exceptions;

namespace SourceManager
{
    // Design comments:
    // The `FileSource` pulls its data from a file, represented by a `StreamReader`.
    // When the `FileSource` is constructed, up to `BufferLength` characters are read
    // from the file into the internal char[] `Buffer`.
    // The current token is everything from `TokenStartIndex` up to and not including
    // the character at `CurrentCharacterIndex`.
    /// <summary>
    /// Represents a source for data.
    /// </summary>
    public class FileSource : Source
    {
        /// <summary>
        /// The length of the internal buffer for characters read.
        /// </summary>
        private const int BufferLength = 10000;

        /// <summary>
        /// The source stream for the data.
        /// </summary>
        private StreamReader Reader { get; } = null;
         
        /// <summary>
        /// The current data that we're working with.
        /// </summary>
        private char[] Buffer { get; } = new char[BufferLength];

        /// <summary>
        /// The starting index of the current token.
        /// </summary>
        protected override int TokenStartIndex { get; set; } = 0;

        /// <summary>
        /// The index of the current character.
        /// </summary>
        protected override int CurrentCharacterIndex { get; set; } = 0;

        /// <summary>
        /// The index of the end of <see cref="Buffer"/>
        /// </summary>
        private int? BufferEndIndex { get; set; } = null;

        /// <summary>
        /// The length of the current token.
        /// </summary>
        private int CurrentTokenLength => CurrentCharacterIndex - TokenStartIndex;

        /// <summary>
        /// Indicates if there are more characters to be read.
        /// </summary>
        public override bool IsAtEnd => BufferEndIndex.HasValue && CurrentCharacterIndex >= BufferEndIndex;

        /// <summary>
        /// The current token.
        /// </summary>
        public override string CurrentToken
        {
            get
            {
                if (_currentToken == null)
                {
                    //var segment = new char[CurrentTokenLength];
                    //Array.Copy(Buffer, TokenStartIndex, segment, 0, CurrentTokenLength);
                    var sub = new ArraySegment<char>(Buffer, TokenStartIndex, CurrentTokenLength).ToArray();
                    _currentToken = new string(sub);
                }
                return _currentToken;
            }
        }

        /// <summary>
        /// Creates a new <see cref="FileSource"/>
        /// </summary>
        /// <param name="reader">The <see cref="StreamReader"/> that provides the data for the <see cref="FileSource"/>.</param>
        public FileSource(StreamReader reader) : base()
        {
            Reader = reader ?? throw new ArgumentNullException(nameof(reader));

            // Read the first bit of information into the buffer so it can be worked with.
            // No need to call ReadNextIntoBuffer - that method does too much and accounts
            // for things that don't need to be accounted for in this case.
            Reader.ReadBlock(Buffer, 0, BufferLength);

            // If the total contents of the base stream fits into the buffer, then
            // initialize the buffer end index to the end of the content.
            if (reader.BaseStream.Length < BufferLength)
            {
                // Account for zero-indexing
                BufferEndIndex = (int?)reader.BaseStream.Length - 1;
            }
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
            Reader.Dispose();
        }

        /// <summary>
        /// Reads the next amount of characters into <see cref="Buffer"/>.
        /// </summary>
        private void ReadNextIntoBuffer()
        {
            // First check that the token is not too long.
            if (CurrentTokenLength >= BufferLength)
            {
                // The current token is too long. The maximum token length is {BufferLength}.
                throw new TokenTooLongException(CurrentToken, $"Il token corrente è troppo lungo. La lunghezza massima per un token è {BufferLength}.");
            }

            // Because the tokens will be moved around in the buffer,
            // the current token needs to be reset.
            _currentToken = null;

            // First, move the current token to the beginning of the
            // buffer.
            for (int i = TokenStartIndex, bufferFront = 0; 
                i < CurrentCharacterIndex; 
                i++, bufferFront++)
            {
                Buffer[bufferFront] = Buffer[i];
            }

            // For debugging
            var oldTokenLength = CurrentTokenLength;

            // The character at CurrentCharacterIndex is not included
            // in the token. 
            CurrentCharacterIndex = CurrentTokenLength;
            TokenStartIndex = 0;

            // For debugging
            // The token lengths should be the same after reading.
            if (oldTokenLength != CurrentTokenLength)
            {
                // Token lengths are not equal. Before:{oldTokenLength} After:{CurrentTokenLength}
                throw new Exception($"Le lunghezze dei token non sono uguali. Primo:{oldTokenLength} Dopo:{CurrentTokenLength}");
            }

            var charactersToRead = BufferLength - CurrentTokenLength;

            var charactersRead = Reader.ReadBlock(Buffer, CurrentCharacterIndex, charactersToRead);
            
            // If all characters read fit in the buffer, then set the end of the buffer.
            if (charactersRead <= charactersToRead)
            {
                BufferEndIndex = CurrentTokenLength + charactersRead;
            }
        }

        /// <summary>
        /// Reads the next amount of characters into <see cref="Buffer"/> if necessary.
        /// </summary>
        private void ReadNextIntoBufferIfNecessary()
        {
            if (CurrentCharacterIndex >= BufferLength)
            {
                ReadNextIntoBuffer();
            }
        }

        /// <summary>
        /// Consumes one character.
        /// </summary>
        /// <param name="consumed">The consumed character.</param>
        /// <returns>True if the <see cref="FileSource"/> was successfully advanced, false otherwise.</returns>
        public override bool Advance(out char consumed)
        {
            consumed = default;

            // Every time the source is advanced, the current token needs to be changed.
            _currentToken = null;

            ReadNextIntoBufferIfNecessary();

            if (IsAtEnd)
            {
                return false;
            }

            consumed = Buffer[CurrentCharacterIndex];
            CurrentCharacterIndex++;
            return true;
        }

        /// <summary>
        /// Gets the current character without consuming it.
        /// </summary>
        /// <param name="current">The current character.</param>
        /// <returns>True if the current character was successfully read, false otherwise.</returns>
        public override bool Peek(out char current)
        {
            current = default;

            ReadNextIntoBufferIfNecessary();

            if (IsAtEnd)
            {
                return false;
            }

            current = Buffer[CurrentCharacterIndex];
            return true;
        }

        /// <summary>
        /// Gets the character after the current character without consuming the current character or the next character.
        /// </summary>
        /// <param name="next">The character after the current character.</param>
        /// <returns>True if the character after the current character was successfully read, false otherwise.</returns>
        public override bool PeekNext(out char next)
        {
            next = default;

            if (IsAtEnd)
            {
                return false;
            }
            
            var indexToPeek = CurrentCharacterIndex + 1;
            if (indexToPeek >= BufferLength)
            {
                ReadNextIntoBuffer();

                // We need to check again that there is a character to read.
                // Just because we read the next amount of data into the 
                // buffer doesn't necessarily mean that there's data to read
                if (IsAtEnd)
                {
                    return false;
                }

                // Get the new index to peek because it's updated
                // by ReadNextIntoBuffer.
                indexToPeek = CurrentCharacterIndex + 1;
            }

            next = Buffer[indexToPeek];
            return true;            
        }

        /// <summary>
        /// Consumes the current character if it matches <paramref name="c"/>.
        /// </summary>
        /// <param name="c">The character to match.</param>
        /// <param name="consumed">The consumed character.</param>
        /// <returns>True if the current character matched <paramref name="c"/> and it was consumed, false otherwise.</returns>
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

            // This is safe because we already checked that we're not at the end
            Advance(out consumed);
            return true;
        }
    }
}
