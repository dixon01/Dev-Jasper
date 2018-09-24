// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureProgressPart.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implementation of <see cref="IPartProgressMonitor" /> for FTP uploads and downloads.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.AzureClient
{
    using Gorba.Common.Update.ServiceModel.Utility;

    /// <summary>
    /// Implementation of <see cref="IPartProgressMonitor"/> for FTP uploads and downloads.
    /// </summary>
    internal class AzureProgressPart : IPartProgressMonitor
    {
        private readonly string title;

        private readonly IPartProgressMonitor monitor;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureProgressPart"/> class.
        /// </summary>
        /// <param name="title">
        /// The title of the progress.
        /// </param>
        /// <param name="progressMonitor">
        /// The progress monitor.
        /// </param>
        /// <param name="progress">
        /// The end progress.
        /// </param>
        /// <param name="maxProgress">
        /// The maximum progress.
        /// </param>
        public AzureProgressPart(string title, IProgressMonitor progressMonitor, double progress, double maxProgress)
        {
            this.title = title;
            this.monitor = progressMonitor.CreatePart((progress - 1) / maxProgress, progress / maxProgress);
        }

        /// <summary>
        /// Gets a value indicating whether the operation should be cancelled.
        /// The implementation of this class can cancel the operation that it's
        /// monitoring by setting this flag to true.
        /// </summary>
        public bool IsCancelled
        {
            get
            {
                return this.monitor.IsCancelled;
            }
        }

        /// <summary>
        /// This method is called every time the operation has progressed.
        /// </summary>
        /// <param name="value">
        /// The new progress value; it is always between 0.0 and 1.0.
        /// </param>
        /// <param name="note">
        /// The note.
        /// </param>
        public void Progress(double value, string note)
        {
            this.monitor.Progress(value, string.Format("{0}: {1}", this.title, note));
        }
    }
}
