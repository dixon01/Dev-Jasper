// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuSelectMediaPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MenuSelectMediaPrompt type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Interaction
{
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// The menu select media prompt.
    /// </summary>
    public class MenuSelectMediaPrompt : SelectMediaPrompt
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MenuSelectMediaPrompt"/> class.
        /// </summary>
        /// <param name="mediaElement">
        /// The media element.
        /// </param>
        /// <param name="shell">
        /// The shell.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        /// <param name="recentMediaResources">
        /// The recent media resources.
        /// </param>
        public MenuSelectMediaPrompt(
            DataViewModelBase mediaElement,
            IMediaShell shell,
            ICommandRegistry commandRegistry,
            ExtendedObservableCollection<ResourceInfoDataViewModel> recentMediaResources)
            : base(mediaElement, shell, commandRegistry, recentMediaResources)
        {
            this.IsOpen = true;
        }
    }
}