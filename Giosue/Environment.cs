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

    }
}
