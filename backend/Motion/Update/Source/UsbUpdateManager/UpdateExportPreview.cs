// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateExportPreview.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateExportPreview type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager
{
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Resources;

    /// <summary>
    /// Preview of an update to be exported.
    /// </summary>
    public class UpdateExportPreview
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateExportPreview"/> class.
        /// </summary>
        public UpdateExportPreview()
        {
            this.UnitGroups = new List<UnitGroupExportPreview>();
            this.Resources = new List<ResourceInfo>();
        }

        /// <summary>
        /// Gets the list of unit group previews. Don't modify this list!
        /// </summary>
        public List<UnitGroupExportPreview> UnitGroups { get; private set; }

        /// <summary>
        /// Gets all resources required for this export.
        /// </summary>
        public List<ResourceInfo> Resources { get; private set; }
    }
}