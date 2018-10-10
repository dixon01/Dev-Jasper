// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectManager.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IProjectManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ProjectManagement
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;

    using Gorba.Center.Media.Core.Models;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Motion.Infomedia.RendererBase.Manager.Animation;

    /// <summary>
    /// Defines the component responsible to handle lifetime of projects and resources.
    /// </summary>
    public interface IProjectManager : IResourceProvider, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is in a valid state to call the <see cref="Save"/>
        /// method.
        /// This instance is in a valid state after a call to the <see cref="CreateProject"/> or the
        /// <see cref="LoadProject"/> methods.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is in a valid state to call the <see cref="Save"/> method; otherwise,
        /// <c>false</c>.
        /// </value>
        bool IsFileSelected { get; set; }

        /// <summary>
        /// Gets the full file name of the project.
        /// </summary>
        string FullFileName { get; }

        /// <summary>
        /// Gets the file name with extension of the project.
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// Creates a new project at the given <paramref name="filename"/>.
        /// This will only create an empty file.
        /// </summary>
        /// <param name="filename">
        /// The full filename where to store the project file.
        /// </param>
        void CreateProject(string filename);

        /// <summary>
        /// Loads the project.
        /// </summary>
        /// <param name="filename">Full path to the file containing a project file.</param>
        /// <returns>The <see cref="MediaProjectDataModel"/> loaded.</returns>
        MediaProjectDataModel LoadProject(string filename);

        /// <summary>
        /// Asynchronously saves the specified <paramref name="project"/>.
        /// The project can only be saved after loading an file (<see cref="ProjectManager.LoadProject"/>) or creating
        /// a new empty
        /// one (<see cref="ProjectManager.CreateProject"/>).
        /// </summary>
        /// <param name="project">
        /// The project.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The project parameter is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The project was never created or loaded.
        /// </exception>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task SaveAsync(MediaProjectDataModel project);

        /// <summary>
        /// Tries to get the resource for a given hash from local storage. If it is not found, the resource is
        /// downloaded from server and stored locally.
        /// </summary>
        /// <param name="hash">
        /// The hash.
        /// </param>
        /// <returns>
        /// The <see cref="IResource"/>.
        /// </returns>
        Task<IResource> GetResourceAsync(string hash);
    }
}