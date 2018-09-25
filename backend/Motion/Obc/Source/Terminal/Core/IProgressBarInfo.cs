// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProgressBarInfo.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IProgressBarInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    /// <summary>
    /// The progress bar information.
    /// </summary>
    public interface IProgressBarInfo
    {
        /// <summary>
        /// Gets the maximum time to display the progress bar. Time is in seconds
        /// </summary>
        int MaxTime { get; }

        /// <summary>
        /// Gets the caption.
        /// </summary>
        string Caption { get; }

        /// <summary>
        /// Update the progress.
        /// </summary>
        void ProgressElapsed();
    }
}