using Giosue;namespace Giosue.AST
{
    public class Binary : Expression
    {
        public Expression Left { get; }
        public Token Operator { get; }
        public Expression Right { get; }
    
        public Binary(Expression left, Token @operator, Expression right)
        {
            this.Left = left;
            this.Operator = @operator;
            this.Right = right;
        }
    
        public T Accept(IVisitor<T> visitor)
        {
            return visitor.VisitBinaryExpression(this);
        }
    }
}