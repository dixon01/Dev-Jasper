// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VehicleInfo.cs" company="Luminator LTG">
//   Copyright © 2011-2018 LuminatorLTG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VehicleInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Motion.Infomedia.Entities.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>The vehicle info.</summary>
    [Serializable]
    public class VehicleInfo 
    {
        /// <summary>Initializes a new instance of the <see cref="VehicleInfo"/> class.</summary>
        public VehicleInfo()
        {
            this.VehicleId = string.Empty;
            this.UnitNames = new List<string>();
        }

        /// <summary>Initializes a new instance of the <see cref="VehicleInfo"/> class.</summary>
        /// <param name="vehicleId">The vehicle id.</param>
        /// <param name="unitNames">The unit names.</param>
        public VehicleInfo(string vehicleId, List<string> unitNames)
        {
            this.UnitNames = unitNames;
            this.VehicleId = vehicleId;
        }

        public VehicleInfo(string vehicleId, string unitName)
        {
            this.VehicleId = vehicleId;
            this.UnitNames = new List<string> { unitName };
        }

        /// <summary>Gets or sets the vehicle id.</summary>
        public string VehicleId { get; set; }

        /// <summary>Gets or sets the unit names.</summary>
        public List<string> UnitNames { get; set; }

        public string FirstUnitName => this.UnitNames != null && this.UnitNames.Any() ? this.UnitNames.FirstOrDefault() : string.Empty;

        public override string ToString()
        {
            var names = this.UnitNames != null ? this.UnitNames.Aggregate(string.Empty, (current, m) => current + (m + ",")) : string.Empty;
            return $"VehicleId={this.VehicleId}, Units={names}";
        }
    }
}
