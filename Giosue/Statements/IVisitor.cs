using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Giosue;
using Giosue.Statements;

namespace Giosue.Statements
{
    public interface IVisitor<T>
    {
        T VisitExpressionStatement(Expression statement);
    }
}