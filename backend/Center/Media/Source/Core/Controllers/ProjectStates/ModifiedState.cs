// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModifiedState.cs" company="Gorba AG">
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
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// The state that represents a dirty project. It is not saved locally or checked in.
    /// </summary>
    internal class ModifiedState : StateBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModifiedState"/> class.
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
        public ModifiedState(
            IMediaShell mediaShell,
            ICommandRegistry commandRegistry,
            IProjectControllerContext projectController)
            : base(mediaShell, commandRegistry, projectController)
        {
            this.StateInfo = ProjectStates.Modified;
        }

        /// <summary>
        /// The create async.
        /// </summary>
        /// <param name="mediaProjectDataViewModel">
        /// The media Project Data View Model.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Development in progress.
        /// </exception>
        public async override Task<IState> CreateAsync(MediaProjectDataViewModel mediaProjectDataViewModel)
        {
            var success = await this.ProjectController.CreateDocumentAsync(mediaProjectDataViewModel.ToDataModel());
            if (!success)
            {
                this.MediaShell.IsBusy = false;
                return await this.NoTransitionAsync();
            }

            return await this.TransitionToAsync(ProjectStates.CheckedIn);
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
            var saved = this.ProjectController.ExecuteSaveProjectLocal();

            if (saved)
            {
                return await this.TransitionToAsync(ProjectStates.Saved);
            }

            return await this.NoTransitionAsync();
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
        public async override Task<IState> OpenLocalAsync(string projectName)
        {
            // todo
            // called wen savetrap is executed but user chooses to discard changes
            return await this.NoTransitionAsync();
        }

        /// <summary>
        /// The make dirty.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public override Task<IState> MakeDirtyAsync()
        {
            return this.NoTransitionAsync();
        }

        /// <summary>
        /// Transition to open a project from the server.
        /// </summary>
        /// <param name="mediaConfiguration">
        ///     The media Configuration.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async override Task<IState> OpenFromServerAsync(MediaConfigurationDataViewModel mediaConfiguration)
        {
            var saveUserDecision = this.ProjectController.SaveTrap();

            IState state;
            switch (saveUserDecision)
            {
                case SaveUserDecision.Save:
                    state = await this.SaveAsync();
                    break;

                case SaveUserDecision.Discard:
                    state = await this.ReloadAsync();
                    break;

                case SaveUserDecision.Cancel:
                    return await this.NoTransitionAsync();

                case SaveUserDecision.NoSaveRequired:
                    throw new Exception("No save required in modified state, impossible.");

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (state.StateInfo != ProjectStates.Saved
                && state.StateInfo != ProjectStates.CheckedIn)
            {
                throw new Exception("State is not saved or checked in after successful save, impossible");
            }

            return await state.OpenFromServerAsync(mediaConfiguration);
        }

        /// <summary>
        /// The check in async.
        /// </summary>
        /// <param name="behaviour">
        /// The behaviour.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Development in progress.
        /// </exception>
        public async override Task<IState> CheckInAsync(
            CheckinTrapBehaviour behaviour = CheckinTrapBehaviour.NotSkippable)
        {
            var saveUserDecision = this.ProjectController.SaveTrap();

            IState state;
            switch (saveUserDecision)
            {
                case SaveUserDecision.Save:
                    state = await this.SaveAsync();
                    break;

                case SaveUserDecision.Discard:
                    state = await this.ReloadAsync();
                    break;

                case SaveUserDecision.Cancel:
                    this.CheckinDecision = CheckinUserDecision.Cancel;
                    return await this.NoTransitionAsync();

                case SaveUserDecision.NoSaveRequired:
                    throw new Exception("No save required in modified state, impossible.");

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (state.StateInfo != ProjectStates.Saved
                && state.StateInfo != ProjectStates.CheckedIn)
            {
                throw new Exception("State is not saved or checked in after successful save, impossible");
            }

            return await state.CheckInAsync(behaviour);
        }

        /// <summary>
        /// The reload project. If project is not found locally the project is loaded from the server.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
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
    }
}