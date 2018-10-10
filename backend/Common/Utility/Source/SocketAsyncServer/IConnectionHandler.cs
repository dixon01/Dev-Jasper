// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConnectionHandler.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IConnectionHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.SocketAsyncServer
{
    /// <summary>
    /// Defines the interface IConnectionHandler 
    /// </summary>
    public interface IConnectionHandler
    {
        /// <summary>
        /// Sends the data of the underlying connection identified by the specified connectionId
        /// </summary>
        /// <param name="connectionId">
        /// The connection id.
        /// </param>
        /// <param name="buffer">
        /// The array of bytes to send
        /// </param>
        /// <returns>
        /// <b>true</b> if the operation succeeds, <b>false</b> otherwize.
        /// </returns>
        bool SendData(int connectionId, byte[] buffer);

        /// <summary>
        /// Broadcast the buffer to all 
        /// </summary>
        /// <param name="buffer">
        /// The array of bytes to send
        /// </param>
        void BroadcastData(byte[] buffer);
    }
}
