using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Giosue;
using Giosue.Statements;

namespace Giosue.Statements
{
    public class Block : Statement
    {
        public List<Statements.Statement> Statements { get; }
    
        public Block(List<Statements.Statement> statements)
        {
            this.Statements = statements;
        }
    
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitBlockStatement(this);
        }
    }
}