// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Utf8Transformer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core.Transformations
{
    using System.Text;

    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// Transformer to convert raw byte arrays to strings using UTF-8 decoding.
    /// </summary>
    public class Utf8Transformer : Transformer<byte[], string, Utf8>
    {
        /// <summary>
        /// Transforms the byte array to a string using UTF-8 decoding.
        /// </summary>
        /// <param name="value">the byte array.</param>
        /// <returns>the decoded UTF-8 string.</returns>
        protected override string DoTransform(byte[] value)
        {
            return Encoding.UTF8.GetString(value, 0, value.Length);
        }
    }
}
