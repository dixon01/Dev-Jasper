// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncUserToken.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AsyncUserToken type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.SocketAsyncServer
{
    using System;
    using System.Net.Sockets;

    /// <summary>
    /// Define the tocken that is linked to the assynchroneaou socket opertation. 
    /// You can see <see ref="SocketAsyncEventArgs.UserToken"/>
    /// </summary>
    public class AsyncUserToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncUserToken"/> class.
        /// </summary>
        /// <param name="tokenId">
        /// The token identifier.
        /// </param>
        public AsyncUserToken(Guid tokenId)
        {
            this.TokenId = tokenId;
            this.ConnectionId = 0;
        }

        /// <summary>
        /// Gets the token id. 
        /// Debug purpose mainly 
        /// </summary>
        public Guid TokenId { get; private set; }

        /// <summary>
        /// Gets or sets the handle of the underlying windows <see cref="Socket"/>.
        /// </summary>
        public int ConnectionId { get; set; }
    }
}
