using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Giosue;
using Giosue.Statements;

namespace Giosue.Statements
{
    public interface IVisitor<T>
    {
        public T VisitExpressionStatement(Expression statement);
        public T VisitVarStatement(Var statement);
        public T VisitBlockStatement(Block statement);
        public T VisitIfStatement(If statement);
        public T VisitWhileStatement(While statement);
    }
}