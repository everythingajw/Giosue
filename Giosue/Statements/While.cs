using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Giosue;
using Giosue.Statements;

namespace Giosue.Statements
{
    public class While : Statement
    {
        public AST.Expression Condition { get; }
        public Statements.Statement Body { get; }
    
        public While(AST.Expression condition, Statements.Statement body)
        {
            this.Condition = condition;
            this.Body = body;
        }
    
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitWhileStatement(this);
        }
    }
}