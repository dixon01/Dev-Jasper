// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppInfoPartViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AppInfoPartViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.App
{
    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// View model base class for all parts of information about a single application.
    /// </summary>
    public abstract class AppInfoPartViewModelBase : ViewModelBase
    {
        private bool isExpanded;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppInfoPartViewModelBase"/> class.
        /// </summary>
        /// <param name="application">
        /// The application view model.
        /// </param>
        protected AppInfoPartViewModelBase(RemoteAppViewModel application)
        {
            this.Application = application;
        }

        /// <summary>
        /// Gets the application view model.
        /// </summary>
        public RemoteAppViewModel Application { get; private set; }

        /// <summary>
        /// Gets or sets the AppInfoPart Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this part is expanded (showing its details).
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return this.isExpanded;
            }

            set
            {
                this.SetProperty(ref this.isExpanded, value, () => this.IsExpanded);
            }
        }

        /// <summary>
        /// Gets the shell.
        /// </summary>
        public IDiagShell Shell
        {
            get
            {
                return this.Application.Shell;
            }
        }
    }
}