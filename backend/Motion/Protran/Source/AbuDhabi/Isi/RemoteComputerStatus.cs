// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteComputerStatus.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Isi
{
    /// <summary>
    /// Enumf for the status about the ISI remote computer.
    /// </summary>
    public enum RemoteComputerStatus
    {
        /// <summary>
        /// No idea about the current status.
        /// </summary>
        Unknown,

        /// <summary>
        /// The computer is currently disconnected from us.
        /// </summary>
        Disconnected,

        /// <summary>
        /// Protran and the remote ISI server have established valid a socket.
        /// </summary>
        SocketBind,

        /// <summary>
        /// The computer is currently connected with us.
        /// </summary>
        Connected,

        /// <summary>
        /// We have succesfully subscribed ourself
        /// to the remote computer and also the ISI remote server did it.
        /// </summary>
        BidirectionallySubscribed
    }
}
