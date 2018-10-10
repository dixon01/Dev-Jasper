// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShutDownCatcher.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ShutDownCatcher type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ShutdownCatcherTest
{
    using System;
    using System.Threading;
    using System.Windows.Forms;

    using Microsoft.Win32;

    using NLog;

    /// <summary>
    /// Class to handle shutdown events from the system.
    /// </summary>
    public class ShutDownCatcher
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static ShutDownCatcher instance;

        private readonly ShutDownCatcherUi form;

        private ShutDownCatcher()
        {
            this.form = new ShutDownCatcherUi();
            this.form.SessionEndingCatched += this.FormOnSessionEndingCatched;
            this.form.SessionEndedCatched += this.FormOnSessionEndedCatched;
            this.form.PowerChangedCatched += this.FormOnPowerChangedCatched;
            this.form.FormClosing += (sender, args) => Logger.Info("FormClosing");
            this.form.FormClosed += (sender, args) => Logger.Info("FormClosed");
            this.form.Disposed += (sender, args) => Logger.Info("Form Disposed");

            this.form.WindowState = FormWindowState.Minimized;

            var thread = new Thread(() => this.form.ShowDialog());
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// Event that is fired whenever the O.S. session is ending.
        /// </summary>
        public event EventHandler<SessionEndingEventArgs> SessionEnding;

        /// <summary>
        /// Event that is fired whenever the O.S. session is ended.
        /// </summary>
        public event EventHandler<SessionEndedEventArgs> SessionEnded;

        /// <summary>
        /// Event that is fired whenever the O.S. power mode changes.
        /// </summary>
        public event EventHandler<PowerModeChangedEventArgs> PowerChanged;

        /// <summary>
        /// Gets the single instance of this class.
        /// </summary>
        public static ShutDownCatcher Instance
        {
            get
            {
                return instance ?? (instance = new ShutDownCatcher());
            }
        }

        private void FormOnSessionEndingCatched(object sender, SessionEndingEventArgs e)
        {
            var handler = this.SessionEnding;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void FormOnSessionEndedCatched(object sender, SessionEndedEventArgs e)
        {
            var handler = this.SessionEnded;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void FormOnPowerChangedCatched(object sender, PowerModeChangedEventArgs e)
        {
            var handler = this.PowerChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
