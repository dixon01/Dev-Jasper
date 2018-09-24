// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchedulingForm.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SchedulingForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.TestGui
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Timers;
    using System.Windows.Forms;

    using Gorba.Common.Medi.Core;

    using Timer = System.Timers.Timer;

    /// <summary>
    /// Form that allows to schedule sending a message to a given destination.
    /// </summary>
    public partial class SchedulingForm : Form
    {
        private readonly Timer timer = new Timer(1000);

        private object message;

        private MediAddress destination;

        private int messageCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulingForm"/> class.
        /// </summary>
        public SchedulingForm()
        {
            this.InitializeComponent();
            this.Destination = MediAddress.Broadcast;
            this.timer.Elapsed += this.TimerElapsed;
        }

        /// <summary>
        /// Gets or sets the message to be sent.
        /// </summary>
        [DefaultValue(null)]
        public object Message
        {
            get
            {
                return this.message;
            }

            set
            {
                this.message = value;
                this.textBox1.Text = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the destination address.
        /// </summary>
        public MediAddress Destination
        {
            get
            {
                return this.destination;
            }

            set
            {
                this.destination = value;
                this.textBox3.Text = value.ToString();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Closing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the event data. </param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            this.timer.Stop();
        }

        private void Button1Click(object sender, EventArgs e)
        {
            if (this.button1.Text == "Start")
            {
                this.timer.Start();
                this.button1.Text = "Stop";
            }
            else
            {
                this.timer.Stop();
                this.button1.Text = "Start";
            }
        }

        private void NumericUpDown1ValueChanged(object sender, EventArgs e)
        {
            this.timer.Interval = (double)this.numericUpDown1.Value;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            MessageDispatcher.Instance.Send(this.Destination, this.Message);
            this.BeginInvoke(
                new MethodInvoker(
                    () => this.textBox2.Text = (++this.messageCounter).ToString(CultureInfo.InvariantCulture)));
        }
    }
}
