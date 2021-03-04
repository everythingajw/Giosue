using Giosue;namespace Giosue.AST
{
    public class Get : Expression
    {
        public Expression Object { get; }
        public Token Name { get; }
    
        public Get(Expression @object, Token name)
        {
            this.Object = @object;
            this.Name = name;
        }
    
        public T Accept(IVisitor<T> visitor)
        {
            return visitor.VisitGetExpression(this);
        }
    }
}