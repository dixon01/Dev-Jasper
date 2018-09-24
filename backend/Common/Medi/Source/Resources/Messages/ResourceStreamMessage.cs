// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceStreamMessage.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceStreamMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Resources.Messages
{
    using System;
    using System.IO;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Peers;
    using Gorba.Common.Medi.Core.Peers.Streams;
    using Gorba.Common.Medi.Resources.Data;
    using Gorba.Common.Medi.Resources.Services;
    using Gorba.Common.Utility.Core.IO;

    /// <summary>
    /// Stream message that reads from a file.
    /// </summary>
    internal class ResourceStreamMessage : StreamMessage, IUnknown
    {
        private readonly IResourceAccess resource;

        private readonly IResourceLock resourceLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceStreamMessage"/> class.
        /// </summary>
        /// <param name="header">
        ///     The header.
        /// </param>
        /// <param name="resource">
        ///     The resource.
        /// </param>
        /// <param name="resourceLock">
        ///     A resource lock acquired for this message's resource file or null.
        /// </param>
        public ResourceStreamMessage(StreamHeader header, IResourceAccess resource, IResourceLock resourceLock)
            : base(header)
        {
            if (resource == null)
            {
                throw new ArgumentNullException("resource", "Resource is required");
            }

            this.resource = resource;
            this.resourceLock = resourceLock;
        }

        TypeName IUnknown.TypeName
        {
            get
            {
                // little trick to get it past MessageDispatcher,
                // otherwise it will try to find registered
                // subscriptions for the real type, but everybody just
                // registers to StreamMessage.
                return TypeName.Of<StreamMessage>();
            }
        }

        /// <summary>
        /// Opens the file to read from it.
        /// </summary>
        /// <returns>
        /// a stream from which data can be read.
        /// </returns>
        public override Stream OpenRead()
        {
            var stream = this.resource.OpenRead();
            return this.resourceLock == null ? stream : new ReleaseLockStream(this.resourceLock, stream);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            if (this.resourceLock != null)
            {
                this.resourceLock.Release();
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
            return string.Format("ResourceStreamMessage[{0}, {1}]", this.resource, this.Header);
        }

        /// <summary>
        /// This stream unlocks the given lock once the stream is closed.
        /// </summary>
        private class ReleaseLockStream : WrapperStream
        {
            private readonly IResourceLock resourceLock;

            public ReleaseLockStream(IResourceLock resourceLock, Stream stream)
            {
                this.resourceLock = resourceLock;
                this.Open(stream);
            }

            public override void Close()
            {
                base.Close();
                this.resourceLock.Release();
            }
        }
    }
}