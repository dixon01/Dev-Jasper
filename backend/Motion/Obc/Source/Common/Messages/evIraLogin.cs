// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evIraLogin.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evIraLogin type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// The IRA login event.
    /// </summary>
    public class evIraLogin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evIraLogin"/> class.
        /// </summary>
        /// <param name="personnelId">
        /// The a personnel id.
        /// </param>
        /// <param name="service">
        /// The a service.
        /// </param>
        /// <param name="type">
        /// The a type.
        /// </param>
        public evIraLogin(string personnelId, string service, int type)
        {
            this.PersonelId = personnelId;
            this.Service = service;
            this.Type = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evIraLogin"/> class.
        /// </summary>
        public evIraLogin()
        {
            this.Service = string.Empty;
            this.PersonelId = string.Empty;
            this.Type = 0;
        }

        /// <summary>
        /// Gets or sets the personnel id.
        /// </summary>
        public string PersonelId { get; set; }

        /// <summary>
        /// Gets or sets the service.
        /// </summary>
        public string Service { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public int Type { get; set; }
    }
}