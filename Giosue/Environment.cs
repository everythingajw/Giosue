using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giosue
{
    public class Environment
    {
        private readonly Dictionary<string, object> Variables = new();

        public Environment()
        {

        }

        public void Define(string name, object value)
        {
            Variables.Add(name, value);
        }

        public bool Get(string name, out object value)
        {
            value = default;
            return Variables.TryGetValue(name, out value);
        }

        public bool Assign(string name, object value)
        {
            if (Variables.TryGetValue(name, out var v))
            {
                // Redefine the variable only if the types match
                if (value.GetType() == v.GetType())
                {
                    Define(name, value);
                    return true;
                }
            }

            // Types didn't match or variable doesn't exist.
            return false;
        }

    }
}
