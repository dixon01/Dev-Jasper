// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnetEventMessage.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implementation of the qnet vdv messages
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System;

    /// <summary>
    /// Implementation of the qnet vdv messages
    /// </summary>
    public class QnetEventMessage : QnetMessageBase
    {
        /// <summary>
        /// Contains data of the qnet message for iqube command message
        /// </summary>
        private readonly QnetEventStruct qnetEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="QnetEventMessage"/> class. Used for QMAIL with TFTP protocol.
        /// </summary>
        /// <param name="srcAddr">
        /// The qnet source address of the sender of the message.
        /// </param>
        /// <param name="destAddr">
        /// The qnet destination address of the message.
        /// </param>
        /// <param name="gatewayAddress">
        /// The gateway Address.
        /// </param>
        /// <remarks>
        /// This message MUST be acknowledged by the recipient.
        /// </remarks>
        public QnetEventMessage(ushort srcAddr, ushort destAddr, ushort gatewayAddress)
            : base(srcAddr, destAddr, gatewayAddress)
        {
            this.qnetEvent = new QnetEventStruct();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QnetEventMessage"/> class.
        /// The source and destination addresses are set with <see cref="QnetConstantes.QnetAddrNone"/> by default.
        /// </summary>
        public QnetEventMessage()
            : this(QnetConstantes.QnetAddrNone, QnetConstantes.QnetAddrNone, QnetConstantes.QnetAddrNone)
        {
        }

        /// <summary>
        /// Gets or sets the code of the event (legacy code = EventId).
        /// </summary>
        public EventCode EventCode { get; set; }

        /// <summary>
        /// Gets or sets the class of the alarm. See <see cref="AlarmClass"/> enumeration for available values.
        /// </summary>
        public AlarmClass AlarmClass { get; set; }

        /// <summary>
        /// Gets or sets the date/time when the alarm occurred.
        /// </summary>
        public DateTime EventStamp { get; set; }

        /// <summary>
        /// Gets or sets the attribute (sub type) of the event. Have a look to <see cref="AttributeCode"/> to see the
        /// entire list of available values.
        /// </summary>
        public byte Attribute { get; set; }

        /// <summary>
        /// Gets or sets the Name of the event.
        /// <remarks>
        /// The FileName is used for alarm to indicate the unique identifier of the alarm.</remarks>
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the main parameter of the event.
        /// </summary>
        public byte Param { get; set; }

        /// <summary>
        /// Gets or sets the parameter number 1 of the event
        /// </summary>
        public ushort Param1 { get; set; }

        /// <summary>
        /// Gets or sets the parameter number 2 of the event
        /// </summary>
        public ushort Param2 { get; set; }

        /// <summary>
        /// Gets or sets the parameter number 3 of the event
        /// </summary>
        public ushort Param3 { get; set; }

        /// <summary>
        /// Gets the qnet message for iqube command message.
        /// </summary>
        public QnetEventStruct QmailMessage
        {
            get
            {
                return this.qnetEvent;
            }
        }
    }
}