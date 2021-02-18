using System;
using System.Collections.Generic;
using System.Text;

namespace Giosue
{
    class Scanner
    {
        private string Source { get; }
        private List<Token> Tokens { get; } = new();

        public Scanner(string source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source), "The source for a scanner cannot be null.");
        }
    }
}
