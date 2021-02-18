using System;
using System.Collections.Generic;
using System.Text;

namespace Giosue
{
    class Scanner
    {
        private static readonly Dictionary<string, TokenType> Keywords = new()
        {
            { "oppure", TokenType.Oppure },
            { "falso", TokenType.Falso },
            { "se", TokenType.Se },
            { "niente", TokenType.Niente },
            { "vero", TokenType.Vero },
            { "mentre", TokenType.Mentre },
        };

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
                // Ignore whitespace 
                case ' ':
                case '\r':
                case '\t':
                    break;
                case '\n':
                    Line++;
                    break;
                case '(': AddToken(TokenType.LeftParenthesis); break;
                case ')': AddToken(TokenType.RightParenthesis); break;
                case '{': AddToken(TokenType.LeftBrace); break;
                case '}': AddToken(TokenType.RightBrace); break;
                case ',': AddToken(TokenType.Comma); break;
                case '.': AddToken(TokenType.Dot); break;
                case '+': AddToken(TokenType.Plus); break;
                case ';': AddToken(TokenType.Semicolon); break;
                case '*': AddToken(TokenType.Star); break;
                case '-':
                    if (Match('-'))
                    {
                        // Completely ignore comments
                        while (Peek() != '\n')
                        {
                            Advance();
                        }
                    }
                    else
                    {
                        AddToken(TokenType.Minus);
                    }
                    break;
                case '!':
                    AddToken(Match('=') ? TokenType.BangEqual : TokenType.Bang);
                    break;
                case '=':
                    AddToken(Match('=') ? TokenType.EqualEqual : TokenType.Equal);
                    break;
                case '<':
                    AddToken(Match('=') ? TokenType.LessEqual : TokenType.Less);
                    break;
                case '>':
                    AddToken(Match('=') ? TokenType.GreaterEqual : TokenType.Greater);
                    break;
                default:
                    if (IsAsciiDigit(ch))
                    {
                        Number();
                    }
                    else if (IsAsciiLetter(ch) || ch == '_')
                    {
                        Identifier();
                    }

                    throw new Exception($"Unexpected character at line {Line}.");
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

        /// <summary>
        /// Consume the current character only if it matches <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The character to match</param>
        /// <returns>True if <paramref name="expected"/> matches the current character, false otherwise.</returns>
        private bool Match(char expected)
        {
            if (IsAtEnd)
            {
                return false;
            }
            if (Source[Current] != expected)
            {
                return false;
            }
            Current++;
            return true;
        }

        private char Peek()
        {
            // TODO: Should this method return '\0' or null when at the end?
            return IsAtEnd ? '\0' : Source[Current];
        }

        private char PeekNext()
        {
            // TODO: Should this method return '\0' or null when at the end?
            return Current + 1 >= Source.Length ? '\0' : Source[Current + 1];
        }

        private void String()
        {
            while (Peek() != '"' && !IsAtEnd)
            {
                if (Peek() == '\n')
                {
                    throw new Exception("Multi-line strings are not supported.");
                }
                Advance();
            }
            if (IsAtEnd)
            {
                throw new Exception($"Unterminated string at line {Line}.");
            }
            
            // The closing ".
            Advance();

            // Trim off the opening and closing quotes
            var value = Source[(Start + 1)..(Current - 1)];
            AddToken(TokenType.String, value);
        }

        /// <summary>
        /// Tests if a character is an ASCII character. 
        /// </summary>
        /// <remarks>
        /// We use this to avoid accepting digits from other writing systems.
        /// </remarks>
        /// <param name="c">A character</param>
        /// <returns>True if the character is an ASCII digit, false otherwise.</returns>
        private static bool IsAsciiDigit(char c)
        {
            // Visual Studio thinks that casts are redundant here.
            return c >= '0' && c <= '9';
        }

        private static bool IsAsciiLetter(char c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
        }

        private static bool IsAsciiLetterOrUnderscore(char c)
        {
            return (c >= 'A' && c <= 'Z') || 
                   (c >= 'a' && c <= 'z') || 
                   (c == '_');
        }

        private static bool IsAsciiAlphanumericOrUnderscore(char c)
        {
            return IsAsciiLetterOrUnderscore(c) || IsAsciiDigit(c);
        }

        private void Number()
        {
            while (IsAsciiDigit(Peek()))
            {
                Advance();
            }

            var hasFractionalPart = Peek() == '.';
            if (hasFractionalPart && IsAsciiDigit(PeekNext()))
            {
                // Consume the '.'
                Advance();

                while (IsAsciiDigit(Peek()))
                {
                    Advance();
                }
            }

            var lexeme = Source[Start..Current];
            if (hasFractionalPart)
            {
                AddToken(TokenType.Float, double.Parse(lexeme));
            }
            else
            {
                AddToken(TokenType.Integer, int.Parse(lexeme));
            }
        }

        private void Identifier()
        {
            while (IsAsciiAlphanumericOrUnderscore(Peek()))
            {
                Advance();
            }

            var lexeme = Source[Start..Current];
            if (Keywords.TryGetValue(lexeme, out var type))
            {
                AddToken(type);
            }
        }
    }
}
