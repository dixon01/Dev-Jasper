// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtraService.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ExtraService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// The extra service data.
    /// </summary>
    public class ExtraService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtraService"/> class.
        /// </summary>
        public ExtraService()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtraService"/> class.
        /// </summary>
        /// <param name="service">
        /// The service.
        /// </param>
        /// <param name="destinationCode">
        /// The destination code.
        /// </param>
        public ExtraService(int service, int destinationCode)
        {
            this.Service = service;
            this.DestinationCode = destinationCode;
        }

        /// <summary>
        /// Gets or sets the current extra service number
        /// </summary>
        public int Service { get; set; }

        /// <summary>
        /// Gets or sets the current destination code
        /// </summary>
        public int DestinationCode { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return this.GetType() + ", Service: " + this.Service + ", Destination: " + this.DestinationCode;
        }
    }
}