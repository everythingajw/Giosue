using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giosue
{
    public class Parser
    {
        private List<Token> Tokens { get; }
        private int CurrentTokenIndex = 0;

        public Parser(List<Token> tokens)
        {
            Tokens = tokens ?? throw new ArgumentNullException(nameof(tokens), $"The source tokens for a {nameof(Parser)} cannot be null");
        }
    }
}
