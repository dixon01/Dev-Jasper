// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpecialInputInfo.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ctu.Notifications
{
    using System.IO;

    using Gorba.Common.Protocols.Ctu.Datagram;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SpecialInputInfo : Triplet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpecialInputInfo"/> class.
        /// </summary>
        /// <param name="specialInputState">
        /// The special input state.
        /// </param>
        public SpecialInputInfo(bool specialInputState)
            : base(TagName.SpecialInputInfo)
        {
            this.SpecialInputState = specialInputState;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecialInputInfo"/> class.
        /// </summary>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <param name="reader">
        /// The reader.
        /// </param>
        internal SpecialInputInfo(int length, BinaryReader reader)
            : base(TagName.SpecialInputInfo)
        {
            this.SpecialInputState = reader.ReadBoolean();
        }

        /// <summary>
        /// Gets or sets a value indicating whether special input state.
        /// </summary>
        public bool SpecialInputState { get; set; }

        /// <summary>
        /// Gets the length.
        /// </summary>
        public override int Length
        {
            get
            {
                return sizeof(bool);
            }
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}", this.SpecialInputState);
        }

        /// <summary>
        /// The write payload.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void WritePayload(BinaryWriter writer)
        {
            writer.Write(this.SpecialInputState);
        }
    }
}
