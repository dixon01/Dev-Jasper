// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisFallbackConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisFallbackConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Config
{
    using System;

    /// <summary>
    /// Configuration of IBIS for Abu Dhabi.
    /// This only contains information about the usage
    /// of IBIS inside the Abu Dhabi Protocol, the rest of
    /// the IBIS behavior is configured in ibis.xml.
    /// </summary>
    [Serializable]
    public class IbisFallbackConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IbisFallbackConfig"/> class.
        /// </summary>
        public IbisFallbackConfig()
        {
            this.Enabled = false;
            this.RestartOnTimeout = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the IBIS fallback is enabled or not.
        /// If IBIS fallback is disabled, so is CTS detection (i.e. Stop Requested doesn't work).
        /// Default value: false.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Protran should restart (actually just
        /// stop, it will then be restarted by System Manager) if there is a timeout on the
        /// IBIS bus (i.e. not getting any telegrams for a defined time).
        /// Default value: false.
        /// </summary>
        public bool RestartOnTimeout { get; set; }
    }
}