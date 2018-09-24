// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DriveWaitScreen.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DriveWaitScreen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Gui.MainFields
{
    using System;
    using System.Drawing;

    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Obc.Terminal.Core;
    using Gorba.Motion.Obc.Terminal.Gui.Utility;

    /// <summary>
    /// The drive wait screen main field.
    /// </summary>
    public partial class DriveWaitScreen : MainField, IBlockDriveWait, ISpecialDestinationDrive
    {
        private string destinationText = "Honolulu"; // MLHIDE

        private DateTime driveTimeStart = DateTime.MinValue;

        private string startText;

        /// <summary>
        /// Initializes a new instance of the <see cref="DriveWaitScreen"/> class.
        /// </summary>
        public DriveWaitScreen()
        {
            this.InitializeComponent();
            this.HideWaitScreenElements();

            ScreenUtil.Adapt4Ihmi(this, false, false);

            this.UpdateLanguage();
            LanguageManager.Instance.CurrentLanguageChanged += this.CurrentLanguageChanged;
        }

        /// <summary>
        /// Initializes the field by setting its data.
        /// </summary>
        /// <param name="destination">
        /// The destination text.
        /// </param>
        /// <param name="start">
        /// The start text.
        /// </param>
        /// <param name="startTime">
        /// The drive time start.
        /// </param>
        public void Init(string destination, string start, DateTime startTime)
        {
            this.destinationText = destination;
            this.startText = start;
            this.driveTimeStart = startTime;
            this.Init();
        }

        /// <summary>
        /// Make sure that calling code is running in the GUI thread!!!
        ///   Check with GetControl().InvokeRequired
        /// </summary>
        /// <param name="visible">
        /// A flag indicating if this field should be made visible.
        /// </param>
        public override void MakeVisible(bool visible)
        {
            if (!visible)
            {
                this.timer1.Enabled = false;
            }
            else if (this.startText != null)
            {
                this.timer1.Enabled = true;
            }

            base.MakeVisible(visible);
        }

        /// <summary>
        /// Initializes this field.
        /// </summary>
        /// <param name="destText">
        /// The destination text.
        /// </param>
        public void Init(string destText)
        {
            this.destinationText = destText;
            this.startText = null;
            this.Init();
        }

        private void HideWaitScreenElements()
        {
            this.lblDestinationStart.Visible = false;
            this.lblStartTime.Visible = false;
            this.pnlLight.Visible = false;
            this.txtStartStop.Visible = false;
            this.txtStartTime.Visible = false;
        }

        private void ShowAllElements()
        {
            this.lblDestinationStart.Visible = true;
            this.lblStartTime.Visible = true;
            this.pnlLight.Visible = true;
            this.txtStartStop.Visible = true;
            this.txtStartTime.Visible = true;
        }

        private void Init()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(this.Init));
            }
            else
            {
                this.txtDestinationStop.Text = this.destinationText;
                /*  txtDestinationStop.Invalidate();
        this.Invalidate();
        this.Refresh();*/
                if (this.startText == null)
                {
                    // Special destination drive...
                    this.timer1.Enabled = false;
                    this.HideWaitScreenElements();
                }
                else if (this.startText == string.Empty && this.destinationText == string.Empty)
                {
                    // Block drive. But evTripLoaded event not yet received
                    this.txtStartStop.Text = string.Empty;
                    this.txtStartTime.Text = string.Empty;
                    this.txtDestinationStop.Text = string.Empty;
                    this.timer1.Enabled = false;
                    this.ShowAllElements();
                }
                else
                {
                    this.txtStartStop.Text = this.startText;
                    this.DisplayStartCountDown();
                    this.timer1.Enabled = true;
                    this.ShowAllElements();
                }
            }
        }

        private void DisplayStartCountDown()
        {
            if (TimeProvider.Current.Now >= this.driveTimeStart)
            {
                this.txtStartTime.Text = this.driveTimeStart.ToString("HH:mm:ss");
            }
            else
            {
                TimeSpan wait = this.driveTimeStart - TimeProvider.Current.Now;
                this.txtStartTime.Text = string.Format(
                    "{0:HH:mm:ss}  (-{1:00}:{2:00}:{3:00})",
                    this.driveTimeStart,
                    (int)wait.TotalHours,
                    wait.Minutes,
                    wait.Seconds);
            }
        }

        private void Timer1Tick(object sender, EventArgs e)
        {
            if (TimeProvider.Current.Now >= this.driveTimeStart)
            {
                this.pnlLight.BackColor = Color.Green;
                this.timer1.Enabled = false;
            }
            else
            {
                this.pnlLight.BackColor = Color.Red;
            }

            this.DisplayStartCountDown();
        }

        private void CurrentLanguageChanged(object sender, EventArgs e)
        {
            this.UpdateLanguage();
        }

        private void UpdateLanguage()
        {
            this.lblDestinationStop.Text = ml.ml_string(14, "Destination Stop");
            this.lblDestinationStart.Text = ml.ml_string(15, "Departure Stop");
            this.lblStartTime.Text = ml.ml_string(16, "Next Departure");
        }
    }
}