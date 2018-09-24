// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegerTransformer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IntegerTransformer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core.Transformations
{
    using System.Globalization;

    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// Transformer that uses <see cref="int.Parse(string,NumberStyles)"/>.
    /// </summary>
    public class IntegerTransformer : Transformer<string, int, Integer>
    {
        /// <summary>
        /// Actual transformation method.
        /// </summary>
        /// <param name="value">the value to be transformed.</param>
        /// <returns>the transformed value.</returns>
        protected override int DoTransform(string value)
        {
            return int.Parse(value, this.Config.NumberStyle);
        }
    }
}
