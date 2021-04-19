using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Giosue;
using Giosue.Statements;

namespace Giosue.Statements
{
    public class If : Statement
    {
        public Statements.Expression Condition { get; }
        public Statements.Statement ThenBranch { get; }
        public Statements.Statement ElseBranch { get; }
    
        public If(Statements.Expression condition, Statements.Statement thenBranch, Statements.Statement ElseBranch)
        {
            this.Condition = condition;
            this.ThenBranch = thenBranch;
            this.ElseBranch = ElseBranch;
        }
    
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitIfStatement(this);
        }
    }
}