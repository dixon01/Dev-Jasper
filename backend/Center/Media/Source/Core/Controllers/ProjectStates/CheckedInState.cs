// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckedInState.cs" company="Gorba AG">
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

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// This is the default state after opening or creating a project from server.
    /// </summary>
    internal class CheckedInState : StateBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckedInState"/> class.
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
        public CheckedInState(
            IMediaShell mediaShell,
            ICommandRegistry commandRegistry,
            IProjectControllerContext projectController)
            : base(mediaShell, commandRegistry, projectController)
        {
            this.StateInfo = ProjectStates.CheckedIn;
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
            if (this.MediaApplicationState.CurrentProject == null)
            {
                // should never happen
                throw new Exception("Project is in checked in state but CurrentProject is null.");
            }

            this.ProjectController.UpdateRecentProjects(true, false);

            var success = await this.ProjectController.ExecuteOpenProject(mediaConfiguration);

            if (success)
            {
                return await this.TransitionToAsync(ProjectStates.CheckedIn);
            }

            return await this.TransitionToAsync(ProjectStates.NoProject);
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
        /// The open local async.
        /// </summary>
        /// <param name="filename">
        ///     The filename.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Development in progress.
        /// </exception>
        public override Task<IState> OpenLocalAsync(string filename)
        {
            // There is no local file for the current project after checkin
            throw new Exception("Tried to load local file in checked in state.");
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
    }
}