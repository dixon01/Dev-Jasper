// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadDataArgs.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Provides the data for the OnReceiveData event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.SocketAsyncServer
{
    using System;

    /// <summary>
    /// Provides the data for the OnReceiveData event. 
    /// </summary>
    public class ReadDataArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadDataArgs"/> class.
        /// </summary>
        /// <param name="socketHandle">
        /// The socket handle.
        /// </param>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        public ReadDataArgs(int socketHandle, byte[] buffer)
        {
            this.Buffer = new byte[buffer.Length];
            Array.Copy(buffer, this.Buffer, buffer.Length);
            this.SocketHandle = socketHandle;
        }

        /// <summary>
        /// Gets the buffer of bytes containing the read data 
        /// </summary>
        public byte[] Buffer { get; private set; }

        /// <summary>
        /// Gets the handle of the socket where the data have been read.
        /// </summary>
        public int SocketHandle { get; private set; }
    }
}
