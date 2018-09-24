// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SavedState.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers.ProjectStates
{
    using System;
    using System.Threading.Tasks;

    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    /// <summary>
    /// The project is saved but not checked in.
    /// </summary>
    internal class SavedState : StateBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SavedState"/> class.
        /// </summary>
        /// <param name="mediaShell">
        ///     The media shell
        /// </param>
        /// <param name="commandRegistry">
        ///     The command registry
        /// </param>
        /// <param name="projectController">
        /// The project controller
        /// </param>
        public SavedState(
            IMediaShell mediaShell,
            ICommandRegistry commandRegistry,
            IProjectControllerContext projectController)
            : base(mediaShell, commandRegistry, projectController)
        {
            this.StateInfo = ProjectStates.Saved;
        }

        /// <summary>
        /// The open local async.
        /// </summary>
        /// <param name="projectName">
        ///     The project name.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public override Task<IState> OpenLocalAsync(string projectName)
        {
            // this action should do nothing.
            return this.NoTransitionAsync();
        }

        /// <summary>
        /// The save async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Development in progress.
        /// </exception>
        public async override Task<IState> SaveAsync()
        {
            return await this.NoTransitionAsync();
        }

        /// <summary>
        /// The reload project. If project is not found locally the project is loaded from the server.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Exception for unknown reload type.
        /// </exception>
        public async override Task<IState> ReloadAsync()
        {
            var success = await this.ProjectController.ReloadProjectAsync();

            switch (success)
            {
                case ReloadProjectResult.Fail:
                    return await this.NoTransitionAsync();
                case ReloadProjectResult.FromLocal:
                    return await this.TransitionToAsync(ProjectStates.Saved);
                case ReloadProjectResult.FromServer:
                    return await this.TransitionToAsync(ProjectStates.CheckedIn);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Transition to open a project from the server.
        /// </summary>
        /// <param name="mediaConfiguration">
        /// The media Configuration.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public override async Task<IState> OpenFromServerAsync(MediaConfigurationDataViewModel mediaConfiguration)
        {
            var state = await this.CheckInAsync();

            switch (state.StateInfo)
            {
                case ProjectStates.CheckedIn:
                case ProjectStates.NoProject:
                    return await state.OpenFromServerAsync(mediaConfiguration);
                case ProjectStates.Saved:
                case ProjectStates.Modified:
                    return await this.NoTransitionAsync();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// The make dirty.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Development in progress.
        /// </exception>
        public override Task<IState> MakeDirtyAsync()
        {
            return this.TransitionToAsync(ProjectStates.Modified);
        }

        /// <summary>
        /// The check in as async.
        /// </summary>
        /// <param name="behaviour">
        ///     The behaviour.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Out of range for trap result.
        /// </exception>
        public async override Task<IState> CheckInAsync(
            CheckinTrapBehaviour behaviour = CheckinTrapBehaviour.NotSkippable)
        {
            var checkInTrapResult = await this.ProjectController.CheckinTrapAsync(behaviour);

            this.CheckinDecision = checkInTrapResult.Decision;

            bool success;
            switch (this.CheckinDecision)
            {
                case CheckinUserDecision.NoCheckinRequired:
                    throw new Exception("Project is in saved state but no check in required. Not possible.");

                case CheckinUserDecision.Cancel:
                    return await this.NoTransitionAsync();

                case CheckinUserDecision.Skip:
                    return await this.NoTransitionAsync();

                case CheckinUserDecision.Checkin:
                    var checkinParameter = new CreateDocumentVersionParameters(
                        checkInTrapResult.Major,
                        checkInTrapResult.Minor,
                        checkInTrapResult.CheckinComment);

                    success = await this.ProjectController.CheckInProjectAsync(checkinParameter);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (success)
            {
                return await this.TransitionToAsync(ProjectStates.CheckedIn);
            }

            return await this.NoTransitionAsync();
        }

        /// <summary>
        /// The save as async.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Development in progress.
        /// </exception>
        public async override Task<IState> CheckInAsAsync(CreateDocumentVersionParameters parameters)
        {
            var checkedIn = await this.ProjectController.CheckInProjectAsync(parameters);

            if (checkedIn)
            {
                return await this.TransitionToAsync(ProjectStates.CheckedIn);
            }

            return await this.NoTransitionAsync();
        }
    }
}