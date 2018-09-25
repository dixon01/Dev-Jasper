// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceInfoBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceInfoBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.IbisIP.Discovery.Messages
{
    using System.Collections.Generic;

    /// <summary>
    /// Base class for messages that contain service information.
    /// Do not use this class outside this namespace, it is only public for XML serialization.
    /// </summary>
    public abstract class ServiceInfoBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceInfoBase"/> class.
        /// </summary>
        protected ServiceInfoBase()
        {
            this.Attributes = new List<ServiceAttribute>();
        }

        /// <summary>
        /// Gets or sets the name of the service.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the protocol.
        /// </summary>
        public string Protocol { get; set; }

        /// <summary>
        /// Gets or sets the TCP or UDP port of the service.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the service attributes.
        /// </summary>
        public List<ServiceAttribute> Attributes { get; set; }
    }
}