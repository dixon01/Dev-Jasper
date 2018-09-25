// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsciiTransformer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core.Transformations
{
    using System.Text;

    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// Transformer to convert raw byte arrays to strings using ASCII decoding.
    /// </summary>
    public class AsciiTransformer : Transformer<byte[], string, TransformationConfig>
    {
        /// <summary>
        /// Transforms the byte array to a string using ASCII decoding.
        /// </summary>
        /// <param name="value">the byte array.</param>
        /// <returns>the decoded ASCII string.</returns>
        protected override string DoTransform(byte[] value)
        {
            return Encoding.ASCII.GetString(value, 0, value.Length);
        }
    }
}
