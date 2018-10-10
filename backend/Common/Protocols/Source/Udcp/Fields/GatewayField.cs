// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GatewayField.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GatewayField type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Udcp.Fields
{
    using System.Net;

    /// <summary>
    /// The <see cref="FieldType.Gateway"/> field.
    /// </summary>
    public class GatewayField : IpAddressFieldBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayField"/> class.
        /// </summary>
        /// <param name="gateway">
        /// The gateway address.
        /// </param>
        public GatewayField(IPAddress gateway)
            : base(FieldType.Gateway, gateway)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayField"/> class.
        /// </summary>
        internal GatewayField()
            : base(FieldType.Gateway)
        {
        }
    }
}