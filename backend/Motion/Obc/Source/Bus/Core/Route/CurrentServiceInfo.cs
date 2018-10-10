// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CurrentServiceInfo.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Motion.Obc.Bus.Core.Route
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about the current service. This object is used for persistence.
    /// </summary>
    [Serializable]
    public class CurrentServiceInfo
    {
        private DateTime date;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentServiceInfo"/> class.
        /// </summary>
        public CurrentServiceInfo()
        {
            this.Services = new List<ServiceInfo>();
        }

        /// <summary>
        /// Gets or sets the driver id.
        /// </summary>
        public int DriverId { get; set; }

        /// <summary>
        /// Gets or sets the service state.
        /// </summary>
        public ServiceState ServiceState { get; set; }

        /// <summary>
        /// Gets or sets the date when the service was approved.
        /// </summary>
        public DateTime Date
        {
            get
            {
                return this.date;
            }

            set
            {
                // make sure we only store the date
                this.date = value.Date;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the service is validated.
        /// </summary>
        public bool IsValidated { get; set; }

        /// <summary>
        /// Gets or sets the list of services.
        /// </summary>
        public List<ServiceInfo> Services { get; set; }
    }
}
