// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BoolFieldBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BoolFieldBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Udcp.Fields
{
    using System.IO;

    /// <summary>
    /// Base class for all fields that contain a boolean value.
    /// </summary>
    public abstract class BoolFieldBase : Field
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoolFieldBase"/> class.
        /// </summary>
        /// <param name="type">
        /// The field type.
        /// </param>
        protected BoolFieldBase(FieldType type)
            : base(type)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoolFieldBase"/> class.
        /// </summary>
        /// <param name="type">
        /// The field type.
        /// </param>
        /// <param name="value">
        /// The boolean value.
        /// </param>
        protected BoolFieldBase(FieldType type, bool value)
            : base(type)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets a value indicating whether the boolean value of this field is set.
        /// </summary>
        public bool Value { get; private set; }

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
                this.Value = data[0] != 0;
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
            writer.Write((byte)(this.Value ? 1 : 0));
        }
    }
}