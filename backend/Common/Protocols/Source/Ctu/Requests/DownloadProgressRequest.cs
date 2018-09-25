// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DownloadProgressRequest.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ctu.Requests
{
    using System.IO;

    using Gorba.Common.Protocols.Ctu.Datagram;

    /// <summary>
    /// Object tasked to represent the request
    /// CTU datagram regarding a download process,
    /// uniquely identified by the tag number 1026.
    /// </summary>
    public class DownloadProgressRequest : Triplet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadProgressRequest"/> class
        /// whit a specific set of files to be downloaded.
        /// </summary>
        public DownloadProgressRequest()
            : base(TagName.DownloadProgressRequest)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadProgressRequest"/> class.
        /// </summary>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <param name="reader">
        /// The reader.
        /// </param>
        public DownloadProgressRequest(int length, BinaryReader reader)
            : base(TagName.DownloadProgressRequest)
        {
            if (length != 0)
            {
                // ignore payload (this should never happen)
                reader.ReadBytes(length);
            }
        }

        /// <summary>
        /// Gets the triplet's payload length.
        /// </summary>
        public override int Length
        {
            get
            {
                return 0;
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
            // nothing to translate to a string.
            return string.Empty;
        }

        /// <summary>
        /// Writes the payload to the given writer.
        /// </summary>
        /// <param name="writer">The object tasked to write the bytes through the UDP socket.</param>
        protected override void WritePayload(BinaryWriter writer)
        {
            // nothing to write.
            // this datagram doesn't have any byte in it.
        }
    }
}
