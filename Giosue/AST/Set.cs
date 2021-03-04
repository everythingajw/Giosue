using Giosue;
using Giosue.AST;


namespace Giosue.AST
{
    public class Set : Expression
    {
        public Expression Object { get; }
        public Token Name { get; }
        public Expression Value { get; }
    
        public Set(Expression @object, Token name, Expression @value)
        {
            this.Object = @object;
            this.Name = name;
            this.Value = @value;
        }
    
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitSetExpression(this);
        }
    }
}