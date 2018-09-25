// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CapitalizeTransformer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CapitalizeTransformer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core.Transformations
{
    using System;
    using System.Globalization;

    using Gorba.Common.Configuration.Protran.Transformations;
    using Gorba.Common.Utility.Compatibility;

    /// <summary>
    /// Transforms strings by making the first letter of each word uppercase and
    /// the rest lowercase, taking into account the configured exceptions.
    /// </summary>
    public class CapitalizeTransformer : Transformer<string, string, Capitalize>
    {
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        /// <summary>
        /// Capitalizes the given string.
        /// </summary>
        /// <param name="value">the value to be transformed.</param>
        /// <returns>the transformed value.</returns>
        protected override string DoTransform(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            if (this.Config.Exceptions != null)
            {
                var exception = ArrayUtil.Find(
                    this.Config.Exceptions, s => value.Equals(s, StringComparison.InvariantCultureIgnoreCase));
                if (exception != null)
                {
                    return exception;
                }
            }

            char first = value[0];
            if (this.Config.Mode != CapitalizeMode.LowerOnly)
            {
                first = char.ToUpper(first, Culture);
            }

            if (value.Length == 1)
            {
                return first.ToString(Culture);
            }

            var reminder = value.Substring(1);
            if (this.Config.Mode != CapitalizeMode.UpperOnly)
            {
                reminder = reminder.ToLower(Culture);
            }

            return first + reminder;
        }
    }
}
