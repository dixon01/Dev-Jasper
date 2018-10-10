// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PartProgressMonitor.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PartProgressMonitor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Utility
{
    using System;

    /// <summary>
    /// Default implementation of <see cref="IPartProgressMonitor"/> that forwards
    /// progress updates to an <see cref="IProgressMonitor"/>.
    /// </summary>
    public class PartProgressMonitor : IPartProgressMonitor
    {
        private readonly IProgressMonitor progressMonitor;
        private readonly double startValue;
        private readonly double endValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartProgressMonitor"/> class.
        /// </summary>
        /// <param name="progressMonitor">
        /// The progress monitor to which the progress will be forwarded.
        /// </param>
        /// <param name="startValue">
        /// The start value of this part.
        /// </param>
        /// <param name="endValue">
        /// The end value of this part.
        /// </param>
        public PartProgressMonitor(IProgressMonitor progressMonitor, double startValue, double endValue)
        {
            this.progressMonitor = progressMonitor;
            this.startValue = startValue;
            this.endValue = endValue;
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
                return this.progressMonitor.IsCancelled;
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
            var current = this.startValue + (value * (this.endValue - this.startValue));
            this.progressMonitor.Progress(Math.Max(this.startValue, Math.Min(this.endValue, current)), note);
        }
    }
}