// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPart.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IPart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Text
{
    using System;

    /// <summary>
    /// Interface to be implemented by a part of a formatted text.
    /// </summary>
    public interface IPart : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether this part should blink.
        /// </summary>
        bool Blink { get; }

        /// <summary>
        /// Duplicates this part if necessary.
        /// The duplicates are used for alternatives.
        /// </summary>
        /// <returns>
        /// The same or an equal <see cref="IPart"/>.
        /// </returns>
        IPart Duplicate();
    }
}