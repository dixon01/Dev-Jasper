// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtocolEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProtocolEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core.Protocols
{
    using System;

    /// <summary>
    /// Event arguments with a protocol as parameter.
    /// </summary>
    public class ProtocolEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolEventArgs"/> class.
        /// </summary>
        /// <param name="protocol">
        /// The protocol.
        /// </param>
        public ProtocolEventArgs(IProtocol protocol)
        {
            this.Protocol = protocol;
        }

        /// <summary>
        /// Gets the protocol.
        /// </summary>
        public IProtocol Protocol { get; private set; }
    }
}