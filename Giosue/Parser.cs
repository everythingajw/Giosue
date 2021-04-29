using System;
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
        /// Signals that a synchronization must take place.
        /// </summary>
        private class SynchronizeSignal : Exception
        {
            public SynchronizeSignal() : base() { }
            public SynchronizeSignal(string message) : base(message) { }
        }

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
        public bool TryParse(out List<Statement> statements, out ParserException exception)
        {
            // TODO: Wrap this in a try block to prevent any possible errors.

            var success = true;
            statements = new List<Statement>();
            exception = default;

            // No tokens means nothing to do.
            if (Tokens.Count <= 0)
            {
                return success;
            }
            // If there's only one EOF token, there's also nothing to do.
            if (Tokens.Count == 1 && Tokens[0].Type == TokenType.EOF)
            {
                return success;
            }
            while (!IsAtEnd)
            {
                if (!TryDeclaration(out var parsedStatement))
                {
                    return false;
                }
                statements.Add(parsedStatement);
            }
            return true;
        }

        private List<Statement> Block()
        {
            var statements = new List<Statement>();

            while (!CurrentTokenTypeEquals(TokenType.RightBrace))
            {
                TryDeclaration(out var statement);
                statements.Add(statement);
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

        private Statement MentreStatement()
        {
            // A '(' was expected after 'mentre'.
            AdvanceIfMatchesOrCrashIfNotMatches(TokenType.LeftParenthesis, "Un '(' in atteso dopo 'mentre'.", out _);

            var condition = Expression();

            // A ')' was expected after the condition.
            AdvanceIfMatchesOrCrashIfNotMatches(TokenType.RightParenthesis, "Un ')' in atteso dopo il condizione.", out _);

            var body = Statement();

            return new Statements.While(condition, body);
        }

        private bool TryDeclaration(out Statement statement)
        {
            statement = default;
            try
            {
                if (AdvanceIfMatches(out var consumed, TokenType.Fun))
                {
                    statement = FunctionDeclaration();
                }
                else if (AdvanceIfMatches(out consumed, TokenType.Var))
                {
                    statement = VariableDeclaration();
                }
                else
                {
                    statement = Statement();
                }
                return true;
            }
            catch (SynchronizeSignal s)
            {
                Synchronize();
                Console.Error.WriteLine(s.Message);
                return false;
            }
        }

        private Statement VariableDeclaration()
        {
            AdvanceIfMatchesOrCrashIfNotMatches(TokenType.Identifier, "Expected variable name.", out var variableNameToken);

            AST.Expression initializer = null;
            if (AdvanceIfMatches(out _, TokenType.Equal))
            {
                initializer = Expression();
            }

            AdvanceIfMatchesOrCrashIfNotMatches(TokenType.Semicolon, "Expected ';' after variable declaration.", out _);
            return new Statements.Var(variableNameToken, initializer);
        }

        private Statement Statement()
        {
            if (AdvanceIfMatches(out _, TokenType.Se))
            {
                return IfStatement();
            }
            if (AdvanceIfMatches(out _, TokenType.Mentre))
            {
                return MentreStatement();
            }
            if (AdvanceIfMatches(out _, TokenType.LeftBrace))
            {
                return new Statements.Block(Block());
            }
            return ExpressionStatement();
        }

        private Statement ExpressionStatement()
        {
            var expression = Expression();
            AdvanceIfMatchesOrCrashIfNotMatches(TokenType.Semicolon, "Expected ';' after expression.", out _);
            return new Statements.Expression(expression);
        }

        /// <summary>
        /// Parse a function definition.
        /// </summary>
        /// <returns></returns>
        private Statements.Function FunctionDeclaration()
        {
            // A name of a function was expected.
            AdvanceIfMatchesOrCrashIfNotMatches(TokenType.Identifier, "Un nome di una funzione in atteso.", out var name);

            // A '(' was expected before the arguments of a function.
            AdvanceIfMatchesOrCrashIfNotMatches(TokenType.LeftParenthesis, "Un '(' in atteso primo di gli argomenti per una funzione.", out _);

            var arguments = new List<Token>();
            if (!CurrentTokenTypeEquals(TokenType.RightParenthesis))
            {
                do
                {
                    AdvanceIfMatchesOrCrashIfNotMatches(TokenType.Identifier, "Un identificatore in atteso per un parametro", out var parameterName);
                    arguments.Add(parameterName);
                } while (AdvanceIfMatches(out _, TokenType.Comma));
            }

            AdvanceIfMatchesOrCrashIfNotMatches(TokenType.RightParenthesis, "Un ')' in atteso dopo gli argomenti per una funzione.", out _);
            AdvanceIfMatchesOrCrashIfNotMatches(TokenType.LeftBrace, "Un '{' in atteso primo di il corpo per una funzione.", out _);
            var body = Block();
            return new Statements.Function(name, arguments, body);
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
            return BinaryExpression(Term, TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Expression Term()
        {
            return BinaryExpression(Factor, TokenType.Minus, TokenType.Plus, TokenType.At);
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

            return Call();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callee"></param>
        /// <returns></returns>
        private Expression FinishCall(Expression callee)
        {
            var arguments = new List<Expression>();
            if (!CurrentTokenTypeEquals(TokenType.RightParenthesis))
            {
                do
                {
                    arguments.Add(Expression());
                } while (AdvanceIfMatches(out _, TokenType.Comma));
            }

            AdvanceIfMatchesOrCrashIfNotMatches(TokenType.RightParenthesis, "Un ')' in atteso dopo gli argomenti per una funzione.", out var consumed);
            return new AST.Call(callee, consumed, arguments);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Expression Call()
        {
            var expression = Primary();

            while (AdvanceIfMatches(out _, TokenType.LeftParenthesis))
            {
                expression = FinishCall(expression);
            }

            return expression;
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

            if (AdvanceIfMatches(out var consumed, TokenType.Integer, TokenType.Float, TokenType.String))
            {
                return new Literal(consumed.Literal);
            }

            if (AdvanceIfMatches(out _, TokenType.LeftParenthesis))
            {
                var expression = Expression();
                AdvanceIfMatchesOrCrashIfNotMatches(TokenType.RightParenthesis, "Expected ')' after expression.", out _);
                return new Grouping(expression);
            }

            if (AdvanceIfMatches(out _, TokenType.Identifier))
            {
                if (PreviousToken(out var previous))
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
            //throw ParseException(current, "Una espressione era previsto.");
            return null;
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
            return new ParserException(ParserExceptionType.Unknown, token, message);

            //return new SynchronizeSignal(message);
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
                        case TokenType.Var:
                        case TokenType.Fun:
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
