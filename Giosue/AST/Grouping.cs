namespace MyAST
{
    public class Grouping : Expression
    {
        public Expression Expression { get; }
    
        public Grouping(Expression expression)
        {
            this.Expression = expression;
        }
    
        public T Accept(IVisitor<T> visitor)
        {
            return visitor.VisitGroupingExpression(this);
        }
    }
}