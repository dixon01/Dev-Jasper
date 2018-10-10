// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUpdateVisualization.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IUpdateVisualization type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Progress
{
    using Gorba.Common.Update.ServiceModel.Common;

    /// <summary>
    /// Visualization interface for update progress.
    /// </summary>
    public interface IUpdateVisualization
    {
        /// <summary>
        /// Shows this visualization.
        /// </summary>
        void Show();

        /// <summary>
        /// Updates the progress shown with the visualization.
        /// </summary>
        /// <param name="progress">
        /// The progress in percentage (0.0...0.1).
        /// </param>
        /// <param name="stage">
        /// The update stage.
        /// </param>
        /// <param name="note">
        /// The details about the current state of the progress.
        /// </param>
        void UpdateProgress(double progress, UpdateStage stage, string note);

        /// <summary>
        /// Adds an error message to this visualization.
        /// </summary>
        /// <param name="errorMessage">
        /// The error message, never null.
        /// </param>
        void AddErrorMessage(string errorMessage);

        /// <summary>
        /// Tells the visualization that the current progress has finished.
        /// </summary>
        void Finished();

        /// <summary>
        /// Hides this visualization.
        /// </summary>
        void Hide();

        /// <summary>
        /// Adds a success message to this visualization.
        /// </summary>
        /// <param name="successMessage">
        /// The success message, never null.
        /// </param>
        void AddSuccessMessage(string successMessage);
    }
}