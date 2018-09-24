using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace MyHyperTerminal
{
    public partial class SendFileUI : Form
    {
        #region DELEGATES
        public delegate void SendFileWriteOperation(object sender, SendFileEvent e);
        public event SendFileWriteOperation SendFileWriteOperationAllarmer;
        #endregion DELEGATES

        #region VARIABLES
        private bool started;
        private bool toContinueSend;
        #endregion VARIABLES

        public SendFileUI()
        {
            InitializeComponent();
        }

        private void SendFileUI_Load(object sender, EventArgs e)
        {
            this.started = false;
            this.toContinueSend = true;
        }

        private void button_selectAFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                this.textBox_fileAbsPath.Clear();
                return;
            }
            // else...

            FileInfo info = new FileInfo(dialog.FileName);
            if (!info.Exists)
            {
                this.textBox_fileAbsPath.Clear();
                return;
            }
            // else...

            this.textBox_fileAbsPath.Text = info.FullName;
        }

        private void button_send_Click(object sender, EventArgs e)
        {
            if (this.started)
            {
                // the user want to stop
                this.Stop();
                this.button_send.Text = "Start";
            }
            else
            {
                // the user want to start
                this.Start();
                this.button_send.Text = "Stop";
            }
        }

        public void Start()
        {
            FileStream fileStream = null;
            StreamReader streamReader = null;
            try
            {
                fileStream = new FileStream(this.textBox_fileAbsPath.Text, FileMode.Open, FileAccess.Read);
                streamReader = new StreamReader(fileStream);
            }
            catch (Exception) { }

            if (fileStream == null || streamReader == null)
            {
                // impossible to open the file
                return;
            }
            // else...

            int bytesRead = 0;
            byte[] buffer = new byte[100];
            do
            {
                bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                SendFileWriteOperation handler = SendFileWriteOperationAllarmer;
                if (handler != null)
                {
                    SendFileEvent sendEvent = new SendFileEvent(buffer, bytesRead);
                    handler(this, sendEvent);
                }
                Thread.Sleep(1);
            } while (this.toContinueSend && bytesRead > 0);

            this.started = false;
            this.toContinueSend = true;
            this.button_send.Text = "Start";
        }

        public void Stop()
        {
            this.started = false;
            this.toContinueSend = false;
            Thread.Sleep(1000);
            this.button_send.Text = "Stop";
        }
    }
}