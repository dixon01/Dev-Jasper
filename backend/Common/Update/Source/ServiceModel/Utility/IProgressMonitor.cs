// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProgressMonitor.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IProgressMonitor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Utility
{
    /// <summary>
    /// Interface that provides feedback about the progress of a lengthy operation.
    /// </summary>
    public interface IProgressMonitor
    {
        /// <summary>
        /// Gets a value indicating whether the operation should be cancelled.
        /// The implementation of this class can cancel the operation that it's
        /// monitoring by setting this flag to true.
        /// </summary>
        bool IsCancelled { get; }

        /// <summary>
        /// This method is called exactly once at the beginning of the operation.
        /// It provides the minimum and maximum value of the progress.
        /// </summary>
        void Start();

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

        /// <summary>
        /// Creates a progress monitor for a part of the process.
        /// </summary>
        /// <param name="startValue">
        /// The start value (between 0.0 and 1.0).
        /// </param>
        /// <param name="endValue">
        /// The end value (between 0.0 and 1.0; greater than <see cref="startValue"/>).
        /// </param>
        /// <returns>
        /// The <see cref="IPartProgressMonitor"/> which will map its own range (0.0 to 1.0)
        /// to the given start and end values.
        /// </returns>
        IPartProgressMonitor CreatePart(double startValue, double endValue);

        /// <summary>
        /// This method is called exactly once at the end of the operation,
        /// independently whether the operation was successful or not.
        /// </summary>
        /// <param name="errorMessage">
        /// The error message or null if no error occurred.
        /// </param>
        /// <param name="successMessage">
        /// The success message or null if no message.
        /// </param>
        void Complete(string errorMessage, string successMessage);
    }
}
