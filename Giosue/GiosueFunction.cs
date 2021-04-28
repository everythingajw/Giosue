using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giosue
{
    class GiosueFunction : GiosueCallable
    {
        private readonly Statements.Function Declaration;

        public int Arity => Declaration.Parameters.Count;

        public GiosueFunction(Statements.Function declaration)
        {
            Declaration = declaration;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var environment = new Environment(interpreter.Globals);
            for (int i = 0; i < arguments.Count; i++)
            {
                environment.DefineOrOverwrite(Declaration.Parameters[i].Lexeme, arguments[i]);
            }

            interpreter.ExecuteBlock(Declaration.Body, environment);
            return null;
        }
    }
}
