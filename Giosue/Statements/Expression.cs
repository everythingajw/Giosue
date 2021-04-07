using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Giosue;
using Giosue.Statements;

namespace Giosue.Statements
{
    public class Expression : Statement
    {
        public Expression Expr { get; }
    
        public Expression(Expression expression)
        {
            this.Expr = expression;
        }
    
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitExpressionStatement(this);
        }
    }
}