// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IpAddressFieldBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IpAddressFieldBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Udcp.Fields
{
    using System.IO;
    using System.Net;

    /// <summary>
    /// Base class for all fields that contain an IP address value.
    /// </summary>
    public abstract class IpAddressFieldBase : Field
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IpAddressFieldBase"/> class.
        /// </summary>
        /// <param name="type">
        /// The field type.
        /// </param>
        protected IpAddressFieldBase(FieldType type)
            : base(type)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IpAddressFieldBase"/> class.
        /// </summary>
        /// <param name="type">
        /// The field type.
        /// </param>
        /// <param name="value">
        /// The IP address.
        /// </param>
        protected IpAddressFieldBase(FieldType type, IPAddress value)
            : base(type)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the IP address value.
        /// </summary>
        public IPAddress Value { get; private set; }

        /// <summary>
        /// Converts the given data to the value of this field.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        protected override void ReadContentFrom(byte[] data)
        {
            this.Value = new IPAddress(data);
        }

        /// <summary>
        /// Writes the length and the contents of this field to the given writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void WriteLengthAndContentTo(BinaryWriter writer)
        {
            var data = this.Value.GetAddressBytes();
            writer.Write((byte)data.Length);
            writer.Write(data);
        }
    }
}