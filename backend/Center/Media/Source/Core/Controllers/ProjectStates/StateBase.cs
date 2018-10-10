// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
// Base state for the project statemachine.
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
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// The project states.
    /// </summary>
    public enum ProjectStates
    {
        /// <summary>
        /// The undefined default.
        /// </summary>
        Undefined,

        /// <summary>
        /// The no project.
        /// </summary>
        NoProject,

        /// <summary>
        /// The checked in.
        /// </summary>
        CheckedIn,

        /// <summary>
        /// The saved.
        /// </summary>
        Saved,

        /// <summary>
        /// The modified.
        /// </summary>
        Modified
    }

    /// <summary>
    /// The state base.
    /// </summary>
    public abstract class StateBase : IState
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The media shell.
        /// </summary>
        protected readonly IMediaShell MediaShell;

        /// <summary>
        /// The command registry.
        /// </summary>
        protected readonly ICommandRegistry CommandRegistry;

        /// <summary>
        /// The project controller.
        /// </summary>
        protected readonly IProjectControllerContext ProjectController;

        /// <summary>
        /// The media application state.
        /// </summary>
        protected readonly IMediaApplicationState MediaApplicationState;

        private readonly Lazy<IResourceManager> lazyResourceManager =
            new Lazy<IResourceManager>(() => ServiceLocator.Current.GetInstance<IResourceManager>());

        /// <summary>
        /// Initializes a new instance of the <see cref="StateBase"/> class.
        /// </summary>
        /// <param name="mediaShell">
        /// The media shell
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry
        /// </param>
        /// <param name="projectController">
        /// The project controller
        /// </param>
        protected StateBase(
            IMediaShell mediaShell,
            ICommandRegistry commandRegistry,
            IProjectControllerContext projectController)
        {
            this.MediaShell = mediaShell;
            this.CommandRegistry = commandRegistry;
            this.ProjectController = projectController;
            this.MediaApplicationState = mediaShell.MediaApplicationState;
        }

        /// <summary>
        /// Gets or sets the state info which can be used to determine the current state.
        /// </summary>
        public ProjectStates StateInfo { get; set; }

        /// <summary>
        /// Gets or sets the check in decision.
        /// </summary>
        public CheckinUserDecision CheckinDecision { get; protected set; }

        /// <summary>
        /// Gets the resource manager.
        /// </summary>
        protected IResourceManager ResourceManager
        {
            get
            {
                return this.lazyResourceManager.Value;
            }
        }

        /// <summary>
        /// Transition to create a new project
        /// </summary>
        /// <param name="mediaProjectDataViewModel">
        /// The media Project Data View Model.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public virtual Task<IState> CreateAsync(MediaProjectDataViewModel mediaProjectDataViewModel)
        {
            return this.NoTransitionAsync();
        }

        /// <summary>
        /// Transition to open a file from the local file system
        /// </summary>
        /// <param name="filename">
        ///     The filename.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public virtual Task<IState> OpenLocalAsync(string filename)
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
        public virtual Task<IState> OpenFromServerAsync(MediaConfigurationDataViewModel mediaConfiguration)
        {
            return this.NoTransitionAsync();
        }

        /// <summary>
        /// Transition to save current project to the local file system.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public virtual Task<IState> SaveAsync()
        {
            return this.NoTransitionAsync();
        }

        /// <summary>
        /// The reload project. If project is not found locally the project is loaded from the server.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Can not be done in this state.
        /// </exception>
        public virtual Task<IState> ReloadAsync()
        {
            return this.NoTransitionAsync();
        }

        /// <summary>
        /// Transition to check in the project to the server.
        /// </summary>
        /// <param name="behaviour">
        /// May the check in be skipped.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public virtual Task<IState> CheckInAsync(CheckinTrapBehaviour behaviour = CheckinTrapBehaviour.NotSkippable)
        {
            return this.NoTransitionAsync();
        }

        /// <summary>
        /// Transition to save current project under a different name
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public virtual Task<IState> CheckInAsAsync(CreateDocumentVersionParameters parameters)
        {
            return this.NoTransitionAsync();
        }

        /// <summary>
        /// Transition called when modifying the current project.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public virtual Task<IState> MakeDirtyAsync()
        {
            return this.NoTransitionAsync();
        }

        /// <summary>
        /// The no transition.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected Task<IState> NoTransitionAsync()
        {
            return Task.FromResult((IState)this);
        }

        /// <summary>
        /// The transition to.
        /// </summary>
        /// <param name="newState">
        /// The new state.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected Task<IState> TransitionToAsync(ProjectStates newState)
        {
            return Task.FromResult(this.Create(newState));
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <returns>
        /// The <see cref="StateBase"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws Exception if enum is ProjectStates.Undefined
        /// </exception>
        private IState Create(ProjectStates state)
        {
            if (this.ProjectController == null)
            {
                throw new NullReferenceException("No Project controller set.");
            }

            switch (state)
            {
                case ProjectStates.NoProject:
                    return new NoProjectState(this.MediaShell, this.CommandRegistry, this.ProjectController);
                case ProjectStates.CheckedIn:
                    return new CheckedInState(this.MediaShell, this.CommandRegistry, this.ProjectController);
                case ProjectStates.Saved:
                    return new SavedState(this.MediaShell, this.CommandRegistry, this.ProjectController);
                case ProjectStates.Modified:
                    return new ModifiedState(this.MediaShell, this.CommandRegistry, this.ProjectController);
                default:
                    throw new Exception("Unknown state to create.");
            }
        }
    }
}
