// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBISIPstring.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IBISIPstring type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv301.Messages
{
    // ReSharper disable InconsistentNaming

    /// <summary>
    /// The IBIS-IP.string.
    /// </summary>
    public partial class IBISIPstring
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IBISIPstring"/> class.
        /// </summary>
        public IBISIPstring()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IBISIPstring"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public IBISIPstring(string value)
        {
            this.Value = value;
        }
    }

    // ReSharper restore InconsistentNaming
}