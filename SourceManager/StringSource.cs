using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceManager
{
    public class StringSource : ISource
    {
        public bool IsAtEnd => throw new NotImplementedException();

        public string CurrentToken => throw new NotImplementedException();

        public bool Advance(out char consumed)
        {
            throw new NotImplementedException();
        }

        public bool AdvanceIfMatches(char c, out char consumed)
        {
            throw new NotImplementedException();
        }

        public void ClearToken()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool Peek(out char current)
        {
            throw new NotImplementedException();
        }

        public bool PeekNext(out char next)
        {
            throw new NotImplementedException();
        }
    }
}
