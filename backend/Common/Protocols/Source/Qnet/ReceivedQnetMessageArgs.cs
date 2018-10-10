// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReceivedQnetMessageArgs.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Provides data for the <see cref="QnetProtocolStack.ReceivedQnetMessage" /> event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System;

    /// <summary>
    /// Provides data for the <see cref="QnetProtocolStack.ReceivedQnetMessage"/> event. 
    /// </summary>
    public class ReceivedQnetMessageArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivedQnetMessageArgs"/> class.
        /// </summary>
        /// <param name="qnetMessage">
        /// The received qnet message.
        /// </param>
        public ReceivedQnetMessageArgs(QnetMessageBase qnetMessage)
        {
            this.DecodedQnetMessage = qnetMessage;
        }

        /// <summary>
        /// Gets or sets the the received qnet message.
        /// </summary>
        public QnetMessageBase DecodedQnetMessage { get; set; }
    }
}
