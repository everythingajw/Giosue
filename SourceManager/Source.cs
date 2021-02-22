using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SourceManager
{
    public class Source : IDisposable
    {
        private const int BufferLength = 10000;

        private StreamReader Reader { get; } = null;

        private char[] Buffer { get; } = new char[BufferLength];

        private int TokenStartIndex { get; set; } = 0;

        private int CurrentCharacterIndex { get; set; } = 0;

        private int? BufferEndIndex { get; set; } = null;

        private int CurrentTokenLength => CurrentCharacterIndex - TokenStartIndex;

        // The buffer is empty if the end index is the same as the index
        // of the current character.
        public bool IsAtEnd => BufferEndIndex == CurrentCharacterIndex;

        private string _currentToken;
        public string CurrentToken
        {
            get
            {
                if(_currentToken == null)
                {
                    var segment = new char[CurrentTokenLength];
                    Array.Copy(Buffer, TokenStartIndex, segment, 0, CurrentTokenLength);
                    _currentToken = new string(segment);
                }
                return _currentToken;
            }
        }

        public Source(StreamReader reader)
        {
            Reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public void Dispose()
        {
            Reader.Dispose();
        }

        public void ClearToken()
        {
            _currentToken = null;
            TokenStartIndex = CurrentCharacterIndex + 1;
            CurrentCharacterIndex = TokenStartIndex;
        }

        private void ReadNextIntoBuffer()
        {
            // First, we need to move the current token to the beginning.
            // Let's do this by copying the current token to a temporary array,
            // and replacing the content at the beginning of the buffer with 
            // the current token
            var currentToken = new char[CurrentTokenLength];
            
            // Copy the current token
            Array.Copy(Buffer, currentToken, CurrentTokenLength);
            
            // Copy the current token back to the beginning
            Array.Copy(currentToken, 0, Buffer, 0, CurrentTokenLength);

            // Update the indexes accordingly
            TokenStartIndex = 0;
            CurrentCharacterIndex = CurrentTokenLength;

            // Now that we've copied the current token, read the next bit
            // of data into the buffer.
            var charactersToRead = BufferLength - CurrentCharacterIndex;
            var charactersRead = Reader.ReadBlock(Buffer, CurrentCharacterIndex + 1, charactersToRead);

            // If the number of characters read does not equal the number
            // of characters required to fill the buffer, that's the end.
            if (charactersToRead < charactersRead)
            {
                // To find the index of the end of the buffer, we take the length
                // of the current token we copied initially (which is at the beginning
                // of the array) and the number of characters just read (which is 
                // directly after the current token) and add them together.
                BufferEndIndex = CurrentTokenLength + charactersToRead;
            }
        }

        private void ReadNextIntoBufferIfNecessary()
        {
            if (CurrentCharacterIndex >= BufferLength)
            {
                ReadNextIntoBuffer();
            }
        }

        public bool Advance(out char consumed)
        {
            ReadNextIntoBufferIfNecessary();
            
            consumed = default;

            if (IsAtEnd)
            {
                return false;
            }

            consumed = (char)Reader.Read();
            return true;
        }

        public bool Peek(out char current)
        {
            current = default;

            if (IsAtEnd)
            {
                return false;
            }

            current = (char)Reader.Peek();
            return true;
        }

        public bool PeekNext(out char next)
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

        public bool AdvanceIfMatches(char c, out char consumed)
        {
            consumed = default;

            if (IsAtEnd)
            {
                return false;
            }
            if (Peek(out var current) && current != c)
            {
                return false;
            }

            // This is safe because we already checked that we're not at the end
            Advance(out consumed);
            return true;
        }
    }
}
