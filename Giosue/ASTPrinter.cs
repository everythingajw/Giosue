using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Giosue.AST;

namespace Giosue
{
    public class ASTPrinter : IVisitor<string>
    {
        public string StringifyExpression(Expression expression)
        {
            return expression.Accept(this);
        }

        private string Parenthesize(string name, params Expression[] expressions)
        {
            var sb = new StringBuilder("(").Append(name);

            foreach (var expression in expressions)
            {
                sb.Append(' ').Append(expression.Accept(this));
            }

            sb.Append(')');

            return sb.ToString();
        }

        public string VisitAssignExpression(Assign expression)
        {
            throw new NotImplementedException();
        }

        public string VisitBinaryExpression(Binary expression)
        {
            return Parenthesize(expression.Operator.Lexeme, expression.Left, expression.Right);
        }

        public string VisitCallExpression(Call expression)
        {
            throw new NotImplementedException();
        }

        public string VisitGetExpression(Get expression)
        {
            throw new NotImplementedException();
        }

        public string VisitGroupingExpression(Grouping expression)
        {
            return Parenthesize("group", expression.Expression);
        }

        public string VisitLiteralExpression(Literal expression)
        {
            return expression.Value?.ToString() ?? "niente";
        }

        public string VisitLogicalExpression(Logical expression)
        {
            throw new NotImplementedException();
        }

        public string VisitSetExpression(Set expression)
        {
            throw new NotImplementedException();
        }

        public string VisitSuperExpression(Super expression)
        {
            throw new NotImplementedException();
        }

        public string VisitThisExpression(This expression)
        {
            throw new NotImplementedException();
        }

        public string VisitUnaryExpression(Unary expression)
        {
            return Parenthesize(expression.Operator.Lexeme, expression.Right);
        }

        public string VisitVariableExpression(Variable expression)
        {
            throw new NotImplementedException();
        }
    }
}
