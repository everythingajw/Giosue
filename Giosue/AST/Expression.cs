using Giosue;
namespace Giosue.AST
{
    public abstract class Expression
    {
        public abstract T Accept<T>(IVisitor<T> visitor);
    }
}
