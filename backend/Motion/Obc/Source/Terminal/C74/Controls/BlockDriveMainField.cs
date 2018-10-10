// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlockDriveMainField.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BlockDriveMainField type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.C74.Controls
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Gorba.Motion.Obc.Terminal.C74.Properties;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The block drive main field.
    /// </summary>
    public partial class BlockDriveMainField : MainField, IBlockDrive
    {
        private bool isAtBusStop;

        private int delaySec;

        private string delayDriving;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockDriveMainField"/> class.
        /// </summary>
        public BlockDriveMainField()
        {
            this.InitializeComponent();

            this.buttonMenu.IsSelected = true;
        }

        /// <summary>
        ///   Sets the next three stations
        /// </summary>
        /// <param name = "stat1">next station</param>
        /// <param name = "stat2">next after next station</param>
        /// <param name = "stat3">next after next next station</param>
        public void SetStations(string stat1, string stat2, string stat3)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => this.SetStations(stat1, stat2, stat3)));
                return;
            }

            this.labelStop1.Text = stat1;
            this.labelStop2.Text = stat2;
            this.labelStop3.Text = stat3;
        }

        /// <summary>
        ///   The delay which the bus have.
        ///   0: in time
        ///   positive: bus has a delay -> drive faster
        ///   negative: bus is too fast -> drive slower
        /// </summary>
        /// <param name = "delay">delay in seconds</param>
        /// <param name = "atBusStop">
        ///   Describes if the bus is at a bus stop/station.
        ///   Default value is true
        ///   true: Bus is currently at a bus stop. Countdown window will be shown
        ///   false: Bus is driving
        /// </param>
        public void SetDelaySec(int delay, bool atBusStop)
        {
            this.isAtBusStop = atBusStop;
            this.delaySec = delay;
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
            base.MakeVisible(visible);
            this.timer.Enabled = visible;
        }

        private static string SecToString(int seconds)
        {
            var ts = TimeSpan.FromSeconds(seconds);
            return Math.Abs(ts.Minutes).ToString("00") + ":" + Math.Abs(ts.Seconds).ToString("00");
        }

        private void DisplayInfoAtStop()
        {
            this.panelLight.BackColor = (this.delaySec >= 0) ? Color.Green : Color.Red;
            this.textDelay.Text = (this.delaySec >= 0 ? "+ " : "- ") + SecToString(this.delaySec);
            this.SetDrivingPicture();
            this.panelLight.Visible = true;
        }

        private void DisplayInfoDriving()
        {
            this.panelLight.Visible = false;

            this.SetDrivingPicture();
            this.textDelay.Text = this.delayDriving;
        }

        private void SetDrivingPicture()
        {
            // Specs MG 03/03/2015
            // Pour corriger cela on devrait modifier l’affichage entre les arrêts comme suit (en rouge):
            // - Une avance > 30s est marquée „rouge“ et on affiche -1Min
            // - Une avance < 30s est marquée „rouge“ et on affiche - 1Min ou -30sec
            // - Un retard > 150s est marqué „jaune“ et on affiche +3Min
            var delayMin = (int)Math.Round(this.delaySec / 60.0);

            if (this.delaySec >= 0)
            {
                // in time or delayed
                this.delayDriving = string.Format("+ {0} min", delayMin);

                if (this.delaySec < 150)
                {
                    // show ontime picture
                    this.pictureBox.Image = Resources.BlockDriveOnTime;
                }
                else if (this.delaySec < 240)
                {
                    this.pictureBox.Image = Resources.BlockDriveDelay1;
                }
                else
                {
                    this.pictureBox.Image = Resources.BlockDriveDelay2;
                }
            }
            else
            {
                // Advance
                if (this.delaySec > -30)
                {
                    this.delayDriving = "- 30 sec";
                    this.pictureBox.Image = Resources.BlockDriveAdvanced1;
                }
                else
                {
                    this.delayDriving = string.Format("- {0} min", Math.Abs(delayMin));
                    this.pictureBox.Image = Resources.BlockDriveAdvanced2;
                }
            }
        }

        private void TimerOnTick(object sender, EventArgs e)
        {
            this.delaySec--;

            if (this.isAtBusStop)
            {
                this.DisplayInfoAtStop();
            }
            else
            {
                this.DisplayInfoDriving();
            }
        }
    }
}
