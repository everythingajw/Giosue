﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Giosue.AST;
using Giosue.Exceptions;

namespace Giosue
{
    class Interpreter : IVisitor<object>
    {
        public object Interpret(Expression expression)
        {
            return EvaluateExpression(expression);
        }

        private string Stringify(object obj)
        {
            if (obj == null)
            {
                return "niente";
            }

            return obj?.ToString() ?? "niente";
        }

        private object EvaluateExpression(Expression expression)
        {
            return expression.Accept(this);
        }

        private bool IsTruthy(object expression)
        {
            if (expression is bool b)
            {
                return b;
            }

            // TODO: Everything else except true and false should not be a bool.
            return true;
        }

        private bool AreEqual(object a, object b)
        {
            if (a == null)
            {
                if (b == null)
                {
                    return true;
                }
                return false;
            }

            return a.Equals(b);
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

        //private static int Add(int l, int r) => l + r;
        //private static double Add(double l, double r) => l + r;

        //private static int Subtract(int l, int r) => l - r;
        //private static double Subtract(double l, double r) => l - r;

        //private static int Multiply(int l, int r) => l * r;
        //private static double Multiply(double l, double r) => l * r;

        //private static double Divide(int l, int r) => Divide((double)l, (double)r);
        //private static double Divide(double l, double r) => l / r;

        public object VisitAssignExpression(Assign expression)
        {
            throw new NotImplementedException();
        }

        private enum NumericType
        {
            None,
            Integer,
            Double
        }

        public object VisitBinaryExpression(Binary expression)
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
                case TokenType.Equal: return AreEqual(left, right);
                default: break;
            }

            var numericType = NumericType.None;
            int leftInt = 0;
            int rightInt = 0;
            double leftDouble = 0;
            double rightDouble = 0;

            if (left is int i1)
            {
                if (right is int i2)
                {
                    numericType = NumericType.Integer;
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
                    numericType = NumericType.Double;
                    leftDouble = d1;
                    rightDouble = d2;
                }
                else
                {
                    throw new MismatchedTypeException(typeof(double), right.GetType());
                }
            }
            else
            {
                throw new MismatchedTypeException(new List<Type>() { typeof(int), typeof(double) }, left.GetType());
            }

            return (expression.Operator.Type, numericType) switch
            {
                (_, NumericType.None) => throw new NotImplementedException(),
                (var t, NumericType.Integer) => t switch
                {
                    TokenType.Plus => leftInt + rightInt,
                    TokenType.Minus => leftInt - rightInt,
                    TokenType.Star => leftInt * rightInt,
                    TokenType.Slash => (double)((double)leftInt / (double)rightInt),
                    _ => throw new NotImplementedException()
                },
                (var t, NumericType.Double) => t switch
                {
                    TokenType.Plus => leftDouble + rightDouble,
                    TokenType.Minus => leftDouble - rightDouble,
                    TokenType.Star => leftDouble * rightDouble,
                    TokenType.Slash => (double)((double)leftDouble / (double)rightDouble),
                    _ => throw new NotImplementedException()
                },
                _ => throw new NotImplementedException()
            };
            
            // return null;
        }

        public object VisitCallExpression(Call expression)
        {
            throw new NotImplementedException();
        }

        public object VisitGetExpression(Get expression)
        {
            throw new NotImplementedException();
        }

        public object VisitGroupingExpression(Grouping expression)
        {
            return EvaluateExpression(expression.Expression);
        }

        public object VisitLiteralExpression(Literal expression)
        {
            return expression.Value;
        }

        public object VisitLogicalExpression(Logical expression)
        {
            throw new NotImplementedException();
        }

        public object VisitSetExpression(Set expression)
        {
            throw new NotImplementedException();
        }

        public object VisitSuperExpression(Super expression)
        {
            throw new NotImplementedException();
        }

        public object VisitThisExpression(This expression)
        {
            throw new NotImplementedException();
        }

        public object VisitUnaryExpression(Unary expression)
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

        public object VisitVariableExpression(Variable expression)
        {
            throw new NotImplementedException();
        }
    }
}
