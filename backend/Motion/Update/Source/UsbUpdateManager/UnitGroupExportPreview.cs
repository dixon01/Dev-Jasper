// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitGroupExportPreview.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitGroupExportPreview type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager
{
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Motion.Update.UsbUpdateManager.Data;

    /// <summary>
    /// Preview of an update export for a single unit group.
    /// </summary>
    public class UnitGroupExportPreview
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitGroupExportPreview"/> class.
        /// </summary>
        /// <param name="unitGroup">
        /// The unit group.
        /// </param>
        /// <param name="updateRoot">
        /// The update root folder.
        /// </param>
        public UnitGroupExportPreview(UnitGroup unitGroup, FolderUpdate updateRoot)
        {
            this.UnitGroup = unitGroup;
            this.UpdateRoot = updateRoot;

            this.PreInstallation = new RunCommands();
            this.PostInstallation = new RunCommands();
        }

        /// <summary>
        /// Gets the unit group.
        /// </summary>
        public UnitGroup UnitGroup { get; private set; }

        /// <summary>
        /// Gets the update root in which all updated folders can be found.
        /// </summary>
        public FolderUpdate UpdateRoot { get; private set; }

        /// <summary>
        /// Gets or sets the pre-installation commands.
        /// </summary>
        public RunCommands PreInstallation { get; set; }

        /// <summary>
        /// Gets or sets the post-installation commands.
        /// </summary>
        public RunCommands PostInstallation { get; set; }
    }
}