using Giosue;
using Giosue.AST;


namespace Giosue.AST
{
    public class Grouping : Expression
    {
        public Expression Expression { get; }
    
        public Grouping(Expression expression)
        {
            this.Expression = expression;
        }
    
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitGroupingExpression(this);
        }
    }
}