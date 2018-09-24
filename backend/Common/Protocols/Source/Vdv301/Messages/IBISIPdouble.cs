// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBISIPdouble.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IBISIPdouble type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv301.Messages
{
    // ReSharper disable InconsistentNaming

    /// <summary>
    /// The IBIS-IP.double.
    /// </summary>
    public partial class IBISIPdouble
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IBISIPdouble"/> class.
        /// </summary>
        public IBISIPdouble()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IBISIPdouble"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public IBISIPdouble(double value)
        {
            this.Value = value;
        }
    }

    // ReSharper restore InconsistentNaming
}
