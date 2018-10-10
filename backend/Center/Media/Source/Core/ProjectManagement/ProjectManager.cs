// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ProjectManagement
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Common.Update.ServiceModel;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// The project manager is responsible to handle the creation, the saving and the loading of projects.
    /// It encapsulates the <see cref="ProjectFile"/>.
    /// </summary>
    public class ProjectManager : IProjectManager
    {
        private const int BufferSize = 2048;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IResourceProvider localResourceProvider;

        private ProjectFile projectFile;

        private bool isFileSelected;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectManager"/> class.
        /// </summary>
        /// <param name="localResourceProvider">The local resource provider.</param>
        public ProjectManager(IResourceProvider localResourceProvider)
        {
            this.localResourceProvider = localResourceProvider;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is in a valid state to call
        /// the <see cref="SaveAsync"/> method.
        /// This instance is in a valid state after a call to the <see cref="CreateProject"/> or the
        /// <see cref="LoadProject"/> methods.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is in a valid state to call the <see cref="SaveAsync"/> method; otherwise,
        /// <c>false</c>.
        /// </value>
        public bool IsFileSelected
        {
            get
            {
                return this.isFileSelected;
            }

            set
            {
                if (this.isFileSelected == value)
                {
                    return;
                }

                this.isFileSelected = value;
                this.RaisePropertyChanged("IsFileSelected");
            }
        }

        /// <summary>
        /// Gets the full file name of the project.
        /// </summary>
        public string FullFileName
        {
            get
            {
                return this.projectFile == null ? null : this.projectFile.FullFileName;
            }
        }

        /// <summary>
        /// Gets the file name with extension of the project.
        /// </summary>
        public string FileName
        {
            get
            {
                return this.projectFile == null ? null : this.projectFile.FileName;
            }
        }

        /// <summary>
        /// Gets a resource for a given hash.
        /// </summary>
        /// <param name="hash">The hash.</param>
        /// <returns>
        /// The <see cref="IResource"/>.
        /// </returns>
        /// <exception cref="UpdateException">
        /// If the resource couldn't be found or is otherwise invalid.
        /// </exception>
        [Obsolete("Use GetResourceAsync instead.")]
        public IResource GetResource(string hash)
        {
            try
            {
                var resource = this.localResourceProvider.GetResource(hash);
                return resource;
            }
            catch (UpdateException exception)
            {
                Logger.WarnException(
                    string.Format("Failed loading the resource with hash '{0}' from the local resource provider", hash),
                    exception);
            }

            if (this.projectFile == null)
            {
                throw new UpdateException("Can't find resource with hash '" + hash + "'");
            }

            var extractedResource = this.projectFile.GetResource(hash);
            var tempPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), hash + ".tmp");
            var fileInfo = ((IWritableFileSystem)FileSystemManager.Local).CreateFile(tempPath);
            using (var targetStream = fileInfo.OpenWrite())
            {
                using (var sourceStream = extractedResource.OpenRead())
                {
                    sourceStream.CopyTo(targetStream);
                }
            }

            // Setting the deleteFile flag will move the temp file to the right storage location
            this.localResourceProvider.AddResource(hash, fileInfo.FullName, true);
            return extractedResource;
        }

        /// <summary>
        /// Tries to get the resource for a given hash from local storage. If it is not found, the resource is
        /// downloaded from server and stored locally.
        /// </summary>
        /// <param name="hash">
        /// The hash.
        /// </param>
        /// <returns>
        /// The resource.
        /// </returns>
        public async Task<IResource> GetResourceAsync(string hash)
        {
            try
            {
                var resource = this.localResourceProvider.GetResource(hash);
                return resource;
            }
            catch (UpdateException exception)
            {
                Logger.WarnException(
                    string.Format("Failed loading the resource with hash '{0}' from the local resource provider", hash),
                    exception);
            }

            return await this.DownloadResource(hash).ContinueWith(
                t =>
                    {
                        if (t.IsFaulted)
                        {
                            throw new UpdateException(
                                    "Resource with hash " + hash + " couldn't be found on server", t.Exception);
                        }

                        Logger.Info("Resource with hash " + hash + " downloaded from server.");
                        return t.Result;
                    });
        }

        /// <summary>
        /// Adds a resource to the provider.
        /// </summary>
        /// <param name="hash">The expected hash of the resource file (the name from where it was copied).</param>
        /// <param name="resourceFile">The full resource file path.</param>
        /// <param name="deleteFile">
        /// A flag indicating whether the <see cref="resourceFile"/> should be deleted after being registered.
        /// </param>
        /// <exception cref="UpdateException">
        /// If the resource file doesn't match the given hash.
        /// </exception>
        public void AddResource(string hash, string resourceFile, bool deleteFile)
        {
            this.localResourceProvider.AddResource(hash, resourceFile, deleteFile);
        }

        /// <summary>
        /// Creates a new project at the given <paramref name="filename"/>.
        /// This will only create an empty file.
        /// </summary>
        /// <param name="filename">
        /// The full filename where to store the project file.
        /// </param>
        public void CreateProject(string filename)
        {
            this.projectFile = ProjectFile.CreateNew(filename);
            this.IsFileSelected = true;
        }

        /// <summary>
        /// Loads the project.
        /// </summary>
        /// <param name="filename">Full path to the file containing a project file.</param>
        /// <returns>The <see cref="MediaProjectDataModel"/> loaded.</returns>
        public MediaProjectDataModel LoadProject(string filename)
        {
            this.projectFile = ProjectFile.Load(filename);
            this.IsFileSelected = true;
            return this.projectFile.MediaProject;
        }

        /// <summary>
        /// Asynchronously saves the specified <paramref name="project"/>.
        /// The project can only be saved after loading an file (<see cref="LoadProject"/>) or creating a new empty
        /// one (<see cref="CreateProject"/>).
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
        public async Task SaveAsync(MediaProjectDataModel project)
        {
            if (project == null)
            {
                throw new ArgumentNullException("project");
            }

            if (this.projectFile == null)
            {
                throw new InvalidOperationException("Can't save a project if it was never created or loaded");
            }

            this.projectFile.MediaProject = project;
            await this.projectFile.SaveAsync(this.localResourceProvider);
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler == null)
            {
                return;
            }

            handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task<IResource> DownloadResource(string hash)
        {
            var controller = ServiceLocator.Current.GetInstance<IMediaApplicationController>();
            using (var channel = controller.ConnectionController.CreateChannelScope<IResourceService>())
            {
                var request = new ResourceDownloadRequest { Hash = hash };
                var downloadResult = await channel.Channel.DownloadAsync(request);
                var tempPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), hash + ".tmp");
                var fileInfo = ((IWritableFileSystem)FileSystemManager.Local).CreateFile(tempPath);
                using (var targetStream = fileInfo.OpenWrite())
                {
                    await downloadResult.Content.CopyToAsync(targetStream, BufferSize);
                }

                // Setting the deleteFile flag will move the temp file to the right storage location
                this.localResourceProvider.AddResource(hash, fileInfo.FullName, true);
            }

            return this.localResourceProvider.GetResource(hash);
        }
    }
}