using System;
using System.Collections.Generic;
using System.Text;

namespace Giosue
{
    class Scanner
    {
        private string Source { get; }
        private List<Token> Tokens { get; } = new();
        private int Start { get; set; } = 0;
        private int Current { get; set; } = 0;
        private int Line { get; set; } = 1;
        private bool IsAtEnd => Current >= Source.Length;

        public Scanner(string source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source), "The source for a scanner cannot be null.");
        }

        List<Token> ScanTokens()
        {
            while (!IsAtEnd)
            {
                Start = Current;
                ScanToken();
            }

            Tokens.Add(new Token(TokenType.EOF, "", null, Line));
            return Tokens;
        }

        private void ScanToken()
        {
            var ch = Advance();

            switch (ch)
            {
                case '(': AddToken(TokenType.LeftParenthesis); break;
                case ')': AddToken(TokenType.RightParenthesis); break;
                case '{': AddToken(TokenType.LeftBrace); break;
                case '}': AddToken(TokenType.RightBrace); break;
                case ',': AddToken(TokenType.Comma); break;
                case '.': AddToken(TokenType.Dot); break;
                case '-': AddToken(TokenType.Minus); break;
                case '+': AddToken(TokenType.Plus); break;
                case ';': AddToken(TokenType.Semicolon); break;
                case '*': AddToken(TokenType.Star); break;
            }
        }

        private char Advance()
        {
            Current++;
            return Source[Current - 1];
        }

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object literal)
        {
            var lexeme = Source[Start..Current];
            Tokens.Add(new Token(type, lexeme, literal, Line));
        }
    }
}
