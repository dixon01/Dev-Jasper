using System;
using System.Text;
using System.Threading;
using System.Collections;
using System.Windows.Forms;

namespace Server.GUIS
{
    public partial class GenViewEditorUI : Form
    {
        #region DELEGATES
        /// <summary>
        /// Delegate for the eventual send of a XIMPLE structure.
        /// <param name="xml">The XIMPLE xml string to be sent.</param>
        public delegate void XimpleSendHandler(string xml);
        /// <summary>
        /// The launcher for the event about the send of a XIMPLE structure.
        /// </summary>
        public event XimpleSendHandler XimpleSendAllarmer;
        #endregion DELEGATES

        #region VARIABLES
        private bool toSend;
        private bool isStarted;
        private int autoSendPeriod;
        private bool isRandomValues;
        private Queue logsQueue;
        private Random randomizer;
        private Thread threadAutoSender;
        #endregion VARIABLES

        public GenViewEditorUI()
        {
            InitializeComponent();
        }

        private void GenViewEditorUI_Load(object sender, EventArgs e)
        {
            this.toSend = false;
            this.isStarted = false;
            this.autoSendPeriod = -1;
            this.isRandomValues = false;
            this.threadAutoSender = null;
            this.logsQueue = new Queue();
            this.randomizer = new Random();
            this.MinimumSize = this.MaximumSize = this.Size;
            this.checkBox_enableAuto.Checked = false;
            this.checkBox_enableAuto_CheckedChanged(this, null);
            this.timer_logsFlusher.Start();
        }

        private void GenViewEditorUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Stop();
            while (this.timer_logsFlusher.Enabled)
            {
                this.timer_logsFlusher.Stop();
                Thread.Sleep(100);
            }
            this.timer_logsFlusher.Dispose();
            this.timer_logsFlusher = null;
            lock (this.logsQueue)
            {
                this.logsQueue.Clear();
            }
        }

        private void checkBox_enableAuto_CheckedChanged(object sender, EventArgs e)
        {
            bool toEnable = false;
            if (this.checkBox_enableAuto.Checked)
            {
                // I've to disable the widgets for the manual behaviour
                // and all the widgets referring to the cell's customization
                toEnable = false;
            }
            else
            {
                // I've to enable the widgets for the manual behaviour
                // and all the widgets referring to the cell's customization
                toEnable = true;
            }

            this.groupBox_coordinates.Enabled =
            this.groupBox_addrType.Enabled =
            this.groupBox_valueType.Enabled =
            this.groupBox_value.Enabled =
            this.groupBox_manual.Enabled = toEnable;

            this.numericUpDown_period.Enabled =
            this.checkBox_randomValues.Enabled =
            this.button_startStopAuto.Enabled = !toEnable;
        }

        private void button_startStopAuto_Click(object sender, EventArgs e)
        {
            if (this.isStarted)
            {
                // the user wants to stop the auto sending.
                this.Stop();
            }
            else
            {
                // the user wants to start the auto sending.
                this.Start();
            }

            this.button_startStopAuto.Text = this.isStarted ? "Stop" : "Start";
        }

        private void button_send_Click(object sender, EventArgs e)
        {
            string xml = this.CreateXml();
            this.Send(xml);
        }

        private void Start()
        {
            if (this.threadAutoSender != null)
            {
                this.Stop();
            }

            this.toSend = true;
            this.threadAutoSender = null;
            this.isRandomValues = this.checkBox_randomValues.Checked;
            this.autoSendPeriod = (int)this.numericUpDown_period.Value;
            this.threadAutoSender = new Thread(new ThreadStart(SendThreaded));
            this.threadAutoSender.Start();
            Thread.Sleep(100);
            this.button_startStopAuto.Text = this.isStarted ? "Stop" : "Start";
            this.checkBox_enableAuto.Enabled =
            this.checkBox_randomValues.Enabled =
            this.numericUpDown_period.Enabled = !this.isStarted;
        }

        private void Stop()
        {
            this.toSend = false;
            while (this.isStarted)
            {
                Thread.Sleep(100);
            }
            this.threadAutoSender = null;
            this.button_startStopAuto.Text = this.isStarted ? "Stop" : "Start";
            this.checkBox_enableAuto.Enabled =
            this.checkBox_randomValues.Enabled =
            this.numericUpDown_period.Enabled = !this.isStarted;
        }

        private void SendThreaded()
        {
            this.isStarted = true;
            while (this.toSend)
            {
                Thread.Sleep(this.autoSendPeriod);
                string xml = this.isRandomValues ? this.CreateXmlRandom() : this.CreateXml();
                this.Send(xml);
            }
            this.isStarted = false;
        }

        private string CreateXml()
        {
            int tableNumber = (int)this.numericUpDown_tableNumber.Value;
            int rowNumber = (int)this.numericUpDown_rowNumber.Value;
            int columnNumber = (int)this.numericUpDown_columnNumber.Value;

            bool isDirect = this.radioButton_addrTypeDirect.Checked;

            bool isTypeText = this.radioButton_valueTypeText.Checked;
            bool isTypeMedia = this.radioButton_valueTypeMedia.Checked;

            string value = this.textBox_value.Text;

            string xml = this.CreateXIMPLE(tableNumber, rowNumber, columnNumber, isDirect, isTypeText, isTypeMedia, value);
            return xml;
        }

        private string CreateXmlRandom()
        {
            int max = 10;
            int min = 0;
            int tableNumber = this.randomizer.Next(min, max);
            int rowNumber = this.randomizer.Next(min, max);
            int columnNumber = this.randomizer.Next(min, max);

            bool isDirect = ((this.randomizer.Next(min, max) % 2) == 0);

            int valueType = this.randomizer.Next(0, 2);
            bool isTypeText = false;
            bool isTypeMedia = false;
            switch( valueType )
            {
                case 0: {isTypeText = true; } break;
                case 1: { isTypeMedia = true; } break;
                default: { isTypeText = true; } break;
            }

            int valueLength = this.randomizer.Next(0, 1 * 1024);
            byte[] valueBuffer = new byte[valueLength];
            for (int i = 0; i < valueBuffer.Length; i++)
            {
                valueBuffer[i] = (byte)this.randomizer.Next(32, 127);
                if( (char)valueBuffer[i] == '<' ||
                    (char)valueBuffer[i] == '>' ||
                    (char)valueBuffer[i] == '"' ||
                    (char)valueBuffer[i] == '\'' ||
                    (char)valueBuffer[i] == '&'
                  )
                {
                    valueBuffer[i] = (byte)' ';
                }
            }
            string value = Encoding.UTF8.GetString(valueBuffer);
            string xml = this.CreateXIMPLE(tableNumber, rowNumber, columnNumber, isDirect, isTypeText, isTypeMedia, value);
            return xml;
        }

        private string CreateXIMPLE(int tableNumber, int rowNumber, int columnNumber, bool isDirect, bool isTypeText, bool isTypeMedia, string value)
        {
            string xml = string.Empty;
            xml += "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + System.Environment.NewLine;
            xml += "<Ximple>" + System.Environment.NewLine;
            xml += "<InfoDatas>" + System.Environment.NewLine;
            xml += "<Cells>" + System.Environment.NewLine;
            xml += "<Cell>" + System.Environment.NewLine;
            xml += "<Table>" + tableNumber + "</Table>" + System.Environment.NewLine;
            xml += "<Row>" + rowNumber + "</Row>" + System.Environment.NewLine;
            xml += "<Column>" + columnNumber + "</Column>" + System.Environment.NewLine;
            xml += "<AddressType>" + (isDirect ? "Direct" : "Indirect") + "</AddressType>" + System.Environment.NewLine;
            xml += "<ValueType>" + (isTypeText ? "TEXT" : "MEDIA") + "</ValueType>" + System.Environment.NewLine;
            xml += "<Value>" + value + "</Value>" + System.Environment.NewLine;
            xml += "</Cell>" + System.Environment.NewLine;
            xml += "</Cells>" + System.Environment.NewLine;
            xml += "</InfoDatas>" + System.Environment.NewLine;
            xml += "</Ximple>" + System.Environment.NewLine;
            return xml;
        }

        private void Send(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                // invalid string.
                return;
            }
            // else...

            lock (this.logsQueue)
            {
                this.logsQueue.Enqueue(xml);
            }
            XimpleSendHandler handler = XimpleSendAllarmer;
            if (handler != null)
            {
                handler(xml);
            }
        }

        private void timer_logsFlusher_Tick(object sender, EventArgs e)
        {
            lock (this.logsQueue)
            {
                while (this.logsQueue.Count != 0)
                {
                    string xml = this.logsQueue.Dequeue() as string;
                    if (xml == null)
                    {
                        // invalid string.
                        return;
                    }
                    if (this.textBox_ximpleArea.Text.Length > 5 * 1024)
                    {
                        this.textBox_ximpleArea.Clear();
                    }
                    this.textBox_ximpleArea.Text += xml;
                }
            }
        }
    }
}
