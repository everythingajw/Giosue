using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SourceManager
{
    /// <summary>
    /// Represents a source for data.
    /// </summary>
    public class FileSource : ISource
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
        private int TokenStartIndex { get; set; } = 0;

        /// <summary>
        /// The index of the current character.
        /// </summary>
        private int CurrentCharacterIndex { get; set; } = 0;

        /// <summary>
        /// The index of the end of <see cref="Buffer"/>
        /// </summary>
        private int? BufferEndIndex { get; set; } = null;

        // Do not add 1.
        // Since the character at CurrentCharacterIndex has not been consumed,
        // it's not added to the current token and is therefore not counted
        // towards the current token's length.
        /// <summary>
        /// The length of the current token.
        /// </summary>
        private int CurrentTokenLength => CurrentCharacterIndex - TokenStartIndex;

        // The buffer is empty if the end index is the same as the index of the current character.
        /// <summary>
        /// Indicates if there are more characters to be read.
        /// </summary>
        public bool IsAtEnd => BufferEndIndex == CurrentCharacterIndex;

        /// <summary>
        /// Backing field for <see cref="CurrentToken"/>.
        /// </summary>
        private string _currentToken;

        /// <summary>
        /// The current token.
        /// </summary>
        public string CurrentToken
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
        public FileSource(StreamReader reader)
        {
            Reader = reader ?? throw new ArgumentNullException(nameof(reader));

            // Read the first bit of information into the buffer so it can be worked with.
            // No need to call ReadNextIntoBuffer - that method does too much and accounts
            // for things that don't need to be accounted for in this case.
            Reader.ReadBlock(Buffer, 0, BufferLength);

            // If the total contents of the base stream fits into the buffer, then
            // initialize the buffer end index to the end of the content.
            Console.WriteLine(reader.BaseStream.Length);
            if (reader.BaseStream.Length < BufferLength)
            {
                // Account for zero-indexing
                BufferEndIndex = (int?)reader.BaseStream.Length - 1;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Reader.Dispose();
        }

        /// <summary>
        /// Clears the current token and prepares for reading the next token.
        /// </summary>
        public void ClearToken()
        {
            _currentToken = null;
            TokenStartIndex = CurrentCharacterIndex;
            CurrentCharacterIndex = TokenStartIndex;
        }

        /// <summary>
        /// Reads the next amount of characters into <see cref="Buffer"/>.
        /// </summary>
        private void ReadNextIntoBuffer()
        {
            Console.WriteLine("Read next into buffer");
            Console.WriteLine($"tsi: {TokenStartIndex}");
            Console.WriteLine($"cci: {CurrentCharacterIndex}");
            var oldTokenLength = CurrentTokenLength;

            File.Delete(@"D:\tmp\d\beforemove_dump.txt");
            File.WriteAllText(@"D:\tmp\d\beforemove_dump.txt", string.Join('\n', Buffer));

            // First, we need to move the current token to the beginning.
            //for (int bufferFront = 0, currentTokenIndex = TokenStartIndex; 
            //     currentTokenIndex < CurrentCharacterIndex; 
            //     bufferFront++, currentTokenIndex++)
            //{
            //    Buffer[bufferFront] = Buffer[currentTokenIndex];
            //    Console.WriteLine($"move {currentTokenIndex} -> {bufferFront}");
            //}

            var tempArray = new char[CurrentTokenLength];
            Array.Copy(Buffer, TokenStartIndex, tempArray, 0, CurrentTokenLength);
            Array.Copy(tempArray, 0, Buffer, 0, tempArray.Length);

            // Update the indexes accordingly
            TokenStartIndex = 0;
            CurrentCharacterIndex = oldTokenLength - 1;
            Console.WriteLine($"tsi: {TokenStartIndex}");
            Console.WriteLine($"cci: {CurrentCharacterIndex}");

            File.Delete(@"D:\tmp\d\aftermove_dump.txt");
            File.WriteAllText(@"D:\tmp\d\aftermove_dump.txt", string.Join('\n', Buffer));

            // Now that we've copied the current token, read the next bit
            // of data into the buffer.
            var charactersToRead = BufferLength - CurrentCharacterIndex;
            var charactersRead = Reader.ReadBlock(Buffer, CurrentCharacterIndex + 1, 10);

            for (int i = CurrentTokenLength + charactersRead; i < BufferLength; i++)
            {
                Buffer[i] = '\0';
            }

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
        public bool Advance(out char consumed)
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
        public bool Peek(out char current)
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

        /// <summary>
        /// Consumes the current character if it matches <paramref name="c"/>.
        /// </summary>
        /// <param name="c">The character to match.</param>
        /// <param name="consumed">The consumed character.</param>
        /// <returns>True if the current character matched <paramref name="c"/> and it was consumed, false otherwise.</returns>
        public bool AdvanceIfMatches(char c, out char consumed)
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
