// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LineInfo.cs" company="Gorba AG">
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
    /// CTU datagram regarding the "Line Info",
    /// uniquely identified by the tag number 103.
    /// </summary>
    public class LineInfo : Triplet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineInfo"/> class.
        /// </summary>
        /// <param name="destinationNumber">
        /// The destination number.
        /// </param>
        /// <param name="currentDirectionNumber">
        /// The current direction number.
        /// </param>
        /// <param name="lineNumber">
        /// The line number.
        /// </param>
        public LineInfo(string destinationNumber, string currentDirectionNumber, string lineNumber)
            : base(TagName.LineInfo)
        {
            this.DestinationNo = destinationNumber;
            this.CurrentDirectionNo = currentDirectionNumber;
            this.LineNumber = lineNumber;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineInfo"/> class.
        /// </summary>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <param name="reader">
        /// The reader.
        /// </param>
        internal LineInfo(int length, BinaryReader reader)
            : base(TagName.LineInfo)
        {
            byte destinationNoLength = reader.ReadByte();
            byte currentDirectionNoLength = reader.ReadByte();
            length -= 2;
            this.DestinationNo = Triplet.ReadString(reader, destinationNoLength * 2);
            length -= destinationNoLength * 2;
            this.CurrentDirectionNo = Triplet.ReadString(reader, currentDirectionNoLength * 2);
            length -= currentDirectionNoLength * 2;
            this.LineNumber = Triplet.ReadString(reader, length);
        }

        /// <summary>
        /// Gets or sets the line number.
        /// </summary>
        public string DestinationNo { get; set; }

        /// <summary>
        /// Gets or sets the English destination.
        /// </summary>
        public string CurrentDirectionNo { get; set; }

        /// <summary>
        /// Gets or sets the Arabic destination.
        /// </summary>
        public string LineNumber { get; set; }

        /// <summary>
        /// Gets the triplet's payload length.
        /// </summary>
        public override int Length
        {
            get
            {
                return sizeof(byte) + sizeof(byte)
                    + (this.DestinationNo.Length * 2) + (this.CurrentDirectionNo.Length * 2)
                    + (this.LineNumber.Length * 2);
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
            return string.Format("{0}: {1}: {2}", this.DestinationNo, this.CurrentDirectionNo, this.LineNumber);
        }

        /// <summary>
        /// Writes the payload to the given writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void WritePayload(BinaryWriter writer)
        {
            writer.Write((byte)this.DestinationNo.Length); // important: this is in 16-bit characters, not bytes
            writer.Write((byte)this.CurrentDirectionNo.Length); // important: this is in 16-bit characters, not bytes
            writer.Write(Encoding.Unicode.GetBytes(this.DestinationNo));
            writer.Write(Encoding.Unicode.GetBytes(this.CurrentDirectionNo));
            writer.Write(Encoding.Unicode.GetBytes(this.LineNumber));
        }
    }
}
