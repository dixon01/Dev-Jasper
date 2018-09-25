// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evDuty.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evDuty type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Event to request Duty on and off from control center
    /// </summary>
    public class evDuty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evDuty"/> class.
        /// </summary>
        /// <param name="personelId">
        /// The personnel id.
        /// </param>
        /// <param name="personelPin">
        /// The personnel pin.
        /// </param>
        /// <param name="service">
        /// The service.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="option">
        /// The option.
        /// </param>
        /// <summary>
        /// Constructor for event
        /// </summary>
        public evDuty(string personelId, string personelPin, string service, Types type, Options option)
        {
            this.PersonelId = personelId;
            this.PersonelPin = personelPin;
            this.Service = service;
            this.Type = type;
            this.OptionEnum = option;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evDuty"/> class.
        /// </summary>
        public evDuty()
        {
            this.PersonelId = string.Empty;
            this.PersonelPin = string.Empty;
            this.Service = string.Empty;
            this.Type = Types.DutyOff;
            this.OptionEnum = Options.None;
        }

        /// <summary>
        /// Type defining the mode of duty on.
        /// </summary>
        public enum Types
        {
            /// <summary>
            /// Duty on for regular blocks.
            /// </summary>
            DutyOnRegular,

            /// <summary>
            /// Duty on for driver blocks.
            /// </summary>
            DutyOnDriver,

            /// <summary>
            /// Extra block (no trip data).
            /// </summary>
            DutyOnSpecialService,

            /// <summary>
            /// Duty off for regular blocks.
            /// </summary>
            DutyOff
        }

        /// <summary>
        /// Type defining different option for duty on
        /// The options can be used in combination (bit set)
        /// </summary>
        [Flags]
        public enum Options : byte
        {
            /// <summary>
            /// No option.
            /// </summary>
            None = 0x00,

            /// <summary>
            /// Driving school active.
            /// </summary>
            School = 0x01,

            /// <summary>
            /// Extension block.
            /// </summary>
            Extension = 0x02,

            /// <summary>
            /// Duty on forced by control center.
            /// </summary>
            Forced = 0x04,

            /// <summary>
            /// Duty on by system.
            /// </summary>
            System = 0x08
        }

        /// <summary>
        /// Gets or sets the personnel id of driver
        /// </summary>
        public string PersonelId { get; set; }

        /// <summary>
        /// Gets or sets the personnel id of driver
        /// </summary>
        public string PersonelPin { get; set; }

        /// <summary>
        /// Gets or sets the service
        /// </summary>
        public string Service { get; set; }

        /// <summary>
        /// Gets or sets the type of the login
        /// </summary>
        public Types Type { get; set; }

        /// <summary>
        /// Gets or sets the option for XML serialization.
        /// </summary>
        public byte Option
        {
            get
            {
                return (byte)this.OptionEnum;
            }

            set
            {
                this.OptionEnum = (Options)value;
            }
        }

        /// <summary>
        /// Gets or sets the option value.
        /// </summary>
        [XmlIgnore]
        public Options OptionEnum { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format(
                "evDuty. DriverID: {0}, Service: {1}, Type: {2}, Option: {3}",
                this.PersonelId,
                this.Service,
                this.Type,
                this.Option);
        }
    }
}