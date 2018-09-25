// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SymbolDividerTransformer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core.Transformations
{
    using Gorba.Common.Configuration.Protran.Transformations;
    using Gorba.Common.Utility.Compatibility;

    /// <summary>
    /// Transformer that divides a given string using the configured symbol.
    /// </summary>
    public class SymbolDividerTransformer : Transformer<string, string[], SymbolDivider>
    {
        /// <summary>
        /// Splits the given string into an array of strings.
        /// </summary>
        /// <param name="value">the value to be transformed.</param>
        /// <returns>the transformed value.</returns>
        protected override string[] DoTransform(string value)
        {
            return ArrayUtil.SplitString(value, this.Config.Symbol);
        }
    }
}
