// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationInfosResponse.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationInfosResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.ServiceModel.Messages
{
    using System.Collections.Generic;

    /// <summary>
    /// Message sent from System Manager to clients.
    /// Do not use outside this DLL.
    /// Response to an <see cref="ApplicationInfosRequest"/> containing the information.
    /// </summary>
    public class ApplicationInfosResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationInfosResponse"/> class.
        /// </summary>
        public ApplicationInfosResponse()
        {
            this.Infos = new List<ApplicationInfo>();
        }

        /// <summary>
        /// Gets or sets the information objects.
        /// </summary>
        public List<ApplicationInfo> Infos { get; set; }
    }
}