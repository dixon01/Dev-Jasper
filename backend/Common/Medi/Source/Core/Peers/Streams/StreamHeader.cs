// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamHeader.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StreamHeader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers.Streams
{
    /// <summary>
    /// Header for sending resource streams.
    /// </summary>
    public class StreamHeader
    {
        /// <summary>
        /// Gets or sets the source of the stream.
        /// </summary>
        public MediAddress Source { get; set; }

        /// <summary>
        /// Gets or sets the destination of the stream.
        /// </summary>
        public MediAddress Destination { get; set; }

        /// <summary>
        /// Gets or sets offset into the resource at which the contents begins.
        /// </summary>
        public long Offset { get; set; }

        /// <summary>
        /// Gets or sets the total length of the resource, not starting at offset
        /// <see cref="Offset"/>.
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// Gets or sets the hash identifier of the resource.
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "StreamHeader[{0}->{1}, {2}, {3} / {4}]",
                this.Source,
                this.Destination,
                this.Hash,
                this.Offset,
                this.Length);
        }
    }
}