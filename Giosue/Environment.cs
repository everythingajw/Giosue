using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Giosue.Exceptions;

namespace Giosue
{
    public class Environment
    {
        private readonly Environment ParentEnvironment = null;

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
        /// Creates a new <see cref="Environment"/>.
        /// </summary>
        /// <param name="parentEnvironment">The parent <see cref="Environment"/>.</param>
        public Environment(Environment parentEnvironment)
        {
            ParentEnvironment = parentEnvironment;
        }

        /// <summary>
        /// Defines a variable with a name and a value.
        /// </summary>
        /// <remarks>
        /// If the variable is already defined, the current value is overwritten.
        /// </remarks>
        /// <param name="name">The name of the variable to define.</param>
        /// <param name="value">The value of the variable to define.</param>
        public void DefineOrOverwrite(string name, object value)
        {
            Variables.Add(name, value);
        }

        /// <summary>
        /// Gets a variable's value.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <returns>The variable's value.</returns>
        /// <exception cref="EnvironmentException">Thrown if the variable cannot be found.</exception>
        public object GetValue(string name)
        {
            if (TryGetValue(name, out var value))
            {
                return value;
            }

            // The variable '{name}' is undefined.
            throw new EnvironmentException(EnvironmentExceptionType.UndefinedVariable, $"La variablie '{name}' è imprecisato.");
        }

        /// <summary>
        /// Gets a variable's value.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The value of the variable.</param>
        /// <returns>True if the variable's value was found, false otherwise.</returns>
        public bool TryGetValue(string name, out object value)
        {
            value = default;

            // Search the current environment first.
            if (Variables.TryGetValue(name, out value))
            {
                return true;
            }

            // Search parent environments after.
            if (ParentEnvironment != null)
            {
                return ParentEnvironment.TryGetValue(name, out value);
            }

            // The variable doesn't exist anywhere.
            return false;
        }

        /// <summary>
        /// Assigns a variable a value if it exists.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The value to assign to the variable.</param>
        /// <exception cref="EnvironmentException">Thrown if the variable is not defined or assignment fails.</exception>
        public void AssignIfExists(string name, object value)
        {
            if (!TryAssignIfExists(name, value))
            {
                // The variable '{name}' is undefined
                throw new EnvironmentException(EnvironmentExceptionType.UndefinedVariable, $"La variablie '{name}' è imprecisato.");
            }
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
        public bool TryAssignIfExists(string name, object value)
        {
            // Try to update the variable in the current environment first
            if (Variables.TryGetValue(name, out var v))
            {
                // Redefine the variable only if the types match
                if (value.GetType() == v.GetType())
                {
                    DefineOrOverwrite(name, value);
                    return true;
                }
            }

            // If no such variable exists in the current environment,
            // assign it in the parent environment
            if (ParentEnvironment != null)
            {
                return ParentEnvironment.TryAssignIfExists(name, value);
            }

            // Types didn't match or variable doesn't exist anywhere.
            return false;
        }

    }
}
