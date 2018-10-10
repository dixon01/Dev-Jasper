// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateProject.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateProject type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager.Data
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Update.Providers;

    /// <summary>
    /// The root object for an update project configuration.
    /// </summary>
    public class UpdateProject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateProject"/> class.
        /// </summary>
        public UpdateProject()
        {
            this.UnitGroups = new List<UnitGroup>();
            this.FtpServers = new List<FtpUpdateProviderConfig>();
        }

        /// <summary>
        /// Gets or sets the unique ID of this project.
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// Gets or sets the index of the last update.
        /// </summary>
        public int LastUpdateIndex { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the last import from the USB stick.
        /// </summary>
        public DateTime LastImportTimestamp { get; set; }

        /// <summary>
        /// Gets or sets the list of unit groups.
        /// </summary>
        public List<UnitGroup> UnitGroups { get; set; }

        /// <summary>
        /// Gets or sets the FTP server configurations to use with this project.
        /// </summary>
        public List<FtpUpdateProviderConfig> FtpServers { get; set; }
    }
}
