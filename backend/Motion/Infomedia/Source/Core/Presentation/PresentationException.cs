// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PresentationException.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PresentationException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation
{
    using System;

    /// <summary>
    /// The presentation exception.
    /// </summary>
    [Serializable]
    public partial class PresentationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PresentationException"/> class.
        /// </summary>
        public PresentationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PresentationException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public PresentationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PresentationException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="inner">
        /// The inner.
        /// </param>
        public PresentationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}