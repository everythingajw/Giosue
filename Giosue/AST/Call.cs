using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Giosue;
using Giosue.AST;

namespace Giosue.AST
{
    public class Call : Expression
    {
        public Expression Callee { get; }
        public Token Paren { get; }
        public List<Expression> Arguments { get; }
    
        public Call(Expression callee, Token paren, List<Expression> arguments)
        {
            this.Callee = callee;
            this.Paren = paren;
            this.Arguments = arguments;
        }
    
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitCallExpression(this);
        }
    }
}