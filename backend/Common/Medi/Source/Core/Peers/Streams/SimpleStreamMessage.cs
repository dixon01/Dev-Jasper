// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleStreamMessage.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SimpleStreamMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers.Streams
{
    using System;
    using System.IO;

    /// <summary>
    /// Implementation of <see cref="StreamMessage"/> that has a single
    /// input stream that can be used exactly once.
    /// </summary>
    internal class SimpleStreamMessage : StreamMessage
    {
        private Stream input;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleStreamMessage"/> class.
        /// </summary>
        /// <param name="header">
        /// The header.
        /// </param>
        /// <param name="input">
        /// The input.
        /// </param>
        public SimpleStreamMessage(StreamHeader header, Stream input)
            : base(header)
        {
            this.input = input;
        }

        /// <summary>
        /// Opens the stream to read from it.
        /// </summary>
        /// <returns>
        /// a stream from which data can be read.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// if the method has already been called before.
        /// </exception>
        public override Stream OpenRead()
        {
            if (this.input == null)
            {
                throw new NotSupportedException("Can't open stream twice");
            }

            var ret = this.input;
            this.input = null;
            return ret;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            IDisposable stream = this.input;
            this.input = null;
            if (stream != null)
            {
                stream.Dispose();
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
            return string.Format("SimpleStreamMessage[{0}]", this.Header);
        }
    }
}