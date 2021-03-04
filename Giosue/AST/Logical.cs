namespace MyAST
{
    public class Logical : Expression
    {
        public Expression Left { get; }
        public Token Operator { get; }
        public Expression Right { get; }
    
        public Logical(Expression left, Token @operator, Expression right)
        {
            this.Left = left;
            this.Operator = @operator;
            this.Right = right;
        }
    
        public T Accept(IVisitor<T> visitor)
        {
            return visitor.VisitLogicalExpression(this);
        }
    }
}