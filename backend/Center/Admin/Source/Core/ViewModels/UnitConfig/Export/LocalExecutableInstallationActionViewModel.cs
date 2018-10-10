// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalExecutableInstallationActionViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LocalExecutableInstallationActionViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export
{
    using System.IO;

    /// <summary>
    /// The installation action view model base.
    /// </summary>
    public class LocalExecutableInstallationActionViewModel : InstallationActionViewModelBase, IExportableFile
    {
        private string executablePathBase;

        /// <summary>
        /// Gets or sets the executable path.
        /// </summary>
        public override string ExecutablePathBase
        {
            get
            {
                return this.executablePathBase;
            }

            set
            {
                this.executablePathBase = value;
                this.DisplayName = Path.GetFileName(this.executablePathBase);
                this.RaisePropertyChanged(() => this.DisplayName);
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