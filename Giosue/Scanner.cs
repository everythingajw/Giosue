using System;
using System.Collections.Generic;
using System.Text;

namespace Giosue
{
    /// <summary>
    /// Represents a scanner that converts source code to a <see cref="List{T}"/> of <see cref="Tokens"/>.
    /// </summary>
    public class Scanner
    {
        /// <summary>
        /// A dictionary of <see cref="string"/> to <see cref="TokenType"/>.
        /// </summary>
        private static readonly Dictionary<string, TokenType> Keywords = new()
        {
            { "oppure", TokenType.Oppure },
            { "falso", TokenType.Falso },
            { "se", TokenType.Se },
            { "niente", TokenType.Niente },
            { "vero", TokenType.Vero },
            { "mentre", TokenType.Mentre },
        };

        /// <summary>
        /// The source code for the scanner.
        /// </summary>
        private string Source { get; }

        /// <summary>
        /// The list of tokens produced after scanning.
        /// </summary>
        private List<Token> Tokens { get; } = new();

        /// <summary>
        /// The index of the first character of the current token in <see cref="Source"/>.
        /// </summary>
        private int Start { get; set; } = 0;

        /// <summary>
        /// The index of the current character of the current token in <see cref="Source"/>.
        /// </summary>
        private int Current { get; set; } = 0;

        /// <summary>
        /// The line that the current character is on.
        /// </summary>
        private int Line { get; set; } = 1;
        
        /// <summary>
        /// Indicates if the scanner has reached the end of <see cref="Source"/>.
        /// </summary>
        /// <remarks>
        /// True if the scanner has reached the end of <see cref="Source"/>, false otherwise.
        /// </remarks>
        private bool IsAtEnd => Current >= Source.Length;
        
        /// <summary>
        /// Creates a new <see cref="Scanner"/>.
        /// </summary>
        /// <param name="source">The source code for the <see cref="Scanner"/>.</param>
        public Scanner(string source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source), "The source for a scanner cannot be null.");
        }

        /// <summary>
        /// Scans all the tokens in the source.
        /// </summary>
        /// <returns>The list of scanned tokens.</returns>
        public List<Token> ScanTokens()
        {
            while (!IsAtEnd)
            {
                Start = Current;
                ScanToken();
            }

            Tokens.Add(new Token(TokenType.EOF, "", null, Line));
            return Tokens;
        }

        /// <summary>
        /// Scans one token.
        /// </summary>
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

        /// <summary>
        /// Advances the scanner one character forward, consuming one character.
        /// </summary>
        /// <returns>The consumed character.</returns>
        private char Advance()
        {
            Current++;
            return Source[Current - 1];
        }

        /// <summary>
        /// Adds a token to <see cref="Tokens"/>.
        /// </summary>
        /// <param name="type">The type of the token to add.</param>
        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        /// <summary>
        /// Adds a token to <see cref="Tokens"/>.
        /// </summary>
        /// <param name="type">The type of the token to add.</param>
        /// <param name="literal">The token's literal.</param>
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

        /// <summary>
        /// Gets the current character without consuming it.
        /// </summary>
        /// <returns>The current character or <c>'\0'</c> if <see cref="Source"/> is empty.</returns>
        private char Peek()
        {
            // TODO: Should this method return '\0' or null when at the end?
            return IsAtEnd ? '\0' : Source[Current];
        }

        /// <summary>
        /// Gets the next character without consuming it.
        /// </summary>
        /// <returns>The next character or <c>'\0'</c> if <see cref="Source"/> is empty.</returns>
        private char PeekNext()
        {
            // TODO: Should this method return '\0' or null when at the end?
            return Current + 1 >= Source.Length ? '\0' : Source[Current + 1];
        }

        /// <summary>
        /// Scans a string.
        /// </summary>
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
        /// Tests if a character is an ASCII digit. 
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
        
        /// <summary>
        /// Tests if <paramref name="c"/> is an ASCII letter.
        /// </summary>
        /// <remarks>
        /// We use this to avoid accepting letters from other writing systems.
        /// </remarks>
        /// <param name="c">A character.</param>
        /// <returns>True if <paramref name="c"/> is an ASCII letter, false otherwise.</returns>
        private static bool IsAsciiLetter(char c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
        }

        /// <summary>
        /// Tests if <paramref name="c"/> is an ASCII letter or an underscore.
        /// </summary>
        /// <param name="c">A character.</param>
        /// <returns>True if <paramref name="c"/> is an ASCII letter or an underscore, false otherwise.</returns>
        /// <seealso cref="IsAsciiLetter(char)"/>
        private static bool IsAsciiLetterOrUnderscore(char c)
        {
            return (c >= 'A' && c <= 'Z') || 
                   (c >= 'a' && c <= 'z') || 
                   (c == '_');
        }

        /// <summary>
        /// Tests if <paramref name="c"/> is an ASCII alphanumeric character.
        /// </summary>
        /// <param name="c">A character.</param>
        /// <returns>True if <paramref name="c"/> is an ASCII alphanumeric character, false otherwise.</returns>
        /// <seealso cref="IsAsciiLetter(char)"/>
        /// <seealso cref="IsAsciiDigit(char)"/>
        private static bool IsAsciiAlphanumeric(char c)
        {
            return IsAsciiLetter(c) || IsAsciiDigit(c);
        }

        /// <summary>
        /// Tests if <paramref name="c"/> is an ASCII alphanumeric character or an underscore.
        /// </summary>
        /// <param name="c">A character.</param>
        /// <returns>True if <paramref name="c"/> is an ASCII alphanumeric character or an underscore, false otherwise.</returns>
        /// <seealso cref="IsAsciiAlphanumeric(char)"/>
        /// <seealso cref="IsAsciiLetterOrUnderscore(char)"/>
        private static bool IsAsciiAlphanumericOrUnderscore(char c)
        {
            return IsAsciiAlphanumeric(c) || IsAsciiLetterOrUnderscore(c);
        }

        /// <summary>
        /// Scans a number (integer or float).
        /// </summary>
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

        /// <summary>
        /// Scans an identifier (user-defined or keyword).
        /// </summary>
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
