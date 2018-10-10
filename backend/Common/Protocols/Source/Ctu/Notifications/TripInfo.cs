// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TripInfo.cs" company="Gorba AG">
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
    /// CTU datagram regarding the "Trip Info",
    /// uniquely identified by the tag number 100.
    /// </summary>
    public class TripInfo : Triplet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TripInfo"/> class
        /// whit a specific line number, destination and destination in arabic.
        /// </summary>
        /// <param name="lineNumber">The line Number.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="destArabic">The arabic destination.</param>
        public TripInfo(string lineNumber, string destination, string destArabic)
            : base(TagName.TripInfo)
        {
            this.LineNumber = lineNumber;
            this.Destination = destination;
            this.DestinationArabic = destArabic;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TripInfo"/> class.
        /// </summary>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <param name="reader">
        /// The reader.
        /// </param>
        internal TripInfo(int length, BinaryReader reader)
            : base(TagName.TripInfo)
        {
            byte lineNoLength = reader.ReadByte();
            byte destLength = reader.ReadByte();
            length -= 2;
            this.LineNumber = Triplet.ReadString(reader, lineNoLength * 2);
            length -= lineNoLength * 2;
            this.Destination = Triplet.ReadString(reader, destLength * 2);
            length -= destLength * 2;
            this.DestinationArabic = Triplet.ReadString(reader, length);
        }

        /// <summary>
        /// Gets or sets the line number.
        /// </summary>
        public string LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the English destination.
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// Gets or sets the Arabic destination.
        /// </summary>
        public string DestinationArabic { get; set; }

        /// <summary>
        /// Gets the triplet's payload length.
        /// </summary>
        public override int Length
        {
            get
            {
                return sizeof(byte) + sizeof(byte) + (this.LineNumber.Length * 2) + (this.Destination.Length * 2)
                       + (this.DestinationArabic.Length * 2);
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
            return string.Format("{0}: {1} / {2}", this.LineNumber, this.Destination, this.DestinationArabic);
        }

        /// <summary>
        /// Writes the payload to the given writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void WritePayload(BinaryWriter writer)
        {
            writer.Write((byte)this.LineNumber.Length); // important: this is in 16-bit characters, not bytes
            writer.Write((byte)this.Destination.Length); // important: this is in 16-bit characters, not bytes
            writer.Write(Encoding.Unicode.GetBytes(this.LineNumber));
            writer.Write(Encoding.Unicode.GetBytes(this.Destination));
            writer.Write(Encoding.Unicode.GetBytes(this.DestinationArabic));
        }
    }
}
