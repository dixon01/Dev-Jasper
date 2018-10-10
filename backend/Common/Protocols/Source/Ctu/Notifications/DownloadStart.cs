// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DownloadStart.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ctu.Notifications
{
    using System.IO;
    using System.Text;

    using Gorba.Common.Protocols.Ctu.Datagram;

    /// <summary>
    /// Object tasked to represent the notification
    /// CTU datagram regarding the "Download Start",
    /// uniquely identified by the tag number 101.
    /// </summary>
    public class DownloadStart : Triplet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadStart"/> class
        /// whit a specific set of files to be downloaded.
        /// </summary>
        public DownloadStart()
            : base(TagName.DownloadStart)
        {
            this.FileCrc = 0;
            this.FileSize = 0;
            this.FileAbsPath = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadStart"/> class.
        /// </summary>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <param name="reader">
        /// The reader.
        /// </param>
        public DownloadStart(int length, BinaryReader reader)
            : base(TagName.DownloadStart)
        {
            this.FileSize = reader.ReadUInt32();
            this.FileCrc = reader.ReadInt32();
            this.FileAbsPath = Triplet.ReadString(reader, length - 4 - 4);
        }

        /// <summary>
        /// Gets or sets size of the file to be downloaded.
        /// </summary>
        public uint FileSize { get; set; }

        /// <summary>
        /// Gets or sets CRC code of the file to be downloaded.
        /// </summary>
        public int FileCrc { get; set; }

        /// <summary>
        /// Gets or sets the absolute path of the file to be downloaded.
        /// </summary>
        public string FileAbsPath { get; set; }

        /// <summary>
        /// Gets the triplet's payload length.
        /// </summary>
        public override int Length
        {
            get
            {
                return sizeof(uint) + sizeof(int) + (this.FileAbsPath.Length * 2);
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
            return string.Format("Size:{0} - CRC:{1} - File:{2}", this.FileSize, this.FileCrc, this.FileAbsPath);
        }

        /// <summary>
        /// Writes the payload to the given writer.
        /// </summary>
        /// <param name="writer">The object tasked to write the bytes through the UDP socket.</param>
        protected override void WritePayload(BinaryWriter writer)
        {
            writer.Write(this.FileSize);
            writer.Write(this.FileCrc);
            writer.Write(Encoding.Unicode.GetBytes(this.FileAbsPath));
        }
    }
}
