// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evDutyAck.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evDutyAck type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The duty acknowledge event.
    /// </summary>
    public class evDutyAck
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evDutyAck"/> class.
        /// </summary>
        /// <param name="response">
        /// The response.
        /// </param>
        public evDutyAck(Acks response)
        {
            this.Response = response;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evDutyAck"/> class.
        /// </summary>
        public evDutyAck()
        {
            this.Response = Acks.UnknownError;
        }

        /// <summary>
        /// Defines the possible answers to a duty on
        /// </summary>
        [Flags]
        public enum Acks
        {
            /// <summary>
            /// The ok.
            /// </summary>
            OK = 0,

            /// <summary>
            /// The vehicle is invalid.
            /// </summary>
            InvalidVehicle = 0x1,

            /// <summary>
            /// The block number is invalid
            /// </summary>
            InvalidBlock = 0x2,

            /// <summary>
            /// The driver number is invalid.
            /// </summary>
            InvalidDriver = 0x4,

            /// <summary>
            /// The block is already assigned.
            /// </summary>
            BlockAlreadyAssigned = 0x8,

            /// <summary>
            /// The bus is already assigned.
            /// </summary>
            BusAlreadyAssigned = 0x20,

            /// <summary>
            /// The driver is already assigned.
            /// </summary>
            DriverAlreadyAssigned = 0x40,

            /// <summary>
            /// The system error.
            /// </summary>
            SystemError = 0x80,

            /// <summary>
            /// The unknown error.
            /// </summary>
            UnknownError = -1
        }

        /// <summary>
        /// Gets or sets the response flags in short
        /// Serialization is not able to handle flags
        /// </summary>
        public short ResponseFlags
        {
            get
            {
                return (short)this.Response;
            }

            set
            {
                this.Response = (Acks)value;
            }
        }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        [XmlIgnore]
        public Acks Response { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return "evDutyAck. Response: " + this.Response;
        }
    }
}