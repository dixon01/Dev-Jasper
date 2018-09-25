// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBISIPdateTime.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IBISIPdateTime type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv301.Messages
{
    using System;

    // ReSharper disable InconsistentNaming

    /// <summary>
    /// The IBIS-IP.dateTime.
    /// </summary>
    public partial class IBISIPdateTime
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IBISIPdateTime"/> class.
        /// </summary>
        public IBISIPdateTime()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IBISIPdateTime"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public IBISIPdateTime(DateTime value)
        {
            this.Value = value;
        }
    }

    // ReSharper restore InconsistentNaming
}
