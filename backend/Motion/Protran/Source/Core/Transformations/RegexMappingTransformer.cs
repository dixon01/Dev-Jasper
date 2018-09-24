// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegexMappingTransformer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core.Transformations
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// Transformer that uses regular expressions to change substrings inside a string.
    /// Each regular expression is evaluated one after the other.
    /// </summary>
    public class RegexMappingTransformer : Transformer<string, string, RegexMapping>
    {
        /// <summary>
        /// The list of precompiled regular expressions.
        /// </summary>
        private List<Regex> regexes;

        /// <summary>
        /// Configures this transformer and creates the regular expression
        /// objects for later use by <see cref="DoTransform(string)"/>
        /// </summary>
        /// <param name="config">regex mapping configuration.</param>
        protected override void Configure(RegexMapping config)
        {
            base.Configure(config);
            this.regexes = new List<Regex>(config.Mappings.Count);
            foreach (var mapping in config.Mappings)
            {
                this.regexes.Add(new Regex(mapping.From));
            }
        }

        /// <summary>
        /// Transforms the given string by applying the regular expression
        /// replacements one after the other.
        /// </summary>
        /// <param name="value">the value to be transformed.</param>
        /// <returns>the transformed value.</returns>
        protected override string DoTransform(string value)
        {
            for (int i = 0; i < this.regexes.Count; i++)
            {
                value = this.regexes[i].Replace(value, this.Config.Mappings[i].To);
            }

            return value;
        }
    }
}
