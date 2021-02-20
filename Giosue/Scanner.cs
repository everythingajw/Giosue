using System;
using System.Collections.Generic;
using System.Text;
using Giosue.Extensions;

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

        public IReadOnlyList<Token> GetTokens()
        {
            return Tokens.AsReadOnly();
        }

        /// <summary>
        /// Scans one token.
        /// </summary>
        private void ScanToken()
        {
            var ch = Advance();

            switch (ch)
            {
                // Whitespace and newlines
                case ' ':
                case '\r':
                case '\t':
                    break;
                case '\n':
                    Line++;
                    break;
                
                // Grouping operators
                case '(': AddToken(TokenType.LeftParenthesis); break;
                case ')': AddToken(TokenType.RightParenthesis); break;
                case '{': AddToken(TokenType.LeftBrace); break;
                case '}': AddToken(TokenType.RightBrace); break;
                
                // Other operators
                case ',': AddToken(TokenType.Comma); break;
                case '.': AddToken(TokenType.Dot); break;
                case ';': AddToken(TokenType.Semicolon); break;
                
                // Math operators
                case '+': AddToken(TokenType.Plus); break;
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
                
                // Comparison operators
                case '!': AddToken(Match('=') ? TokenType.BangEqual : TokenType.Bang); break;
                case '=': AddToken(Match('=') ? TokenType.EqualEqual : TokenType.Equal); break;
                case '<': AddToken(Match('=') ? TokenType.LessEqual : TokenType.Less); break;
                case '>': AddToken(Match('=') ? TokenType.GreaterEqual : TokenType.Greater); break;
                
                // Boolean operators
                case '&': AddToken(Match('&') ? TokenType.AndAnd : TokenType.And); break;
                case '|': AddToken(Match('|') ? TokenType.PipePipe : TokenType.Pipe); break;
                case '^': AddToken(Match('^') ? TokenType.Caret : TokenType.CaretCaret); break;
                
                // Strings and numbers
                case '"': String(); break;
                case var c when c.IsAsciiDigit(): Number(); break;
                case var c when c.IsAsciiAlphanumericOrUnderscore(): Identifier(); break;
                
                // Catch all
                default:
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
        /// <returns>The current character or <c>null</c> if <see cref="Source"/> is empty.</returns>
        private char? Peek()
        {
            return IsAtEnd ? null : Source[Current];
        }

        /// <summary>
        /// Gets the next character without consuming it.
        /// </summary>
        /// <returns>The next character or <c>null</c> if there are not enough characters in <see cref="Source"/>.</returns>
        private char? PeekNext()
        {
            return Current + 1 >= Source.Length ? null : Source[Current + 1];
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
        /// Scans a number (integer or float).
        /// </summary>
        private void Number()
        {
            while (Peek().IsAsciiDigit())
            {
                Advance();
            }

            var hasFractionalPart = Peek() == '.';
            if (hasFractionalPart && PeekNext().IsAsciiDigit())
            {
                // Consume the '.'
                Advance();

                while (Peek().IsAsciiDigit())
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
            while (Peek().IsAsciiAlphanumericOrUnderscore())
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
