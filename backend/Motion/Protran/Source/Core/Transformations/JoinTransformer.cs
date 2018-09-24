// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JoinTransformer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the JoinTransformer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core.Transformations
{
    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// Join transformation that converts a string array to a string
    /// by combining the elements with the defined <see cref="Join.Separator"/>.
    /// </summary>
    public class JoinTransformer : Transformer<string[], string, Join>
    {
        /// <summary>
        /// Actual transformation method to be implemented by subclasses.
        /// </summary>
        /// <param name="value">the value to be transformed.</param>
        /// <returns>the transformed value.</returns>
        protected override string DoTransform(string[] value)
        {
            return string.Join(this.Config.Separator, value);
        }
    }
}
