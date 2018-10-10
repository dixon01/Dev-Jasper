// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DhcpEnabledField.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DhcpEnabledField type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Udcp.Fields
{
    /// <summary>
    /// The <see cref="FieldType.DhcpEnabled"/> field.
    /// </summary>
    public class DhcpEnabledField : BoolFieldBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DhcpEnabledField"/> class.
        /// </summary>
        /// <param name="dhcpEnabled">
        /// A flag indicating if DHCP should be (or is) enabled.
        /// </param>
        public DhcpEnabledField(bool dhcpEnabled)
            : base(FieldType.DhcpEnabled, dhcpEnabled)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DhcpEnabledField"/> class.
        /// </summary>
        internal DhcpEnabledField()
            : base(FieldType.DhcpEnabled)
        {
        }
    }
}