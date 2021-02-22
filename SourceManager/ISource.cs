using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceManager
{
    /// <summary>
    /// Represents a source.
    /// </summary>
    public interface ISource : IDisposable
    {
        /// <summary>
        /// Indicates if there are more characters to be read.
        /// </summary>
        bool IsAtEnd { get; }

        /// <summary>
        /// The current token.
        /// </summary>
        string CurrentToken { get; }

        /// <summary>
        /// Clears the current token and prepares for reading the next token.
        /// </summary>
        void ClearToken();

        /// <summary>
        /// Consumes one character.
        /// </summary>
        /// <param name="consumed">The consumed character.</param>
        /// <returns>True if the <see cref="ISource"/> was successfully advanced, false otherwise.</returns>
        bool Advance(out char consumed);

        /// <summary>
        /// Gets the current character without consuming it.
        /// </summary>
        /// <param name="current">The current character.</param>
        /// <returns>True if the current character was successfully read, false otherwise.</returns>
        bool Peek(out char current);

        /// <summary>
        /// Gets the character after the current character without consuming the current character or the next character.
        /// </summary>
        /// <param name="next">The character after the current character.</param>
        /// <returns>True if the character after the current character was successfully read, false otherwise.</returns>
        bool PeekNext(out char next);

        /// <summary>
        /// Consumes the current character if it matches <paramref name="c"/>.
        /// </summary>
        /// <param name="c">The character to match.</param>
        /// <param name="consumed">The consumed character.</param>
        /// <returns>True if the current character matched <paramref name="c"/> and it was consumed, false otherwise.</returns>
        bool AdvanceIfMatches(char c, out char consumed);
    }
}
