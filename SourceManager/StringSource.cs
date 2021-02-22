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

        private int TokenStartIndex { get; } = 0;

        private int CurrentCharacterIndex { get; } = 0;

        /// <inheritdoc/>
        public bool IsAtEnd => CurrentCharacterIndex >= Source.Length;

        /// <summary>
        /// Backing field for 
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
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool AdvanceIfMatches(char c, out char consumed)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void ClearToken()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool Peek(out char current)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool PeekNext(out char next)
        {
            throw new NotImplementedException();
        }
    }
}
