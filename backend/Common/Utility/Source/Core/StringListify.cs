// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringListify.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StringListify type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Class that allows to concat strings using a delimiter (and an escape
    /// character to escape the delimiter and the escape itself) and then
    /// also split the items again.
    /// </summary>
    public class StringListify
    {
        private readonly char delim;

        private readonly char escape;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringListify"/> class.
        /// </summary>
        /// <param name="delim">
        /// The delimiter character.
        /// </param>
        /// <param name="escape">
        /// The escape character.
        /// </param>
        public StringListify(char delim, char escape)
        {
            this.delim = delim;
            this.escape = escape;
        }

        /// <summary>
        /// Concats the parts into a single string using the delimiter and the escape
        /// character.
        /// </summary>
        /// <param name="parts">
        /// The parts.
        /// </param>
        /// <returns>
        /// the concatenated string.
        /// </returns>
        public string FromList(IEnumerable<string> parts)
        {
            var builder = new StringBuilder();
            foreach (var s in parts)
            {
                foreach (var c in s)
                {
                    if (c == this.escape || c == this.delim)
                    {
                        builder.Append(this.escape);
                    }

                    builder.Append(c);
                }

                builder.Append(this.delim);
            }

            builder.Length--;
            return builder.ToString();
        }

        /// <summary>
        /// Splits the given string into a list of strings using the delimiter and the escape
        /// character.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The list of parts found in the input string.
        /// </returns>
        /// <exception cref="FormatException">
        /// if the input string has an invalid format (ending with an escape character).
        /// </exception>
        public List<string> ToList(string input)
        {
            var parts = new List<string>();
            var escaped = false;
            var current = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                var c = input[i];
                if (escaped)
                {
                    escaped = false;
                }
                else if (c == this.delim)
                {
                    parts.Add(current.ToString());
                    current.Length = 0;
                    continue;
                }
                else if (c == this.escape)
                {
                    escaped = true;
                    continue;
                }

                current.Append(c);
            }

            if (escaped)
            {
                throw new FormatException("Input can't end with an escape character");
            }

            parts.Add(current.ToString());

            return parts;
        }
    }
}
