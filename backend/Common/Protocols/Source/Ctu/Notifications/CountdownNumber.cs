// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CountdownNumber.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CountdownNumber type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ctu.Notifications
{
    using System.IO;

    using Gorba.Common.Protocols.Ctu.Datagram;

    /// <summary>
    /// Object tasked to represent the countdown number,
    /// uniquely identified by the tag number 105.
    /// </summary>
    public class CountdownNumber : Triplet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CountdownNumber"/> class.
        /// </summary>
        public CountdownNumber()
            : base(TagName.CountdownNumber)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CountdownNumber"/> class.
        /// </summary>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <param name="reader">
        /// The reader.
        /// </param>
        public CountdownNumber(int length, BinaryReader reader)
            : base(TagName.CountdownNumber)
        {
            this.Number = reader.ReadSByte();
        }

        /// <summary>
        /// Gets or sets the countdown number.
        /// </summary>
        public sbyte Number { get; set; }

        /// <summary>
        /// Gets the triplet's payload length.
        /// </summary>
        public override int Length
        {
            get
            {
                return sizeof(sbyte);
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
            return string.Format("{0}", this.Number);
        }

        /// <summary>
        /// Subclasses have to implement this method to write their
        /// respective payload to the given writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void WritePayload(BinaryWriter writer)
        {
            writer.Write(this.Number);
        }
    }
}
