// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetworkMaskField.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NetworkMaskField type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Udcp.Fields
{
    using System.Net;

    /// <summary>
    /// The <see cref="FieldType.NetworkMask"/> field.
    /// </summary>
    public class NetworkMaskField : IpAddressFieldBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkMaskField"/> class.
        /// </summary>
        /// <param name="mask">
        /// The network mask.
        /// </param>
        public NetworkMaskField(IPAddress mask)
            : base(FieldType.NetworkMask, mask)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkMaskField"/> class.
        /// </summary>
        internal NetworkMaskField()
            : base(FieldType.NetworkMask)
        {
        }
    }
}