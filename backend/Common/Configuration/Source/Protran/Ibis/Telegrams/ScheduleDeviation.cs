// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScheduleDeviation.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis.Telegrams
{
    using System;

    /// <summary>
    /// Schedule Deviation is the container of the configuration for the deviation
    /// </summary>
    [Serializable]
    public class ScheduleDeviation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleDeviation"/> class with the default values.
        /// </summary>
        public ScheduleDeviation()
        {
            this.OnTime = "OK";
            this.Ahead = "-{0}";
            this.Delayed = "+{0}";
        }

        /// <summary>
        /// Gets or sets OnTime deviation.
        /// </summary>
        public string OnTime { get; set; }

        /// <summary>
        /// Gets or sets Ahead deviation.
        /// </summary>
        public string Ahead { get; set; }

        /// <summary>
        /// Gets or sets Delayed deviation.
        /// </summary>
        public string Delayed { get; set; }
    }
}
