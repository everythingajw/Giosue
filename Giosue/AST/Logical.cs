using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Giosue;
using Giosue.AST;

namespace Giosue.AST
{
    public class Logical : Expression
    {
        public Expression Left { get; }
        public Token Operator { get; }
        public Expression Right { get; }
    
        public Logical(Expression left, Token @operator, Expression right)
        {
            this.Left = left;
            this.Operator = @operator;
            this.Right = right;
        }
    
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitLogicalExpression(this);
        }
    }
}