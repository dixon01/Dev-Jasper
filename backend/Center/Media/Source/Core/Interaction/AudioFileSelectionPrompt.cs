// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioFileSelectionPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The audio file selection prompt.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Interaction
{
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// The audio file selection prompt.
    /// </summary>
    public class AudioFileSelectionPrompt : SelectMediaPrompt
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioFileSelectionPrompt"/> class.
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
        public AudioFileSelectionPrompt(
            DataViewModelBase mediaElement,
            IMediaShell shell,
            ICommandRegistry commandRegistry,
            ExtendedObservableCollection<ResourceInfoDataViewModel> recentMediaResources)
            : base(mediaElement, shell, commandRegistry, recentMediaResources)
        {
        }
    }
}
