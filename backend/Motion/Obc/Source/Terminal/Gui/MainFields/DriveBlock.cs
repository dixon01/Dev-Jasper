// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DriveBlock.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DriveBlock type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Gui.MainFields
{
    using System;
    using System.Drawing;

    using Gorba.Motion.Obc.Terminal.Core;
    using Gorba.Motion.Obc.Terminal.Gui.Control;
    using Gorba.Motion.Obc.Terminal.Gui.Utility;

    /// <summary>
    /// The drive block main field.
    /// </summary>
    public partial class DriveBlock : MainField, IBlockDrive, IUpdateable
    {
        private const int PicAdvanced2 = 0;

        private const int PicAdvanced1 = 1;

        private const int PicOnTime = 2;

        private const int PicDelay1 = 3;

        private const int PicDelay2 = 4;

        private int delaySec;

        private bool isAtBusStop = true;

        private string delayDriving = string.Empty;

        private string station1 = string.Empty;

        private string station2 = string.Empty;

        private string station3 = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="DriveBlock"/> class.
        /// </summary>
        public DriveBlock()
        {
            this.InitializeComponent();
            VMxDigitalClock.AddUpdateable(this);

            ScreenUtil.Adapt4Ihmi(this, false, false);
        }

        /// <summary>
        ///   Sets the next three stations
        /// </summary>
        /// <param name = "stat1">next station</param>
        /// <param name = "stat2">next after next station</param>
        /// <param name = "stat3">next after next next station</param>
        public void SetStations(string stat1, string stat2, string stat3)
        {
            this.station1 = stat1;
            this.station2 = stat2;
            this.station3 = stat3;
            this.RealSetStations();
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
            ////this.delayMin = (int)Math.Round((delaySec / 60.0));
        }

        /// <summary>
        ///   Will be called from the Updater when the interval time is expired
        ///   The updater will run ever in the same thread.
        ///   But if the Update method can be called from an other part you may think about locking your implementation.
        ///   Make sure that this object will handle fast the Update() method. Otherwise it may block the hole system!
        /// </summary>
        public void SecondUpdate()
        {
            if (!this.IsActive)
            {
                return;
            }

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

        private static string SecToString(int seconds)
        {
            TimeSpan ts = TimeSpan.FromSeconds(seconds);
            return Math.Abs(ts.Minutes).ToString("00") + ":" + Math.Abs(ts.Seconds).ToString("00");
        }

        private void DisplayInfoAtStop()
        {
            this.lblLight.BackColor = (this.delaySec >= 0) ? Color.Green : Color.Red;
            this.txtDelayTime.Text = (this.delaySec >= 0 ? "+ " : "- ") + SecToString(this.delaySec);
            this.SetDrivingPicture();
            this.lblLight.Visible = true;
        }

        private void DisplayInfoDriving()
        {
            this.lblLight.Visible = false;

            this.SetDrivingPicture();
            this.txtDelayTime.Text = this.delayDriving;
        }

        private void RealSetStations()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(this.RealSetStations));
            }
            else
            {
                this.txtStation1.Text = this.station1;
                this.txtStation2.Text = this.station2;
                this.txtStation3.Text = this.station3;
            }
        }

        private void SetDrivingPicture()
        {
            // Specs MG 03/03/2015
            // Pour corriger cela on devrait modifier l’affichage entre les arrêts comme suit (en rouge):
            // - Une avance > 30s est marquée „rouge“ et on affiche -1Min
            // - Une avance < 30s est marquée „rouge“ et on affiche - 1Min ou -30sec
            // - Un retard > 150s est marqué „jaune“ et on affiche +3Min
            int delayMin = (int)Math.Round(this.delaySec / 60.0);

            if (this.delaySec >= 0)
            {
                // in time or delayed
                this.delayDriving = string.Format("+ {0} min", delayMin);

                if (this.delaySec < 150)
                {
                    // show ontime picture
                    this.pictureBox1.Image = this.imageList1.Images[PicOnTime];
                }
                else if (this.delaySec < 240)
                {
                    this.pictureBox1.Image = this.imageList1.Images[PicDelay1];
                }
                else
                {
                    this.pictureBox1.Image = this.imageList1.Images[PicDelay2];
                }
            }
            else
            {
                // Advance
                if (this.delaySec > -30)
                {
                    this.delayDriving = "- 30 sec";
                    this.pictureBox1.Image = this.imageList1.Images[PicAdvanced1];
                }
                else
                {
                    this.delayDriving = string.Format("- {0} min", Math.Abs(delayMin));
                    this.pictureBox1.Image = this.imageList1.Images[PicAdvanced2];
                }
            }
        }

        /* code avant modif MG ci-dessus
   int delayMin = (int)Math.Round((this.delaySec / 60.0));

  if (this.delaySec >= 0)
  {
    // in time or delayed
    this.lblLight.BackColor = Color.Green;

    this.delayBusStop = "+ " + SecToString(this.delaySec);
    if (delayMin == 0)
      this.delayDriving = string.Format("+/- {0} min", delayMin);
    else
      this.delayDriving = string.Format("+ {0} min", delayMin);

    if (this.delaySec < 120)
      // show ontime picture
      this.pictureBox1.Image = this.imageList1.Images[PicOnTime];

    else if (this.delaySec < 240)
      this.pictureBox1.Image = this.imageList1.Images[PicDelay1];

    else
      this.pictureBox1.Image = this.imageList1.Images[PicDelay2];
  }
  else
  {
    // Advanced
    this.lblLight.BackColor = Color.Red;
    this.delayBusStop = "- " + SecToString(this.delaySec);
    if (delayMin == 0)
    {
      this.delayDriving = string.Format("+/- {0} min", delayMin);

      // Bugzilla #105
      // a l'arret, on synchronise à la seconde pres
      // en roulage, à la minute pres (pour l'arrondi 30s)
      if (isAtBusStop)
        this.pictureBox1.Image = this.imageList1.Images[PicAdvanced1];
      else
        this.pictureBox1.Image = this.imageList1.Images[PicOnTime];
    }
    else
    {
      this.delayDriving = string.Format("- {0} min", Math.Abs(delayMin));

      if (this.delaySec > -240)
        this.pictureBox1.Image = this.imageList1.Images[PicAdvanced1];
      else
        this.pictureBox1.Image = this.imageList1.Images[PicAdvanced2];
    }
  }*/
    }
}