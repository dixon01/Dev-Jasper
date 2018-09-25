// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMediaApplicationController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IMediaApplicationController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Gorba.Center.Common.Wpf.Client.Controllers;

    /// <summary>
    /// Defines the application controller specific for the Media application.
    /// </summary>
    public interface IMediaApplicationController : IClientApplicationController
    {
        /// <summary>
        /// Event that is raised if a new project has been added to the list.
        /// </summary>
        event EventHandler<EventArgs> ProjectAdded;

        /// <summary>
        /// The projects loaded.
        /// </summary>
        event EventHandler<EventArgs> ProjectsLoaded;

        /// <summary>
        /// Gets or sets the shell controller.
        /// </summary>
        /// <value>
        /// The shell controller.
        /// </value>
        IMediaShellController ShellController { get; set; }

        /// <summary>
        /// Initializes the layout editor controller.
        /// </summary>
        void InitializeLayoutEditorControllers();

        /// <summary>
        /// Gets the existing projects for the selected tenant.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the projects could be loaded; <c>false</c> otherwise.
        /// </returns>
        Task<bool> GetExistingProjectsAsync();

        /// <summary>
        /// Gets the existing update groups for the selected tenant.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the update groups could be loaded; <c>false</c> otherwise.
        /// </returns>
        Task<bool> GetExistingUpdateGroupsAsync();

        /// <summary>
        /// Saves the application state.
        /// </summary>
        /// <param name="force">
        /// Forces the application to save the state even if it is not dirty.
        /// </param>
        void SaveState(bool force = false);
    }
}