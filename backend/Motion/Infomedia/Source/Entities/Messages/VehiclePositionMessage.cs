// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator" file="VehiclePositionMessage.cs">
//   Copyright © 2011-2017 Luminator. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Entities.Messages
{
    using System;
    using System.Device.Location;
    using System.Globalization;

    /// <summary>The vehicle position message.</summary>
    [Serializable]
    public class VehiclePositionMessage : IVehiclePositionMessage
    {
        /// <summary>Initializes a new instance of the <see cref="VehiclePositionMessage"/> class.</summary>
        /// <param name="geoCoordinate">The geo coordinate.</param>
        public VehiclePositionMessage(GeoCoordinate geoCoordinate, string route = "", string trip="")
        {
            this.GeoCoordinate = geoCoordinate;
            this.Route = route;
            this.Trip = trip;
        }

        /// <summary>Initializes a new instance of the <see cref="VehiclePositionMessage" /> class.</summary>
        public VehiclePositionMessage()
        {
            this.GeoCoordinate = GeoCoordinate.Unknown;            
            this.Route = string.Empty;
            this.Trip = string.Empty;
        }

        /// <summary>Initializes a new instance of the <see cref="VehiclePositionMessage"/> class.</summary>
        /// <param name="latitudeDecimalDegrees">The latitude decimal degrees.</param>
        /// <param name="longitudeDecimalDegrees">The longitude decimal degrees.</param>
        /// <param name="altitude">The optional altitude.</param>
        /// <param name="route">The optional route.</param>
        /// <param name="trip">The optional trip.</param>
        public VehiclePositionMessage(
            string latitudeDecimalDegrees,
            string longitudeDecimalDegrees,
            string altitude = "0.0",
            string route = "",
            string trip = "")
            : this(new GeoCoordinate(
                    double.Parse(latitudeDecimalDegrees, CultureInfo.InvariantCulture),
                    double.Parse(longitudeDecimalDegrees, CultureInfo.InvariantCulture),
                    double.Parse(altitude, CultureInfo.InvariantCulture)), route, trip)
        {
        }

        /// <summary>Gets or sets the geo coordinate.</summary>
        public GeoCoordinate GeoCoordinate { get; set; }

        /// <summary>Gets a value indicating whether is valid.</summary>
        public bool IsValid => this.GeoCoordinate != null && this.GeoCoordinate.IsUnknown == false;

        /// <summary>Gets a value indicating the current Route if present.</summary>
        public string Route { get; set; }

        /// <summary>Gets a value indicating the current Trip if present.</summary>
        public string Trip { get; set; }

        /// <summary>The to string.</summary>
        /// <returns>The <see cref="string" />.</returns>
        public override string ToString()
        {
            if (this.GeoCoordinate != null)
            {
                return $"Lat={this.GeoCoordinate.Latitude}, Long={this.GeoCoordinate.Longitude}, Alt={this.GeoCoordinate.Altitude}, Route={this.Route}, Trip={this.Trip}";
            }

            return base.ToString();
        }
    }
}