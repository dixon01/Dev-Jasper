// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceEntity.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceEntity type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.AzureStorage.WpfApplication.ViewModels
{
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// Defines a resource table entity.
    /// </summary>
    public class ResourceEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets the original file name.
        /// </summary>
        public string OriginalFileName { get; set; }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        public long Length { get; set; }
    }
}