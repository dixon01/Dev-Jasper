// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IconBar.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IconBar type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Gui.MainFields
{
    using System;
    using System.Windows.Forms;

    using Gorba.Motion.Obc.Terminal.Core;
    using Gorba.Motion.Obc.Terminal.Gui.Utility;

    /// <summary>
    /// The icon bar.
    /// </summary>
    public partial class IconBar : UserControl, IIconBar
    {
        private readonly FieldIcon1 fieldIcon1;

        private readonly FieldIcon2 fieldIcon2;

        private readonly FieldIcon4 fieldIcon4;

        private readonly FieldIcon5 fieldIcon5;

        /// <summary>
        /// Initializes a new instance of the <see cref="IconBar"/> class.
        /// </summary>
        public IconBar()
        {
            this.InitializeComponent();
            this.fieldIcon1 = new FieldIcon1(this.imageListField1);
            this.fieldIcon2 = new FieldIcon2(this.imageListField2);
            this.fieldIcon4 = new FieldIcon4(this.imageListField4);
            this.fieldIcon5 = new FieldIcon5(this.imageListField5);

            this.SetStopRequestedIcon(false);

            ScreenUtil.Adapt4Ihmi(this, true, false);
        }

        /// <summary>
        ///   The event when the status field is touched from the user. Is used to sign to menu screen!
        /// </summary>
        public event EventHandler ContextIconClick;

        /// <summary>
        /// Set the traffic light icon
        /// </summary>
        /// <param name="state">state of the icon; which icon is displayed</param>
        public void SetTrafficLightIcon(TrafficLightIconState state)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => this.SetTrafficLightIcon(state)));
            }
            else
            {
                this.fieldIcon4.TrafficLight = state;
                this.picField4.Image = this.fieldIcon4.GetImage();
            }
        }

        /// <summary>
        /// Set the detour icon
        /// </summary>
        /// <param name="visible">if true, the icon will be visible</param>
        public void SetDetourIcon(bool visible)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => this.SetDetourIcon(visible)));
            }
            else
            {
                this.fieldIcon4.Detour = visible;
                this.picField4.Image = this.fieldIcon4.GetImage();
            }
        }

        /// <summary>
        /// Set the voice icon
        /// </summary>
        /// <param name="state">state of the icon; which icon is displayed</param>
        public void SetVoiceIcon(VoiceIconState state)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => this.SetVoiceIcon(state)));
            }
            else
            {
                this.fieldIcon5.Voice = state;
                this.picField5.Image = this.fieldIcon5.GetImage();
            }
        }

        /// <summary>
        /// Set the driver alarm icon
        /// </summary>
        /// <param name="state">state of the icon; which icon is displayed</param>
        public void SetDriverAlarmIcon(DriverAlarmIconState state)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => this.SetDriverAlarmIcon(state)));
            }
            else
            {
                this.fieldIcon1.DriverAlarm = state;
                this.picField1.Image = this.fieldIcon1.GetImage();
            }
        }

        /// <summary>
        /// Set the information message icon
        /// </summary>
        /// <param name="visible">if true, the icon will be visible</param>
        public void SetInformationMessageIcon(bool visible)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => this.SetInformationMessageIcon(visible)));
            }
            else
            {
                this.fieldIcon2.InfoMessage = visible;
                this.picField2.Image = this.fieldIcon2.GetImage();
            }
        }

        /// <summary>
        /// Sets the stop requested icon.
        /// </summary>
        /// <param name="visible">
        /// if true, the icon will be visible
        /// </param>
        public void SetStopRequestedIcon(bool visible)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => this.SetStopRequestedIcon(visible)));
            }
            else
            {
                this.pbStopRequested.Visible = visible;
            }
        }

        /// <summary>
        /// Set the alarm message icon
        /// </summary>
        /// <param name="visible">if true, the icon will be visible</param>
        public void SetAlarmMessageIcon(bool visible)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => this.SetAlarmMessageIcon(visible)));
            }
            else
            {
                this.fieldIcon2.AlarmMessage = visible;
                this.picField2.Image = this.fieldIcon2.GetImage();
            }
        }

        /// <summary>
        /// Set the razzia icon
        /// </summary>
        /// <param name="visible">if true, the icon will be visible</param>
        public void SetRazziaIcon(bool visible)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => this.SetRazziaIcon(visible)));
            }
            else
            {
                this.picField3Razzia.Visible = visible;

                // [wes] why don't we update the pic field here?
            }
        }

        /// <summary>
        /// Set the driving school icon
        /// </summary>
        /// <param name="visible">if true, the icon will be visible</param>
        public void SetDrivingSchoolIcon(bool visible)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => this.SetDrivingSchoolIcon(visible)));
            }
            else
            {
                this.picfield7School.Visible = visible;
            }
        }

        /// <summary>
        /// Set the additional trip icon
        /// </summary>
        /// <param name="visible">if true, the icon will be visible</param>
        public void SetAdditionalTripIcon(bool visible)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => this.SetAdditionalTripIcon(visible)));
            }
            else
            {
                this.picfield8Additional.Visible = visible;
            }
        }

        /// <summary>
        /// Raises the <see cref="ContextIconClick"/> event.
        /// </summary>
        protected virtual void RaiseContextIconClick()
        {
            var handler = this.ContextIconClick;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void PictureBox1Click(object sender, EventArgs e)
        {
            this.RaiseContextIconClick();
        }

        private void PictureBox1MouseDown(object sender, MouseEventArgs e)
        {
            this.pictureBox1.Image = this.imageListBtnMenu.Images[1];
        }

        private void PictureBox1MouseUp(object sender, MouseEventArgs e)
        {
            this.pictureBox1.Image = this.imageListBtnMenu.Images[0];
        }
    }
}