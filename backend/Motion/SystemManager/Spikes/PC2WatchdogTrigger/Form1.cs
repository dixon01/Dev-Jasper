using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PC2WatchdogTrigger
{
    using Kontron.Jida32;
    using Kontron.Jida32.WD;

    public partial class Form1 : Form
    {
        private readonly JidaApi jida;

        private JidaBoard board;

        private Watchdog watchdog;

        private DateTime lastTrigger;

        public Form1()
        {
            this.InitializeComponent();
            this.jida = new JidaApi();
        }

        private void ButtonOpenClick(object sender, EventArgs e)
        {
            try
            {
                if (!this.jida.Initialize())
                {
                    this.ShowError("Couldn't initialize JIDA");
                    return;
                }

                this.board = this.jida.OpenBoard(JidaApi.BoardClassCpu, 0);
                if (this.board == null)
                {
                    this.ShowError("Couldn't get CPU board");
                    return;
                }

                this.watchdog = this.board.Watchdog;
                if (this.watchdog == null)
                {
                    this.ShowError("Couldn't get watchdog");
                    return;
                }

                this.groupBox1.Enabled = true;
            }
            catch (Exception ex)
            {
                this.ShowError(ex.ToString());
            }
        }

        private void Trigger()
        {
            try
            {
                if (this.watchdog == null)
                {
                    return;
                }

                if (this.watchdog.Trigger())
                {
                    this.lastTrigger = DateTime.Now;
                    this.timerUpdateLastTrigger.Enabled = true;
                }
                else
                {
                    this.ShowError("Couldn't trigger watchdog");
                }
            }
            catch (Exception ex)
            {
                this.ShowError(ex.ToString());
            }
        }

        private void ShowError(string message)
        {
            MessageBox.Show(this, message, "Watchdog", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ButtonTriggerClick(object sender, EventArgs e)
        {
            this.Trigger();
        }

        private void CheckBoxAutoTriggerCheckedChanged(object sender, EventArgs e)
        {
            this.timerWatchdog.Stop();
            if (this.checkBoxAutoTrigger.Checked)
            {
                this.timerWatchdog.Start();
            }
        }

        private void TimerWatchdogTick(object sender, EventArgs e)
        {
            this.Trigger();
        }

        private void TimerUpdateLastTriggerTick(object sender, EventArgs e)
        {
            var delta = DateTime.Now - this.lastTrigger;
            this.textBoxLastTrigger.Text = string.Format(
                "{0:D2}:{1:D2}:{2:D2}", delta.Hours, delta.Minutes, delta.Seconds);
        }
    }
}
