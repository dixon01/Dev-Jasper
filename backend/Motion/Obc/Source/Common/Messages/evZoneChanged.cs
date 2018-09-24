// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evZoneChanged.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evZoneChanged type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// Raised when the billing zone just changed
    /// </summary>
    public class evZoneChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evZoneChanged"/> class.
        /// </summary>
        public evZoneChanged()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evZoneChanged"/> class.
        /// </summary>
        /// <param name="zoneId">
        /// The zone id.
        /// </param>
        public evZoneChanged(int zoneId)
        {
            this.ZoneId = zoneId;
        }

        /// <summary>
        /// Gets or sets the ticketing zone id.
        /// </summary>
        public int ZoneId { get; set; }
    }
}