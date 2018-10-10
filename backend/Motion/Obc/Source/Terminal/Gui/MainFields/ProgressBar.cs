// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgressBar.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProgressBar type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Gui.MainFields
{
    using System;
    using System.Windows.Forms;

    using Gorba.Motion.Obc.Terminal.Core;
    using Gorba.Motion.Obc.Terminal.Gui.Hardware;
    using Gorba.Motion.Obc.Terminal.Gui.Utility;

    /// <summary>
    /// The progress bar.
    /// </summary>
    internal partial class ProgressBar : UserControl
    {
        private const int SpeedFactor = 10;

        private readonly KeyBoard keyboard;

        private readonly IProgressBarInfo progressInfo;

        private bool isInitialized;

        private int ticksCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressBar"/> class.
        /// </summary>
        /// <param name="progressInfo">
        /// The progress info.
        /// </param>
        public ProgressBar(IProgressBarInfo progressInfo)
        {
            this.InitializeComponent();

            ScreenUtil.Adapt4Ihmi(this, true, true);

            this.progressInfo = progressInfo;
            this.lblCaption.Text = progressInfo.Caption;
            this.progressBar1.Maximum = progressInfo.MaxTime * SpeedFactor;
            this.keyboard = KeyBoard.Instance;
            this.keyboard.KeyboardEnabled = false;
            this.timer1.Interval = 1000 / SpeedFactor;
            this.timer1.Enabled = true;

            ////ScreenUtil.Adapt4Ihmi(this, false, true);
        }

        /// <summary>
        /// The closed event.
        /// </summary>
        public event EventHandler Closed;

        private void ProgressBarPaint(object sender, PaintEventArgs e)
        {
            if (this.isInitialized == false)
            {
                this.isInitialized = true;
                this.BringToFront();
                this.Focus();
            }
        }

        private void ProgressBarEnabledChanged(object sender, EventArgs e)
        {
            if (this.Enabled == false)
            {
                this.Enabled = true;
            }
        }

        private void Timer1Tick(object sender, EventArgs e)
        {
            this.ticksCounter++;
            if (this.ticksCounter >= this.progressBar1.Maximum)
            {
                this.timer1.Enabled = false;
                this.keyboard.KeyboardEnabled = true;
                this.Visible = false;
                this.OnClosed(EventArgs.Empty);
                this.progressInfo.ProgressElapsed();
            }
            else
            {
                this.progressBar1.Value = this.ticksCounter;
            }
        }

        private void OnClosed(EventArgs e)
        {
            if (this.Closed != null)
            {
                this.Closed(this, e);
            }
        }
    }
}