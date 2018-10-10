// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IChangeHistoryController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the controller for history entry handling.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers
{
    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// Defines the controller for history entry handling.
    /// </summary>
    public interface IChangeHistoryController
    {
        /// <summary>
        /// Gets or sets the media shell.
        /// </summary>
        IMediaShell MediaShell { get; set; }

        /// <summary>
        /// Gets or sets the parent controller.
        /// </summary>
        IMediaShellController ParentController { get; set; }

        /// <summary>
        /// Adds the history entry.
        /// </summary>
        /// <param name="historyEntry">The history entry.</param>
        /// <param name="skipInitialDo">
        /// Value indicating if the history entry should skip the execution of "Do" after adding it. Default is false.
        /// </param>
        void AddHistoryEntry(IHistoryEntry historyEntry, bool skipInitialDo = false);

        /// <summary>
        /// Adds a save marker to the undo stack.
        /// </summary>
        void AddSaveMarker();

        /// <summary>
        /// Resets the <see cref="ChangeHistory"/>.
        /// </summary>
        void ResetChangeHistory();
    }
}