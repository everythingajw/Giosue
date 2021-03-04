using Giosue;
using Giosue.AST;


namespace Giosue.AST
{
    public interface IVisitor<T>
    {
        T VisitAssignExpression(Assign expression);
        T VisitBinaryExpression(Binary expression);
        T VisitCallExpression(Call expression);
        T VisitGetExpression(Get expression);
        T VisitGroupingExpression(Grouping expression);
        T VisitLiteralExpression(Literal expression);
        T VisitLogicalExpression(Logical expression);
        T VisitSetExpression(Set expression);
        T VisitSuperExpression(Super expression);
        T VisitThisExpression(This expression);
        T VisitUnaryExpression(Unary expression);
        T VisitVariableExpression(Variable expression);
    }
}