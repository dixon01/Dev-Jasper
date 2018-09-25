// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceServiceConfigBase.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceServiceConfigBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Resources
{
    using System.IO;

    using Gorba.Common.Medi.Core.Config;

    /// <summary>
    /// Resource management configuration.
    /// </summary>
    public abstract class ResourceServiceConfigBase : ServiceConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceServiceConfigBase"/> class.
        /// </summary>
        protected ResourceServiceConfigBase()
        {
            this.ResourceDirectory = Path.Combine(Path.GetTempPath(), "MediResources\\${AppName}");
            this.MaxSizeMb = 0;
        }

        /// <summary>
        /// Gets or sets the local directory to store resources and temporary files.
        /// </summary>
        public string ResourceDirectory { get; set; }

        /// <summary>
        /// Gets or sets the directory maximum size MB. Zero means no limit.
        /// </summary>
        public int MaxSizeMb { get; set; }
    }
}
