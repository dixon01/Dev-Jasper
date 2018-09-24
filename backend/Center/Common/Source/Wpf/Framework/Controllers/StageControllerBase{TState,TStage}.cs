// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StageControllerBase{TState,TStage}.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StageControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Controllers
{
    using System;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Startup;

    /// <summary>
    /// Defines a base abstract class with common logic for stage controllers.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TStage">The type of the stage.</typeparam>
    public abstract class StageControllerBase<TState, TStage> : IStageController
        where TState : IApplicationState
        where TStage : IViewModel
    {
        private readonly Lazy<TStage> stage;

        private readonly Lazy<TState> applicationState;

        /// <summary>
        /// Initializes a new instance of the <see cref="StageControllerBase&lt;TState, TStage&gt;"/> class.
        /// </summary>
        /// <param name="applicationState">State of the application.</param>
        /// <param name="stage">The stage.</param>
        protected StageControllerBase(Lazy<TState> applicationState, Lazy<TStage> stage)
        {
            this.applicationState = applicationState;
            this.stage = stage;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is loading stage.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is loading stage; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoadingStage { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether this controller is active.
        /// </summary>
        /// <value>
        /// <c>true</c> if this controller is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Gets the stage.
        /// </summary>
        protected TStage Stage
        {
            get
            {
                return this.stage.Value;
            }
        }

        /// <summary>
        /// Gets the state of the application.
        /// </summary>
        /// <value>
        /// The state of the application.
        /// </value>
        protected TState ApplicationState
        {
            get
            {
                return this.applicationState.Value;
            }
        }

        /// <summary>
        /// Activates this controller.
        /// </summary>
        public void Activate()
        {
            this.OnActivating();
            this.IsActive = true;
        }

        /// <summary>
        /// Deactivates this controller.
        /// </summary>
        public void Deactivate()
        {
            this.OnDeactivating();
            this.IsActive = false;
        }

        /// <summary>
        /// Determines whether this instance [can reload stage].
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can reload stage]; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanReloadStage(object parameter)
        {
            return !this.IsLoadingStage;
        }

        /// <summary>
        /// Called when [deactivating].
        /// </summary>
        protected virtual void OnDeactivating()
        {
        }

        /// <summary>
        /// Called when [activating].
        /// </summary>
        protected virtual void OnActivating()
        {
        }

        /// <summary>
        /// Reloads the stage core.
        /// </summary>
        protected virtual void ReloadStageCore()
        {
        }

        /// <summary>
        /// Reloads the stage.
        /// </summary>
        protected void ReloadStage()
        {
            this.ReloadStageCore();
        }
    }
}