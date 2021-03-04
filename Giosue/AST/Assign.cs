using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Giosue;
using Giosue.AST;

namespace Giosue.AST
{
    public class Assign : Expression
    {
        public Token Name { get; }
        public Expression Value { get; }
    
        public Assign(Token name, Expression @value)
        {
            this.Name = name;
            this.Value = @value;
        }
    
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitAssignExpression(this);
        }
    }
}