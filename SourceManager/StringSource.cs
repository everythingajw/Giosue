using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceManager
{
    public class StringSource : ISource
    {
        private string Source { get; }

        private int TokenStartIndex { get; }

        private int CurrentCharacterIndex { get; }

        /// <inheritdoc/>
        public bool IsAtEnd => throw new NotImplementedException();

        /// <inheritdoc/>
        public string CurrentToken => throw new NotImplementedException();

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
