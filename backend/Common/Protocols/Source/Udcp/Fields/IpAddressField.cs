// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IpAddressField.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IpAddressField type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Udcp.Fields
{
    using System.Net;

    /// <summary>
    /// The <see cref="FieldType.IpAddress"/> field.
    /// </summary>
    public class IpAddressField : IpAddressFieldBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IpAddressField"/> class.
        /// </summary>
        /// <param name="address">
        /// The IP address.
        /// </param>
        public IpAddressField(IPAddress address)
            : base(FieldType.IpAddress, address)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IpAddressField"/> class.
        /// </summary>
        internal IpAddressField()
            : base(FieldType.IpAddress)
        {
        }
    }
}