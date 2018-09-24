// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExteriorSignTexts.cs" company="Gorba AG">
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
    /// CTU datagram regarding the "Exterior Sign Texts",
    /// uniquely identified by the tag number 120.
    /// </summary>
    public class ExteriorSignTexts : Triplet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExteriorSignTexts"/> class
        /// whit a specific line number, destination and destination in arabic.
        /// </summary>
        /// <param name="signAddress"> Sign address.</param>
        /// <param name="text1">Text line 1.</param>
        /// <param name="text2">Text line 2.</param>
        public ExteriorSignTexts(byte signAddress, string text1, string text2)
            : base(TagName.ExteriorSignTexts)
        {
            this.SignAddress = signAddress;
            this.CurrentText1 = text1;
            this.CurrentText2 = text2;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExteriorSignTexts"/> class.
        /// </summary>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <param name="reader">
        /// The reader.
        /// </param>
        internal ExteriorSignTexts(int length, BinaryReader reader)
            : base(TagName.ExteriorSignTexts)
        {
            this.SignAddress = reader.ReadByte();
            byte currentText1Length = reader.ReadByte();
            length -= 2;
            this.CurrentText1 = Triplet.ReadString(reader, currentText1Length * 2);
            length -= currentText1Length * 2;
            this.CurrentText2 = Triplet.ReadString(reader, length * 2);
        }

        /// <summary>
        /// Gets or sets the sign address.
        /// </summary>
        public byte SignAddress { get; set; }

        /// <summary>
        /// Gets or sets the Text1.
        /// </summary>
        public string CurrentText1 { get; set; }

        /// <summary>
        /// Gets or sets the Text2.
        /// </summary>
        public string CurrentText2 { get; set; }

        /// <summary>
        /// Gets the triplet's payload length.
        /// </summary>
        public override int Length
        {
            get
            {
                return sizeof(byte) + sizeof(byte) + (this.CurrentText1.Length * 2) + (this.CurrentText2.Length * 2);
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
            return string.Format("{0}: {1}: {2}", this.SignAddress, this.CurrentText1, this.CurrentText2);
        }

        /// <summary>
        /// Writes the payload to the given writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void WritePayload(BinaryWriter writer)
        {
            writer.Write((byte)this.SignAddress);
            writer.Write((byte)this.CurrentText1.Length); // important: this is in 16-bit characters, not bytes
            writer.Write(Encoding.Unicode.GetBytes(this.CurrentText1));
            writer.Write(Encoding.Unicode.GetBytes(this.CurrentText2));
        }
    }
}