using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giosue
{
    public class Environment
    {
        /// <summary>
        /// The collection of variables.
        /// </summary>
        private readonly Dictionary<string, object> Variables = new();

        /// <summary>
        /// Creates a new <see cref="Environment"/>.
        /// </summary>
        public Environment()
        {

        }

        /// <summary>
        /// Defines a variable with a name and a value.
        /// </summary>
        /// <remarks>
        /// If the variable is already defined, the current value is overwritten.
        /// </remarks>
        /// <param name="name">The name of the variable to define.</param>
        /// <param name="value">The value of the variable to define.</param>
        public void Define(string name, object value)
        {
            Variables.Add(name, value);
        }

        /// <summary>
        /// Gets a variable's value.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The value of the variable.</param>
        /// <returns>True if the variable's value was found, false otherwise.</returns>
        public bool GetValue(string name, out object value)
        {
            value = default;
            return Variables.TryGetValue(name, out value);
        }

        /// <summary>
        /// Assigns a variable a certain value.
        /// </summary>
        /// <remarks>
        /// The variable's value will only be updated if it exists and the type of the new value is equal to the type of the old value.
        /// </remarks>
        /// <param name="name">The name of the variable to update.</param>
        /// <param name="value">The variable's new value.</param>
        /// <returns>True if the variable's value was successfully updated, false otherwise.</returns>
        public bool AssignIfExists(string name, object value)
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
