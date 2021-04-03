using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Giosue.AST;

namespace Giosue
{
    class ASTPrinter : IVisitor<string>
    {
        public string VisitAssignExpression(Assign expression)
        {
            throw new NotImplementedException();
        }

        public string VisitBinaryExpression(Binary expression)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public string VisitLiteralExpression(Literal expression)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public string VisitVariableExpression(Variable expression)
        {
            throw new NotImplementedException();
        }
    }
}
