// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PipeConnection.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PipeConnection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Tests.Transport.Stream
{
    using System;
    using System.IO;

    using Gorba.Common.Medi.Core.Transport.Stream;

    /// <summary>
    /// <see cref="IStreamConnection"/> implementation that
    /// contains a <see cref="PipeStream"/>.
    /// </summary>
    internal class PipeConnection : IStreamConnection
    {
        private readonly string connectionName;
        private readonly PipeStream stream;

        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipeConnection"/> class.
        /// </summary>
        /// <param name="connectionName">
        /// The connection name.
        /// </param>
        /// <param name="stream">
        /// The stream.
        /// </param>
        public PipeConnection(string connectionName, PipeStream stream)
        {
            this.connectionName = connectionName;
            this.stream = stream;
        }

        /// <summary>
        /// Event that is fired when this connection is closed.
        /// </summary>
        public event EventHandler Disposed;

        Stream IStreamConnection.Stream
        {
            get
            {
                return this.stream;
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("PipeConnection#{0}", this.connectionName);
        }

        void IDisposable.Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.stream.Dispose();

            var handler = this.Disposed;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        int IStreamConnection.CreateId()
        {
            return this.GetHashCode();
        }
    }
}