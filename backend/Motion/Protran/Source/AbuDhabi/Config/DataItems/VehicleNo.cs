// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VehicleNo.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Config.DataItems
{
    using System;

    /// <summary>
    /// Config class for Vehicle number config item.
    /// </summary>
    [Serializable]
    public class VehicleNo : DataItemConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleNo"/> class.
        /// </summary>
        public VehicleNo()
        {
            this.TopboxSerialNumberFile = @"D:\Progs\Protran\VehicleNoscumTopBoxSN.csv";
        }

        /// <summary>
        /// Gets or sets the location of the topbox serial number CSV file.
        /// Default value is: <code>D:\Progs\Protran\VehicleNoscumTopBoxSN.csv</code>
        /// </summary>
        public string TopboxSerialNumberFile { get; set; }
    }
}
