// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IqubeRadio.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IqubeRadio type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Gui.MainFields
{
    using System;
    using System.Collections.Generic;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The iqube radio main field.
    /// </summary>
    public partial class IqubeRadio : MainField, IIqubeRadio
    {
        private string mainCaption = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="IqubeRadio"/> class.
        /// </summary>
        public IqubeRadio()
        {
            this.InitializeComponent();
        }

        /// <summary>
        ///   Confirmed Event
        /// </summary>
        public event EventHandler<IqubeRadioEventArgs> InputDone;

        /// <summary>
        ///   Initialization from the iqube.radio screen
        /// </summary>
        /// <param name = "caption">
        /// The caption.
        /// </param>
        public void Init(string caption)
        {
            this.mainCaption = caption;
            this.RealInit();
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
                this.Invalidate();
            }

            base.MakeVisible(visible);
        }

        /// <summary>
        /// Raises the <see cref="MainField.ReturnPressed"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void OnReturnPressed(EventArgs e)
        {
            this.CallReturnEvent();
        }

        private void RealInit()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(this.RealInit));
            }
            else
            {
                this.lblMainCaption.Text = this.mainCaption;
            }
        }

        private void CallReturnEvent()
        {
            var receivers = new List<string>();
            if (this.btnSpeechLS.IsPushed)
            {
                receivers.Add("LST");                                 // MLHIDE
            }

            if (this.btnSpeechWS.IsPushed)
            {
                receivers.Add("WRK");                                 // MLHIDE
            }

            if (this.btnSpeechL1.IsPushed)
            {
                receivers.Add("L01");                                 // MLHIDE
            }

            if (this.btnSpeechL2.IsPushed)
            {
                receivers.Add("L02");                                 // MLHIDE
            }

            if (this.btnSpeechL3.IsPushed)
            {
                receivers.Add("L03");                                 // MLHIDE
            }

            if (this.btnSpeechL4.IsPushed)
            {
                receivers.Add("L04");                                 // MLHIDE
            }

            if (this.btnSpeechL5.IsPushed)
            {
                receivers.Add("L05");                                 // MLHIDE
            }

            if (this.btnSpeechL6.IsPushed)
            {
                receivers.Add("L06");                                 // MLHIDE
            }

            if (this.btnSpeechL7.IsPushed)
            {
                receivers.Add("L07");                                 // MLHIDE
            }

            if (this.btnSpeechL8.IsPushed)
            {
                receivers.Add("L08");                                 // MLHIDE
            }

            if (this.btnSpeechL74.IsPushed)
            {
                receivers.Add("L74");                                 // MLHIDE
            }

            if (this.btnSpeechL75.IsPushed)
            {
                receivers.Add("L75");                                 // MLHIDE
            }

            if (this.btnSpeechL99.IsPushed)
            {
                receivers.Add("L99");                                 // MLHIDE
            }

            if (this.Visible)
            {
                if (this.InputDone != null)
                {
                    this.InputDone(this, new IqubeRadioEventArgs(receivers.ToArray()));
                }
            }
        }

        private void BtnEnterClick(object sender, EventArgs e)
        {
            this.CallReturnEvent();
        }
    }
}