// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShutDownCatcherUi.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ShutDownCatcherUi type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ShutdownCatcherTest
{
    using System;
    using System.Windows.Forms;

    using Microsoft.Win32;

    /// <summary>
    /// The shut down catcher UI.
    /// </summary>
    public partial class ShutDownCatcherUi : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShutDownCatcherUi"/> class.
        /// </summary>
        public ShutDownCatcherUi()
        {
            this.InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
           // this.ShowInTaskbar = false;
        }

        /// <summary>
        /// Event that is fired whenever the O.S. session is ending.
        /// </summary>
        public event EventHandler<SessionEndingEventArgs> SessionEndingCatched;

        /// <summary>
        /// Event that is fired whenever the O.S. session is ended.
        /// </summary>
        public event EventHandler<SessionEndedEventArgs> SessionEndedCatched;

        /// <summary>
        /// Event that is fired whenever the O.S. power mode changes.
        /// </summary>
        public event EventHandler<PowerModeChangedEventArgs> PowerChangedCatched;

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Load"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data. </param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // I register my self to all the possible interesting events.
            SystemEvents.SessionEnding += this.OnSystemSessionEnding;
            SystemEvents.SessionEnded += this.OnSystemSessionEnded;
            SystemEvents.PowerModeChanged += this.OnSystemPowerModeChanged;
        }

        /// <summary>
        /// Function called by the O.S. when the power mode changes.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void OnSystemPowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (this.PowerChangedCatched != null)
            {
                this.PowerChangedCatched(this, e);
            }
        }

        /// <summary>
        /// Function called by the O.S. when the session ends.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void OnSystemSessionEnded(object sender, SessionEndedEventArgs e)
        {
            if (this.SessionEndedCatched != null)
            {
                this.SessionEndedCatched(this, e);
            }
        }

        /// <summary>
        /// Function called by the O.S. when the session is going to be ended.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void OnSystemSessionEnding(object sender, SessionEndingEventArgs e)
        {
            if (this.SessionEndingCatched != null)
            {
                this.SessionEndingCatched(this, e);
            }
        }
    }
}
