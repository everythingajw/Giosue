using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Giosue.Exceptions;
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

        private static readonly Dictionary<char, TokenType> SingleCharacterTokens = new()
        {
            { '(', TokenType.LeftParenthesis },
            { ')', TokenType.RightParenthesis },
            { '{', TokenType.LeftBrace },
            { '}', TokenType.RightBrace },
            { ',', TokenType.Comma },
            { '.', TokenType.Dot },
            { ';', TokenType.Semicolon },
            { '+', TokenType.Plus },
            { '*', TokenType.Star },
            { '/', TokenType.Slash },
        };

        // FUTURE
        // The logic for this is messy. I'll do this later.
        //private static readonly Dictionary<(char, char), (TokenType, TokenType)> DoubleCharacterTokens = new()
        //{
        //    { ('!', '='), (TokenType.Bang, TokenType.BangEqual) },
        //    { ('=', '='), (TokenType.Equal, TokenType.EqualEqual) },
        //    { ('<', '='), (TokenType.Less, TokenType.LessEqual) },
        //    { ('>', '='), (TokenType.Greater, TokenType.GreaterEqual) },
        //    { ('&', '&'), (TokenType.And, TokenType.AndAnd) },
        //    { ('|', '|'), (TokenType.Pipe, TokenType.PipePipe) },
        //    { ('^', '^'), (TokenType.Caret, TokenType.CaretCaret) },
        //};

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
        private int TokenStartIndex { get; set; } = 0;

        /// <summary>
        /// The index of the current character of the current token in <see cref="Source"/>.
        /// </summary>
        private int CurrentCharacterIndex { get; set; } = 0;

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
        private bool IsAtEnd => CurrentCharacterIndex >= Source.Length;
        
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
                TokenStartIndex = CurrentCharacterIndex;
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

            if (SingleCharacterTokens.TryGetValue(ch, out var type))
            {
                AddToken(type);
                return;
            }

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
                
                case '-': 
                    if (AdvanceIfMatches('-'))
                    {
                        Comment();
                    }
                    else
                    {
                        AddToken(TokenType.Minus);
                    }
                    break;
                
                // Comparison operators
                case '!': AddToken(AdvanceIfMatches('=') ? TokenType.BangEqual : TokenType.Bang); break;
                case '=': AddToken(AdvanceIfMatches('=') ? TokenType.EqualEqual : TokenType.Equal); break;
                case '<': AddToken(AdvanceIfMatches('=') ? TokenType.LessEqual : TokenType.Less); break;
                case '>': AddToken(AdvanceIfMatches('=') ? TokenType.GreaterEqual : TokenType.Greater); break;

                // Boolean and bitwise operators
                case '&': AddToken(AdvanceIfMatches('&') ? TokenType.AndAnd : TokenType.And); break;
                case '|': AddToken(AdvanceIfMatches('|') ? TokenType.PipePipe : TokenType.Pipe); break;
                case '^': AddToken(AdvanceIfMatches('^') ? TokenType.CaretCaret : TokenType.Caret); break;
                
                // Strings, numbers, and identifiers
                case '"':
                    {
                        var parsedString = String();
                        AddToken(TokenType.String, parsedString);
                        break;
                    }
                case var c when c.IsAsciiDigit():
                    {
                        var (numberType, parsedNumber) = Number();
                        AddToken(numberType, parsedNumber);
                        break;
                    }
                case var c when c.IsAsciiAlphanumericOrUnderscore():
                    { 
                        var tokenType = Identifier();
                        AddToken(tokenType);
                        break; 
                    }
                
                // Catch all
                default:
                    throw new UnexpectedCharacterException(Line, ch, "Unexpected character.");
            }
        }

        /// <summary>
        /// Advances the scanner one character forward, consuming one character.
        /// </summary>
        /// <returns>The consumed character.</returns>
        private char Advance()
        {
            CurrentCharacterIndex++;
            return Source[CurrentCharacterIndex - 1];
        }

        /// <summary>
        /// Adds a token to <see cref="Tokens"/>.
        /// </summary>
        /// <param name="type">The type of the token to add.</param>
        /// <param name="literal">The token's literal.</param>
        private void AddToken(TokenType type, object literal = null)
        {
            var lexeme = Source[TokenStartIndex..CurrentCharacterIndex];
            Tokens.Add(new Token(type, lexeme, literal, Line));
        }

        /// <summary>
        /// Consume the current character only if it matches <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The character to match</param>
        /// <returns>True if <paramref name="expected"/> matches the current character, false otherwise.</returns>
        private bool AdvanceIfMatches(char expected)
        {
            if (IsAtEnd)
            {
                return false;
            }
            if (Source[CurrentCharacterIndex] != expected)
            {
                return false;
            }
            CurrentCharacterIndex++;
            return true;
        }

        /// <summary>
        /// Gets the current character without consuming it.
        /// </summary>
        /// <returns>The current character or <c>null</c> if <see cref="Source"/> is empty.</returns>
        private char? Peek()
        {
            return IsAtEnd ? null : Source[CurrentCharacterIndex];
        }

        /// <summary>
        /// Gets the next character without consuming it.
        /// </summary>
        /// <returns>The next character or <c>null</c> if there are not enough characters in <see cref="Source"/>.</returns>
        private char? PeekNext()
        {
            return CurrentCharacterIndex + 1 >= Source.Length ? null : Source[CurrentCharacterIndex + 1];
        }

        /// <summary>
        /// Scans a string.
        /// </summary>
        /// <returns>The scanned string.</returns>
        private string String()
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
            return Source[(TokenStartIndex + 1)..(CurrentCharacterIndex - 1)];
        }

        /// <summary>
        /// Scans a number (integer or float).
        /// </summary>
        /// <returns>A pair of the type of the number and the parsed number itself.</returns>
        private (TokenType, object) Number()
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

            var lexeme = Source[TokenStartIndex..CurrentCharacterIndex];
            
            return hasFractionalPart ? (TokenType.Float, double.Parse(lexeme)) : (TokenType.Integer, int.Parse(lexeme));
        }

        /// <summary>
        /// Scans an identifier (user-defined or keyword).
        /// </summary>
        /// <returns>The type of the identifier.</returns>
        private TokenType Identifier()
        {
            while (Peek().IsAsciiAlphanumericOrUnderscore())
            {
                Advance();
            }

            var lexeme = Source[TokenStartIndex..CurrentCharacterIndex];
            var tokenType = TokenType.Identifier;
            if (Keywords.TryGetValue(lexeme, out var keywordType))
            {
                tokenType = keywordType;
            }
            return tokenType;
        }

        /// <summary>
        /// Scans a line comment.
        /// </summary>
        /// <remarks>
        /// Comments are completely ignored and are thrown away by the scanner.
        /// </remarks>
        private void Comment()
        {
            // Completely ignore comments.
            while (Peek() != '\n')
            {
                Advance();
            }
        }
    }
}
