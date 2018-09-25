// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArrivaHeaderData.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ArrivaHeaderData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Arriva
{
    /// <summary>
    /// Represents header of binary buffer
    /// </summary>
    public class ArrivaHeaderData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrivaHeaderData"/> class.
        /// Allocates all the resources needed by this object.
        /// </summary>
        public ArrivaHeaderData()
        {
            this.Clear();
        }

        /// <summary>
        /// Gets or sets MessageCode.
        /// </summary>
        public int MessageCode { get; set; }

        /// <summary>
        /// Gets or sets MessageTransactionID1.
        /// </summary>
        public byte MessageTransactionId1 { get; set; }

        /// <summary>
        /// Gets or sets MessageTransactionID2.
        /// </summary>
        public byte MessageTransactionId2 { get; set; }

        /// <summary>
        /// Gets or sets MessageTransactionID3.
        /// </summary>
        public byte MessageTransactionId3 { get; set; }

        /// <summary>
        /// Gets or sets MessageTransactionID4.
        /// </summary>
        public byte MessageTransactionId4 { get; set; }

        /// <summary>
        /// Clears the message with the right values to do so.
        /// </summary>
        public void Clear()
        {
            this.MessageCode = -1;
            this.MessageTransactionId1 = 0x00;
            this.MessageTransactionId2 = 0x00;
            this.MessageTransactionId3 = 0x00;
            this.MessageTransactionId4 = 0x00;
        }

        /// <summary>
        /// Sets a specific Id to this message.
        /// </summary>
        /// <param name="id">
        /// The ID.
        /// </param>
        public void SetMessageTransactionId(int id)
        {
            var highInt = this.HighWord((uint)id);
            var lowInt = this.LowWord((uint)id);

            this.MessageTransactionId1 = this.HighByte(highInt);
            this.MessageTransactionId2 = this.LowByte(highInt);
            this.MessageTransactionId3 = this.HighByte(lowInt);
            this.MessageTransactionId4 = this.LowByte(lowInt);
        }

        private ushort LowWord(uint value)
        {
            return (ushort)(value & 0xFFFF);
        }

        private ushort HighWord(uint value)
        {
            return (ushort)(value >> 16);
        }

        private byte LowByte(ushort value)
        {
            return (byte)(value & 0xFF);
        }

        private byte HighByte(ushort value)
        {
            return (byte)(value >> 8);
        }
    }
}
