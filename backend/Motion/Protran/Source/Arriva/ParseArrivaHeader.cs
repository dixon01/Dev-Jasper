// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParseArrivaHeader.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   COS: 24 November 2010
//   The information requested when a "EvtReqStatusInfo" is received.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Arriva
{
    using NLog;

    #region ENUMS
    /// <summary>
    /// COS: 24 November 2010
    /// The information requested when a <code>EvtReqStatusInfo</code> is received.
    /// </summary>
    public enum InfoRequested
    {
        /// <summary>
        /// Value for null info
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Value for status info
        /// </summary>
        Status = 0x01,

        /// <summary>
        /// Value for version info
        /// </summary>
        Version = 0x02
    }
    #endregion ENUMS

    /// <summary>
    /// Class that creates the object representation of binary buffers
    /// </summary>
    public class ParseArrivaHeader
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public byte[] Msg { get; set; }

        #region PUBLIC_FUNCTIONS
        /// <summary>
        /// Checks the header of an Arriva's message.
        /// </summary>
        /// <returns>The Arriva's header in a suitable object.</returns>
        public ArrivaHeaderData CheckHeader()
        {
            var ahd = new ArrivaHeaderData();
            ahd.MessageCode = -1;
            if (this.Msg.Length < 32)
            {
                Logger.Info("Invalid arriva header data {0}", ahd);
                return ahd;
            }

            if (!this.CheckSenderAndReceiver())
            {
                return ahd;
            }

            if (!this.CheckProtocolVersion())
            {
                return ahd;
            }

            ahd.MessageCode = this.GetMessageCode();
            this.GetMessageTransactionId(ref ahd);
            return ahd;
        }

        /// <summary>
        /// COS: 24 November 2010
        /// Gets the desired message type analyzing a buffer representing
        /// a <code>EvtReqStatusInfo</code> packet.
        /// </summary>
        /// <returns>The desired message type, or "Unknown" if the packet is not
        /// well formatted.</returns>
        public InfoRequested GetInfoRequested()
        {
            if (this.Msg == null || this.Msg.Length != 34)
            {
                // this message is not a suitable "EvtReqStatusInfo" message.
                return InfoRequested.None;
            }

            byte versionByte = this.Msg[32];
            if (versionByte != 1)
            {
                // error. the "EvtReqStatusInfo" must have an 1 into this position
                return InfoRequested.None;
            }

            byte desiredMsgType = this.Msg[33];
            if (desiredMsgType != 1 && desiredMsgType != 2)
            {
                // error. the "EvtReqStatusInfo" must have an 1 or an 2 into this position
                return InfoRequested.None;
            }

            return (desiredMsgType == 1) ? InfoRequested.Status : InfoRequested.Version;
        }
        #endregion PUBLIC_FUNCTIONS

        #region PRIVATE_FUNCTIONS
        private void GetMessageTransactionId(ref ArrivaHeaderData ahd)
        {
            if (ahd == null || this.Msg == null)
            {
                // invalid param.
                return;
            }

            ahd.MessageTransactionId1 = this.Msg[8];
            ahd.MessageTransactionId2 = this.Msg[9];
            ahd.MessageTransactionId3 = this.Msg[10];
            ahd.MessageTransactionId4 = this.Msg[11];
        }

        private bool CheckSenderAndReceiver()
        {
            if (this.Msg == null || this.Msg.Length == 0)
            {
                // invalid buffer.
                return false;
            }

            if (this.Msg[0] != 0x64 || this.Msg[1] != 0x32)
            {
                // invalid values.
                return false;
            }

            return true;
        }

        private bool CheckProtocolVersion()
        {
            if (this.Msg == null || this.Msg.Length == 0)
            {
                // invalid buffer.
                return false;
            }

            if (this.Msg[14] == 0x10 && this.Msg[15] == 0x00)
            {
                return true;
            }

            return false;
        }

        private int GetMessageCode()
        {
            int value = this.Msg[6] << 8;
            value += this.Msg[7];
            return value;
        }
        #endregion PRIVATE_FUNCTIONS
    }
}
