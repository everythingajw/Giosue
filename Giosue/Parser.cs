using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Giosue.AST;
using Giosue.Exceptions;

namespace Giosue
{
    // TODO: Documentation
    /// <summary>
    /// 
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// The <see cref="Token"/>s to be parsed.
        /// </summary>
        private List<Token> Tokens { get; }

        /// <summary>
        /// The index into <see cref="Tokens"/> of the current token.
        /// </summary>
        private int CurrentTokenIndex = 0;

        // The index check isn't in the book, but I think it's a really good idea to have it here.
        /// <summary>
        /// Tests if there are any more tokens to parse.
        /// </summary>
        private bool IsAtEnd => (CurrentTokenIndex >= Tokens.Count) || (Tokens[CurrentTokenIndex].Type == TokenType.EOF);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokens"></param>
        public Parser(List<Token> tokens)
        {
            Tokens = tokens ?? throw new ArgumentNullException(nameof(tokens), $"The source tokens for a {nameof(Parser)} cannot be null");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Expression Parse()
        {
            try
            {
                return Expression();
            }
            catch (Exception e)
            {
                var thisMethod = MethodBase.GetCurrentMethod();
                Console.Error.WriteLine($"{thisMethod.DeclaringType.FullName}.{thisMethod.Name} :: {e.GetType()} occurred during parsing");
                Console.Error.WriteLine(e);
                Console.Error.WriteLine();
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Expression Expression()
        {
            return Equality();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="tokens"></param>
        /// <returns></returns>
        private Expression BinaryExpression(Func<Expression> callback, params TokenType[] tokens)
        {
            var expression = callback();

            while (AdvanceIfMatches(out _, tokens))
            {
                if (PreviousToken(out var @operator))
                {
                    expression = new Binary(expression, @operator, callback());
                }
                else
                {
                    // No previous token was available.
                    break;
                }
            }

            return expression;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Expression Equality()
        {
            return BinaryExpression(Comparison, TokenType.BangEqual, TokenType.EqualEqual);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Expression Comparison()
        {
            return BinaryExpression(Term, TokenType.GreaterEqual, TokenType.LessEqual, TokenType.LessEqual);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Expression Term()
        {
            return BinaryExpression(Factor, TokenType.Minus, TokenType.Plus);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Expression Factor()
        {
            return BinaryExpression(Bitwise, TokenType.Slash, TokenType.Star);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Expression Bitwise()
        {
            return BinaryExpression(Unary, TokenType.And, TokenType.Pipe, TokenType.Caret);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Expression Unary()
        {
            if (AdvanceIfMatches(out _, TokenType.Bang, TokenType.Minus))
            {
                if (PreviousToken(out var @operator))
                {
                    return new Unary(@operator, Unary());
                }
            }

            return Primary();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Expression Primary()
        {
            if (AdvanceIfMatches(out _, TokenType.Falso))
            {
                return new Literal(false);
            }
            if (AdvanceIfMatches(out _, TokenType.Vero))
            {
                return new Literal(true);
            }
            if (AdvanceIfMatches(out _, TokenType.Niente))
            {
                return new Literal(null);
            }

            if (AdvanceIfMatches(out _, TokenType.Integer, TokenType.Float, TokenType.String))
            {
                if (PreviousToken(out var previous))
                {
                    return new Literal(previous.Literal);
                }

                // TODO: What should happen here? Exception?
                return null;
            }

            if (AdvanceIfMatches(out _, TokenType.LeftParenthesis))
            {
                var expression = Expression();
                AdvanceIfMatchesOrCrashIfNotMatches(TokenType.RightParenthesis, "Expected ')' after expression.", out _);
                return new Grouping(expression);
            }

            if (!Peek(out var current))
            {
                current = null;
            }
            throw ParseException(current, "Expected expression.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="message"></param>
        /// <param name="consumed"></param>
        /// <returns></returns>
        private bool AdvanceIfMatchesOrCrashIfNotMatches(TokenType tokenType, string message, out Token consumed)
        {
            consumed = default;

            if (CurrentTokenTypeEquals(tokenType))
            {
                return Advance(out consumed);
            }

            if (!Peek(out var current))
            {
                current = null;
            }

            throw ParseException(current, message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private static Exception ParseException(Token token, string message)
        {
            // The book handles exceptions differently than I do.\
            // TODO: Figure this out
            return new ParserException(ParserExceptionType.Unknown, token, "Invalid token");
        }

        /// <summary>
        /// Synchronize the <see cref="Parser"/>'s state by clearing out the call frames on the stack.
        /// </summary>
        private void Synchronize()
        {
            Advance(out _);

            while (!IsAtEnd)
            {
                if (PreviousToken(out var previous))
                {
                    if (previous.Type == TokenType.Semicolon)
                    {
                        return;
                    }
                }

                if (Peek(out var current))
                {
                    switch (current.Type)
                    {
                        case TokenType.Se:
                        case TokenType.Mentre:
                            return;
                    }
                }

                Advance(out _);
            }
        }

        /// <summary>
        /// Advance the <see cref="Parser"/> if the type of the current token equals one of <paramref name="tokenTypes"/>.
        /// </summary>
        /// <param name="tokenTypes">The token types to check.</param>
        /// <returns>True if the type of the current token equals one of <paramref name="tokenTypes"/> and the <see cref="Parser"/> was successfully advanced.</returns>
        private bool AdvanceIfMatches(out Token consumed, params TokenType[] tokenTypes)
        {
            consumed = default;

            foreach (var t in tokenTypes)
            {
                if (CurrentTokenTypeEquals(t))
                {
                    return Advance(out consumed);
                }
            }

            // No match, not advanced.
            return false;
        }

        /// <summary>
        /// Check if the current <see cref="Token"/>'s <see cref="Token.Type"/> equals <paramref name="t"/>
        /// </summary>
        /// <param name="t">The type of the token to check.</param>
        /// <returns>True if the current <see cref="Token"/>'s <see cref="Token.Type"/> equals <paramref name="t"/>, false otherwise.</returns>
        private bool CurrentTokenTypeEquals(TokenType t)
        {
            if (Peek(out var current))
            {
                return current.Type == t;
            }

            // Peek failed for whatever reason. No match.
            return false;
        }

        /// <summary>
        /// Consume the current <see cref="Token"/>.
        /// </summary>
        /// <param name="consumed">The consumed token.</param>
        /// <returns>True if the <see cref="Parser"/> was successfully advanced, false otherwise.</returns>
        private bool Advance(out Token consumed)
        {
            consumed = default;

            if (IsAtEnd)
            {
                return false;
            }

            CurrentTokenIndex++;
            if (PreviousToken(out var previous))
            {
                consumed = previous;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the current <see cref="Token"/> (not yet consumed).
        /// </summary>
        /// <param name="current">The current token.</param>
        /// <returns>True if there is a current token, false otherwise.</returns>
        private bool Peek(out Token current)
        {
            current = default;

            if (IsAtEnd)
            {
                return false;
            }

            current = Tokens[CurrentTokenIndex];
            return true;
        }

        /// <summary>
        /// Gets the most recently consumed <see cref="Token"/>.
        /// </summary>
        /// <param name="last">The most recently consumed <see cref="Token"/></param>
        /// <returns>True if there is a most recently consumed <see cref="Token"/>, false otherwise.</returns>
        private bool PreviousToken(out Token last)
        {
            last = default;

            // If the current token index is before the second token in the list,
            // there's nothing to see.
            if (CurrentTokenIndex < 1)
            {
                return false;
            }

            last = Tokens[CurrentTokenIndex - 1];
            return true;
        }
    }
}
