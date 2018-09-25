// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringMappingTransformer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core.Transformations
{
    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// Transformer that uses simple string replacement to change substrings inside a string.
    /// Each replacement is done one after the other.
    /// </summary>
    public class StringMappingTransformer : Transformer<string, string, StringMapping>
    {
        /// <summary>
        /// Transforms the given string by replacing one substring after the other.
        /// </summary>
        /// <param name="value">the value to be transformed.</param>
        /// <returns>the transformed value.</returns>
        protected override string DoTransform(string value)
        {
            foreach (var mapping in this.Config.Mappings)
            {
                value = value.Replace(mapping.From, mapping.To);
            }

            return value;
        }
    }
}
