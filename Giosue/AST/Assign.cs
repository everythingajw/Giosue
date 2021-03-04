using Giosue;namespace Giosue.AST
{
    public class Assign : Expression
    {
        public Token Name { get; }
        public Expression Value { get; }
    
        public Assign(Token name, Expression @value)
        {
            this.Name = name;
            this.Value = @value;
        }
    
        public T Accept(IVisitor<T> visitor)
        {
            return visitor.VisitAssignExpression(this);
        }
    }
}