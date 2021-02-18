﻿using System;
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
                default: throw new Exception($"Unexpected character at line {Line}.");
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

        private void String()
        {
            var foundClosingQuote = false;
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
    }
}
