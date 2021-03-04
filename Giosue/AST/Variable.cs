namespace MyAST
{
    public class Variable : Expression
    {
        public Token Name { get; }
    
        public Variable(Token name)
        {
            this.Name = name;
        }
    
        public T Accept(IVisitor<T> visitor)
        {
            return visitor.VisitVariableExpression(this);
        }
    }
}