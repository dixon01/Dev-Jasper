// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IhmiRightButton.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IhmiRightButton type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Gui.Control
{
    using System;
    using System.Windows.Forms;

    using Gorba.Common.Configuration.Obc.Terminal;
    using Gorba.Common.Gioom.Core.Utility;
    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Common.Medi.Core;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The control containing all additional buttons on IHMI (VM.cu).
    /// </summary>
    public partial class IhmiRightButton : UserControl, IButtonBar
    {
        private readonly PortListener gprsState;

        private readonly PortListener gpsCoverage;

        /// <summary>
        /// Initializes a new instance of the <see cref="IhmiRightButton"/> class.
        /// </summary>
        public IhmiRightButton()
        {
            this.InitializeComponent();

            this.gpsCoverage = new PortListener(MediAddress.Broadcast, "GpsCoverage");
            this.gpsCoverage.ValueChanged += this.CoveragesOnValueChanged;
            this.gpsCoverage.Start(TimeSpan.FromSeconds(5));

            this.gprsState = new PortListener(MediAddress.Broadcast, "GprsState");
            this.gprsState.ValueChanged += this.CoveragesOnValueChanged;
            this.gprsState.Start(TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// The button click event.
        /// </summary>
        public event EventHandler<CommandEventArgs> ButtonClick;

        /// <summary>
        /// The escape click event.
        /// </summary>
        public event EventHandler EscapeClick;

        /// <summary>
        /// Raises the <see cref="EscapeClick"/> event.
        /// </summary>
        protected virtual void RaiseEscapeClick()
        {
            var handler = this.EscapeClick;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the <see cref="ButtonClick"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseButtonClicked(CommandEventArgs e)
        {
            var handler = this.ButtonClick;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void CoveragesOnValueChanged(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => this.CoveragesOnValueChanged(sender, e)));
                return;
            }

            var s = FlagValues.True.Equals(this.gpsCoverage.Value) ? "GPS | " : "--- | ";

            s += FlagValues.True.Equals(this.gprsState.Value) ? "GPRS" : "----";

            this.lbLeds.Text = s;
        }

        private void BtnAlarmOnClick(object sender, EventArgs e)
        {
            this.RaiseButtonClicked(new CommandEventArgs(CommandName.ShowAlarms));
        }

        private void BtnEscapeOnClick(object sender, EventArgs e)
        {
            this.RaiseEscapeClick();
        }

        private void BtnBlocageOnClick(object sender, EventArgs e)
        {
            this.RaiseButtonClicked(new CommandEventArgs(CommandName.Razzia));
        }

        private void BtnPhonieOnClick(object sender, EventArgs e)
        {
            this.RaiseButtonClicked(new CommandEventArgs(CommandName.SpeechGsm));
        }

        private void BtnInfoOnClick(object sender, EventArgs e)
        {
            this.RaiseButtonClicked(new CommandEventArgs(CommandName.InformationMessages));
        }

        private void BtnServiceEndOnClick(object sender, EventArgs e)
        {
            this.RaiseButtonClicked(new CommandEventArgs(CommandName.LogOffAll));
        }

        private void BtnDriverChangeOnClick(object sender, EventArgs e)
        {
            // TODO a faire uniquement lorsqu'on est en service agent
            this.RaiseButtonClicked(new CommandEventArgs(CommandName.LogOffDrive));
        }

        private void CmDeviationOnClick(object sender, EventArgs e)
        {
            this.RaiseButtonClicked(new CommandEventArgs(CommandName.Detour));
        }
    }
}