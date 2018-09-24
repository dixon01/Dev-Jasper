// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemManagerDefaults.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemManagerDefaults type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager
{
    using System;

    using Gorba.Common.Configuration.SystemManager.Application;

    /// <summary>
    /// Class holding the default values for several configurable items in the
    /// System Manager configuration.
    /// </summary>
    [Serializable]
    public class SystemManagerDefaults
    {
        /// <summary>
        /// Gets or sets the <see cref="ProcessConfig"/> defaults
        /// for <see cref="SystemManagerConfig.Applications"/>.
        /// </summary>
        public ProcessConfig Process { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ComponentConfig"/> defaults
        /// for <see cref="SystemManagerConfig.Applications"/>.
        /// </summary>
        public ComponentConfig Component { get; set; }
    }
}