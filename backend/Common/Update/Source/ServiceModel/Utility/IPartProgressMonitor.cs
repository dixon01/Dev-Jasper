// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPartProgressMonitor.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IPartProgressMonitor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Utility
{
    /// <summary>
    /// Progress monitor for a part of a <see cref="IProgressMonitor"/>.
    /// </summary>
    public interface IPartProgressMonitor
    {
        /// <summary>
        /// Gets a value indicating whether the operation should be cancelled.
        /// The implementation of this class can cancel the operation that it's
        /// monitoring by setting this flag to true.
        /// </summary>
        bool IsCancelled { get; }

        /// <summary>
        /// This method is called every time the operation has progressed.
        /// </summary>
        /// <param name="value">
        /// The new progress value; it is always between 0.0 and 1.0.
        /// </param>
        /// <param name="note">
        /// The note.
        /// </param>
        void Progress(double value, string note);
    }
}