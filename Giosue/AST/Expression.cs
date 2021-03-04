using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Giosue;
using Giosue.AST;

namespace Giosue.AST
{
    public abstract class Expression
    {
        public abstract T Accept<T>(IVisitor<T> visitor);
    }
}