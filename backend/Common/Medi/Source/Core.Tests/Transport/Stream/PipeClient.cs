// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PipeClient.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PipeClient type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Tests.Transport.Stream
{
    using System;

    using Gorba.Common.Medi.Core.Transport.Stream;

    /// <summary>
    /// <see cref="IStreamClient"/> implementation that is connected to a <see cref="PipeServer"/>.
    /// </summary>
    internal class PipeClient : IStreamClient
    {
        private readonly PipeServer server;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipeClient"/> class.
        /// </summary>
        /// <param name="server">
        /// The server.
        /// </param>
        public PipeClient(PipeServer server)
        {
            this.server = server;
        }

        /// <summary>
        /// Event that is fired when this object is being disposed.
        /// </summary>
        public event EventHandler Disposing;

        /// <summary>
        /// Gets or sets after how many bytes the connection should be closed.
        /// This is used for testing to make sure connections can be reopened and transfers recovered.
        /// </summary>
        public int DisconnectAfterBytes { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("PipeClient#{0}", this.server.PipeId);
        }

        void IDisposable.Dispose()
        {
            var handler = this.Disposing;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }

            this.server.Disconnect(this);
        }

        IAsyncResult IStreamClient.BeginConnect(AsyncCallback callback, object state)
        {
            return this.server.BeginConnect(this, callback, state);
        }

        IStreamConnection IStreamClient.EndConnect(IAsyncResult result)
        {
            var connection = this.server.EndConnect(result);
            this.Disposing += (sender, args) => connection.Dispose();
            return connection;
        }
    }
}