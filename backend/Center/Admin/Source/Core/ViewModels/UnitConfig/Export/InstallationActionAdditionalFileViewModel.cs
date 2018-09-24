// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstallationActionAdditionalFileViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InstallationActionAdditionalFileViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export
{
    using System.IO;

    /// <summary>
    /// The installation action view model base.
    /// </summary>
    public class InstallationActionAdditionalFileViewModel : IExportableFile
    {
        private string fileName;

        /// <summary>
        /// Gets the display name.
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Gets or sets the executable path.
        /// </summary>
        public string FileName
        {
            get
            {
                return this.fileName;
            }

            set
            {
                this.fileName = value;
                this.DisplayName = Path.GetFileName(this.fileName);
            }
        }

        string IExportableFile.ContentType
        {
            get
            {
                return "application/octet-stream";
            }
        }
    }
}