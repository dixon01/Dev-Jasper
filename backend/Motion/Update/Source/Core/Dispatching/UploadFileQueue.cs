// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UploadFileQueue.cs" company="Luminator LTG">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateQueueBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Dispatching
{
    using System.IO;

    using Gorba.Common.Update.ServiceModel.Common;

    /// <summary>
    /// The upload file queue.
    /// </summary>
    internal class UploadFileQueue : UpdateQueueBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UploadFileQueue"/> class. 
        /// </summary>
        /// <param name="updateSource">
        /// The update source.
        /// </param>
        /// <param name="queueBaseDirectory">
        /// The base directory for all queues of this type.
        /// </param>
        /// <param name="poolDirectory">
        /// The pool directory.
        /// </param>
        public UploadFileQueue(IUpdateSource updateSource, string queueBaseDirectory, string poolDirectory)
            : base(updateSource, queueBaseDirectory, poolDirectory)
        {
            this.UpdateSource = updateSource;
            this.UpdateSource.UploadsDirectory = queueBaseDirectory;
        }

        /// <summary>
        /// Gets the update source for which this queue is responsible.
        /// </summary>
        public IUpdateSource UpdateSource { get; }

        /// <summary>
        /// Gets the queue directory for this queue.
        /// </summary>
        protected string UploadsDirectory { get; private set; }

        /// <summary>
        /// The Upload queue uses the same queue for everyone.
        /// </summary>
        /// <param name="component">The update component using the upload file queue</param>
        /// <param name="queueBaseDirectory">The directory to upload files from</param>
        public override void InitializeQueueDirectories(IUpdateComponent component, string queueBaseDirectory)
        {
            this.UploadsDirectory = queueBaseDirectory;

            Directory.CreateDirectory(this.UploadsDirectory);

            this.Component.IsAvailableChanged += this.ComponentOnIsAvailableChanged;
        }

        /// <summary>
        /// Upload our files to their remote destination.
        /// </summary>
        protected override void DoNotify()
        {
            this.UpdateSource.UploadFiles();

            this.Logger.Trace("Upload complete.");
        }
    }
}