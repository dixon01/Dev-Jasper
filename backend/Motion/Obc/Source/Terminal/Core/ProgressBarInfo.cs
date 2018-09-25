// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgressBarInfo.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProgressBarInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    using System;

    /// <summary>
    /// The progress bar information implementation.
    /// </summary>
    public class ProgressBarInfo : IProgressBarInfo
    {
        private readonly string caption;
        private readonly int maxTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressBarInfo"/> class.
        /// </summary>
        /// <param name="caption">
        /// The caption.
        /// </param>
        /// <param name="maxTime">
        /// The maximum time that the ProgressBar will be displayed. Time is in seconds
        /// </param>
        public ProgressBarInfo(string caption, int maxTime)
        {
            this.caption = caption;
            this.maxTime = maxTime;
        }

        /// <summary>
        ///   Use this event when you use a progress bar.
        ///   It's better to not use it. because there are strange behavior of the UI. some elements will not be
        ///   refreshed correctly.... un-comment the MainField.SetControlsEnable(). But then the buttons can be pressed.
        /// </summary>
        public event EventHandler ProgressBarElapsed;

        /// <summary>
        /// Gets the maximum time to display the progress bar. Time is in seconds
        /// </summary>
        public int MaxTime
        {
            get
            {
                return this.maxTime;
            }
        }

        /// <summary>
        /// Gets the caption.
        /// </summary>
        public string Caption
        {
            get
            {
                return this.caption;
            }
        }

        /// <summary>
        /// Update the progress.
        /// </summary>
        public void ProgressElapsed()
        {
            if (this.ProgressBarElapsed != null)
            {
                this.ProgressBarElapsed(this, EventArgs.Empty);
            }
        }
    }
}