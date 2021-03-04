using Giosue;
using Giosue.AST;


namespace Giosue.AST
{
    public class Variable : Expression
    {
        public Token Name { get; }
    
        public Variable(Token name)
        {
            this.Name = name;
        }
    
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitVariableExpression(this);
        }
    }
}