// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceAttribute.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceAttribute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.IbisIP.Discovery.Messages
{
    using Gorba.Common.Protocols.DnsServiceDiscovery;

    /// <summary>
    /// The service attribute.
    /// Do not use this class outside this namespace, it is only public for XML serialization.
    /// </summary>
    public class ServiceAttribute : IServiceAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceAttribute"/> class.
        /// </summary>
        public ServiceAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceAttribute"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public ServiceAttribute(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the name of the attribute.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the attribute.
        /// </summary>
        public string Value { get; set; }
    }
}