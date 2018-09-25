// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameDecodingException.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FrameDecodingException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception thrown when decoding AHDLC frames.
    /// </summary>
    public partial class FrameDecodingException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameDecodingException"/> class.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected FrameDecodingException(
            SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}