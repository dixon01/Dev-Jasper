// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Shell.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Shell type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.AzureStorage.WpfApplication.ViewModels
{
    using System.Windows.Threading;

    /// <summary>
    /// Defines the shell.
    /// </summary>
    public class Shell : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Shell"/> class.
        /// </summary>
        /// <param name="dispatcher">
        /// The dispatcher.
        /// </param>
        public Shell(Dispatcher dispatcher)
        {
            this.Dispatcher = dispatcher;
            this.BlobStorageImplementation = new BlobStorageImplementation(this);
            this.FileStorageImplementation = new FileStorageImplementation(this);
        }

        /// <summary>
        /// Gets the dispatcher.
        /// </summary>
        public Dispatcher Dispatcher { get; private set; }

        /// <summary>
        /// Gets the blob storage.
        /// </summary>
        public BlobStorageImplementation BlobStorageImplementation { get; private set; }

        /// <summary>
        /// Gets the file storage.
        /// </summary>
        public FileStorageImplementation FileStorageImplementation { get; private set; }
    }
}