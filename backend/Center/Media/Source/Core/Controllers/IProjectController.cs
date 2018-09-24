// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IProjectController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers
{
    using System.Threading.Tasks;

    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Center.Media.Core.DataViewModels.Compatibility;
    using Gorba.Center.Media.Core.DataViewModels.Consistency;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    /// <summary>
    /// Defines the controller handling projects.
    /// </summary>
    public interface IProjectController
    {
        /// <summary>
        /// Gets the parent controller.
        /// </summary>
        IMediaShellController ParentController { get; }

        /// <summary>
        /// Gets the Consistency Checker
        /// </summary>
        ConsistencyChecker ConsistencyChecker { get; }

        /// <summary>
        /// Gets the compatibility checker.
        /// </summary>
        CompatibilityChecker CompatibilityChecker { get; }

        /// <summary>
        /// Initializes the controller.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Opens a recent project that was not checked in.
        /// </summary>
        /// <param name="projectName">
        /// The project Name.
        /// </param>
        /// <returns>
        /// True if open was successful.
        /// </returns>
        Task<bool> OpenLocalProjectAsync(string projectName);

        /// <summary>
        /// Checks in the project with a different name and/or on a different tenant on the server.
        /// </summary>
        /// <param name="parameters">
        /// The parameters needed for checking in a project.
        /// </param>
        void CheckInAsProject(SaveAsParameters parameters);

        /// <summary>
        /// Handles situations when the project should be closed and, therefore, saved or not.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the user took a decision on save (yes or no); otherwise (cancel), <c>false</c>.
        /// </returns>
        SaveUserDecision SaveTrap();

        /// <summary>
        /// Tries to achieve a checked in state.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>. On success true.
        /// </returns>
        Task<bool> EnsureCheckInAsync();

        /// <summary>
        /// Tries to achieve a checked in state. Skip is allowed and returns true.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>. On success true.
        /// </returns>
        Task<bool> EnsureCheckInSkippableAsync();

        /// <summary>
        /// The ensure project controller has nothing left to do before exit.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>. On success true.
        /// </returns>
        Task<bool> EnsureCanExitAsync();

        /// <summary>
        /// The ensure project is checked in or reset.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>. True if done.
        /// </returns>
        Task<bool> EnsureCheckInOrResetAsync();

        /// <summary>
        /// The import project.
        /// </summary>
        /// <param name="projectToImport">
        /// The project to import.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task ImportProject(MediaProjectDataViewModel projectToImport);

        /// <summary>
        /// The check in trap.
        /// </summary>
        /// <param name="skippable">
        /// Enables skip by user or if permission do not allow check in.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<CheckinTrapResult> CheckinTrapAsync(CheckinTrapBehaviour skippable = CheckinTrapBehaviour.NotSkippable);

        /// <summary>
        /// create a new default resolution data view model
        /// </summary>
        /// <param name="resolutionWidth">
        /// The resolution Width
        /// </param>
        /// <param name="resolutionHeight">
        /// The resolution Height.
        /// </param>
        /// <returns>
        /// the new ResolutionConfigDataViewModel
        /// </returns>
        ResolutionConfigDataViewModel CreateDefaultResolutionConfigDataViewModel(
            int resolutionWidth, int resolutionHeight);

        /// <summary>
        /// Refreshes the layout usage references of the project.
        /// </summary>
        /// <param name="currentInfomediaConfig">
        /// The current infomedia config.
        /// </param>
        void RefreshLayoutUsageReferences(InfomediaConfigDataViewModel currentInfomediaConfig);

        /// <summary>
        /// The on project got dirty.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task OnProjectGotDirty();

        /// <summary>
        /// Sets the state of the project back to saved.
        /// </summary>
        void OnUndoToSaveMark();

        /// <summary>
        /// Clears all project related settings.
        /// </summary>
        void ClearCurrentProjectSettings();
    }
}