// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ehConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ehConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    using System.Diagnostics.CodeAnalysis;
    using Gorba.Common.Configuration.Obc.Bus;

    /// <summary>
    /// The bus configuration.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
        Justification = "Reviewed. Suppression is OK here.")]
    public class ehConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ehConfig"/> class.
        /// </summary>
        public ehConfig()
        {
            this.VehicleType = VehicleTypeEnum.Unknow;
            this.VehicleId = 0;
            this.DeviceId = 0;
            this.ConfigType = 0;
        }

        /// <summary>
        /// Gets or sets the config type of the bus. Was important in the French code.
        /// In the new Gorba code this parameter is obsolete.
        /// </summary>
        public int ConfigType { get; set; }

        /// <summary>
        /// Gets or sets the device (OBC) number
        /// </summary>
        public int DeviceId { get; set; }

        /// <summary>
        /// Gets or sets the bus number
        /// </summary>
        public int VehicleId { get; set; }

        /// <summary>
        /// Gets or sets the bus type (normal floor, low floor, …)
        /// </summary>
        public VehicleTypeEnum VehicleType { get; set; }

        /// <summary>
        /// Gets or sets the day kind.
        /// </summary>
        public int DayKind { get; set; }

        /// <summary>
        /// Allow to know if the config has been sent from Bus.exe
        /// </summary>
        /// <returns>
        /// True if it was received from Medi.
        /// </returns>
        public bool IsValid()
        {
            return this.DeviceId != 0;
        }
    }
}
