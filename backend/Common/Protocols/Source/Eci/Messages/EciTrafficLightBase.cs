// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EciTrafficLightBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Messages
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The traffic light base.
    /// </summary>
    public abstract class EciTrafficLightBase : EciMessageBase
    {
        /// <summary>
        /// Gets the sub type.
        /// </summary>
        public abstract EciTrafficLightCode SubType { get; }

        /// <summary>
        /// Gets the message type.
        /// </summary>
        public override EciMessageCode MessageType
        {
            get
            {
                return EciMessageCode.TrafficLight;
            }
        }

        /// <summary>
        /// Gets or sets the inter section id.
        /// </summary>
        public int IntersectionId { get; set; }

        /// <summary>
        /// Gets or sets the route id.
        /// </summary>
        public int RouteId { get; set; }

        /// <summary>
        /// Gets or sets the GPS time stamp.
        /// </summary>
        public DateTime GpsUtcTimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the speed in m/s.
        /// </summary>
        public double Speed { get; set; }

        /// <summary>
        /// Gets or sets the speed in Km/s.
        /// </summary>
        [XmlIgnore]
        public double SpeedKmS
        {
            get
            {
                return this.Speed / 3.6;
            }

            set
            {
                this.Speed = value * 3.6;
            }
        }
    }
}
