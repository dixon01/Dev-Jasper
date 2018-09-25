// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ByteFieldBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ByteFieldBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Udcp.Fields
{
    using System.IO;

    /// <summary>
    /// Base class for all fields that contain a single byte value.
    /// </summary>
    public abstract class ByteFieldBase : Field
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ByteFieldBase"/> class.
        /// </summary>
        /// <param name="type">
        /// The field type.
        /// </param>
        protected ByteFieldBase(FieldType type)
            : base(type)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByteFieldBase"/> class.
        /// </summary>
        /// <param name="type">
        /// The field type.
        /// </param>
        /// <param name="value">
        /// The byte value.
        /// </param>
        protected ByteFieldBase(FieldType type, byte value)
            : base(type)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the byte value. Subclasses should provide a public read-only property
        /// forwarding the request to this property.
        /// </summary>
        protected byte Value { get; private set; }

        /// <summary>
        /// Converts the given data to the value of this field.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        protected override void ReadContentFrom(byte[] data)
        {
            if (data.Length > 0)
            {
                this.Value = data[0];
            }
        }

        /// <summary>
        /// Writes the length and the contents of this field to the given writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void WriteLengthAndContentTo(BinaryWriter writer)
        {
            writer.Write((byte)1);
            writer.Write(this.Value);
        }
    }
}