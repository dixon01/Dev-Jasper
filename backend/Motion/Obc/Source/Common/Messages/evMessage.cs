// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evMessage.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Messages mainly used to display information of the system
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Messages mainly used to display information of the system
    /// </summary>
    public class evMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evMessage"/> class.
        /// </summary>
        public evMessage()
        {
            this.Destination = Destinations.Undef;
            this.MessageText = string.Empty;
            this.Message = Messages.Undef;
            this.SubType = SubTypes.Undef;
            this.MessageType = Types.Undef;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evMessage"/> class.
        /// </summary>
        /// <param name="messageId">Message ID as further reference defined by sender</param>
        /// <param name="messageType">Type of message</param>
        /// <param name="subType">Subtype of message (Error/Info...)</param>
        /// <param name="message">Predefined message ID</param>
        /// <param name="messageText">Message text</param>
        /// <param name="destination">Display destination</param>
        public evMessage(
            int messageId,
            Types messageType,
            SubTypes subType,
            Messages message,
            string messageText,
            Destinations destination)
        {
            this.MessageId = messageId;
            this.MessageType = messageType;
            this.SubType = subType;
            this.MessageText = messageText;
            this.Message = message;
            this.Destination = destination;
        }

        /// <summary>
        /// Type of message
        /// </summary>
        public enum Types
        {
            /// <summary>
            /// Predefined message ID is sent
            /// </summary>
            Predefined,

            /// <summary>
            /// Specific string is sent in the message
            /// </summary>
            Text,

            /// <summary>
            /// Undefined message.
            /// </summary>
            Undef
        }

        /// <summary>
        /// The sub types.
        /// </summary>
        public enum SubTypes
        {
            /// <summary>
            /// Error message
            /// </summary>
            Error,

            /// <summary>
            /// Instruction need acknowledge from driver
            /// </summary>
            Instruction,

            /// <summary>
            /// General information
            /// </summary>
            Info,

            /// <summary>
            /// Internal system message
            /// </summary>
            System,

            /// <summary>
            /// Undefined message.
            /// </summary>
            Undef
        }

        /// <summary>
        /// Definition of predefined messages sent to control center and back
        /// </summary>
        public enum Messages
        {
            /// <summary>
            /// Traffic jam.
            /// </summary>
            TrafficJam = 0x00,

            /// <summary>
            /// The vehicle failure.
            /// </summary>
            VehicleFailure = 0x01,

            /// <summary>
            /// The vehicle loss.
            /// </summary>
            VehicleLoss = 0x02,

            /// <summary>
            /// The derailing.
            /// </summary>
            Derailing = 0x03,

            /// <summary>
            /// The catenary failure.
            /// </summary>
            CatnaryFailure = 0x04,

            /// <summary>
            /// The slide danger.
            /// </summary>
            SlideDanger = 0x05,

            /// <summary>
            /// The replacement missing.
            /// </summary>
            ReplacementMissing = 0x06,

            /// <summary>
            /// The passenger neglect.
            /// </summary>
            PassengerNeglect = 0x07,

            /// <summary>
            /// The on time.
            /// </summary>
            OnTime = 0x08,

            /// <summary>
            /// The connection assured.
            /// </summary>
            ConnectionAssured = 0x09,

            /// <summary>
            /// The control on vehicle.
            /// </summary>
            Control = 0x0A,

            /// <summary>
            /// The failure repaired.
            /// </summary>
            FailureRepaired = 0x0B,

            /// <summary>
            /// The delay.
            /// </summary>
            Delay = 0x0C,

            /// <summary>
            /// The passenger number.
            /// </summary>
            PassengerNumber = 0x0D,

            /// <summary>
            /// The assure connection.
            /// </summary>
            AssureConnection = 0x10,

            /// <summary>
            /// The message understood.
            /// </summary>
            MessageUnderstood = 0x11,

            /// <summary>
            /// The slow down.
            /// </summary>
            SlowDown = 0x12,

            /// <summary>
            /// The call control center.
            /// </summary>
            CallControlCenter = 0x13,

            /// <summary>
            /// The enter car wash.
            /// </summary>
            EnterCarWash = 0x14,

            /// <summary>
            /// The FAK.
            /// </summary>
            FAK = 0x15,

            /// <summary>
            /// The vehicle arrives.
            /// </summary>
            VehicleArrives = 0x16,

            /// <summary>
            /// The police arrives.
            /// </summary>
            PoliceArrives = 0x17,

            /// <summary>
            /// The medic arrives.
            /// </summary>
            MedicArrives = 0x18,

            /// <summary>
            /// The wash vehicle.
            /// </summary>
            WashVehicle = 0x19,

            /// <summary>
            /// The wrong message.
            /// </summary>
            WrongMessage = 0x1A,

            /// <summary>
            /// The duty off requested.
            /// </summary>
            DutyOffRequested = 0x1B,

            /// <summary>
            /// The failure ticket canceler 1.
            /// </summary>
            FailureTicketCanceler1 = 0x20,

            /// <summary>
            /// The failure ticket canceler 2.
            /// </summary>
            FailureTicketCanceler2 = 0x21,

            /// <summary>
            /// The failure ticket canceler 3.
            /// </summary>
            FailureTicketCanceler3 = 0x22,

            /// <summary>
            /// The failure ticketing Krauth 1.
            /// </summary>
            FailureTicketing_Krauth_1 = 0x23,

            /// <summary>
            /// The failure ticketing Krauth 2.
            /// </summary>
            FailureTicketing_Krauth_2 = 0x24,

            /// <summary>
            /// The warning paper 1.
            /// </summary>
            WarningPaper1 = 0x25,

            /// <summary>
            /// The warning paper 2.
            /// </summary>
            WarningPaper2 = 0x26,

            /// <summary>
            /// The warning cash box 1.
            /// </summary>
            WarningCashBox1 = 0x27,

            /// <summary>
            /// The warning cash box 2.
            /// </summary>
            WarningCashBox2 = 0x28,

            /// <summary>
            /// The failure ticketing Atron 1.
            /// </summary>
            FailureTicketing_Atron_1 = 0x29,

            /// <summary>
            /// The failure ticketing Atron 2.
            /// </summary>
            FailureTicketing_Atron_2 = 0x2A,

            /// <summary>
            /// The failure ticketing Atron 3.
            /// </summary>
            FailureTicketing_Atron_3 = 0x2B,

            /// <summary>
            /// Undefined message.
            /// </summary>
            Undef = 0xff
        }

        /// <summary>
        /// Destination where to display
        /// </summary>
        [Flags]
        public enum Destinations : byte
        {
            /// <summary>
            /// The driver.
            /// </summary>
            Driver = 0x01,

            /// <summary>
            /// The control center.
            /// </summary>
            ControlCenter = 0x02,

            /// <summary>
            /// Undefined message.
            /// </summary>
            Undef = 0xff
        }

        /// <summary>
        /// Gets or sets the identifier for this message
        /// </summary>
        public int MessageId { get; set; }

        /// <summary>
        /// Gets or sets the type of the message. Defines if ASCII text is sent or predefined message is used
        /// </summary>
        public Types MessageType { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the message
        /// </summary>
        public SubTypes SubType { get; set; }

        /// <summary>
        /// Gets or sets the predefined message (used depending on chosen type)
        /// </summary>
        public Messages Message { get; set; }

        /// <summary>
        /// Gets or sets the ASCII text for the message (used depending on chosen type)
        /// </summary>
        public string MessageText { get; set; }

        /// <summary>
        /// Gets or sets the destination of output
        /// Property to access options as integer
        /// Serialization is not able to handle flags
        /// </summary>
        public byte DestinationFlags
        {
            get
            {
                return (byte)this.Destination;
            }
            set
            {
                this.Destination = (Destinations)value;
            }
        }

        /// <summary>
        /// Gets or sets the destination of output
        /// </summary>
        [XmlIgnore]
        public Destinations Destination { get; set; }
    }
}