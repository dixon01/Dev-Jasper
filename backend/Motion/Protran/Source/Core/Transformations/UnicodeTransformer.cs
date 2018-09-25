// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnicodeTransformer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core.Transformations
{
    using System.Text;

    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// Transformer to convert raw byte arrays to strings using Unicode (16 bit) decoding.
    /// </summary>
    public class UnicodeTransformer : Transformer<byte[], string, TransformationConfig>
    {
        /// <summary>
        /// Transforms the byte array to a string using unicode decoding.
        /// </summary>
        /// <param name="value">the byte array.</param>
        /// <returns>the decoded unicode string.</returns>
        protected override string DoTransform(byte[] value)
        {
            return Encoding.BigEndianUnicode.GetString(value, 0, value.Length);
        }
    }
}
