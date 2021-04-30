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
using System.Text;
using System.Threading.Tasks;

namespace Giosue
{
    class GiosueFunction : IGiosueCallable
    {
        private readonly Statements.Function Declaration;
        private readonly Environment Closure;

        public int Arity => Declaration.Parameters.Count;

        public GiosueFunction(Statements.Function declaration, Environment closure)
        {
            Declaration = declaration;
            Closure = closure;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var environment = new Environment(Closure);
            for (int i = 0; i < arguments.Count; i++)
            {
                environment.DefineOrOverwrite(Declaration.Parameters[i].Lexeme, arguments[i]);
            }

            interpreter.ExecuteBlock(Declaration.Body, environment);
            return null;
        }
    }
}
