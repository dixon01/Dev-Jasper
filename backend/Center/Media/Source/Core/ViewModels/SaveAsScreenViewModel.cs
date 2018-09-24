// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaveAsScreenViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The save as screen view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Input;

    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Media.Core.Resources;

    /// <summary>
    /// The save as screen view model.
    /// </summary>
    public class SaveAsScreenViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly ICommandRegistry commandRegistry;
        private string name;
        private TenantReadableModel selectedTenant;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveAsScreenViewModel"/> class.
        /// </summary>
        /// <param name="shell">
        /// The shell.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public SaveAsScreenViewModel(IMediaShell shell, ICommandRegistry commandRegistry)
        {
            this.Shell = shell;
            this.commandRegistry = commandRegistry;
        }

        /// <summary>
        /// Gets the shell.
        /// </summary>
        public IMediaShell Shell { get; private set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.SetProperty(ref this.name, value, () => this.Name);
            }
        }

        /// <summary>
        /// Gets or sets the selected tenant.
        /// </summary>
        public TenantReadableModel SelectedTenant
        {
            get
            {
                return this.selectedTenant;
            }

            set
            {
                this.SetProperty(ref this.selectedTenant, value, () => this.SelectedTenant);
                this.RaisePropertyChanged(() => this.Name);
            }
        }

        /// <summary>
        /// Gets the save as command.
        /// </summary>
        public ICommand SaveAsCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SaveAs);
            }
        }

        /// <summary>
        /// Gets the error.
        /// </summary>
        public string Error { get; private set; }

        /// <summary>
        /// The this.
        /// </summary>
        /// <param name="columnName">
        /// The column name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string this[string columnName]
        {
            get
            {
                var error = string.Empty;
                if (columnName == "Name")
                {
                    this.Error = string.Empty;
                    if (string.IsNullOrEmpty(this.Name))
                    {
                        this.Error = MediaStrings.MainMenu_ProjectNameEmpty;
                        return this.Error;
                    }

                    if (this.SelectedTenant == null)
                    {
                        return error;
                    }

                    if (!this.Shell.MediaApplicationState.AllExistingProjects.ContainsKey(this.SelectedTenant.Id))
                    {
                        return error;
                    }

                    var projects = this.Shell.MediaApplicationState.AllExistingProjects[this.SelectedTenant.Id];
                    if (projects.Any(
                        p => p.Document.Name.Equals(this.Name, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        this.Error = MediaStrings.MainMenu_DuplicateProjectName;
                        return MediaStrings.MainMenu_DuplicateProjectName;
                    }
                }

                return error;
            }
        }
    }
}
