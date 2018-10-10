// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecentlyEditedWidgetViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RecentlyEditedWidgetViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.Widgets
{
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Common.Wpf.Framework.Controllers;

    /// <summary>
    /// The widget view model showing the recently edited entities.
    /// </summary>
    public class RecentlyEditedWidgetViewModel : WidgetViewModelBase
    {
        private readonly ICommandRegistry commandRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecentlyEditedWidgetViewModel"/> class.
        /// </summary>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public RecentlyEditedWidgetViewModel(ICommandRegistry commandRegistry)
        {
            this.commandRegistry = commandRegistry;
            this.Name = AdminStrings.Widget_Recently_Edited;
            this.RecentlyEditedEntities = new ObservableCollection<RecentlyEditedEntityViewModel>();
        }

        /// <summary>
        /// Gets the recently edited entities.
        /// </summary>
        public ObservableCollection<RecentlyEditedEntityViewModel> RecentlyEditedEntities { get; private set; }

        /// <summary>
        /// Gets the navigate to recent entity command.
        /// </summary>
        public ICommand NavigateToRecentEntityCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Widgets.NavigateToRecentEntity);
            }
        }
    }
}