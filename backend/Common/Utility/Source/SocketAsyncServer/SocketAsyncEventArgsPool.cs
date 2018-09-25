// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SocketAsyncEventArgsPool.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Based on example from http://msdn2.microsoft.com/en-us/library/system.net.sockets.socketasynceventargs.socketasynceventargs.aspx
//   Represents a collection of reusable SocketAsyncEventArgs objects.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.SocketAsyncServer
{
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using System.Threading;

    /// <summary>
    /// Based on example from http://msdn2.microsoft.com/en-us/library/system.net.sockets.socketasynceventargs.socketasynceventargs.aspx
    /// Represents a collection of reusable SocketAsyncEventArgs objects.  
    /// </summary>
    internal sealed class SocketAsyncEventArgsPool
    {
        /// <summary>
        /// Pool of SocketAsyncEventArgs.
        /// </summary>
        private readonly Stack<SocketAsyncEventArgs> pool;

        /// <summary>
        /// ID of the tocken to watch our objects while testing.
        /// </summary>
        private int nextTokenId;

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketAsyncEventArgsPool"/> class that 
        /// handles the pool of <see cref="SocketAsyncEventArgs"/>. 
        /// </summary>
        /// <param name="capacity">
        /// Maximum number of SocketAsyncEventArgs objects the pool can hold.
        /// </param>
        internal SocketAsyncEventArgsPool(int capacity)
        {
            this.pool = new Stack<SocketAsyncEventArgs>(capacity);
        }

        /// <summary>
        /// Gets the number of SocketAsyncEventArgs instances in the pool. 
        /// </summary>
        internal int Count
        {
            get { return this.pool.Count; }
        }

        /// <summary>
        /// Increments safetly the nextTokenId and return the new value. 
        /// </summary>
        /// <returns>
        /// The unique next token id. 
        /// </returns>
        internal int GetNextTokenId()
        {
            int tokenId = Interlocked.Increment(ref this.nextTokenId);
            return tokenId;
        }

        /// <summary>
        /// Removes a SocketAsyncEventArgs instance from the pool.
        /// </summary>
        /// <returns>
        /// SocketAsyncEventArgs removed from the pool.
        /// </returns>
        internal SocketAsyncEventArgs Pop()
        {
            lock (this.pool) 
            {               
                return this.pool.Pop();
            }
        }

        /// <summary>
        /// Add a SocketAsyncEventArg instance to the pool. 
        /// </summary>
        /// <param name="item">SocketAsyncEventArgs instance to add to the pool.</param>
        internal void Push(SocketAsyncEventArgs item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item", "Items added to a SocketAsyncEventArgsPool cannot be null");
            }

            lock (this.pool) 
            {
                this.pool.Push(item);
            }
        }
    }
}
