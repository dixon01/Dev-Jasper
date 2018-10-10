// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BusInfo.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Bus
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The bus info.
    /// </summary>
    [Serializable]
    public class BusInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BusInfo"/> class.
        /// </summary>
        public BusInfo()
        {
            this.VehicleId = 0;
            this.VehicleType = VehicleTypeEnum.Unknow;
            this.ConfigType = 0;
        }

        /// <summary>
        /// Gets or sets the bus number
        /// </summary>
        [XmlElement("VehicleId")]
        public int VehicleId { get; set; }

        /// <summary>
        /// Gets or sets the bus type (normal floor, low floor, …)
        /// </summary>
        [XmlElement("VehicleType")]
        public VehicleTypeEnum VehicleType { get; set; }

        /// <summary>
        /// Gets or sets the config type
        /// </summary>
        [XmlElement("ConfigType")]
        public int ConfigType { get; set; }
    }
}
