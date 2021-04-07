using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Giosue;
using Giosue.Statements;

namespace Giosue.Statements
{
    public abstract class Statement
    {
        public abstract T Accept<T>(IVisitor<T> visitor);
    }
}