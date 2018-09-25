// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PresentationException.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PresentationException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The presentation exception.
    /// </summary>
    public partial class PresentationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PresentationException"/> class.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected PresentationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}