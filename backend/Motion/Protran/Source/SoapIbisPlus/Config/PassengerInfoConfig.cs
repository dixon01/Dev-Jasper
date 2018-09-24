// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PassengerInfoConfig.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.SoapIbisPlus.Config
{
    using System;

    /// <summary>
    /// Container of passenger message configuration
    /// </summary>
    [Serializable]
    public class PassengerInfoConfig
    {
        /// <summary>
        /// Gets or sets the body config.
        /// </summary>
        public DataItemConfig Body { get; set; }
    }
}
