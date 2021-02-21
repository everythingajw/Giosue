﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giosue.Extensions
{
    internal static class CharExtensions
    {
        #region Non-null

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
            return ((char?)c).IsAsciiDigit();
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
            return ((char?)c).IsAsciiLetter();
        }

        /// <summary>
        /// Tests if <paramref name="c"/> is an ASCII letter or an underscore.
        /// </summary>
        /// <param name="c">A character.</param>
        /// <returns>True if <paramref name="c"/> is an ASCII letter or an underscore, false otherwise.</returns>
        /// <seealso cref="IsAsciiLetter(char)"/>
        public static bool IsAsciiLetterOrUnderscore(this char c)
        {
            return ((char?)c).IsAsciiLetterOrUnderscore();
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
            return ((char?)c).IsAsciiAlphanumeric();
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
            return ((char?)c).IsAsciiAlphanumericOrUnderscore();
        }

        #endregion Non-null

        #region Nullable

        // I don't like short-circuit evaluations, but not using them for these cases would be rather verbose.

        /// <summary>
        /// Tests if a character is an ASCII digit. 
        /// </summary>
        /// <remarks>
        /// We use this to avoid accepting digits from other writing systems.
        /// </remarks>
        /// <param name="c">A character</param>
        /// <returns>True if <paramref name="c"/> is an ASCII digit and not null, false otherwise.</returns>
        public static bool IsAsciiDigit(this char? c)
        {
            return c.HasValue && c.Value >= '0' && c.Value <= '9';
        }

        /// <summary>
        /// Tests if <paramref name="c"/> is an ASCII letter.
        /// </summary>
        /// <remarks>
        /// We use this to avoid accepting letters from other writing systems.
        /// </remarks>
        /// <param name="c">A character.</param>
        /// <returns>True if <paramref name="c"/> is an ASCII letter and not null, false otherwise.</returns>
        public static bool IsAsciiLetter(this char? c)
        {
            return c.HasValue && ((c.Value >= 'A' && c.Value <= 'Z') || (c.Value >= 'a' && c.Value <= 'z'));
        }

        /// <summary>
        /// Tests if <paramref name="c"/> is an underscore.
        /// </summary>
        /// <param name="c">A character.</param>
        /// <returns>True if <paramref name="c"/> is an underscore and not null, false otherwise.</returns>
        public static bool IsUnderscore(this char? c)
        {
            return c.HasValue && (c.Value == '_');
        }

        /// <summary>
        /// Tests if <paramref name="c"/> is an ASCII letter or an underscore.
        /// </summary>
        /// <param name="c">A character.</param>
        /// <returns>True if <paramref name="c"/> is an ASCII letter or an underscore and not null, false otherwise.</returns>
        /// <seealso cref="IsAsciiLetter(char)"/>
        public static bool IsAsciiLetterOrUnderscore(this char? c)
        {
            return c.IsAsciiLetter() || c.IsUnderscore();
        }

        /// <summary>
        /// Tests if <paramref name="c"/> is an ASCII alphanumeric character.
        /// </summary>
        /// <param name="c">A character.</param>
        /// <returns>True if <paramref name="c"/> is an ASCII alphanumeric character and not null, false otherwise.</returns>
        /// <seealso cref="IsAsciiLetter(char)"/>
        /// <seealso cref="IsAsciiDigit(char)"/>
        public static bool IsAsciiAlphanumeric(this char? c)
        {
            return c.IsAsciiLetter() || c.IsAsciiDigit();
        }

        /// <summary>
        /// Tests if <paramref name="c"/> is an ASCII alphanumeric character or an underscore.
        /// </summary>
        /// <param name="c">A character.</param>
        /// <returns>True if <paramref name="c"/> is an ASCII alphanumeric character or an underscore and not null, false otherwise.</returns>
        /// <seealso cref="IsAsciiAlphanumeric(char)"/>
        /// <seealso cref="IsAsciiLetterOrUnderscore(char)"/>
        public static bool IsAsciiAlphanumericOrUnderscore(this char? c)
        {
            return c.IsAsciiAlphanumeric() || c.IsUnderscore();
        }

        #endregion Nullable
    }
}
