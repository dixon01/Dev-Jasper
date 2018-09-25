// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceListResponse.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceListResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.IbisIP.Discovery.Messages
{
    using System.Collections.Generic;

    /// <summary>
    /// Response to a <see cref="ServiceListRequest"/>.
    /// Do not use this class outside this namespace, it is only public for XML serialization.
    /// </summary>
    public class ServiceListResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceListResponse"/> class.
        /// </summary>
        public ServiceListResponse()
        {
            this.KnownServices = new List<ServiceInfo>();
        }

        /// <summary>
        /// Gets or sets the list of known services.
        /// </summary>
        public List<ServiceInfo> KnownServices { get; set; }
    }
}