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
using Giosue.Statements;

namespace Giosue.Statements
{
    public class While : Statement
    {
        public AST.Expression Condition { get; }
        public Statements.Statement Body { get; }
    
        public While(AST.Expression condition, Statements.Statement body)
        {
            this.Condition = condition;
            this.Body = body;
        }
    
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitWhileStatement(this);
        }
    }
}