// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateRegistration.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateRegistration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.Medi.Messages
{
    using System.Collections.Generic;

    /// <summary>
    /// The update registration message.
    /// </summary>
    public class UpdateRegistration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateRegistration"/> class.
        /// </summary>
        public UpdateRegistration()
        {
            this.UnitNames = new List<string>();
        }

        /// <summary>
        /// Gets or sets the registration id used to match an <see cref="UpdateRegistrationAck"/>.
        /// </summary>
        public string RegistrationId { get; set; }

        /// <summary>
        /// Gets or sets the list of unit names for which we want to receive updates.
        /// </summary>
        public List<string> UnitNames { get; set; }
    }
}
