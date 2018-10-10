// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReverseTransformer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ReverseTransformer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core.Transformations
{
    using System;

    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// Transformation that reverses a string array.
    /// </summary>
    public class ReverseTransformer : Transformer<string[], string[], Reverse>
    {
        /// <summary>
        /// Actual transformation method to be implemented by subclasses.
        /// </summary>
        /// <param name="value">the value to be transformed.</param>
        /// <returns>the transformed value.</returns>
        protected override string[] DoTransform(string[] value)
        {
            var result = (string[])value.Clone();
            Array.Reverse(result);
            return result;
        }
    }
}
