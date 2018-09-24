// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SocketConnectArgs.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Provides the data for the OnSoketConnect event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.SocketAsyncServer
{
    using System;

    /// <summary>
    /// Provides the data for the OnSoketConnect event.
    /// </summary>
    public class SocketConnectArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SocketConnectArgs"/> class.
        /// </summary>
        /// <param name="socketHandle">
        /// The socket handle.
        /// </param>
        public SocketConnectArgs(int socketHandle)
        {
            this.SocketHandle = socketHandle;
        }

        /// <summary>
        /// Gets the handle of the socket where the data have been read.
        /// </summary>
        public int SocketHandle { get; private set; }
    }
}
