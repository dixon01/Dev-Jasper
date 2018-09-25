// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeviceInfoRequest.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DeviceInfoRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ctu.Requests
{
    using System.IO;

    using Gorba.Common.Protocols.Ctu.Datagram;

    /// <summary>
    /// Device info request triplet.
    /// </summary>
    public class DeviceInfoRequest : Triplet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceInfoRequest"/> class.
        /// </summary>
        public DeviceInfoRequest()
            : base(TagName.DeviceInfoRequest)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceInfoRequest"/> class.
        /// </summary>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <param name="reader">
        /// The reader.
        /// </param>
        public DeviceInfoRequest(int length, BinaryReader reader)
            : base(TagName.DeviceInfoRequest)
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
            return string.Empty;
        }

        /// <summary>
        /// Writes the payload to the given writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void WritePayload(BinaryWriter writer)
        {
            // empty triplet
        }
    }
}
