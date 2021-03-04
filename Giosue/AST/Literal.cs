namespace MyAST
{
    public class Literal : Expression
    {
        public object Value { get; }
    
        public Literal(object @value)
        {
            this.Value = @value;
        }
    
        public T Accept(IVisitor<T> visitor)
        {
            return visitor.VisitLiteralExpression(this);
        }
    }
}