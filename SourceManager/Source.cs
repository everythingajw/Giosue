using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceManager
{
    // TODO: IDisposable
    // All sources should be disposable to free resources.
    // This will also avoid using the using block for some
    // code and not for other code.
    /// <summary>
    /// Represents a source.
    /// </summary>
    public abstract class Source
    {
        /// <summary>
        /// The starting index of the current token.
        /// </summary>
        protected virtual int TokenStartIndex { get; set; } = 0;

        /// <summary>
        /// The index of the current character.
        /// </summary>
        protected virtual int CurrentCharacterIndex { get; set; } = 0;

        /// <summary>
        /// Indicates if there are more characters to be read.
        /// </summary>
        public abstract bool IsAtEnd { get; }

        /// <summary>
        /// Backing field for <see cref="CurrentToken"/>.
        /// </summary>
        protected string _currentToken;

        /// <summary>
        /// The current token.
        /// </summary>
        public abstract string CurrentToken { get; }

        protected Source()
        {
            TokenStartIndex = 0;
            CurrentCharacterIndex = 0;
            _currentToken = null;
        }

        /// <summary>
        /// Clears the current token and prepares for reading the next token.
        /// </summary>
        public virtual void ClearToken()
        {
            _currentToken = null;
            TokenStartIndex = CurrentCharacterIndex;
            CurrentCharacterIndex = TokenStartIndex;
        }

        /// <summary>
        /// Consumes one character.
        /// </summary>
        /// <param name="consumed">The consumed character.</param>
        /// <returns>True if the <see cref="Source"/> was successfully advanced, false otherwise.</returns>
        public abstract bool Advance(out char consumed);

        /// <summary>
        /// Gets the current character without consuming it.
        /// </summary>
        /// <param name="current">The current character.</param>
        /// <returns>True if the current character was successfully read, false otherwise.</returns>
        public abstract bool Peek(out char current);

        /// <summary>
        /// Gets the character after the current character without consuming the current character or the next character.
        /// </summary>
        /// <param name="next">The character after the current character.</param>
        /// <returns>True if the character after the current character was successfully read, false otherwise.</returns>
        public abstract bool PeekNext(out char next);

        /// <summary>
        /// Consumes the current character if it matches <paramref name="c"/>.
        /// </summary>
        /// <param name="c">The character to match.</param>
        /// <param name="consumed">The consumed character.</param>
        /// <returns>True if the current character matched <paramref name="c"/> and it was consumed, false otherwise.</returns>
        public abstract bool AdvanceIfMatches(char c, out char consumed);
    }
}
