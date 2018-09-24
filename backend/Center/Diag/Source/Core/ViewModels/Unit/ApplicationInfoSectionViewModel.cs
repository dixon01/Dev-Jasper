// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationInfoSectionViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationInfoSectionViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.Unit
{
    using System.Collections.ObjectModel;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Diag.Core.ViewModels.App;

    /// <summary>
    /// The view model for the section that shows information about a single application.
    /// </summary>
    public class ApplicationInfoSectionViewModel : InfoSectionViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationInfoSectionViewModel"/> class.
        /// </summary>
        /// <param name="application">
        /// The application.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public ApplicationInfoSectionViewModel(RemoteAppViewModel application, ICommandRegistry commandRegistry)
            : base(application.Unit)
        {
            this.Application = application;
            this.Parts = new ObservableCollection<AppInfoPartViewModelBase>
                         {
                             new MediTreeInfoPartViewModel(this.Application, commandRegistry),
                             new ApplIoInfoPartViewModel(this.Application),
                             new RemoteLogInfoPartViewModel(this.Application),
                         };
        }

        /// <summary>
        /// Gets the application.
        /// </summary>
        public RemoteAppViewModel Application { get; private set; }

        /// <summary>
        /// Gets the list of parts to show for this application.
        /// </summary>
        public ObservableCollection<AppInfoPartViewModelBase> Parts { get; private set; }
    }
}