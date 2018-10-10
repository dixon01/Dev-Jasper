// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegexDividerTransformer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Transformer that devides a given string using the configured regular expression.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core.Transformations
{
    using System.Text.RegularExpressions;

    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// Transformer that divides a given string using the configured regular expression.
    /// </summary>
    public class RegexDividerTransformer : Transformer<string, string[], RegexDivider>
    {
        /// <summary>
        /// List of separators (always one element)
        /// </summary>
        private Regex regex;

        /// <summary>
        /// Configures this transformer.
        /// </summary>
        /// <param name="config">the configuration.</param>
        protected override void Configure(RegexDivider config)
        {
            base.Configure(config);
            this.regex = new Regex(config.Regex, config.Options);
        }

        /// <summary>
        /// Splits the given string into an array of strings.
        /// </summary>
        /// <param name="value">the value to be transformed.</param>
        /// <returns>the transformed value.</returns>
        protected override string[] DoTransform(string value)
        {
            return this.regex.Split(value);
        }
    }
}