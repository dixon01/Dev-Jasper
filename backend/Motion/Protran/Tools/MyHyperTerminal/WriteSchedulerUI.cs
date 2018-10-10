using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using MyHyperTerminal.Properties;
using System.IO;

namespace MyHyperTerminal
{
    public partial class WriteSchedulerUI : Form
    {
        #region DELEGATES
        public delegate void WriteOperationScheduled(object sender, WriteOperationEvent e);
        public event WriteOperationScheduled WriteOperationScheduledAllarmer;
        #endregion DELEGATES

        #region VARIABLES
        private List<GroupBox> groupBoxList;
        private Thread threadScheduler;
        private StreamWriter streamWriter;
        private StringBuilder formatter;

        private ulong packetsSentCounter;
        private bool started;
        #endregion VARIABLES

        public WriteSchedulerUI()
        {
            InitializeComponent();
        }

        private void WriteSchedulerUI_Load(object sender, EventArgs e)
        {
            this.groupBoxList = new List<GroupBox>();
            for (int i = 0; i < 10; i++)
            {
                AddSchedulePacket();
            }

            this.streamWriter = null;
            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;
            this.formatter = new StringBuilder();
        }

        private void AddSchedulePacket()
        {
            // i create a group box
            GroupBox box = new GroupBox();
            box.Size = new Size(600, 50);
            box.Text = "Packet" + " " + this.groupBoxList.Count;

            CheckBox checkBoxAscii = new CheckBox();
            checkBoxAscii.Text = "Ascii";
            checkBoxAscii.Size = new Size(50, 20);

            TextBox textBox = new TextBox();
            textBox.Size = new Size(200, 20);

            Label timeOutLabel = new Label();
            timeOutLabel.Text = "Timeout(ms)";
            timeOutLabel.Size = new Size(65, 20);

            NumericUpDown timerNumUpDown = new NumericUpDown();
            timerNumUpDown.Size = new Size(50, 20);
            timerNumUpDown.TextAlign = HorizontalAlignment.Center;
            timerNumUpDown.Minimum = 15;
            timerNumUpDown.Maximum = 10000;

            Label howManyTimes = new Label();
            howManyTimes.Text = "How many";
            howManyTimes.Size = new Size(60, 20);

            NumericUpDown howManyUpDown = new NumericUpDown();
            howManyUpDown.Size = new Size(50, 20);
            howManyUpDown.TextAlign = HorizontalAlignment.Center;
            howManyUpDown.Minimum = 1;
            howManyUpDown.Maximum = 1000;

            CheckBox checkBoxInfinite = new CheckBox();
            checkBoxInfinite.Text = "Inf";
            checkBoxInfinite.Size = new Size(40, 20);
            
            PictureBox pBox = new PictureBox();
            pBox.Size = new Size(20, 20);
            pBox.SizeMode = PictureBoxSizeMode.StretchImage;
            pBox.Image = Resources.grey;

            FlowLayoutPanel packetPanel = new FlowLayoutPanel();
            packetPanel.Dock = DockStyle.Fill;

            packetPanel.Controls.Add(checkBoxAscii);
            packetPanel.Controls.Add(textBox);
            packetPanel.Controls.Add(timeOutLabel);
            packetPanel.Controls.Add(timerNumUpDown);

            packetPanel.Controls.Add(howManyTimes);
            packetPanel.Controls.Add(howManyUpDown);
            packetPanel.Controls.Add(checkBoxInfinite);

            packetPanel.Controls.Add(pBox);

            box.Controls.Add(packetPanel);
            this.groupBoxList.Add(box);
            this.main_flowLayoutPanel.Controls.Add(box);
        }

        private void button_addTaskPacket_Click(object sender, EventArgs e)
        {
            AddSchedulePacket();
        }

        private void button_start_Click(object sender, EventArgs e)
        {
            if (this.started)
            {
                // the user want to stop a started schedule process
                this.threadScheduler.Abort();
                this.threadScheduler = null;
                this.started = false;
                this.button_start.Text = "Start";
                this.checkBox_enableLog.Enabled = true;
                this.textBox_logFileAbsPath.Enabled = true;
                this.checkBox_loopOnPackets.Enabled = true;
                this.button_resetPacketsSent.Enabled = true;

                this.streamWriter.Flush();
                this.streamWriter.Close();
                this.streamWriter.Dispose();
                this.streamWriter = null;
                return;
            }
            // else...
            // the user want to start a schedule process

            if (string.IsNullOrEmpty(this.textBox_logFileAbsPath.Text))
            {
                this.textBox_logFileAbsPath.Text = "./packets.log";
            }            
            this.streamWriter = new StreamWriter(this.textBox_logFileAbsPath.Text);
            this.started = true;
            this.button_start.Text = "Stop";
            this.checkBox_enableLog.Enabled = false;
            this.textBox_logFileAbsPath.Enabled = false;
            this.checkBox_loopOnPackets.Enabled = false;
            this.button_resetPacketsSent.Enabled = false;

            for (int i = 0; i < this.groupBoxList.Count; i++)
            {
                GroupBox gBox = this.groupBoxList[i] as GroupBox;
                if (gBox == null)
                {
                    continue;
                }
                // else...
                FlowLayoutPanel gBoxPanel = gBox.Controls[0] as FlowLayoutPanel;
                PictureBox pBox = gBoxPanel.Controls[ 7 ] as PictureBox;
                pBox.Image = Resources.grey;
            }
            this.Refresh();

            this.threadScheduler = new Thread(new ThreadStart(StartSchedule));
            this.threadScheduler.Name = "Thread Scheduler";
            this.threadScheduler.Start();

            #region DISABLE CONTROLS
            //// disable all the group boxes
            //for (int i = 0; i < this.groupBoxList.Count; i++)
            //{
            //    GroupBox gBox = this.groupBoxList[i] as GroupBox;
            //    if (gBox == null)
            //    {
            //        continue;
            //    }
            //    // else...
            //    FlowLayoutPanel gBoxPanel = gBox.Controls[0] as FlowLayoutPanel;
            //    if (gBoxPanel == null)
            //    {
            //        continue;
            //    }
            //    // else...
            //    for (int j = 0; j < gBoxPanel.Controls.Count; j++)
            //    {
            //        gBoxPanel.Controls[j].Enabled = false;
            //    }
            //}
            #endregion DISABLE CONTROLS

            #region ENABLE CONTROLS
            //// enable all the group boxes
            //for (int i = 0; i < this.groupBoxList.Count; i++)
            //{
            //    GroupBox gBox = this.groupBoxList[i] as GroupBox;
            //    if (gBox == null)
            //    {
            //        continue;
            //    }
            //    // else...
            //    FlowLayoutPanel gBoxPanel = gBox.Controls[0] as FlowLayoutPanel;
            //    if (gBoxPanel == null)
            //    {
            //        continue;
            //    }
            //    // else...
            //    for (int j = 0; j < gBoxPanel.Controls.Count; j++)
            //    {
            //        gBoxPanel.Controls[j].Enabled = true;
            //    }
            //}
            #endregion ENABLE CONTROLS
        }

        private void StartSchedule()
        {
            bool loopOnPackets = false;
            Invoke(new MethodInvoker(delegate()
                { loopOnPackets = this.checkBox_loopOnPackets.Checked; }));

            // send all the packets
            do
            {
                for (int i = 0; i < this.groupBoxList.Count; i++)
                {
                    GroupBox gBox = this.groupBoxList[i] as GroupBox;
                    if (gBox == null)
                    {
                        continue;
                    }
                    // else...
                    FlowLayoutPanel gBoxPanel = gBox.Controls[0] as FlowLayoutPanel;
                    if (gBoxPanel == null)
                    {
                        continue;
                    }
                    // else...

                    bool isAscii = ((CheckBox)gBoxPanel.Controls[0]).Checked;
                    string text = ((TextBox)gBoxPanel.Controls[1]).Text;
                    int timeOut = (int)((NumericUpDown)gBoxPanel.Controls[3]).Value;
                    int howManyTimes = (int)((NumericUpDown)gBoxPanel.Controls[5]).Value;
                    bool infinite = ((CheckBox)gBoxPanel.Controls[6]).Checked;

                    if (string.IsNullOrEmpty(text))
                    {
                        continue;
                    }
                    // else...

                    if (infinite)
                    {
                        Invoke(
                            new MethodInvoker(
                                delegate()
                                    {
                                        PictureBox pBox = gBoxPanel.Controls[7] as PictureBox;
                                        pBox.Image = Resources.red;
                                        this.Refresh();
                                    }));

                        while (true)
                        {
                            Thread.Sleep(timeOut);
                            NotifyScheduledPacket(new WriteOperationEvent(text, isAscii));
                        }
                    }
                    else
                    {
                        Invoke(
                            new MethodInvoker(
                                delegate()
                                    {
                                        PictureBox pBox = gBoxPanel.Controls[7] as PictureBox;
                                        pBox.Image = Resources.yellow;
                                        this.Refresh();
                                    }));

                        for (int j = 0; j < howManyTimes; j++)
                        {
                            Thread.Sleep(timeOut);
                            NotifyScheduledPacket(new WriteOperationEvent(text, isAscii));
                        }
                    }

                    Invoke(
                        new MethodInvoker(
                            delegate()
                                {
                                    PictureBox pBox = gBoxPanel.Controls[7] as PictureBox;
                                    pBox.Image = Resources.green;
                                    this.Refresh();
                                }));

                }
            }
            while (loopOnPackets);
            this.started = false;

            Invoke( (MethodInvoker) (delegate()
            {
                this.button_start.Text = "Start";
            }));
        }

        private void NotifyScheduledPacket(WriteOperationEvent writeOperationEvent)
        {
            WriteOperationScheduled handler = WriteOperationScheduledAllarmer;
            if (handler != null)
            {
                handler(this, writeOperationEvent);
                Invoke(new MethodInvoker(delegate()
                    { this.textBox_packetsSent.Text = (++this.packetsSentCounter).ToString(); }));

                if(this.streamWriter != null)
                {
                    this.formatter.Remove(0, this.formatter.Length);
                    // it's valid the first formatting style.
                    DateTime now = DateTime.Now;
                    this.formatter.AppendFormat("{0:yyyy:MM:dd HH:mm:ss.fff}", now);
                    this.formatter.Append(' ');
                    this.formatter.Append(writeOperationEvent.Text);
                    this.streamWriter.WriteLine(this.formatter.ToString());
                    this.streamWriter.Flush();
                }
            }
        }

        private byte[] HexStringToByteArray(string s)
        {
            byte[] buf = new byte[s.Length / 2];
            try
            {
                s = s.Replace(" ", "");
                for (int i = 0; i < s.Length; i += 2)
                {
                    buf[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
                }
                return buf;
            }
            catch (FormatException)
            {
                return null;
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
            catch (OverflowException)
            {
                return null;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        private void WriteSchedulerUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.threadScheduler != null)
            {
                this.threadScheduler.Abort();
                this.threadScheduler = null;
            }
        }

        internal void StopThread()
        {
            if (this.threadScheduler != null)
            {
                this.threadScheduler.Abort();
                this.threadScheduler = null;
            }
        }

        private void button_resetPacketsSent_Click(object sender, EventArgs e)
        {
            this.packetsSentCounter = 0;
            this.textBox_packetsSent.Text = string.Empty;
        }
    }
}
