﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Giosue.AST;
using Giosue.Exceptions;

using Statement = Giosue.Statements.Statement;

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
            // The source tokens for a {nameof(Parser)} cannot be null
            Tokens = tokens ?? throw new ArgumentNullException(nameof(tokens), $"È vietato creare un {nameof(Parser)} da una lista dei tokens nulla");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ParserException Parse(out List<Statement> statements)
        {
            statements = new List<Statement>();
            try
            {
                // No tokens means nothing to do.
                if (Tokens.Count <= 0)
                {
                    return null;
                }
                // If there's only one EOF token, there's also nothing to do.
                if (Tokens.Count == 1 && Tokens[0].Type == TokenType.EOF)
                {
                    return null;
                }
                while (!IsAtEnd)
                {
                    statements.Add(Statement());
                }
                return null;
            }
            catch (ParserException e)
            {
                return e;
            }
        }

        private List<Statement> Block()
        {
            var statements = new List<Statement>();

            while (!AdvanceIfMatches(out _, TokenType.RightBrace))
            {
                statements.Add(Declaration());
            }

            AdvanceIfMatchesOrCrashIfNotMatches(TokenType.RightBrace, "Expected '}' after block.", out _);
            return statements;
        }

        private Statement IfStatement()
        {
            AdvanceIfMatchesOrCrashIfNotMatches(TokenType.LeftParenthesis, "Expected '(' after 'if'.", out _);
            var condition = Expression();
            AdvanceIfMatchesOrCrashIfNotMatches(TokenType.RightParenthesis, "Expected ')' after 'if'.", out _);

            var thenBranch = Statement();
            Statement elseBranch = default;
            if (AdvanceIfMatches(out _, TokenType.Oppure))
            {
                elseBranch = Statement();
            }
            return new Statements.If(condition, thenBranch, elseBranch);
        }

        private Statement Declaration()
        {
            try
            {
                if (AdvanceIfMatches(out _, TokenType.Var))
                {
                    return VariableDeclaration();
                }
                return Statement();
            }
            catch (ParserException)
            {
                Synchronize();
                return null;
            }
        }

        private Statement VariableDeclaration()
        {
            AdvanceIfMatchesOrCrashIfNotMatches(TokenType.Identifier, "Expected variable name.", out var variableNameToken);

            AST.Expression initializer = null;
            if (AdvanceIfMatches(out _, TokenType.Integer))
            {
                initializer = Expression();
            }

            AdvanceIfMatchesOrCrashIfNotMatches(TokenType.Semicolon, "Expected ';' after variable declaration.", out _);
            return new Statements.Var(variableNameToken, initializer);
        }

        private Statement Statement()
        {
            return ExpressionStatement();
        }

        private Statement ExpressionStatement()
        {
            var expression = Expression();
            AdvanceIfMatchesOrCrashIfNotMatches(TokenType.Semicolon, "Expected ';' after expression.", out _);
            return new Statements.Expression(expression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Expression Expression()
        {
            return Assignment();
        }

        private Expression Assignment()
        {
            var expression = Equality();
            if (AdvanceIfMatches(out var token, TokenType.Equal))
            {
                var value = Assignment();

                if (expression is AST.Variable v)
                {
                    var name = v.Name;
                    return new AST.Assign(name, value);
                }

                // It's impossible to assign to that expression
                throw new ParserException(ParserExceptionType.Unknown, token, "È vietato assegnare a quella espressione.");
            }

            return expression;
        }

        #region Binary and logical expressions

        /// <summary>
        /// Parses a binary expression.
        /// </summary>
        /// <param name="callback">The function handling the higher precedence operators.</param>
        /// <param name="tokens">The tokens to be consumed.</param>
        /// <returns>An <see cref="AST.Expression"/> representing the expression.</returns>
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
        /// Parses a logical expression.
        /// </summary>
        /// <param name="callback">The function handling the higher precedence operators.</param>
        /// <param name="tokens">The tokens to be consumed.</param>
        /// <returns>An <see cref="AST.Expression"/> representing the expression.</returns>
        private Expression LogicalExpression(Func<Expression> callback, params TokenType[] tokens)
        {
            var expression = callback();

            while (AdvanceIfMatches(out _, tokens))
            {
                if (PreviousToken(out var @operator))
                {
                    expression = new Logical(expression, @operator, callback());
                }
                else
                {
                    // No previous token was available
                    break;
                }
            }

            return expression;
        }

        // TODO: Should this go under logical expression?
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Expression Equality()
        {
            return BinaryExpression(ExclusiveOr, TokenType.BangEqual, TokenType.EqualEqual);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Expression ExclusiveOr()
        {
            return LogicalExpression(Or, TokenType.CaretCaret);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Expression Or()
        {
            return LogicalExpression(And, TokenType.PipePipe);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Expression And()
        {
            return LogicalExpression(Comparison, TokenType.AndAnd);
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

            if (AdvanceIfMatches(out _, TokenType.Identifier))
            {
                if (!PreviousToken(out var previous))
                {
                    return new AST.Variable(previous);
                }
                
                // Could not get previous token for variable declaration.
                throw new ParserException(ParserExceptionType.Unknown, null, "Prendere il token prima per la creazione di una variabile era impossible");
            }

            if (!Peek(out var current))
            {
                current = null;
            }
            
            // Expected expression.
            throw ParseException(current, "Una espressione era previsto.");
        }

        #endregion Binary and logical expressions

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
                        case TokenType.Oppure:
                        case TokenType.Mentre:
                            return;
                    }
                }

                Advance(out _);
            }
        }

        #region Consuming and scanning tokens

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

        #endregion Consuming and scanning tokens
    }
}
