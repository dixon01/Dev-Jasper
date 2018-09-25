// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XimpleEventArgs.cs" company="Gorba AG">
//   Copyright © 2011 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the XimpleEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ximple
{
    using System;

    /// <summary>
    /// Event args for events that have a Ximple object as payload.
    /// </summary>
    public class XimpleEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XimpleEventArgs"/> class.
        /// </summary>
        /// <param name="ximple">
        /// The Ximple object.
        /// </param>
        public XimpleEventArgs(Ximple ximple)
        {
            this.Ximple = ximple;
        }

        /// <summary>
        /// Gets the Ximple object associated with this event.
        /// </summary>
        public Ximple Ximple { get; private set; }
    }
}
