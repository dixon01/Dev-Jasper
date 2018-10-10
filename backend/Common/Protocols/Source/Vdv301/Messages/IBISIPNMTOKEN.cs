// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBISIPNMTOKEN.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IBISIPNMTOKEN type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv301.Messages
{
    using System;

    // ReSharper disable InconsistentNaming

    /// <summary>
    /// The IBIS-IP.NMTOKEN.
    /// </summary>
    public partial class IBISIPNMTOKEN
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IBISIPNMTOKEN"/> class.
        /// </summary>
        public IBISIPNMTOKEN()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IBISIPNMTOKEN"/> class.
        /// </summary>
        /// <param name="value">
        /// The string value.
        /// </param>
        public IBISIPNMTOKEN(string value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IBISIPNMTOKEN"/> class.
        /// </summary>
        /// <param name="value">
        /// The version value.
        /// </param>
        public IBISIPNMTOKEN(Version value)
        {
            this.Value = value.ToString();
        }
    }

    // ReSharper restore InconsistentNaming
}