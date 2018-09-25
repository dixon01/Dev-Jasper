// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramControl.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TelegramControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.Ibis
{
    using System;
    using System.Windows.Forms;

    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Motion.Protran.Controls;
    using Gorba.Motion.Protran.Ibis.Simulation;
    using Gorba.Motion.Protran.Visualizer.Data.Ibis;
    using Gorba.Motion.Protran.Visualizer.Properties;

    /// <summary>
    /// Control that allows telegram to be sent to the controller.
    /// </summary>
    public partial class TelegramControl : UserControl, IIbisVisualizationControl
    {
        private readonly Timer sendTimer = new Timer();

        private readonly PictureBox playPause = new PictureBox();

        private readonly Button reset = new Button();

        private readonly Button step = new Button();

        private IIbisVisualizationService controller;

        private IbisFileReader fileReader;

        private SideTab sideTab;

        private bool stepControlledtelegram;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramControl"/> class.
        /// </summary>
        public TelegramControl()
        {
            this.InitializeComponent();
            this.reset.Text = @"Reset";
            this.step.Text = @"Step";

            this.sendTimer.Tick += (sender, args) => this.ReadNextTelegram();
            this.playPause.Click += this.PlayPauseOnClick;
            this.reset.Click += this.ButtonResetClick;
            this.step.Click += this.ButtonStepClick;
        }

        /// <summary>
        /// Delegate for Display telegram
        /// </summary>
        /// <param name="telegram">
        /// The telegram.
        /// </param>
        public delegate void DisplayTelegramCallBack(byte[] telegram);

        /// <summary>
        /// Delegate for send telegram
        /// </summary>
        /// <param name="telegram">
        /// The telegram.
        /// </param>
        /// <param name="ignoreUnknown">
        /// The ignore unknown.
        /// </param>
        public delegate void SendTelegramCallBack(byte[] telegram, bool ignoreUnknown);

        /// <summary>
        /// Configure the control with the given controller.
        /// </summary>
        /// <param name="ctrl">
        ///   The controller to which you can for example attach event handlers
        /// </param>
        public void Configure(IIbisVisualizationService ctrl)
        {
            this.controller = ctrl;

            switch (ctrl.Config.Behaviour.ByteType)
            {
                case ByteType.Ascii7:
                    this.telegramCreationControl1.BitsPerCharacter = 7;
                    break;
                case ByteType.Hengartner8:
                    this.telegramCreationControl1.BitsPerCharacter = 8;
                    break;
                case ByteType.UnicodeBigEndian:
                    this.telegramCreationControl1.BitsPerCharacter = 16;
                    break;
            }

            this.controller.SetControl(this);
            this.listBoxIbisBusTelegrams.Items.Clear();

            if (ctrl.Config.Sources.SerialPort == null)
            {
                this.tabControl1.Controls.Remove(this.tabPage3);
            }
        }

        /// <summary>
        /// Assigns a <see cref="SideTab"/> to this control.
        /// This can be used to keep a reference to the tab
        /// and update it when events arrive.
        /// </summary>
        /// <param name="tab">
        /// The side tab.
        /// </param>
        public void SetSideTab(SideTab tab)
        {
            this.sideTab = tab;

            var img = Resources.Play;
            this.playPause.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            this.playPause.Left = (tab.Width - img.Width) / 2;
            this.playPause.Top = (tab.Height - img.Height) / 2;
            this.playPause.Size = img.Size;
            this.playPause.Visible = false;

            tab.Controls.Add(this.playPause);
            tab.Controls.SetChildIndex(this.playPause, 0);

            this.reset.Visible = false;
            tab.Controls.Add(this.reset);
            tab.Controls.SetChildIndex(this.reset, 0);

            this.step.Visible = false;
            this.step.Left = 80;
            tab.Controls.Add(this.step);
            tab.Controls.SetChildIndex(this.step, 0);
        }

        /// <summary>
        /// Display the telegram in list box
        /// </summary>
        /// <param name="telegram">
        /// The telegram.
        /// </param>
        internal void DisplayTelegram(byte[] telegram)
        {
            if (this.InvokeRequired)
            {
                var d = new DisplayTelegramCallBack(this.DisplayTelegram);
                this.Invoke(d, new object[] { telegram });
                return;
            }

            this.listBoxIbisBusTelegrams.Items.Add(TelegramFormatter.ToHexString(telegram));
            this.listBoxIbisBusTelegrams.SelectedIndex = this.listBoxIbisBusTelegrams.Items.Count - 1;
        }

        /// <summary>
        /// Send telegram
        /// </summary>
        /// <param name="telegram">
        /// The telegram.
        /// </param>
        /// <param name="ignoreUnknown">
        /// The ignore unknown.
        /// </param>
        protected internal void SendTelegram(byte[] telegram, bool ignoreUnknown)
        {
            if (this.InvokeRequired)
            {
                var d = new SendTelegramCallBack(this.SendTelegram);
                this.Invoke(d, new object[] { telegram, ignoreUnknown });
                return;
            }

            if (this.controller == null)
            {
                return;
            }

            if (!this.controller.EnqueueTelegram(telegram, ignoreUnknown))
            {
                return;
            }

            this.textBoxTelegram.Text = TelegramFormatter.ToHexString(telegram);
            if (this.sideTab != null)
            {
                this.sideTab.Description =
                    TelegramFormatter.ToTelegramString(telegram, this.controller.Config.Behaviour.ByteType);
            }
        }

        private void OnButtonResendClick(object sender, EventArgs e)
        {
            this.SendTelegram(TelegramFormatter.FromHexString(this.textBoxTelegram.Text), false);
        }

        private void OnIbisTelegramCreated(object sender, DataEventArgs e)
        {
            this.SendTelegram(e.Data, false);
        }

        private void OpenFile()
        {
            this.sendTimer.Stop();
            this.fileReader.Reset();

            this.buttonPlay.Enabled = true;
            this.buttonPause.Enabled = false;
            this.buttonReset.Enabled = true;
            this.buttonStep.Enabled = true;

            this.playPause.Visible = true;
            this.playPause.Image = Resources.Play;

            this.reset.Visible = true;
            this.step.Visible = true;
            this.listBoxFileTelegrams.Items.Clear();
        }

        private void ButtonOpenFileClick(object sender, EventArgs e)
        {
            if (this.openFileDialogRecordings.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            this.textBoxFileName.Text = this.openFileDialogRecordings.FileName;
            if (this.fileReader != null)
            {
                this.sendTimer.Stop();
                this.fileReader.Close();
                this.fileReader = null;
            }

            var config = new SimulationConfig();
            config.IntervalBetweenTelegrams = null;
            config.SimulationFile = this.textBoxFileName.Text;
            config.TimesToRepeat = 1;
            this.fileReader = IbisFileReader.Create(config);

            this.OpenFile();
        }

        private void ButtonResetClick(object sender, EventArgs e)
        {
            this.buttonPlay.Enabled = true;
            this.buttonPause.Enabled = false;
            this.playPause.Image = Resources.Play;
            this.checkBoxStepTime.Enabled = true;
            this.buttonStep.Enabled = true;

            this.stepControlledtelegram = false;
            this.OpenFile();
        }

        private void ButtonPlayClick(object sender, EventArgs e)
        {
            this.buttonPlay.Enabled = false;
            this.buttonPause.Enabled = true;
            this.playPause.Image = Resources.Pause;
            this.checkBoxStepTime.Enabled = false;
            this.buttonStep.Enabled = false;

            this.stepControlledtelegram = false;
            this.ReadNextTelegram();
        }

        private void PlayPauseOnClick(object sender, EventArgs e)
        {
            if (this.buttonPlay.Enabled)
            {
                this.ButtonPlayClick(sender, e);
            }
            else
            {
                this.ButtonPauseClick(sender, e);
            }
        }

        private void ReadNextTelegram()
        {
            this.sendTimer.Stop();
            if (this.fileReader == null || !this.fileReader.ReadNext())
            {
                this.buttonPlay.Enabled = false;
                this.buttonPause.Enabled = false;
                this.playPause.Visible = false;
                this.checkBoxStepTime.Enabled = false;
                this.buttonStep.Enabled = false;
                this.stepControlledtelegram = false;
                this.reset.Visible = false;
                this.step.Visible = false;
                return;
            }

            var telegram = this.fileReader.CurrentTelegram;
            this.listBoxFileTelegrams.Items.Add(TelegramFormatter.ToHexString(telegram));
            this.listBoxFileTelegrams.SelectedIndex = this.listBoxFileTelegrams.Items.Count - 1;

            this.SendTelegram(telegram, this.checkBoxIgnoreUnknown.Checked);

            if (this.checkBoxStepTime.Checked)
            {
                var time = this.numericUpDownStepTime.Value;
                this.sendTimer.Interval = Math.Max((int)(time * 1000), 1);
            }
            else
            {
                this.sendTimer.Interval = Math.Max((int)this.fileReader.CurrentWaitTime.TotalMilliseconds, 1);
            }

            if (!this.stepControlledtelegram)
            {
                this.sendTimer.Start();
            }
        }

        private void ButtonPauseClick(object sender, EventArgs e)
        {
            this.sendTimer.Stop();
            this.buttonPlay.Enabled = true;
            this.buttonPause.Enabled = false;
            this.checkBoxStepTime.Enabled = true;
            this.buttonStep.Enabled = true;
            this.playPause.Image = Resources.Play;
        }

        private void ListBoxFileTelegramsDoubleClick(object sender, EventArgs e)
        {
            var selectedTelegram = this.listBoxFileTelegrams.SelectedItem as string;
            if (selectedTelegram == null)
            {
                // the selected telegram is not valid.
                return;
            }

            this.SendTelegram(TelegramFormatter.FromHexString(selectedTelegram), this.checkBoxIgnoreUnknown.Checked);
        }

        private void ListBoxIbisBusTelegramsDoubleClick(object sender, EventArgs e)
        {
            var selectedTelegram = this.listBoxIbisBusTelegrams.SelectedItem as string;
            if (selectedTelegram == null)
            {
                // the selected telegram is not valid.
                return;
            }

            this.SendTelegram(TelegramFormatter.FromHexString(selectedTelegram), true);
        }

        private void CheckBoxStepTimeCheckedChanged(object sender, EventArgs e)
        {
            this.numericUpDownStepTime.Enabled = this.checkBoxStepTime.Checked;
        }

        private void ButtonStepClick(object sender, EventArgs e)
        {
            this.stepControlledtelegram = true;
            this.ReadNextTelegram();
        }

        private void ButtonStartIbisBusClick(object sender, EventArgs e)
        {
            this.buttonStartIbisBus.Enabled = false;
            this.controller.EnableSerialPort(true);
        }

        private void ButtonStopIbisBusClick(object sender, EventArgs e)
        {
            this.buttonStartIbisBus.Enabled = true;
            this.controller.EnableSerialPort(false);
        }

        private void ButtonClearClick(object sender, EventArgs e)
        {
            this.listBoxIbisBusTelegrams.Items.Clear();
        }
    }
}
