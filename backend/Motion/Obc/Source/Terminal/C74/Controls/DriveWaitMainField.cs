// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DriveWaitMainField.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DriveWaitMainField type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.C74.Controls
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The drive wait main field which is used for block drive waiting
    /// and special destination drive.
    /// </summary>
    public partial class DriveWaitMainField : MainField, IBlockDriveWait, ISpecialDestinationDrive
    {
        private DateTime driveTimeStart;

        /// <summary>
        /// Initializes a new instance of the <see cref="DriveWaitMainField"/> class.
        /// </summary>
        public DriveWaitMainField()
        {
            this.InitializeComponent();
            this.HideWaitScreenElements();

            this.UpdateLanguage();
            LanguageManager.Instance.CurrentLanguageChanged += (s, e) => this.UpdateLanguage();
        }

        /// <summary>
        /// Initializes this field.
        /// </summary>
        /// <param name="destinationText">
        /// The destination text.
        /// </param>
        public void Init(string destinationText)
        {
            this.Init(destinationText, null, default(DateTime));
        }

        /// <summary>
        /// Initializes the field by setting its data.
        /// </summary>
        /// <param name="destinationText">
        /// The destination text.
        /// </param>
        /// <param name="startText">
        /// The start text.
        /// </param>
        /// <param name="startTime">
        /// The drive time start.
        /// </param>
        public void Init(string destinationText, string startText, DateTime startTime)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => this.Init(destinationText, startText, startTime)));
                return;
            }

            this.Init();
            this.buttonMenu.IsSelected = true;

            this.driveTimeStart = startTime;
            this.textDestination.Text = destinationText;
            if (startText == null)
            {
                // Special destination drive...
                this.timer.Enabled = false;
                this.HideWaitScreenElements();
                return;
            }

            if (startText == string.Empty && destinationText == string.Empty)
            {
                // Block drive. But evTripLoaded event not yet received
                this.textDeparture.Text = string.Empty;
                this.textStartTime.Text = string.Empty;
                this.textDestination.Text = string.Empty;
                this.timer.Enabled = false;
            }
            else
            {
                this.textDeparture.Text = startText;
                this.DisplayStartCountDown();
                this.timer.Enabled = true;
            }

            this.ShowAllElements();
        }

        private void HideWaitScreenElements()
        {
            this.labelDeparture.Visible = false;
            this.labelStartTime.Visible = false;
            this.panelLight.Visible = false;
            this.textDeparture.Visible = false;
            this.textStartTime.Visible = false;
        }

        private void ShowAllElements()
        {
            this.labelDeparture.Visible = true;
            this.labelStartTime.Visible = true;
            this.panelLight.Visible = true;
            this.textDeparture.Visible = true;
            this.textStartTime.Visible = true;
        }

        private void DisplayStartCountDown()
        {
            var now = TimeProvider.Current.Now;
            if (now >= this.driveTimeStart)
            {
                this.textStartTime.Text = this.driveTimeStart.ToString("HH:mm:ss");
                this.panelLight.BackColor = Color.Green;
                this.timer.Enabled = false;
            }
            else
            {
                var wait = this.driveTimeStart - now;
                this.textStartTime.Text = string.Format(
                    "{0:HH:mm:ss}  (-{1:00}:{2:00}:{3:00})",
                    this.driveTimeStart,
                    (int)wait.TotalHours,
                    wait.Minutes,
                    wait.Seconds);
                this.panelLight.BackColor = Color.Red;
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            this.DisplayStartCountDown();
        }

        private void UpdateLanguage()
        {
            this.labelDestination.Text = "Destination Stop"; // TODO: translate
            this.labelDeparture.Text = "Departure Stop"; // TODO: translate
            this.labelStartTime.Text = "Next Departure"; // TODO: translate
        }
    }
}
