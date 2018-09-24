// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateProgressForm.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateProgressForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Progress
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Reflection;
    using System.Text;
    using System.Windows.Forms;

    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Motion.Common.Utility.SplashScreen;

    using NLog;

    using Math = System.Math;

    /// <summary>
    /// Form that shows the current update progress.
    /// </summary>
    public sealed partial class UpdateProgressForm : SplashScreenFormBase
    {
        private const int MaxProgress = 10000;

        private readonly List<string> errorMessages = new List<string>();

        private readonly List<string> successMessages = new List<string>();

        private bool showDetails;

        private double currentProgress;

        private UpdateStage currentStage;

        private string currentNote;

        private bool finished;

        private Rectangle visibleArea;

        private Logger Logger { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateProgressForm"/> class.
        /// </summary>
        public UpdateProgressForm()
        {
            this.InitializeComponent();

            this.progressBar.Minimum = 0;
            this.progressBar.Maximum = MaxProgress;
            this.Logger = LogManager.GetLogger(this.GetType().FullName);
            this.PrepareForm();
        }

        /// <summary>
        /// Gets or sets the visible area in which the progress information should be shown.
        /// This is used to show the information only in the upper part of a screen for a
        /// wide-screen (~16:5).
        /// </summary>
        public Rectangle VisibleArea
        {
            get
            {
                return this.visibleArea;
            }

            set
            {
                this.visibleArea = value;
                this.PrepareForm();
            }
        }

        /// <summary>
        /// Updates the progress shown on this form.
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
            this.currentProgress = progress;
            this.currentStage = stage;
            this.currentNote = note;

            progress = Math.Min(Math.Max(progress, 0), 1);
            this.progressBar.Value = (int)(progress * MaxProgress);

            string text;
            switch (stage)
            {
                case UpdateStage.ReceivingUpdate:
                    text = "Downloading...";
                    break;
                case UpdateStage.Installing:
                    text = "Installing...";
                    break;
                case UpdateStage.SendingFeedback:
                    text = "Sending Feedback...";
                    break;
                case UpdateStage.ForwardingUpdate:
                    text = "Forwarding...";
                    break;
                case UpdateStage.ForwardingFeedback:
                    text = "Forwarding...";
                    break;
                default:
                    text = string.Empty;
                    break;
            }

            if (this.showDetails)
            {
                text = string.Format("{0} ({1})", text, note);
            }
            this.Logger.Info(MethodBase.GetCurrentMethod().Name + $" Update State Info : {text}");
            this.labelState.Text = text;
        }

        /// <summary>
        /// Adds an error message to this visualization.
        /// </summary>
        /// <param name="errorMessage">
        /// The error message, never null.
        /// </param>
        public void AddErrorMessage(string errorMessage)
        {
            this.errorMessages.Add(errorMessage);
            if (this.finished)
            {
                this.SetCompletedState();
            }
        }

        /// <summary>
        /// Adds a success message to this visualization.
        /// </summary>
        /// <param name="successMessage">
        /// The success message, never null.
        /// </param>
        public void AddSuccessMessage(string successMessage)
        {
            this.successMessages.Add(successMessage);
            if (this.finished)
            {
                this.SetCompletedState();
            }
        }

        /// <summary>
        /// Tells the form that the current progress has finished.
        /// </summary>
        public void Finished()
        {
            this.Logger.Info(MethodBase.GetCurrentMethod().Name + $" Update State Progress: {MaxProgress}");
            this.finished = true;
            this.progressBar.Value = MaxProgress;
            this.SetCompletedState();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.KeyDown"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.KeyEventArgs"/> that contains the event data. </param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Hide();
            }
            else
            {
                this.showDetails = !this.showDetails;
                this.UpdateProgress(this.currentProgress, this.currentStage, this.currentNote);
            }

            base.OnKeyDown(e);
        }

        partial void PrepareForm();

        private void SetCompletedState()
        {
            var text = new StringBuilder("Completed");

            if (this.errorMessages.Count > 0)
            {
                text.Append(" with errors:\r\n");
                foreach (var message in this.errorMessages)
                {
                    text.Append(message).Append("\r\n");
                }
            }

            if (this.successMessages.Count > 0)
            {
                text.Append(". ");
                foreach (var successMessage in this.successMessages)
                {
                    text.Append(successMessage).Append("\r\n");
                }
            }
            this.Logger.Info(MethodBase.GetCurrentMethod().Name + $" Update State:  {text}");
            this.labelState.Text = text.ToString();
        }
    }
}
