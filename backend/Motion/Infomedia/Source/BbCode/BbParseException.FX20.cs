// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BbParseException.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BbParseException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.BbCode
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception that is thrown whenever something goes
    /// wrong during <see cref="BbParser.Parse(string)"/>.
    /// </summary>
    public partial class BbParseException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BbParseException"/> class.
        /// </summary>
        /// <param name="info">
        /// The serialization information.
        /// </param>
        /// <param name="context">
        /// The streaming context.
        /// </param>
        protected BbParseException(
            SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}