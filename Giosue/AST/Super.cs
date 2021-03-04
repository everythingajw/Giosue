using Giosue;namespace Giosue.AST
{
    public class Super : Expression
    {
        public Token Keyword { get; }
        public Token Method { get; }
    
        public Super(Token keyword, Token method)
        {
            this.Keyword = keyword;
            this.Method = method;
        }
    
        public T Accept(IVisitor<T> visitor)
        {
            return visitor.VisitSuperExpression(this);
        }
    }
}