using Giosue;
using Giosue.AST;


namespace Giosue.AST
{
    public class Literal : Expression
    {
        public object Value { get; }
    
        public Literal(object @value)
        {
            this.Value = @value;
        }
    
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitLiteralExpression(this);
        }
    }
}