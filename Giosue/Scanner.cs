﻿// Giosue language interpreter
// The interpreter for the Giosue programming language.
// Copyright (C) 2021  Anthony Webster
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Giosue.Exceptions;
using Giosue.Extensions;
using SourceManager;

namespace Giosue
{
    /// <summary>
    /// Represents a scanner that converts source code to a <see cref="List{T}"/> of <see cref="Tokens"/>.
    /// </summary>
    public class Scanner
    {
        private const char StringTerminator = '"';

        /// <summary>
        /// A dictionary of <see cref="string"/> to <see cref="TokenType"/>.
        /// </summary>
        private static readonly Dictionary<string, TokenType> Keywords = new()
        {
            { "var", TokenType.Var },
            { "fun", TokenType.Fun },
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
            { '@', TokenType.At },
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
        /// The source code for the <see cref="Scanner"/>.
        /// </summary>
        private Source Source { get; }

        /// <summary>
        /// The list of tokens produced after scanning.
        /// </summary>
        private List<Token> Tokens { get; } = new();

        /// <summary>
        /// The line that the current character is on.
        /// </summary>
        private int Line { get; set; } = 1;

        /// <summary>
        /// Creates a new <see cref="Scanner"/>.
        /// </summary>
        /// <param name="source">The source code for the <see cref="Scanner"/>.</param>
        public Scanner(Source source)
        {
            // The source for a {nameof(Scanner)} cannot be null.
            Source = source ?? throw new ArgumentNullException(nameof(source), $"La fonte per un {nameof(Scanner)} non può essere nulla.");
        }

        /// <summary>
        /// Scans all the tokens in the source.
        /// </summary>
        /// <returns>The list of scanned tokens.</returns>
        public List<Token> ScanTokens()
        {
            while (!Source.IsAtEnd)
            {
                ScanToken();
                
                // We scanned one token. Clear out the token from the source
                // so the next token can be read.
                // Do this here - ScanToken should handle scanning the token and that's it.
                Source.ClearToken();
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
            if (!Source.Advance(out var ch))
            {
                // On failure to advance, don't bother clearing the token.
                // TODO: Should this be an exception?
                return;
            }

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
                    if (Source.AdvanceIfMatches('-', out _))
                    {
                        Comment();
                    }
                    else
                    {
                        AddToken(TokenType.Minus);
                    }
                    break;

                // Comparison operators
                case '!': AddToken(Source.AdvanceIfMatches('=', out _) ? TokenType.BangEqual : TokenType.Bang); break;
                case '=': AddToken(Source.AdvanceIfMatches('=', out _) ? TokenType.EqualEqual : TokenType.Equal); break;
                case '<': AddToken(Source.AdvanceIfMatches('=', out _) ? TokenType.LessEqual : TokenType.Less); break;
                case '>': AddToken(Source.AdvanceIfMatches('=', out _) ? TokenType.GreaterEqual : TokenType.Greater); break;

                // Boolean and bitwise operators
                case '&': AddToken(Source.AdvanceIfMatches('&', out _) ? TokenType.AndAnd : TokenType.And); break;
                case '|': AddToken(Source.AdvanceIfMatches('|', out _) ? TokenType.PipePipe : TokenType.Pipe); break;
                case '^': AddToken(Source.AdvanceIfMatches('^', out _) ? TokenType.CaretCaret : TokenType.Caret); break;

                // Strings, numbers, and identifiers
                case StringTerminator:
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
                    // Unexpected character '{ch}'
                    throw new ScannerException(ScannerExceptionType.UnexpectedCharacter, Line, $"Carattere inatteso '{ch}'");
            }
        }

        /// <summary>
        /// Adds a token to <see cref="Tokens"/>.
        /// </summary>
        /// <param name="type">The type of the token to add.</param>
        /// <param name="literal">The token's literal.</param>
        private void AddToken(TokenType type, object literal = null)
        {
            var lexeme = Source.CurrentToken;
            Tokens.Add(new Token(type, lexeme, literal, Line));
        }

        /// <summary>
        /// Scans a string.
        /// </summary>
        /// <returns>The scanned string.</returns>
        private string String()
        {
            if (Source.Peek(out var current))
            {
                while (current != StringTerminator && !Source.IsAtEnd)
                {
                    //if (Source.Peek(out current) && current == '\n')
                    if (Source.Peek(out current))
                    {
                        if (current == '\n')
                        {
                            // TODO: Turn this into a custom form of GiosueException down the line.
                            // For right now, this will work.
                            // Multi-line strings are not supported.
                            throw new NotSupportedException("È vietato creare una stringa su più di una fila.");
                        }
                    }
                    if (!Source.Advance(out _))
                    {
                        break;
                    }
                    Source.Peek(out current);
                }
            }

            if (Source.IsAtEnd)
            {
                // Unterminated string
                throw new ScannerException(ScannerExceptionType.UnterminatedString, Line, "Stringa senza fine.");
            }

            // The closing ".
            Source.Advance(out _);

            // Trim off the opening and closing quotes
            var lexeme = Source.CurrentToken;
            return lexeme[1..(lexeme.Length - 1)];
        }

        /// <summary>
        /// Scans a number (integer or float).
        /// </summary>
        /// <returns>A pair of the type of the number and the parsed number itself.</returns>
        private (TokenType, object) Number()
        {
            if (Source.Peek(out var current))
            {
                while (current.IsAsciiDigit())
                {
                    if (!Source.Advance(out _))
                    {
                        break;
                    }
                    Source.Peek(out current);
                }
            }

            var hasFractionalPart = false;
            if (Source.Peek(out current))
            {
                hasFractionalPart = current == '.';
            }

            var hasNext = Source.PeekNext(out var next);
            if (hasFractionalPart && hasNext && next.IsAsciiDigit())
            {
                // Consume the '.'
                Source.Advance(out _);

                if (Source.Peek(out current))
                {
                    while (current.IsAsciiDigit())
                    {
                        if (!Source.Advance(out _))
                        {
                            break;
                        }
                        Source.Peek(out current);
                    }
                }
            }

            var lexeme = Source.CurrentToken;
            var tokenType = TokenType.None;
            object parsedNumber = null;

            if (hasFractionalPart)
            {
                tokenType = TokenType.Float;
                if (!double.TryParse(lexeme, out var @double))
                {
                    // Could not parse '{lexeme}' to {nameof(Double)}
                    throw new ScannerException(ScannerExceptionType.MalformedNumericLiteral, Line, $"Non poteva trasformare '{lexeme}' a {nameof(Double)}");
                }
                parsedNumber = @double;
            }
            else
            {
                tokenType = TokenType.Integer;
                if (!int.TryParse(lexeme, out var @int))
                {
                    // Could not parse '{lexeme}' to {nameof(Int32)}
                    throw new ScannerException(ScannerExceptionType.MalformedNumericLiteral, Line, $"Non poteva trasformare '{lexeme}' a {nameof(Int32)}");
                }
                parsedNumber = @int;
            }

            return (tokenType, parsedNumber);
        }

        /// <summary>
        /// Scans an identifier (user-defined or keyword).
        /// </summary>
        /// <returns>The type of the identifier.</returns>
        private TokenType Identifier()
        {
            if (Source.Peek(out var current))
            {
                while (current.IsAsciiAlphanumericOrUnderscore())
                {
                    if (!Source.Advance(out _))
                    {
                        break;
                    }
                    Source.Peek(out current);
                }
            }

            var lexeme = Source.CurrentToken;
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
            if (Source.Peek(out var current))
            {
                while (current != '\n')
                {
                    if (!Source.Advance(out _))
                    {
                        break;
                    }
                    Source.Peek(out current);
                }
            }
        }
    }
}
