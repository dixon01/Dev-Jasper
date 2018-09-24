// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BusConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Common.Configuration.Obc.Bus
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The bus.exe configuration.
    /// </summary>
    [Serializable]
    [XmlRoot("Bus")]
    public class BusConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BusConfig"/> class.
        /// </summary>
        public BusConfig()
        {
            this.Schedule = new ScheduleConfig();
            this.CenterClient = new CenterClientConfig();
            this.BusInfo = new BusInfo();
        }

        /// <summary>
        /// Gets or sets the schedule configuration.
        /// </summary>
        public ScheduleConfig Schedule { get; set; }

        /// <summary>
        /// Gets or sets the center client configuration.
        /// </summary>
        public CenterClientConfig CenterClient { get; set; }

        /// <summary>
        /// Gets or sets the bus id and its type.
        /// </summary>
        public BusInfo BusInfo { get; set; }
    }
}
