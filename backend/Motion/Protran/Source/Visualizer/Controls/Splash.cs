// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Splash.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Splash type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls
{
    using System.Drawing;
    using System.Windows.Forms;

    using Gorba.Motion.Protran.Visualizer.Properties;

    using NLog;
    using NLog.Config;
    using NLog.Targets;
    using NLog.Targets.Wrappers;

    /// <summary>
    /// Splash screen for the visualizer app.
    /// </summary>
    public sealed partial class Splash : Form
    {
        private readonly StringFormat logFormat = new StringFormat(StringFormatFlags.NoWrap)
            { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };

        private Rectangle logRect = new Rectangle(0, 294, 388, 42);

        private string logMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="Splash"/> class.
        /// </summary>
        public Splash()
        {
            this.InitializeComponent();

            this.logRect.Inflate(-5, -5);

            this.logMessage = "Loading...";
            this.BackgroundImage = Resources.SplashScreen;
            this.Size = this.BackgroundImage.Size;
            this.InitializeLogging();
        }

        /// <summary>
        /// The paint event handler.
        /// </summary>
        /// <param name="e">
        /// A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data. 
        /// </param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var msg = this.logMessage;
            if (msg == null)
            {
                return;
            }

            e.Graphics.DrawString(msg, this.Font, Brushes.White, this.logRect, this.logFormat);
        }

        private void InitializeLogging()
        {
            var config = LogManager.Configuration ?? new LoggingConfiguration();
            var target = new SplashTarget(this);
            config.AddTarget("splash", target);

            var loggingRule = new LoggingRule("*", LogLevel.Info, target);
            config.LoggingRules.Add(loggingRule);

            if (LogManager.Configuration == null)
            {
                // NLog wasn't configured before, so we have configure it for the first time
                LogManager.Configuration = config;
            }
            else
            {
                // NLog was configured before, so we just reload it
                LogManager.Configuration.Reload();
            }
        }

        private void AddLog(LogEventInfo logEvent)
        {
            if (!this.Visible)
            {
                return;
            }

            if (this.InvokeRequired)
            {
                this.BeginInvoke(new MethodInvoker(() => this.AddLog(logEvent)));
                return;
            }

            this.logMessage = logEvent.FormattedMessage;
            this.Invalidate(this.logRect);
        }

        private class SplashTarget : Target
        {
            private readonly Splash control;

            public SplashTarget(Splash control)
            {
                this.control = control;
            }

            protected override void Write(LogEventInfo logEvent)
            {
                this.control.AddLog(logEvent);
            }
        }
    }
}
