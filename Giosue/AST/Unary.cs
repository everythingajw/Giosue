using Giosue;
using Giosue.AST;


namespace Giosue.AST
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
    
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitUnaryExpression(this);
        }
    }
}