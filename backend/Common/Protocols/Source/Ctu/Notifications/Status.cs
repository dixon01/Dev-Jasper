// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Status.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ctu.Notifications
{
    using System.IO;
    using System.Text;

    using Gorba.Common.Protocols.Ctu.Datagram;

    /// <summary>
    /// Object tasked to represent the notification
    /// CTU datagram regarding the "Status",
    /// uniquely identified by the tag number 1.
    /// </summary>
    public class Status : Triplet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Status"/> class.
        /// </summary>
        /// <param name="code">The current status code</param>
        /// <param name="statusMsg">The string to be inserted into the CTU datagram's triplet payload.</param>
        public Status(StatusCode code, string statusMsg)
            : base(TagName.Status)
        {
            this.Code = code;
            this.StatusMsg = statusMsg;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Status"/> class.
        /// </summary>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <param name="reader">
        /// The reader.
        /// </param>
        internal Status(int length, BinaryReader reader)
            : base(TagName.Status)
        {
            this.Code = (StatusCode)reader.ReadInt32();
            length -= 4;
            this.StatusMsg = Triplet.ReadString(reader, length);
        }

        /// <summary>
        /// Gets or sets the value's code.
        /// </summary>
        public StatusCode Code { get; set; }

        /// <summary>
        /// Gets or sets the value's status message.
        /// </summary>
        public string StatusMsg { get; set; }

        /// <summary>
        /// Gets the triplet's payload length.
        /// </summary>
        public override int Length
        {
            get
            {
                return sizeof(int) + (this.StatusMsg.Length * 2);
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
            return string.Format("{0}: {1}", this.Code, this.StatusMsg);
        }

        /// <summary>
        /// Writes the payload to the given writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void WritePayload(BinaryWriter writer)
        {
            writer.Write((int)this.Code);
            if (!string.IsNullOrEmpty(this.StatusMsg))
            {
                writer.Write(Encoding.Unicode.GetBytes(this.StatusMsg));
            }
        }
    }
}
