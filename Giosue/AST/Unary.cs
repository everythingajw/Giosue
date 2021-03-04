using Giosue;namespace Giosue.AST
{
    public class Unary : Expression
    {
        public Token Operator { get; }
        public Expression Right { get; }
    
        public Unary(Token @operator, Expression right)
        {
            this.Operator = @operator;
            this.Right = right;
        }
    
        public T Accept(IVisitor<T> visitor)
        {
            return visitor.VisitUnaryExpression(this);
        }
    }
}