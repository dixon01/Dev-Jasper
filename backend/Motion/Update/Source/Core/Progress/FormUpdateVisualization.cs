// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormUpdateVisualization.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FormUpdateVisualization type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Progress
{
    using System.Drawing;
    using System.Threading;
    using System.Windows.Forms;

    using Gorba.Common.Configuration.Update.Application;
    using Gorba.Common.Update.ServiceModel.Common;

    /// <summary>
    /// Update visualization that uses a Windows Form.
    /// </summary>
    public class FormUpdateVisualization : IUpdateVisualization
    {
        private readonly ManualResetEvent runWait = new ManualResetEvent(false);

        private readonly SplashScreenVisualizationConfig config;

        private UpdateProgressForm form;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormUpdateVisualization"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public FormUpdateVisualization(SplashScreenVisualizationConfig config)
        {
            this.config = config;
        }

        /// <summary>
        /// Shows this visualization.
        /// </summary>
        public void Show()
        {
            if (this.form == null)
            {
                var thread = new Thread(this.Run);
                thread.IsBackground = true;
                thread.Start();
                this.runWait.WaitOne();
                Thread.Sleep(100);
            }

            if (this.form != null)
            {
                this.form.BeginInvoke(new ThreadStart(() => this.form.ShowForm()));
            }
        }

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
        public void UpdateProgress(double progress, UpdateStage stage, string note)
        {
            if (this.form == null)
            {
                return;
            }

            this.form.BeginInvoke(new ThreadStart(() => this.form.UpdateProgress(progress, stage, note)));
        }

        /// <summary>
        /// Adds an error message to this visualization.
        /// </summary>
        /// <param name="errorMessage">
        /// The error message, never null.
        /// </param>
        public void AddErrorMessage(string errorMessage)
        {
            this.form.BeginInvoke(new ThreadStart(() => this.form.AddErrorMessage(errorMessage)));
        }

        /// <summary>
        /// Adds a success message to this visualization.
        /// </summary>
        /// <param name="successMessage">
        /// The success message, never null.
        /// </param>
        public void AddSuccessMessage(string successMessage)
        {
            this.form.BeginInvoke(new ThreadStart(() => this.form.AddSuccessMessage(successMessage)));
        }

        /// <summary>
        /// Tells the visualization that the current progress has finished.
        /// </summary>
        public void Finished()
        {
            this.form.BeginInvoke(new ThreadStart(() => this.form.Finished()));
        }

        /// <summary>
        /// Hides this visualization.
        /// </summary>
        public void Hide()
        {
            if (this.form == null)
            {
                return;
            }

            this.form.BeginInvoke(new ThreadStart(() => this.form.Close()));
            Thread.Sleep(100);
            this.form = null;
        }

        private void Run()
        {
            this.form = new UpdateProgressForm();
            this.runWait.Set();
            this.form.VisibleArea = new Rectangle(
                this.config.X, this.config.Y, this.config.Width, this.config.Height);
            Application.Run(this.form);
        }
    }
}