// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeviceInfoResponse.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DeviceInfoResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ctu.Responses
{
    using System.IO;
    using System.Text;

    using Gorba.Common.Protocols.Ctu.Datagram;

    /// <summary>
    /// The device info response triplet
    /// </summary>
    public class DeviceInfoResponse : Triplet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceInfoResponse"/> class.
        /// </summary>
        public DeviceInfoResponse()
            : base(TagName.DeviceInfoResponse)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceInfoResponse"/> class.
        /// </summary>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <param name="reader">
        /// The reader.
        /// </param>
        public DeviceInfoResponse(int length, BinaryReader reader)
            : base(TagName.DeviceInfoResponse)
        {
            this.SerialNumber = reader.ReadInt32();
            length -= sizeof(int);
            int versionLen = reader.ReadUInt16();
            length -= sizeof(ushort);
            this.SoftwareVersion = Triplet.ReadString(reader, versionLen * 2);
            length -= versionLen * 2;
            this.DataVersion = Triplet.ReadString(reader, length);
        }

        /// <summary>
        /// Gets the triplet's payload length.
        /// </summary>
        public override int Length
        {
            get
            {
                return sizeof(int) + sizeof(ushort) + (this.SoftwareVersion.Length * 2) + (this.DataVersion.Length * 2);
            }
        }

        /// <summary>
        /// Gets or sets the serial number.
        /// </summary>
        public int SerialNumber { get; set; }

        /// <summary>
        /// Gets or sets the software version string.
        /// </summary>
        public string SoftwareVersion { get; set; }

        /// <summary>
        /// Gets or sets the data version string.
        /// </summary>
        public string DataVersion { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}, {1} / {2}", this.SerialNumber, this.SoftwareVersion, this.DataVersion);
        }

        /// <summary>
        /// Writes the payload to the given writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void WritePayload(BinaryWriter writer)
        {
            writer.Write(this.SerialNumber);
            writer.Write((ushort)this.SoftwareVersion.Length);
            writer.Write(Encoding.Unicode.GetBytes(this.SoftwareVersion));
            writer.Write(Encoding.Unicode.GetBytes(this.DataVersion));
        }
    }
}
