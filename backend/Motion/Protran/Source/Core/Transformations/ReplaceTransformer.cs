// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReplaceTransformer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ReplaceTransformer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core.Transformations
{
    using System;

    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// Transformation that replaces whole strings depending on the
    /// configuration.
    /// </summary>
    public class ReplaceTransformer : Transformer<string, string, Replace>
    {
        private StringComparison comparison;

        /// <summary>
        /// Configures this transformer with the given configuration.
        /// </summary>
        /// <param name="config">
        /// The configuration.
        /// </param>
        protected override void Configure(Replace config)
        {
            base.Configure(config);

            this.comparison = config.CaseSensitive
                                  ? StringComparison.InvariantCulture
                                  : StringComparison.CurrentCultureIgnoreCase;
        }

        /// <summary>
        /// Actual transformation method to be implemented by subclasses.
        /// </summary>
        /// <param name="value">the value to be transformed.</param>
        /// <returns>the transformed value.</returns>
        protected override string DoTransform(string value)
        {
            foreach (var mapping in this.Config.Mappings)
            {
                if (mapping.From.Equals(value, this.comparison))
                {
                    return mapping.To;
                }
            }

            return value;
        }
    }
}
