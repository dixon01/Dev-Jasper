// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evTrafficLightCore.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evTrafficLightCore type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    using System.Xml.Serialization;

    /// <summary>
    /// This event is responsible for the request of traffic light priority bus=>LsaControl
    /// </summary>
    public class evTrafficLightCore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evTrafficLightCore"/> class.
        /// </summary>
        public evTrafficLightCore()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evTrafficLightCore"/> class.
        /// </summary>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="pointId">
        /// The point id.
        /// </param>
        public evTrafficLightCore(TrafficLightPosition position, int pointId)
        {
            this.PositionEnum = position;
            this.PointID = pointId;
        }

        /// <summary>
        /// Gets or sets the current state of in traffic light
        /// </summary>
        public int Position
        {
            get
            {
                return (int)this.PositionEnum;
            }
            set
            {
                this.PositionEnum = (TrafficLightPosition)value;
            }
        }

        /// <summary>
        /// Gets or sets the current state of in traffic light
        /// </summary>
        [XmlIgnore]
        public TrafficLightPosition PositionEnum { get; set; }

        /// <summary>
        /// Gets or sets the point id.
        /// </summary>
        public int PointID { get; set; }

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
                "evTrafficLightCore .TrafficLightPosition: {0}, .PointID: {1} ",
                this.Position,
                this.PointID);
        }
    }
}