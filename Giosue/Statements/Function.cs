using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Giosue;
using Giosue.Statements;

namespace Giosue.Statements
{
    public class Function : Statement
    {
        public Token Name { get; }
        public List<Token> Parameters { get; }
        public List<Statement> Body { get; }
    
        public Function(Token name, List<Token> parameters, List<Statement> body)
        {
            this.Name = name;
            this.Parameters = parameters;
            this.Body = body;
        }
    
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitFunctionStatement(this);
        }
    }
}