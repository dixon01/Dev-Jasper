// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DownloadProgressResponse.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ctu.Responses
{
    using System.IO;
    using System.Text;

    using Gorba.Common.Protocols.Ctu.Datagram;

    /// <summary>
    /// Object tasked to represent the response
    /// CTU datagram regarding a download process,
    /// uniquely identified by the tag number 2050.
    /// </summary>
    public class DownloadProgressResponse : Triplet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadProgressResponse"/> class
        /// whit a specific set of files to be downloaded.
        /// </summary>
        public DownloadProgressResponse()
            : base(TagName.DownloadProgressResponse)
        {
            this.FileAbsName = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadProgressResponse"/> class.
        /// </summary>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <param name="reader">
        /// The reader.
        /// </param>
        public DownloadProgressResponse(int length, BinaryReader reader)
            : base(TagName.DownloadProgressResponse)
        {
            this.StatusCode = (DownloadStatusCode)reader.ReadInt32();
            this.FileAbsName = Triplet.ReadString(reader, length - 4);
        }

        /// <summary>
        /// Gets or sets download status code of a file.
        /// </summary>
        public DownloadStatusCode StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the absolute path of the file being downloaded.
        /// </summary>
        public string FileAbsName { get; set; }

        /// <summary>
        /// Gets the triplet's payload length.
        /// </summary>
        public override int Length
        {
            get
            {
                return sizeof(uint) + (this.FileAbsName.Length * 2);
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
            return string.Format("Status:{0} - File Abs Name:{1}", this.StatusCode, this.FileAbsName);
        }

        /// <summary>
        /// Writes the payload to the given writer.
        /// </summary>
        /// <param name="writer">The object tasked to write the bytes through the UDP socket.</param>
        protected override void WritePayload(BinaryWriter writer)
        {
            writer.Write((int)this.StatusCode);
            writer.Write(Encoding.Unicode.GetBytes(this.FileAbsName));
        }
    }
}
