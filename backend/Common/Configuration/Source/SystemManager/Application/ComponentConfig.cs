// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComponentConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ComponentConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager.Application
{
    using System;

    /// <summary>
    /// Configuration to run a component inside SystemManager.
    /// </summary>
    [Serializable]
    public class ComponentConfig : ApplicationConfigBase
    {
        /// <summary>
        /// Gets or sets the path to the DLL containing the component.
        /// </summary>
        public string LibraryPath { get; set; }

        /// <summary>
        /// Gets or sets the fully qualified class name of the component.
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use an <see cref="AppDomain"/>
        /// to run the component.
        /// If this is false, the component runs directly inside System Manager, otherwise
        /// the component will run in its own AppDomain.
        /// </summary>
        public bool UseAppDomain { get; set; }
    }
}
