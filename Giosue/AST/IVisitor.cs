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

// This code was generated by the AST and Statement generator.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Giosue;
using Giosue.AST;

namespace Giosue.AST
{
    public interface IVisitor<T>
    {
        public T VisitAssignExpression(Assign expression);
        public T VisitBinaryExpression(Binary expression);
        public T VisitCallExpression(Call expression);
        public T VisitGetExpression(Get expression);
        public T VisitGroupingExpression(Grouping expression);
        public T VisitLiteralExpression(Literal expression);
        public T VisitLogicalExpression(Logical expression);
        public T VisitSetExpression(Set expression);
        public T VisitSuperExpression(Super expression);
        public T VisitThisExpression(This expression);
        public T VisitUnaryExpression(Unary expression);
        public T VisitVariableExpression(Variable expression);
    }
}