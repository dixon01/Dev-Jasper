// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IState.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The interface to identify project states.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers.ProjectStates
{
    using System.Threading.Tasks;

    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    /// <summary>
    /// The interface to identify project states.
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// Gets the state info which can be used to determine the current state.
        /// </summary>
        ProjectStates StateInfo { get; }

        /// <summary>
        /// Gets the check in decision.
        /// </summary>
        CheckinUserDecision CheckinDecision { get; }

        /// <summary>
        /// Transition to create a new project
        /// </summary>
        /// <param name="mediaProjectDataViewModel">
        /// The media Project Data View Model.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<IState> CreateAsync(MediaProjectDataViewModel mediaProjectDataViewModel);

        /// <summary>
        /// Transition to open a file from the local file system
        /// </summary>
        /// <param name="filename">
        ///     The filename.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<IState> OpenLocalAsync(string filename);

        /// <summary>
        /// Transition to open a project from the server.
        /// </summary>
        /// <param name="mediaConfiguration">
        ///     The media Configuration.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<IState> OpenFromServerAsync(MediaConfigurationDataViewModel mediaConfiguration);

        /// <summary>
        /// Transition to save current project to the local file system.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<IState> SaveAsync();

        /// <summary>
        /// The reload project. If project is not found locally the project is loaded from the server.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<IState> ReloadAsync();

        /// <summary>
        /// The check in async.
        /// </summary>
        /// <param name="behaviour">
        /// May the check in be skipped by the user.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<IState> CheckInAsync(CheckinTrapBehaviour behaviour = CheckinTrapBehaviour.NotSkippable);

        /// <summary>
        /// Transition to save current project under a different name
        /// </summary>
        /// <param name="parameters">
        /// The parameters to check in with.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<IState> CheckInAsAsync(CreateDocumentVersionParameters parameters);

        /// <summary>
        /// Transition called when modifying the current project.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<IState> MakeDirtyAsync();
    }
}