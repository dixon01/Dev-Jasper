// --------------------------------------------------------------------------------------------------------------------
// <copyright file="States.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the States type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Wizard.ViewModels
{
    /// <summary>
    /// Defines the possible states of an action.
    /// </summary>
    public enum States
    {
        /// <summary>
        /// Default state.
        /// </summary>
        None = 0,

        /// <summary>
        /// The action is waiting other actions to be able to run.
        /// </summary>
        Waiting = 1,

        /// <summary>
        /// The action is in progress.
        /// </summary>
        InProgress = 2,

        /// <summary>
        /// The action has been successfully executed.
        /// </summary>
        Done = 3,

        /// <summary>
        /// The action has been canceled.
        /// </summary>
        Canceled = 4,

        /// <summary>
        /// The action was aborted because of an error.
        /// </summary>
        Error = 5
    }
}