namespace NLogTester
{
    using System;
    using System.Globalization;
    using System.Windows.Forms;

    using NLog;

    public partial class Form1 : Form
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private int counter;

        public Form1()
        {
            this.InitializeComponent();

            this.comboBoxLevel.Items.Add(LogLevel.Fatal);
            this.comboBoxLevel.Items.Add(LogLevel.Error);
            this.comboBoxLevel.Items.Add(LogLevel.Warn);
            this.comboBoxLevel.Items.Add(LogLevel.Info);
            this.comboBoxLevel.Items.Add(LogLevel.Debug);
            this.comboBoxLevel.Items.Add(LogLevel.Trace);
            this.comboBoxLevel.SelectedItem = LogLevel.Info;

            this.checkedListBoxLevels.Items.Add(LogLevel.Fatal);
            this.checkedListBoxLevels.Items.Add(LogLevel.Error);
            this.checkedListBoxLevels.Items.Add(LogLevel.Warn);
            this.checkedListBoxLevels.Items.Add(LogLevel.Info, true);
            this.checkedListBoxLevels.Items.Add(LogLevel.Debug);
            this.checkedListBoxLevels.Items.Add(LogLevel.Trace);

            this.timerBindingSource.DataSource = this.timerBatchMessages;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            LogManager.Flush();
        }

        private void ButtonLogClick(object sender, EventArgs e)
        {
            Logger.Log((LogLevel)this.comboBoxLevel.SelectedItem, this.textBoxMessage.Text);
        }

        private void CheckBoxInfiniteCheckedChanged(object sender, EventArgs e)
        {
            this.numericUpDownCount.Enabled = !this.checkBoxInfinite.Checked;
        }

        private void ButtonStartStopClick(object sender, EventArgs e)
        {
            if (this.timerBatchMessages.Enabled)
            {
                this.StopTimer();
            }
            else
            {
                this.StartTimer();
            }
        }

        private void TimerBatchMessagesTick(object sender, EventArgs e)
        {
            var levels = this.checkedListBoxLevels.CheckedItems;
            var level = (LogLevel)levels[this.counter % levels.Count];

            this.counter++;
            Logger.Log(level, this.textBoxBatchMessage.Text, this.counter);
            this.textBoxCounter.Text = this.counter.ToString(CultureInfo.InvariantCulture);
            if (this.counter >= this.numericUpDownCount.Value && !this.checkBoxInfinite.Checked)
            {
                this.StopTimer();
            }
        }

        private void StartTimer()
        {
            this.counter = 0;
            this.textBoxCounter.Text = this.counter.ToString(CultureInfo.InvariantCulture);
            this.timerBatchMessages.Start();
            this.buttonStartStop.Text = "Stop";
        }

        private void StopTimer()
        {
            this.timerBatchMessages.Stop();
            this.buttonStartStop.Text = "Start";
        }

        private void CheckedListBoxLevelsItemCheck(object sender, ItemCheckEventArgs e)
        {
            var checkCount = this.checkedListBoxLevels.CheckedItems.Count;
            if (e.NewValue == CheckState.Checked)
            {
                checkCount++;
            }
            else
            {
                checkCount--;
            }

            this.buttonStartStop.Enabled = this.timerBatchMessages.Enabled || checkCount > 0;
        }
    }
}
