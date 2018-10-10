// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NonClosingStream.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NonClosingStream type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.Ftp
{
    using System.IO;

    using Gorba.Common.Utility.Core.IO;

    /// <summary>
    /// Wrapper around a stream that filters out the <see cref="Stream.Close"/>
    /// call, so the underlying stream isn't closed.
    /// </summary>
    internal class NonClosingStream : WrapperStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NonClosingStream"/> class.
        /// </summary>
        /// <param name="stream">
        /// The underlying stream.
        /// </param>
        public NonClosingStream(Stream stream)
        {
            this.Open(stream);
        }

        /// <summary>
        /// Closes the current stream and releases any resources
        /// (such as sockets and file handles) associated with the current stream.
        /// </summary>
        /// <filterpriority>1</filterpriority>
        public override void Close()
        {
            // do nothing, since we don't want to close the underlying stream
        }
    }
}