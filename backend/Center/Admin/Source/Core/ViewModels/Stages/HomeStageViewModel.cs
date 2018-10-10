// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeStageViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HomeStageViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.Stages
{
    using System.Collections.ObjectModel;

    using Gorba.Center.Admin.Core.ViewModels.Widgets;
    using Gorba.Center.Common.Wpf.Framework.Controllers;

    /// <summary>
    /// The view model for the home stage.
    /// </summary>
    public class HomeStageViewModel : StageViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HomeStageViewModel"/> class.
        /// </summary>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public HomeStageViewModel(ICommandRegistry commandRegistry)
        {
            this.RecentlyEditedWidget = new RecentlyEditedWidgetViewModel(commandRegistry);
            this.Widgets = new ObservableCollection<WidgetViewModelBase> { this.RecentlyEditedWidget };
        }

        /// <summary>
        /// Gets the widgets view models to be shown on the home stage.
        /// </summary>
        public ObservableCollection<WidgetViewModelBase> Widgets { get; private set; }

        /// <summary>
        /// Gets the recently edited widget.
        /// </summary>
        public RecentlyEditedWidgetViewModel RecentlyEditedWidget { get; private set; }
    }
}
