namespace MyAST
{
    public class Call : Expression
    {
        public Expression Callee { get; }
        public Token Paren { get; }
        public List<Expression> Arguments { get; }
    
        public Call(Expression callee, Token paren, List<Expression> arguments)
        {
            this.Callee = callee;
            this.Paren = paren;
            this.Arguments = arguments;
        }
    
        public T Accept(IVisitor<T> visitor)
        {
            return visitor.VisitCallExpression(this);
        }
    }
}