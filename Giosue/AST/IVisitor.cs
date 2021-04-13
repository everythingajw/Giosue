using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Giosue;
using Giosue.AST;

namespace Giosue.AST
{
    public interface IVisitor<T>
    {
        public T VisitAssignExpression(Assign expression);
        public T VisitBinaryExpression(Binary expression);
        public T VisitCallExpression(Call expression);
        public T VisitGetExpression(Get expression);
        public T VisitGroupingExpression(Grouping expression);
        public T VisitLiteralExpression(Literal expression);
        public T VisitLogicalExpression(Logical expression);
        public T VisitSetExpression(Set expression);
        public T VisitSuperExpression(Super expression);
        public T VisitThisExpression(This expression);
        public T VisitUnaryExpression(Unary expression);
        public T VisitVariableExpression(Variable expression);
    }
}