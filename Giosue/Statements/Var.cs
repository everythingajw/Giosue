using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Giosue;
using Giosue.Statements;

namespace Giosue.Statements
{
    public class Var : Statement
    {
        public Token Name { get; }
        public AST.Expression Initializer { get; }
    
        public Var(Token name, AST.Expression initializer)
        {
            this.Name = name;
            this.Initializer = initializer;
        }
    
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitVarStatement(this);
        }
    }
}