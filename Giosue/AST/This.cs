using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Giosue;
using Giosue.AST;

namespace Giosue.AST
{
    public class This : Expression
    {
        public Token Keyword { get; }
    
        public This(Token keyword)
        {
            this.Keyword = keyword;
        }
    
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitThisExpression(this);
        }
    }
}