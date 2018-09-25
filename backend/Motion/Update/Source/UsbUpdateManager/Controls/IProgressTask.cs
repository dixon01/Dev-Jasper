// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProgressTask.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IProgressTask type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager.Controls
{
    using System;

    /// <summary>
    /// The ProgressTask interface.
    /// </summary>
    public interface IProgressTask
    {
        /// <summary>
        /// Event that is risen when the <see cref="Value"/> changes.
        /// </summary>
        event EventHandler ValueChanged;

        /// <summary>
        /// Event that is risen when the <see cref="State"/> changes.
        /// </summary>
        event EventHandler StateChanged;

        /// <summary>
        /// Gets the current progress value (between 0.0 and 1.0).
        /// </summary>
        double Value { get; }

        /// <summary>
        /// Gets the current state.
        /// This is shown right above the progress bar.
        /// </summary>
        string State { get; }

        /// <summary>
        /// Runs this task.
        /// </summary>
        void Run();

        /// <summary>
        /// Cancels this task.
        /// </summary>
        void Cancel();
    }
}