// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShortKeyEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ShortKeyEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    using System;

    /// <summary>
    /// The short key event arguments.
    /// </summary>
    public class ShortKeyEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShortKeyEventArgs"/> class.
        /// </summary>
        /// <param name="shortKey">
        /// The short key.
        /// </param>
        public ShortKeyEventArgs(ShortKey shortKey)
        {
            this.ShortKey = shortKey;
        }

        /// <summary>
        /// Gets the short key.
        /// </summary>
        public ShortKey ShortKey { get; private set; }
    }
}