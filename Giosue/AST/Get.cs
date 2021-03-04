using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Giosue;
using Giosue.AST;

namespace Giosue.AST
{
    public class Get : Expression
    {
        public Expression Object { get; }
        public Token Name { get; }
    
        public Get(Expression @object, Token name)
        {
            this.Object = @object;
            this.Name = name;
        }
    
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitGetExpression(this);
        }
    }
}