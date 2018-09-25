// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VerifyTimers.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The verify timers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TimersTest
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using Gorba.Common.Utility.Core;

    using Microsoft.Win32;

    /// <summary>
    /// The verify timers.
    /// </summary>
    public partial class VerifyTimers : Form
    {
        private readonly ITimer triggerTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="VerifyTimers"/> class.
        /// </summary>
        public VerifyTimers()
        {
            this.InitializeComponent();
            this.triggerTimer = TimerFactory.Current.CreateTimer("TriggerTimer");
            this.triggerTimer.AutoReset = false;
            this.triggerTimer.Elapsed += this.TriggerTimerOnElapsed;
            SystemEvents.TimeChanged += this.OnTimeChanged;
        }

        private void Button1Click(object sender, EventArgs e)
        {
            this.triggerTimer.Interval = DateTime.Parse(this.textBox1.Text) - TimeProvider.Current.Now;
            this.triggerTimer.Enabled = true;
        }

        private void Button2Click(object sender, EventArgs e)
        {
            var time = DateTime.Parse(this.textBox2.Text);
            this.SetSystemTime(time);
        }

        private void TriggerTimerOnElapsed(object sender, EventArgs eventArgs)
        {
            this.BeginInvoke(
                    new MethodInvoker(() => this.richTextBox1.AppendText("Trigger Timer elapsed")));
        }

        private void SetSystemTime(DateTime dateTime)
        {
            var time = new NativeMethods.SystemTime
            {
                Year = (ushort)dateTime.Year,
                Month = (ushort)dateTime.Month,
                Day = (ushort)dateTime.Day,
                Hour = (ushort)dateTime.Hour,
                Minute = (ushort)dateTime.Minute,
                Second = (ushort)dateTime.Second
            };
            if (!NativeMethods.SetSystemTime(ref time))
            {
                throw new InvalidOperationException(
                    "Couldn't update system time (do you have the necessary access rights?)");
            }
        }

        private void OnTimeChanged(object sender, EventArgs e)
        {
            this.richTextBox1.AppendText("Time changed");
        }

        private static class NativeMethods
        {
            [DllImport("kernel32.dll", EntryPoint = "SetSystemTime", SetLastError = true)]
            public static extern bool SetSystemTime(ref SystemTime st);

            public struct SystemTime
            {
                // ReSharper disable NotAccessedField.Local
                // ReSharper disable UnusedMember.Local
                public ushort Year;
                public ushort Month;
                public ushort DayOfWeek;
                public ushort Day;
                public ushort Hour;
                public ushort Minute;
                public ushort Second;
                public ushort Millisecond;

                // ReSharper restore UnusedMember.Local
                // ReSharper restore NotAccessedField.Local
            }
        }
    }
}
