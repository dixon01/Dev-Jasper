// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SocketDisconnectArgs.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Provides the data for the OnSocketDisconnect event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.SocketAsyncServer
{
    using System;

    /// <summary>
    /// Provides the data for the OnSocketDisconnect event.
    /// </summary>
    public class SocketDisconnectArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SocketDisconnectArgs"/> class.
        /// </summary>
        /// <param name="socketHandle">
        /// The socket handle.
        /// </param>
        /// <param name="receivedBytes">
        /// The received bytes.
        /// </param>
        /// <param name="sentBytes">
        /// The sent bytes.
        /// </param>
        public SocketDisconnectArgs(int socketHandle, int receivedBytes, int sentBytes)
        {
            this.SocketHandle = socketHandle;
            this.ReceivedBytes = receivedBytes;
            this.SentBytes = sentBytes;
        }

        /// <summary>
        /// Gets ReceivedBytes.
        /// </summary>
        public int ReceivedBytes { get; private set; }

        /// <summary>
        /// Gets SocketHandle.
        /// </summary>
        public int SocketHandle { get; private set; }

        /// <summary>
        /// Gets SentBytes.
        /// </summary>
        public int SentBytes { get; private set; }
    }
}
