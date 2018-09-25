// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateProgressManager.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateProgressManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;

namespace Gorba.Motion.Update.Core.Progress
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Update.Application;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Utility;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Manager that monitors the progress of the update process and informs
    /// <see cref="IUpdateVisualization"/>s about the progress.
    /// </summary>
    public partial class UpdateProgressManager
    {
        private static readonly Logger Logger = LogHelper.GetLogger<UpdateProgressManager>();

        private readonly VisualizationConfig config;

        private readonly List<IUpdateVisualization> visualizations = new List<IUpdateVisualization>();

        private readonly List<ProgressMonitor> monitors = new List<ProgressMonitor>();

        private readonly ITimer finishTimer;
        private readonly ITimer hideTimer;

        private bool showing;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateProgressManager"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public UpdateProgressManager(VisualizationConfig config)
        {
            this.config = config;
            this.visualizations.Add(new LoggerVisualization());

            if (this.config.SplashScreen.Enabled)
            {
                this.visualizations.Add(new FormUpdateVisualization(this.config.SplashScreen));
            }

            this.CreatePlatformVisualizations();

            this.finishTimer = TimerFactory.Current.CreateTimer(this.GetType().Name + "-Finish");
            this.finishTimer.Interval = TimeSpan.FromSeconds(5);
            this.finishTimer.AutoReset = false;
            this.finishTimer.Elapsed += this.FinishTimerOnElapsed;

            var hideTimeout = this.config.HideTimeout - this.finishTimer.Interval;
            if (hideTimeout <= TimeSpan.Zero)
            {
                hideTimeout = TimeSpan.FromSeconds(1);
            }

            this.hideTimer = TimerFactory.Current.CreateTimer(this.GetType().Name + "-Hide");
            this.hideTimer.Interval = hideTimeout;
            this.hideTimer.AutoReset = false;
            this.hideTimer.Elapsed += this.HideTimerOnElapsed;
        }

        /// <summary>
        /// Creates a new progress monitor for the given state.
        /// </summary>
        /// <param name="stage">
        /// The update stage for which the progress is shown.
        /// </param>
        /// <returns>
        /// The <see cref="IProgressMonitor"/>; never null.
        /// </returns>
        public IProgressMonitor CreateProgressMonitor(UpdateStage stage)
        {
            Logger.Debug("Creating progress monitor for {0}", stage);
            return new ProgressMonitor(this, stage);
        }

        partial void CreatePlatformVisualizations();

        private void StartMonitor(ProgressMonitor monitor)
        {
            Logger.Info("Starting monitor for {0}", monitor.Stage);

            Logger.Info($"Finish timer status: {finishTimer?.Enabled}");
            Logger.Info($"Hide timer status: {hideTimer?.Enabled}");
            if (hideTimer != null && hideTimer.Enabled)
            {
                // If hide timer is enabled, simply run the elapsed function
                Logger.Info($"Hide timer enabled while update polled; elapsing hide timer now");
                this.hideTimer.Enabled = false;
                HideTimerOnElapsed(this, null);
            }

            this.hideTimer.Enabled = false;
            this.finishTimer.Enabled = false;
            lock (this.monitors)
            {
                this.monitors.Add(monitor);

                if (this.monitors.Count > 1 || this.showing)
                {
                    return;
                }
            }

            lock (this.visualizations)
            {
                this.showing = true;
                foreach (var visualization in this.visualizations)
                {
                    visualization.Show();
                }
            }
        }

        private void ShowProgress(ProgressMonitor monitor)
        {
            lock (this.monitors)
            {
                if (this.monitors.Count == 0 || this.monitors[this.monitors.Count - 1] != monitor)
                {
                    Logger.Info(
                        "Not showing progress for {0}: {1:0}% {2}",
                        monitor.Stage,
                        monitor.Progress * 100,
                        monitor.Note);
                    return;
                }
            }

            lock (this.visualizations)
            {
                foreach (var visualization in this.visualizations)
                {
                    visualization.UpdateProgress(monitor.Progress, monitor.Stage, monitor.Note);
                }
            }
        }

        private void EndMonitor(ProgressMonitor monitor, string errorMessage, string successMessage)
        {
            Logger.Debug("Ending monitor {0}: {1}", monitor.Stage, errorMessage);
            ProgressMonitor next = null;
            lock (this.monitors)
            {
                this.monitors.Remove(monitor);

                if (this.monitors.Count > 0)
                {
                    next = this.monitors[this.monitors.Count - 1];
                }
            }

            if (errorMessage != null)
            {
                this.AddErrorMessage(errorMessage);
            }

            if (successMessage != null)
            {
                this.AddSuccessMessage(successMessage);
            }

            if (next != null)
            {
                Logger.Debug("Showing next monitor {0}", next.Stage);
                this.ShowProgress(next);
                return;
            }

            // we delay showing the finished state because there might
            // be a new progress coming up very soon
            Logger.Info("Starting finish timer");
            this.hideTimer.Enabled = false;
            this.finishTimer.Enabled = false;
            this.finishTimer.Enabled = true;
        }

        private void AddSuccessMessage(string successMessage)
        {
            lock (this.visualizations)
            {
                foreach (var visualization in this.visualizations)
                {
                    visualization.AddSuccessMessage(successMessage);
                }
            }
        }

        private void AddErrorMessage(string errorMessage)
        {
            lock (this.visualizations)
            {
                foreach (var visualization in this.visualizations)
                {
                    visualization.AddErrorMessage(errorMessage);
                }
            }
        }

        private void FinishTimerOnElapsed(object sender, EventArgs e)
        {
            Logger.Info("Finish timer elapsed, starting hide timer");
            this.hideTimer.Enabled = false;
            this.hideTimer.Enabled = true;
            lock (this.visualizations)
            {
                foreach (var visualization in this.visualizations)
                {
                    Logger.Info("Calling visualization.Finished()");
                    visualization.Finished();
                }
            }
        }

        private void HideTimerOnElapsed(object sender, EventArgs e)
        {
            Logger.Info("Hide timer elapsed");
            lock (this.visualizations)
            {
                this.showing = false;
                foreach (var visualization in this.visualizations)
                {
                    Logger.Info("Calling visualization.Hide()");
                    visualization.Hide();
                }
            }
        }

        private class LoggerVisualization : IUpdateVisualization
        {
            public void Show()
            {
                Logger.Info("Showing progress visualizations");
            }

            public void UpdateProgress(double progress, UpdateStage stage, string note)
            {
                Logger.Info("Progress of {0}: {1:0}% {2}", stage, progress * 100, note);
            }

            public void AddErrorMessage(string errorMessage)
            {
                Logger.Warn("Added error message: {0}", errorMessage);
            }

            public void Finished()
            {
                Logger.Debug("Finished, waiting to hide visualizations");
            }

            public void Hide()
            {
                Logger.Info("Hiding progress visualizations");
            }

            public void AddSuccessMessage(string successMessage)
            {
                Logger.Warn("Added success message: {0}", successMessage);
            }
        }

        private class ProgressMonitor : IProgressMonitor
        {
            private readonly UpdateProgressManager owner;

            public ProgressMonitor(UpdateProgressManager owner, UpdateStage stage)
            {
                this.owner = owner;
                this.Stage = stage;
            }

            public UpdateStage Stage { get; private set; }

            public double Progress { get; private set; }

            public string Note { get; private set; }

            bool IProgressMonitor.IsCancelled
            {
                get
                {
                    return false;
                }
            }

            void IProgressMonitor.Start()
            {
                this.owner.StartMonitor(this);
            }

            void IProgressMonitor.Progress(double value, string note)
            {
                this.UpdateProgress(value, note);
            }

            public IPartProgressMonitor CreatePart(double startValue, double endValue)
            {
                if (endValue <= startValue)
                {
                    throw new ArgumentOutOfRangeException("endValue", "End value must be greater than start value");
                }

                return new PartProgressMonitor(this, startValue, endValue);
            }

            void IProgressMonitor.Complete(string errorMessage, string successMessage)
            {
                this.UpdateProgress(1.0, "Done");
                this.owner.EndMonitor(this, errorMessage, successMessage);
            }

            private void UpdateProgress(double value, string note)
            {
                this.Progress = value;
                this.Note = note;
                this.owner.ShowProgress(this);
            }
        }
    }
}
