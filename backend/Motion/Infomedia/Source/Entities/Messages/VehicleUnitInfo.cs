// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VehicleUnitInfo.cs" company="Luminator LTG">
//   Copyright © 2011-2018 LuminatorLTG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VehicleUnitInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Entities.Messages
{
    using System;
    using System.Device.Location;
    using System.Linq;

    public class VehicleUnitInfoRequest : VehicleUnitInfo
    {
    }

    [Serializable]
    public class VehicleUnitInfo
    {
        public VehicleUnitInfo()
        {
            this.VehicleInfo = new VehicleInfo();
            this.VehiclePosition = new VehiclePositionMessage();
        }

        public VehicleUnitInfo(VehicleInfo vehicleInfo, VehiclePositionMessage vehiclePosition)
        {
            this.VehicleInfo = vehicleInfo;
            this.VehiclePosition = vehiclePosition;
        }

        public VehicleInfo VehicleInfo { get; set; }

        public string FirstUnitName => VehicleInfo?.UnitNames != null ? this.VehicleInfo.UnitNames.FirstOrDefault() : string.Empty;

        public VehiclePositionMessage VehiclePosition { get; set; }

        public bool IsValidVehiclePositionMessage => this.VehiclePosition != null && this.VehiclePosition.GeoCoordinate != null;

        public bool IsKnownGpsPosition => this.IsValidVehiclePositionMessage
                                          && this.VehiclePosition.GeoCoordinate != GeoCoordinate.Unknown;

        public bool IsValidVehicleInfo => this.IsValidVehicleId && this.VehicleInfo?.UnitNames != null && this.VehicleInfo.UnitNames.Any();

        public bool IsValidVehicleId => string.IsNullOrEmpty(this.VehicleInfo?.VehicleId) == false;

        public bool IsValidRoute => string.IsNullOrEmpty(this.VehiclePosition?.Route) == false;

        public override string ToString()
        {
            return $"VehicleInfo={this.VehicleInfo}, VehiclePosition={this.VehiclePosition}";
        }
    }
}
