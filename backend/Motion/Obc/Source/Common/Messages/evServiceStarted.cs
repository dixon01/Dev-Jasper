// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evServiceStarted.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evServiceStarted type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// The service started event.
    /// </summary>
    public class evServiceStarted
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evServiceStarted"/> class.
        /// </summary>
        /// <param name="service">Service number</param>
        /// <param name="specialDestination">Extra service or not</param>
        /// <param name="school">School mode activated or not</param>
        /// <param name="extensionCourse">Extension course or not</param>
        public evServiceStarted(int service, bool specialDestination, bool school, bool extensionCourse)
        {
            this.Service = service;
            this.ExtraService = specialDestination;
            this.School = school;
            this.ExtensionCourse = extensionCourse;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evServiceStarted"/> class.
        /// </summary>
        public evServiceStarted()
        {
        }

        /// <summary>
        /// Gets or sets the new service number
        /// </summary>
        public int Service { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is an extra service.
        /// </summary>
        public bool ExtraService { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is a school drive.
        /// </summary>
        public bool School { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether extension course mode is activated
        /// </summary>
        public bool ExtensionCourse { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return this.GetType() + ", Service: " + this.Service;
        }
    }
}