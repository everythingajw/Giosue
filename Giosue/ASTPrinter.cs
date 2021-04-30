// Giosue language interpreter
// The interpreter for the Giosue programming language.
// Copyright (C) 2021  Anthony Webster
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Giosue.AST;

namespace Giosue
{
    public class ASTPrinter : IVisitor<string>
    {
        public string StringifyExpression(Expression expression)
        {
            if (expression == null)
            {
                // I don't think I need this anymore - if there's no expression don't print anything.
                // A touch of reflection so I can print out what methods are causing exceptions.
                //var thisMethod = MethodBase.GetCurrentMethod();
                //Console.Error.WriteLine($"{thisMethod.DeclaringType.FullName}.{thisMethod.Name} :: {nameof(expression)} is null");
                //Console.Error.WriteLine();
            }
            return expression?.Accept(this) ?? null;
        }

        private string Parenthesize(string name, params Expression[] expressions)
        {
            var sb = new StringBuilder("(").Append(name);

            foreach (var expression in expressions)
            {
                sb.Append(' ').Append(expression.Accept(this));
            }

            sb.Append(')');

            return sb.ToString();
        }

        public string VisitAssignExpression(Assign expression)
        {
            throw new NotImplementedException();
        }

        public string VisitBinaryExpression(Binary expression)
        {
            return Parenthesize(expression.Operator.Lexeme, expression.Left, expression.Right);
        }

        public string VisitCallExpression(Call expression)
        {
            throw new NotImplementedException();
        }

        public string VisitGetExpression(Get expression)
        {
            throw new NotImplementedException();
        }

        public string VisitGroupingExpression(Grouping expression)
        {
            return Parenthesize("group", expression.Expression);
        }

        public string VisitLiteralExpression(Literal expression)
        {
            return expression.Value?.ToString() ?? "niente";
        }

        public string VisitLogicalExpression(Logical expression)
        {
            throw new NotImplementedException();
        }

        public string VisitSetExpression(Set expression)
        {
            throw new NotImplementedException();
        }

        public string VisitSuperExpression(Super expression)
        {
            throw new NotImplementedException();
        }

        public string VisitThisExpression(This expression)
        {
            throw new NotImplementedException();
        }

        public string VisitUnaryExpression(Unary expression)
        {
            return Parenthesize(expression.Operator.Lexeme, expression.Right);
        }

        public string VisitVariableExpression(Variable expression)
        {
            throw new NotImplementedException();
        }
    }
}
