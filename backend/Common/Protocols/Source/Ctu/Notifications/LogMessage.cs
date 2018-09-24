// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogMessage.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LogMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ctu.Notifications
{
    using System.IO;
    using System.Text;

    using Gorba.Common.Protocols.Ctu.Datagram;

    /// <summary>
    /// Log message triplet.
    /// </summary>
    public class LogMessage : Triplet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessage"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public LogMessage(string message)
            : base(TagName.LogMessage)
        {
            this.Message = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessage"/> class.
        /// </summary>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <param name="reader">
        /// The reader.
        /// </param>
        internal LogMessage(int length, BinaryReader reader)
            : base(TagName.LogMessage)
        {
            this.Message = Triplet.ReadString(reader, length);
        }

        /// <summary>
        /// Gets or sets the log message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets the triplet's payload length.
        /// </summary>
        public override int Length
        {
            get
            {
                return this.Message.Length * 2;
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
            return this.Message;
        }

        /// <summary>
        /// Writes the payload to the given writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void WritePayload(BinaryWriter writer)
        {
            writer.Write(Encoding.Unicode.GetBytes(this.Message));
        }
    }
}
