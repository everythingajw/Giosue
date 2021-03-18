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
                        if (left is double leftDouble && right is double rightDouble)
                        {
                            return leftDouble - rightDouble;
                        }
                        if (left is int leftInt && right is int rightInt)
                        {
                            return leftInt - rightInt;
                        }
                        throw new MismatchedTypeException(left.GetType(), new() { typeof(int), typeof(double) }, right.GetType(), new() { typeof(int), typeof(double) }, $"The operands for addition must be {nameof(Int32)} or {nameof(Double)}.");
                    }
                case TokenType.Slash:
                    {
                        if (left is double l && right is double r)
                        {
                            return l / r;
                        }
                        break;
                    }
                case TokenType.Star:
                    {
                        if (left is double leftDouble && right is double rightDouble)
                        {
                            return leftDouble * rightDouble;
                        }
                        if (left is int leftInt && right is int rightInt)
                        {
                            return leftInt * rightInt;
                        }
                        break;
                    }
                case TokenType.Greater:
                    {
                        if (left is double ld && right is double rd)
                        {
                            return ld > rd;
                        }
                        if (left is int li && right is int ri)
                        {
                            return li > ri;
                        }
                        break;
                    }
                case TokenType.GreaterEqual:
                    {
                        if (left is double ld && right is double rd)
                        {
                            return ld >= rd;
                        }
                        if (left is int li && right is int ri)
                        {
                            return li >= ri;
                        }
                        break;
                    }
                case TokenType.Less:
                    {
                        if (left is double ld && right is double rd)
                        {
                            return ld < rd;
                        }
                        if (left is int li && right is int ri)
                        {
                            return li < ri;
                        }
                        break;
                    }
                case TokenType.LessEqual:
                    {
                        if (left is double ld && right is double rd)
                        {
                            return ld <= rd;
                        }
                        if (left is int li && right is int ri)
                        {
                            return li <= ri;
                        }
                        break;
                    }
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
