// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExportableFile.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IExportableFile type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export
{
    /// <summary>
    /// Interface to be implemented by all file view models that need to be uploaded to the BGS when creating an update.
    /// </summary>
    internal interface IExportableFile
    {
        /// <summary>
        /// Gets the file name to display when exporting.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Gets the content type (MIME type) of this file.
        /// </summary>
        string ContentType { get; }
    }
}