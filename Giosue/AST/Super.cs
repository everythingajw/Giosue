using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Giosue;
using Giosue.AST;

namespace Giosue.AST
{
    public class Super : Expression
    {
        public Token Keyword { get; }
        public Token Method { get; }
    
        public Super(Token keyword, Token method)
        {
            this.Keyword = keyword;
            this.Method = method;
        }
    
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitSuperExpression(this);
        }
    }
}