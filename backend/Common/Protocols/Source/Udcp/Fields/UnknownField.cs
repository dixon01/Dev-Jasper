// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownField.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnknownField type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Udcp.Fields
{
    using System.IO;

    /// <summary>
    /// Special field that is used to represent unknown fields received.
    /// </summary>
    public class UnknownField : Field
    {
        private byte[] value;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownField"/> class.
        /// </summary>
        /// <param name="type">
        /// The field type.
        /// </param>
        internal UnknownField(FieldType type)
            : base(type)
        {
        }

        /// <summary>
        /// Converts the given data to the value of this field.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        protected override void ReadContentFrom(byte[] data)
        {
            this.value = data;
        }

        /// <summary>
        /// Writes the length and the contents of this field to the given writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void WriteLengthAndContentTo(BinaryWriter writer)
        {
            writer.Write((byte)this.value.Length);
            writer.Write(this.value);
        }
    }
}