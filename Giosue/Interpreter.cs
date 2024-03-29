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
using System.Threading.Tasks;
using Giosue.Exceptions;
using Giosue.Builtins;
using Giosue.Builtins.Casts;
using Giosue.Builtins.ForeignFunctionInterface;

using CastToString = Giosue.Builtins.Casts.ToString;

namespace Giosue
{
    public class Interpreter : AST.IVisitor<object>, Statements.IVisitor<object>
    {
        internal readonly Environment Globals = new();
        public Environment Environment;

        public Interpreter(Environment oldEnvironment = null)
        {
            // If no old environment is given, add globals.
            if (oldEnvironment == null)
            {
                Environment = Globals;
                Globals.DefineOrOverwrite(TimeMillis.Name, new TimeMillis());
                Globals.DefineOrOverwrite(Print.Name, new Print());
                Globals.DefineOrOverwrite(PrintLine.Name, new PrintLine());
                Globals.DefineOrOverwrite(GetTypeOf.Name, new GetTypeOf());

                Globals.DefineOrOverwrite(CastToString.Name, new CastToString());
                Globals.DefineOrOverwrite(ToBool.Name, new ToBool());
                Globals.DefineOrOverwrite(ToInt.Name, new ToInt()); 
                Globals.DefineOrOverwrite(ToDouble.Name, new ToDouble());
            }
            else
            {
                Environment = new Environment(oldEnvironment);
            }
        }

        #region Interpreting and evaluating

        public object Interpret(AST.Expression expression)
        {
            return EvaluateExpression(expression);
        }

        public void Interpret(List<Statements.Statement> statements)
        {
            foreach (var statement in statements)
            {
                ExecuteStatement(statement);
            }
        }

        private void ExecuteStatement(Statements.Statement statement)
        {
            if (statement == null)
            {
                throw new InterpreterException(InterpreterExceptionType.Unknown, "The given statement is null.");
            }
            statement.Accept(this);
        }

        private object EvaluateExpression(AST.Expression expression)
        {
            if (expression == null)
            {
                throw new InterpreterException(InterpreterExceptionType.Unknown, "The given expression is null.");
            }
            return expression.Accept(this);
        }

        public void ExecuteBlock(List<Statements.Statement> statements, Environment environment)
        {
            var previousEnvironment = Environment;

            try
            {
                Environment = environment;
                statements.ForEach(ExecuteStatement);
            }
            finally
            {
                Environment = previousEnvironment;
            }
        }

        #endregion Interpreting and evaluating

        private static string Stringify(object obj)
        {
            return obj?.ToString() ?? "niente";
        }

        #region Comparisons and equality

        private static bool IsTruthy(object expression)
        {
            if (expression is bool b)
            {
                return b;
            }

            // TODO: Everything else except true and false should not be a bool.
            return true;
        }

        private static bool AreEqual(object a, object b)
        {
            if (a == null)
            {
                if (b == null)
                {
                    return true;
                }
                return false;
            }

            return (a.GetType() == b.GetType()) && a.Equals(b);
        }

        private static int? CompareObjects(object left, object right)
        {
            // By the rules of IComparable.CompareTo, 0 means two objects are equal
            if (left == null && right == null)
            {
                return 0;
            }
            if (left is IComparable l && right is IComparable r)
            {
                return l.CompareTo(r);
            }
            return null;
        }

        private static int CompareNumbers(object left, object right)
        {
            if (left is int li)
            {
                if (right is int ri)
                {
                    // This will have a value because int is IComparable
                    return CompareObjects(li, ri).Value;
                }
                throw new MismatchedTypeException(typeof(int), right.GetType());
            }
            if (left is double ld)
            {
                if (right is double rd)
                {
                    // This will have a value because double is IComparable
                    return CompareObjects(ld, rd).Value;
                }
                throw new MismatchedTypeException(typeof(double), right.GetType());
            }

            throw new MismatchedTypeException(new List<Type>() { typeof(int), typeof(double) }, left.GetType());
        }

        #endregion Comparisons and equality

        private static void ThrowIfNotNumbers(object left, object right)
        {
            switch (left, right)
            {
                case (int _, int _):
                case (double _, double _):
                    break;
                case (int _, _):
                    throw new MismatchedTypeException(typeof(int), right.GetType());
                case (double _, _):
                    throw new MismatchedTypeException(typeof(double), right.GetType());
                default:
                    throw new MismatchedTypeException(new List<Type>() { typeof(int), typeof(double) }, left.GetType());
            }
        }

        private enum BinaryOperandType
        {
            None,
            Integer,
            Double,
            String
        }

        #region AST visitors

        object AST.IVisitor<object>.VisitAssignExpression(AST.Assign expression)
        {
            var value = EvaluateExpression(expression.Value);
            Environment.AssignIfExists(expression.Name.Lexeme, value);
            return null;
        }

        object AST.IVisitor<object>.VisitBinaryExpression(AST.Binary expression)
        {
            var left = EvaluateExpression(expression.Left);
            var right = EvaluateExpression(expression.Right);

            switch (expression.Operator.Type)
            {
                case TokenType.Greater: return CompareNumbers(left, right) > 0;
                case TokenType.GreaterEqual: return CompareNumbers(left, right) >= 0;
                case TokenType.Less: return CompareNumbers(left, right) < 0;
                case TokenType.LessEqual: return CompareNumbers(left, right) <= 0;
                case TokenType.BangEqual: return !AreEqual(left, right);
                case TokenType.EqualEqual: return AreEqual(left, right);
                default: break;
            }

            var numericType = BinaryOperandType.None;
            int leftInt = default;
            int rightInt = default;
            double leftDouble = default;
            double rightDouble = default;
            string leftString = null;
            string rightString = null;

            if (left is int i1)
            {
                if (right is int i2)
                {
                    numericType = BinaryOperandType.Integer;
                    leftInt = i1;
                    rightInt = i2;
                }
                else
                {
                    throw new MismatchedTypeException(typeof(int), right.GetType());
                }
            }
            else if (left is double d1)
            {
                if (right is double d2)
                {
                    numericType = BinaryOperandType.Double;
                    leftDouble = d1;
                    rightDouble = d2;
                }
                else
                {
                    throw new MismatchedTypeException(typeof(double), right.GetType());
                }
            }
            else if (left is string s1)
            {
                if (right is string s2)
                {
                    numericType = BinaryOperandType.String;
                    leftString = s1;
                    rightString = s2;
                }
                else
                {
                    throw new MismatchedTypeException(typeof(string), right.GetType());
                }
            }

            else
            {
                throw new MismatchedTypeException(new List<Type>() { typeof(int), typeof(double), typeof(string) }, left.GetType());
            }


            //! WARNING !//
            //! DO NOT USE A SWITCH EXPRESSION HERE !//
            // Using a switch expression instead of a switch statement causes integer
            // addition to result in floating point numbers.
            switch (expression.Operator.Type, numericType)
            {
                case (_, BinaryOperandType.None): throw new NotImplementedException();
                case (var t, BinaryOperandType.Integer):
                    switch (t)
                    {
                        case TokenType.Plus: return leftInt + rightInt;
                        case TokenType.Minus: return leftInt - rightInt;
                        case TokenType.Star: return leftInt * rightInt;
                        case TokenType.Slash: return (double)((double)leftInt / (double)rightInt);

                        case TokenType.And: return leftInt & rightInt;
                        case TokenType.Pipe: return leftInt | rightInt;
                        case TokenType.Caret: return leftInt ^ rightInt;
                        default: throw new NotImplementedException();
                    }
                case (var t, BinaryOperandType.Double):
                    switch (t)
                    {
                        case TokenType.Plus: return leftDouble + rightDouble;
                        case TokenType.Minus: return leftDouble - rightDouble;
                        case TokenType.Star: return leftDouble * rightDouble;
                        case TokenType.Slash: return (double)((double)leftDouble / (double)rightDouble);

                        case TokenType.And:
                        case TokenType.Pipe:
                        case TokenType.Caret:
                            throw new InterpreterException(InterpreterExceptionType.MismatchedTypes, $"Solamente {nameof(Int32)} può essere usato con i operatori bitwise");
                        default: throw new NotImplementedException();
                    }
                case (var t, BinaryOperandType.String):
                    switch (t)
                    {
                        case TokenType.At: return leftString + rightString;
                        default: throw new InterpreterException(InterpreterExceptionType.MismatchedTypes, $"Non è possibile usare quello operatore con le stringhe");
                    }
                default:
                    throw new NotImplementedException();
            }

            // return null;
        }

        object AST.IVisitor<object>.VisitCallExpression(AST.Call expression)
        {
            var callee = EvaluateExpression(expression.Callee);

            var arguments = expression.Arguments.Select(EvaluateExpression).ToList();

            if (callee is IGiosueCallable callable)
            {
                if (callable.Arity != arguments.Count)
                {
                    // It's impossible to that number of parameters with that function.
                    throw new InterpreterException(InterpreterExceptionType.WrongNumberOfArgumentsPassedToFunction, $"È vietato usare quello numero di parametri con quello funzione.");
                }

                return callable.Call(this, arguments);
            }
            // It's impossible to use that object as a function.
            throw new InterpreterException(InterpreterExceptionType.AttemptToCallNonCallableObject, "Non è possible usare quello oggeto come una funzione.");
        }

        object AST.IVisitor<object>.VisitGetExpression(AST.Get expression)
        {
            throw new NotImplementedException();
        }

        object AST.IVisitor<object>.VisitGroupingExpression(AST.Grouping expression)
        {
            return EvaluateExpression(expression.Expression);
        }

        object AST.IVisitor<object>.VisitLiteralExpression(AST.Literal expression)
        {
            return expression.Value;
        }

        object AST.IVisitor<object>.VisitLogicalExpression(AST.Logical expression)
        {
            object left = EvaluateExpression(expression.Left);
            object right = EvaluateExpression(expression.Right);
            
            switch (expression.Operator.Type)
            {
                case TokenType.Greater: return CompareNumbers(left, right) > 0;
                case TokenType.GreaterEqual: return CompareNumbers(left, right) >= 0;
                case TokenType.Less: return CompareNumbers(left, right) < 0;
                case TokenType.LessEqual: return CompareNumbers(left, right) <= 0;
                case TokenType.BangEqual: return !AreEqual(left, right);
                case TokenType.Equal: return AreEqual(left, right);
                default: break;
            }

            if (left is bool l)
            {
                if (right is bool r)
                {
                    return expression.Operator.Type switch
                    {
                        TokenType.AndAnd => l && r,
                        TokenType.PipePipe => l || r,

                        // xor is the same as !=
                        TokenType.CaretCaret => l != r,

                        _ => throw new NotImplementedException()
                    };
                }
                throw new MismatchedTypeException(typeof(bool), right.GetType());
            }
            throw new MismatchedTypeException(typeof(bool), left.GetType());
        }

        object AST.IVisitor<object>.VisitSetExpression(AST.Set expression)
        {
            throw new NotImplementedException();
        }

        object AST.IVisitor<object>.VisitSuperExpression(AST.Super expression)
        {
            throw new NotImplementedException();
        }

        object AST.IVisitor<object>.VisitThisExpression(AST.This expression)
        {
            throw new NotImplementedException();
        }

        object AST.IVisitor<object>.VisitUnaryExpression(AST.Unary expression)
        {
            var right = EvaluateExpression(expression.Right);
            return expression.Operator.Type switch
            {
                TokenType.Minus => right switch
                {
                    double d => -d,
                    int i => -i,
                    _ => null,
                },
                TokenType.Bang => !IsTruthy(right),
                _ => null,
            };
        }

        object AST.IVisitor<object>.VisitVariableExpression(AST.Variable expression)
        {
            return Environment.GetValue(expression.Name.Lexeme);
        }

        #endregion AST visitors

        #region Statement visitors

        object Statements.IVisitor<object>.VisitExpressionStatement(Statements.Expression statement)
        {
            EvaluateExpression(statement.Expr);
            return null;
        }

        object Statements.IVisitor<object>.VisitVarStatement(Statements.Var statement)
        {
            var value = statement.Initializer == null ? null : EvaluateExpression(statement.Initializer);
            Environment.DefineOrOverwrite(statement.Name.Lexeme, value);
            return null;
        }

        object Statements.IVisitor<object>.VisitBlockStatement(Statements.Block statement)
        {
            ExecuteBlock(statement.Statements, new Environment(Environment));
            return null;
        }

        object Statements.IVisitor<object>.VisitIfStatement(Statements.If statement)
        {
            if (IsTruthy(EvaluateExpression(statement.Condition)))
            {
                ExecuteStatement(statement.ThenBranch);
            }
            else if (statement.ElseBranch != null)
            {
                ExecuteStatement(statement.ElseBranch);
            }
            return null;
        }

        object Statements.IVisitor<object>.VisitWhileStatement(Statements.While statement)
        {
            if (statement.Condition == null)
            {
                // A boolean expression is expected after mentre.
                throw new InterpreterException(InterpreterExceptionType.MentreWithoutCondition, "Un espressione booleana in atteso dopo mentre.");
            }

            while (IsTruthy(EvaluateExpression(statement.Condition)))
            {
                ExecuteStatement(statement.Body);
            }

            return null;
        }

        object Statements.IVisitor<object>.VisitFunctionStatement(Statements.Function statement)
        {
            var function = new GiosueFunction(statement, Environment);
            Environment.DefineOrOverwrite(statement.Name.Lexeme, function);
            return null;
        }

        #endregion Statement visitors
    }
}
