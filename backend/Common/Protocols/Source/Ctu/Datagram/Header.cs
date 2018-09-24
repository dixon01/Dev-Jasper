// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Header.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ctu.Datagram
{
    /// <summary>
    /// Representation of the CTU's header
    /// using an object oriented style.
    /// </summary>
    public class Header
    {
        /// <summary>
        /// The variable that represent the sequence number.
        /// </summary>
        private int sequenceNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="Header"/> class.
        /// It contains the default values for version 1.
        /// </summary>
        public Header()
        {
            this.VersionNumber = 1;
            this.Flags = HeaderFlags.LittleEndian;
        }

        /// <summary>
        /// Gets the amount of bytes occupied in memory by this CTU header.
        /// </summary>
        public int SizeOf
        {
            get
            {
                // 1 byte for the version number, 1 byte for the flag
                // and 2 bytes for the sequence number. In total 4 bytes.
                return 4;
            }
        }

        /// <summary>
        /// Gets or sets the CTU's header version number.
        /// </summary>
        public int VersionNumber { get; set; }

        /// <summary>
        /// Gets or sets the CTU's header flag.
        /// </summary>
        public HeaderFlags Flags { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the telegram is in little endian format (bit 0).
        /// </summary>
        public bool IsLittleEndian
        {
            get
            {
                return (this.Flags & HeaderFlags.LittleEndian) == HeaderFlags.LittleEndian;
            }

            set
            {
                this.Flags = (this.Flags & (~HeaderFlags.LittleEndian)) | (value ? HeaderFlags.LittleEndian : 0);
            }
        }

        /// <summary>
        /// Gets or sets the CTU's header sequence number.
        /// </summary>
        public int SequenceNumber
        {
            get
            {
                return this.sequenceNumber;
            }

            set
            {
                this.sequenceNumber = value % 0x10000;
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
            return string.Format("#{0}", this.sequenceNumber);
        }
    }
}
