// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgressTaskBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProgressTaskBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager.Controls
{
    using System;

    /// <summary>
    /// Base implementation of <see cref="IProgressTask"/>.
    /// </summary>
    public abstract class ProgressTaskBase : IProgressTask
    {
        private double value;

        private string state;

        /// <summary>
        /// Event that is risen when the <see cref="Value"/> changes.
        /// </summary>
        public event EventHandler ValueChanged;

        /// <summary>
        /// Event that is risen when the <see cref="State"/> changes.
        /// </summary>
        public event EventHandler StateChanged;

        /// <summary>
        /// Gets or sets the current progress value (between 0.0 and 1.0).
        /// </summary>
        public double Value
        {
            get
            {
                return this.value;
            }

            protected set
            {
                if (this.value >= value)
                {
                    return;
                }

                this.value = value;
                this.RaiseValueChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the current state.
        /// This is shown right above the progress bar.
        /// </summary>
        public string State
        {
            get
            {
                return this.state;
            }

            protected set
            {
                if (this.state == value)
                {
                    return;
                }

                this.state = value;
                this.RaiseStateChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this task was cancelled.
        /// </summary>
        protected bool IsCancelled { get; private set; }

        /// <summary>
        /// Runs this task.
        /// </summary>
        public abstract void Run();

        /// <summary>
        /// Cancels this task.
        /// </summary>
        public virtual void Cancel()
        {
            this.IsCancelled = true;
        }

        /// <summary>
        /// Raises the <see cref="ValueChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseValueChanged(EventArgs e)
        {
            var handler = this.ValueChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="StateChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseStateChanged(EventArgs e)
        {
            var handler = this.StateChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}