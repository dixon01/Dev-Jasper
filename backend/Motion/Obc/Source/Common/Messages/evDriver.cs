// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evDriver.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evDriver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// This event will be called from the VT3 by a driver login / logout. The driver id will be sent.
    /// If the driver id is 0, then it's an logout. At the moment the only consumer is the ergControl.
    /// </summary>
    public class evDriver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evDriver"/> class.
        /// </summary>
        public evDriver()
        {
            this.PersonalId = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evDriver"/> class.
        /// </summary>
        /// <param name="personalId">
        /// The personal id.
        /// </param>
        public evDriver(int personalId)
        {
            this.PersonalId = personalId;
        }

        /// <summary>
        /// Gets or sets the personal id of driver
        /// If driver id is 0, then it's an logout
        /// </summary>
        public int PersonalId { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return this.GetType().Name + ", PersonalID: " + this.PersonalId;
        }
    }
}
