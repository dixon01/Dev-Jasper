// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBISIPint.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IBISIPint type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv301.Messages
{
    // ReSharper disable InconsistentNaming

    /// <summary>
    /// The IBIS-IP.int.
    /// </summary>
    public partial class IBISIPint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IBISIPint"/> class.
        /// </summary>
        public IBISIPint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IBISIPint"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public IBISIPint(int value)
        {
            this.Value = value;
        }
    }

    // ReSharper restore InconsistentNaming
}
