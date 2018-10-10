// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringFieldBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StringFieldBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Udcp.Fields
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Base class for all fields that contain a single byte value.
    /// </summary>
    public abstract class StringFieldBase : Field
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringFieldBase"/> class.
        /// </summary>
        /// <param name="type">
        /// The field type.
        /// </param>
        protected StringFieldBase(FieldType type)
            : base(type)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringFieldBase"/> class.
        /// </summary>
        /// <param name="type">
        /// The field type.
        /// </param>
        /// <param name="value">
        /// The string value.
        /// </param>
        /// <exception cref="ArgumentException">
        /// if the <paramref name="value"/> is longer than 255 characters.
        /// </exception>
        protected StringFieldBase(FieldType type, string value)
            : base(type)
        {
            if (Encoding.UTF8.GetByteCount(value) > byte.MaxValue)
            {
                throw new ArgumentException("Value can't be more than " + byte.MaxValue + " characters long");
            }

            this.Value = value;
        }

        /// <summary>
        /// Gets the string value.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Converts the given data to the value of this field.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        protected override void ReadContentFrom(byte[] data)
        {
            this.Value = Encoding.UTF8.GetString(data, 0, data.Length);
        }

        /// <summary>
        /// Writes the length and the contents of this field to the given writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void WriteLengthAndContentTo(BinaryWriter writer)
        {
            var data = Encoding.UTF8.GetBytes(this.Value);
            writer.Write((byte)data.Length);
            writer.Write(data);
        }
    }
}