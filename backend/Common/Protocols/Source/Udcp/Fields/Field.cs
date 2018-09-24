// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Field.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Field type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Udcp.Fields
{
    using System.IO;

    /// <summary>
    /// The base class for all UDCP fields.
    /// </summary>
    public abstract class Field
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Field"/> class.
        /// </summary>
        /// <param name="type">
        /// The field type.
        /// </param>
        protected Field(FieldType type)
        {
            this.Type = type;
        }

        /// <summary>
        /// Gets the field type.
        /// </summary>
        public FieldType Type { get; private set; }

        /// <summary>
        /// Creates a new field from the data at the current position in the reader.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// An object of a subclass of <see cref="Field"/>.
        /// </returns>
        public static Field CreateFrom(BinaryReader reader)
        {
            var type = reader.ReadByte();
            var field = Create((FieldType)type);
            var length = reader.ReadByte();
            var data = reader.ReadBytes(length);
            field.ReadContentFrom(data);
            return field;
        }

        /// <summary>
        /// Writes this field to the given writer.
        /// The first byte is the <see cref="Type"/>,
        /// the second byte the length of the contents,
        /// the following bytes are the contents of the field.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        internal void WriteTo(BinaryWriter writer)
        {
            writer.Write((byte)this.Type);
            this.WriteLengthAndContentTo(writer);
        }

        /// <summary>
        /// Converts the given data to the value of this field.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        protected abstract void ReadContentFrom(byte[] data);

        /// <summary>
        /// Writes the length and the contents of this field to the given writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected abstract void WriteLengthAndContentTo(BinaryWriter writer);

        private static Field Create(FieldType type)
        {
            switch (type)
            {
                case FieldType.UnitName:
                    return new UnitNameField();
                case FieldType.SoftwareVersion:
                    return new SoftwareVersionField();
                case FieldType.IpAddress:
                    return new IpAddressField();
                case FieldType.NetworkMask:
                    return new NetworkMaskField();
                case FieldType.Gateway:
                    return new GatewayField();
                case FieldType.DhcpEnabled:
                    return new DhcpEnabledField();
                case FieldType.ErrorCode:
                    return new ErrorCodeField();
                case FieldType.ErrorField:
                    return new ErrorFieldField();
                case FieldType.ErrorMessage:
                    return new ErrorMessageField();
                default:
                    return new UnknownField(type);
            }
        }
    }
}