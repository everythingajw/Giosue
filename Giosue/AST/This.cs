namespace MyAST
{
    public class This : Expression
    {
        public Token Keyword { get; }
    
        public This(Token keyword)
        {
            this.Keyword = keyword;
        }
    
        public T Accept(IVisitor<T> visitor)
        {
            return visitor.VisitThisExpression(this);
        }
    }
}