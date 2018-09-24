// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AcceptSocketClosureArgs.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Provides the data for the OnSoketConnect event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.SocketAsyncServer
{
    /// <summary>
    /// Provides the data for the OnSoketConnect event.
    /// </summary>
    public class AcceptSocketClosureArgs : SocketConnectArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AcceptSocketClosureArgs"/> class.
        /// </summary>
        /// <param name="socketHandle">
        /// The socket handle.
        /// </param>
        public AcceptSocketClosureArgs(int socketHandle)
            : base(socketHandle)
        {
            // Accept equals true by default to close socket if Accept is not assigned.
            this.Accept = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the socket closure is accepted or not.
        /// </summary>
        public bool Accept { get; set; }
    }
}
