// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBISIPboolean.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IBISIPboolean type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv301.Messages
{
    // ReSharper disable InconsistentNaming

    /// <summary>
    /// The IBIS-IP.boolean.
    /// </summary>
    public partial class IBISIPboolean
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IBISIPboolean"/> class.
        /// </summary>
        public IBISIPboolean()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IBISIPboolean"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public IBISIPboolean(bool value)
        {
            this.Value = value;
        }
    }

    // ReSharper restore InconsistentNaming
}