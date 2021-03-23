using System;
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

        public object VisitAssignExpression(Assign expression)
        {
            throw new NotImplementedException();
        }

        public object VisitBinaryExpression(Binary expression)
        {
            var left = EvaluateExpression(expression.Left);
            var right = EvaluateExpression(expression.Right);

            // Oh boy. This needs help.
            switch (expression.Operator.Type)
            {
                case TokenType.Minus:
                    {
                        if (left is double leftDouble)
                        {
                            if (right is double rightDouble)
                            {
                                return leftDouble - rightDouble;
                            }
                            throw new MismatchedTypeException(typeof(double), right.GetType());
                        }
                        if (left is int leftInt)
                        {
                            if (right is int rightInt)
                            {
                                return leftInt - rightInt;
                            }
                            throw new MismatchedTypeException(typeof(int), right.GetType());
                        }
                        throw new MismatchedTypeException(new List<Type>() { typeof(int), typeof(double) }, left.GetType());
                    }
                case TokenType.Slash:
                    {
                        if (left is double)
                        {
                            if (right is double)
                            {
                                return (double)((double)left / (double)right);
                            }
                            throw new MismatchedTypeException(typeof(double), right.GetType());
                        }
                        if (left is int)
                        {
                            if (right is int)
                            {
                                return (double)((double)left / (double)right);
                            }
                            throw new MismatchedTypeException(typeof(double), right.GetType());
                        }
                        
                        throw new MismatchedTypeException(new List<Type>() { typeof(int), typeof(double) }, left.GetType());
                    }
                case TokenType.Star:
                    {
                        if (left is double leftDouble)
                        {
                            if (right is double rightDouble)
                            {
                                return leftDouble * rightDouble;
                            }
                            throw new MismatchedTypeException(typeof(double), right.GetType());
                        }
                        if (left is int leftInt)
                        {
                            if (right is int rightInt)
                            {
                                return leftInt * rightInt;
                            }
                            throw new MismatchedTypeException(typeof(int), right.GetType());
                        }
                        throw new MismatchedTypeException(new List<Type>() { typeof(int), typeof(double) }, left.GetType());
                    }
                case TokenType.Greater:
                    return CompareNumbers(left, right) > 0;
                case TokenType.GreaterEqual:
                    return CompareNumbers(left, right) >= 0;
                case TokenType.Less:
                    return CompareNumbers(left, right) < 0;
                case TokenType.LessEqual:
                    return CompareNumbers(left, right) <= 0;
                case TokenType.BangEqual:
                    return !AreEqual(left, right);
                case TokenType.Equal:
                    return AreEqual(left, right);
            }

            return null;
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
                TokenType.Minus => -(double)right,
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
