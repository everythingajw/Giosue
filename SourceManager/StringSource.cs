using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceManager
{
    public class StringSource : ISource
    {
        private string Source { get; } = null;

        private int TokenStartIndex { get; set; } = 0;

        private int CurrentCharacterIndex { get; set; } = 0;

        /// <inheritdoc/>
        public bool IsAtEnd => CurrentCharacterIndex >= Source.Length;

        /// <summary>
        /// Backing field for <see cref="CurrentToken"/>.
        /// </summary>
        private string _currentToken = null;

        /// <inheritdoc/>
        public string CurrentToken 
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
        public StringSource(string source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source), $"The source for a {nameof(StringSource)} cannot be null.");
        }

        /// <inheritdoc/>
        public bool Advance(out char consumed)
        {
            consumed = default;

            if (IsAtEnd)
            {
                return false;
            }

            consumed = Source[CurrentCharacterIndex++];
            return true;
        }

        /// <inheritdoc/>
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

            // This is safe because we already checked if 
            // we reached the end of the source.
            Advance(out consumed);
            
            return true;
        }

        /// <inheritdoc/>
        public void ClearToken()
        {
            TokenStartIndex = CurrentCharacterIndex + 1;
            CurrentCharacterIndex = TokenStartIndex;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            // TODO: What should be cleaned up here?
        }

        /// <inheritdoc/>
        public bool Peek(out char current)
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
        public bool PeekNext(out char next)
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
