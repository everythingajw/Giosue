using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giosue.Extensions
{
    internal static class CharExtensions
    {
        /// <summary>
        /// Tests if a character is an ASCII digit. 
        /// </summary>
        /// <remarks>
        /// We use this to avoid accepting digits from other writing systems.
        /// </remarks>
        /// <param name="c">A character</param>
        /// <returns>True if the character is an ASCII digit, false otherwise.</returns>
        public static bool IsAsciiDigit(this char c)
        {
            // Visual Studio thinks that casts are redundant here.
            return c >= '0' && c <= '9';
        }

        /// <summary>
        /// Tests if <paramref name="c"/> is an ASCII letter.
        /// </summary>
        /// <remarks>
        /// We use this to avoid accepting letters from other writing systems.
        /// </remarks>
        /// <param name="c">A character.</param>
        /// <returns>True if <paramref name="c"/> is an ASCII letter, false otherwise.</returns>
        public static bool IsAsciiLetter(this char c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
        }

        /// <summary>
        /// Tests if <paramref name="c"/> is an ASCII letter or an underscore.
        /// </summary>
        /// <param name="c">A character.</param>
        /// <returns>True if <paramref name="c"/> is an ASCII letter or an underscore, false otherwise.</returns>
        /// <seealso cref="IsAsciiLetter(char)"/>
        public static bool IsAsciiLetterOrUnderscore(this char c)
        {
            return (c >= 'A' && c <= 'Z') ||
                   (c >= 'a' && c <= 'z') ||
                   (c == '_');
        }

        /// <summary>
        /// Tests if <paramref name="c"/> is an ASCII alphanumeric character.
        /// </summary>
        /// <param name="c">A character.</param>
        /// <returns>True if <paramref name="c"/> is an ASCII alphanumeric character, false otherwise.</returns>
        /// <seealso cref="IsAsciiLetter(char)"/>
        /// <seealso cref="IsAsciiDigit(char)"/>
        public static bool IsAsciiAlphanumeric(this char c)
        {
            return IsAsciiLetter(c) || IsAsciiDigit(c);
        }

        /// <summary>
        /// Tests if <paramref name="c"/> is an ASCII alphanumeric character or an underscore.
        /// </summary>
        /// <param name="c">A character.</param>
        /// <returns>True if <paramref name="c"/> is an ASCII alphanumeric character or an underscore, false otherwise.</returns>
        /// <seealso cref="IsAsciiAlphanumeric(char)"/>
        /// <seealso cref="IsAsciiLetterOrUnderscore(char)"/>
        public static bool IsAsciiAlphanumericOrUnderscore(this char c)
        {
            return IsAsciiAlphanumeric(c) || IsAsciiLetterOrUnderscore(c);
        }
    }
}
