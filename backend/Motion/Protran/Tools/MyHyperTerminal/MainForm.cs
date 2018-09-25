// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainForm.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MainForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MyHyperTerminal
{
    using System;
    using System.IO;
    using System.IO.Ports;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;

    using Gorba.Motion.Protran.Controls;

    using MyHyperTerminal.Properties;

    /// <summary>
    /// The main form of the whole application.
    /// </summary>
    public partial class MainForm : Form
    {
        #region VARIABLES
        private SerialPort serialPort;
        private WriteSchedulerUI writeSchedulerUi;
        private SendFileUI sendFileUi;
        private IbisUi ibisUi;

        private Mutex mutexWriteArea = new Mutex();

        private string[] initialValues;
        private string[] comPortInThisComputer;
        
        private bool firstLoadExecuted;

        #endregion VARIABLES

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        private string[] DiscoverSerialPortsWmi()
        {
            // actually, I don't make any discovery.
            // I only return an array with good candidate names.
            for (int i = 0; i < 50; i++)
            {
                this.comPortInThisComputer[i] = "COM" + i;
            }

            return this.comPortInThisComputer;

            //// i create the query in order to retrieve the serial port hide in this PC
            // string theQuery = "SELECT Caption " +
            //                  "FROM CIM_LogicalDevice " +
            //                  "WHERE Caption LIKE \'%COM1%\' OR " +
            //                  "Caption LIKE \'%COM1%\' OR " +
            //                  "Caption LIKE \'%COM2%\' OR " +
            //                  "Caption LIKE \'%COM3%\' OR " +
            //                  "Caption LIKE \'%COM4%\' OR " +
            //                  "Caption LIKE \'%COM5%\' OR " +
            //                  "Caption LIKE \'%COM6%\' OR " +
            //                  "Caption LIKE \'%COM7%\' OR " +
            //                  "Caption LIKE \'%COM8%\' OR " +
            //                  "Caption LIKE \'%COM9%\' OR " +
            //                  "Caption LIKE \'%COM10%\' OR " +
            //                  "Caption LIKE \'%COM11%\' OR " +
            //                  "Caption LIKE \'%COM12%\' OR " +
            //                  "Caption LIKE \'%COM13%\' OR " +
            //                  "Caption LIKE \'%COM14%\' OR " +
            //                  "Caption LIKE \'%COM15%\' OR " +
            //                  "Caption LIKE \'%COM16%\' OR " +
            //                  "Caption LIKE \'%COM17%\' OR " +
            //                  "Caption LIKE \'%COM18%\' OR " +
            //                  "Caption LIKE \'%COM19%\' OR " +
            //                  "Caption LIKE \'%COM20%\' OR " +
            //                  "Caption LIKE \'%COM21%\' OR " +
            //                  "Caption LIKE \'%COM22%\' OR " +
            //                  "Caption LIKE \'%COM23%\' OR " +
            //                  "Caption LIKE \'%COM24%\' OR " +
            //                  "Caption LIKE \'%COM25%\' OR " +
            //                  "Caption LIKE \'%COM26%\' OR " +
            //                  "Caption LIKE \'%COM27%\' OR " +
            //                  "Caption LIKE \'%COM28%\' OR " +
            //                  "Caption LIKE \'%COM29%\' OR " +
            //                  "Caption LIKE \'%COM30%\' OR " +
            //                  "Caption LIKE \'%COM31%\' OR " +
            //                  "Caption LIKE \'%COM32%\' OR " +
            //                  "Caption LIKE \'%COM33%\' OR " +
            //                  "Caption LIKE \'%COM34%\' OR " +
            //                  "Caption LIKE \'%COM35%\' OR " +
            //                  "Caption LIKE \'%COM36%\' OR " +
            //                  "Caption LIKE \'%COM37%\' OR " +
            //                  "Caption LIKE \'%COM38%\' OR " +
            //                  "Caption LIKE \'%COM39%\' OR " +
            //                  "Caption LIKE \'%COM40%\' OR " +
            //                  "Caption LIKE \'%COM41%\' OR " +
            //                  "Caption LIKE \'%COM42%\' OR " +
            //                  "Caption LIKE \'%COM43%\' OR " +
            //                  "Caption LIKE \'%COM44%\' OR " +
            //                  "Caption LIKE \'%COM45%\' OR " +
            //                  "Caption LIKE \'%COM46%\' OR " +
            //                  "Caption LIKE \'%COM47%\' OR " +
            //                  "Caption LIKE \'%COM48%\' OR " +
            //                  "Caption LIKE \'%COM49%\' OR " +
            //                  "Caption LIKE \'%COM50%\'";
            //// now i give a body to the query
            // ManagementObjectSearcher query = new ManagementObjectSearcher(theQuery); // Win32_SerialPort"); //"); //Win32_COMClass"); //;From Win32_OperatingSystem");
            //// now i perform the query in order to obtain the results
            // ManagementObjectCollection queryCollection = query.Get();
            //// now i must make a filter inside the query result because i've not used a DISTINCT clause above
            // try
            // {
            //    foreach (ManagementObject mo in queryCollection)
            //    {
            //        // temporarly i memorize the current object
            //        string tmp = mo["Caption"].ToString();
            //        if (!string.IsNullOrEmpty(tmp))
            //        {
            //            for (int i = 0; i < COMPortInThisComputer.Length; i++)
            //            {
            //                // i make a comparison with the COM ports found at the start of the ConfigSerialPortUI_Load method
            //                if (tmp.Contains(COMPortInThisComputer[i]))
            //                {
            //                    // if i have a match, i complete the previous description with the complete name
            //                    COMPortInThisComputer[i] = tmp;
            //                }
            //            }
            //        }
            //    }
            // }
            // catch (ArgumentNullException a)
            // {
            //    MessageBox.Show("Error.");
            // }
            //// now i can return the values
            // return COMPortInThisComputer;
        } // end method DiscoverSerialPorts_WMI

        private void Form1Load(object sender, EventArgs e)
        {
            // this.SetTextDelegHandler += new SetTextDeleg(this.SetText);
            // this.UpdatePositionDelegHandler += new UpdatePositionDeleg(this.UpdatePosition);
            this.serialPort = new SerialPort();
            this.serialPort.ReceivedBytesThreshold = 1;
            this.serialPort.DataReceived += this.SerialPortDataReceived;

            // now i'm searching for all the peripherals inside the PC
            this.comPortInThisComputer = new string[50]; // System.IO.Ports.SerialPort.GetPortNames();

            // at this point i've only the name of the serial port. For example COM1, COM2, ecc.
            // but i don't have theirs descriptions
            // in order to have also theirs description, i must use WMI
            this.comPortInThisComputer = this.DiscoverSerialPortsWmi();
            if (this.comPortInThisComputer != null)
            {
                this.comboBox_NameSP.Items.AddRange(this.comPortInThisComputer);
            }

            // first thing to do is capture the default values for the serial port, writed the last time in the FileConfig instance
            string defaultValues = this.GetSPDefaultParams();

            // now i must insert these values in the combo box, text box and so on of the ConfigSerialPortUI window GUI
            this.FillTheComboBox(defaultValues);

            // this copy is necessary when the User will press to the button "Reset To Initial Values"
            this.initialValues = defaultValues.Split();
            this.firstLoadExecuted = true;
            this.SetSerialStatusIcon();
            this.OnButtonOpenSpClick(this, null);

            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;
        }

        private string GetSPDefaultParams()
        {
            return "COM1 115200 None 8 One None 4096 4096 ";
        }

        private void FillTheComboBox(string defaultValues)
        {
            string[] values = defaultValues.Split();
            string description = this.DiscoverSerialPortDescription(values[0]);
            comboBox_NameSP.Text = description;
            comboBox_BaudSP.Text = values[1];
            comboBox_ParitySP.Text = values[2];
            comboBox_NBitSP.Text = values[3];
            comboBox_BitStopSP.Text = values[4];
            comboBox_FlowControl.Text = values[5];
            textBox_BufferInputSP.Text = values[6];
            textBox_BufferOutputSP.Text = values[7];
            trackBar_BufferInputSP.Value = int.Parse(values[6]);
            trackBar_BufferOutputSP.Value = int.Parse(values[7]);
        } // end method FillTheComboBox

        private string DiscoverSerialPortName()
        {
            string tmp = comboBox_NameSP.Text;
            string valueToReturn = string.Empty;
            if (tmp.Contains("COM1"))
            {
                valueToReturn = "COM1";
            }

            if (tmp.Contains("COM2"))
            {
                valueToReturn = "COM2";
            }

            if (tmp.Contains("COM3"))
            {
                valueToReturn = "COM3";
            }

            if (tmp.Contains("COM4"))
            {
                valueToReturn = "COM4";
            }

            if (tmp.Contains("COM5"))
            {
                valueToReturn = "COM5";
            }

            if (tmp.Contains("COM6"))
            {
                valueToReturn = "COM6";
            }

            if (tmp.Contains("COM7"))
            {
                valueToReturn = "COM7";
            }

            if (tmp.Contains("COM8"))
            {
                valueToReturn = "COM8";
            }

            if (tmp.Contains("COM9"))
            {
                valueToReturn = "COM9";
            }

            if (tmp.Contains("COM10"))
            {
                valueToReturn = "COM10";
            }

            if (tmp.Contains("COM11"))
            {
                valueToReturn = "COM11";
            }

            if (tmp.Contains("COM12"))
            {
                valueToReturn = "COM12";
            }

            if (tmp.Contains("COM13"))
            {
                valueToReturn = "COM13";
            }

            if (tmp.Contains("COM14"))
            {
                valueToReturn = "COM14";
            }

            if (tmp.Contains("COM15"))
            {
                valueToReturn = "COM15";
            }

            if (tmp.Contains("COM16"))
            {
                valueToReturn = "COM16";
            }

            if (tmp.Contains("COM17"))
            {
                valueToReturn = "COM17";
            }

            if (tmp.Contains("COM18"))
            {
                valueToReturn = "COM18";
            }

            if (tmp.Contains("COM19"))
            {
                valueToReturn = "COM19";
            }

            if (tmp.Contains("COM20"))
            {
                valueToReturn = "COM20";
            }

            if (tmp.Contains("COM21"))
            {
                valueToReturn = "COM21";
            }

            if (tmp.Contains("COM22"))
            {
                valueToReturn = "COM22";
            }

            if (tmp.Contains("COM23"))
            {
                valueToReturn = "COM23";
            }

            if (tmp.Contains("COM24"))
            {
                valueToReturn = "COM24";
            }

            if (tmp.Contains("COM25"))
            {
                valueToReturn = "COM25";
            }

            if (tmp.Contains("COM26"))
            {
                valueToReturn = "COM26";
            }

            if (tmp.Contains("COM27"))
            {
                valueToReturn = "COM27";
            }

            if (tmp.Contains("COM28"))
            {
                valueToReturn = "COM28";
            }

            if (tmp.Contains("COM29"))
            {
                valueToReturn = "COM29";
            }

            if (tmp.Contains("COM30"))
            {
                valueToReturn = "COM30";
            }

            if (tmp.Contains("COM31"))
            {
                valueToReturn = "COM31";
            }

            if (tmp.Contains("COM32"))
            {
                valueToReturn = "COM32";
            }

            if (tmp.Contains("COM33"))
            {
                valueToReturn = "COM33";
            }

            if (tmp.Contains("COM34"))
            {
                valueToReturn = "COM34";
            }

            if (tmp.Contains("COM35"))
            {
                valueToReturn = "COM35";
            }

            if (tmp.Contains("COM36"))
            {
                valueToReturn = "COM36";
            }

            if (tmp.Contains("COM37"))
            {
                valueToReturn = "COM37";
            }

            if (tmp.Contains("COM38"))
            {
                valueToReturn = "COM38";
            }

            if (tmp.Contains("COM39"))
            {
                valueToReturn = "COM39";
            }

            if (tmp.Contains("COM40"))
            {
                valueToReturn = "COM40";
            }

            if (tmp.Contains("COM41"))
            {
                valueToReturn = "COM41";
            }

            if (tmp.Contains("COM42"))
            {
                valueToReturn = "COM42";
            }

            if (tmp.Contains("COM43"))
            {
                valueToReturn = "COM43";
            }

            if (tmp.Contains("COM44"))
            {
                valueToReturn = "COM44";
            }

            if (tmp.Contains("COM45"))
            {
                valueToReturn = "COM45";
            }

            if (tmp.Contains("COM46"))
            {
                valueToReturn = "COM46";
            }

            if (tmp.Contains("COM47"))
            {
                valueToReturn = "COM47";
            }

            if (tmp.Contains("COM48"))
            {
                valueToReturn = "COM48";
            }

            if (tmp.Contains("COM49"))
            {
                valueToReturn = "COM49";
            }

            if (tmp.Contains("COM50"))
            {
                valueToReturn = "COM50";
            }

            return valueToReturn;
        } // end method DiscoverSerialPortName

        private string DiscoverSerialPortDescription(string name)
        {
            for (int i = 0; i < this.comPortInThisComputer.Length; i++)
            {
                if (this.comPortInThisComputer[i].Contains(name))
                {
                    return this.comPortInThisComputer[i];
                }
            }

            return string.Empty;
        } // end method DiscoverSerialPortDescription

        private void SetSerialStatusIcon()
        {
            if (this.serialPort == null)
            {
                return;
            }

            if (this.serialPort.IsOpen)
            {
                this.serialPortStatusPictureBox.Image = Resources.Power;
                if (!this.serialPort.IsOpen)
                {
                    // a problem is occupied by the RTS Dispatcher but during it's initializations
                    // something was gone wrong, so i set the PowerOff image
                    this.serialPortStatusPictureBox.Image = Resources.PowerOff;
                }
            }
            else
            {
                this.serialPortStatusPictureBox.Image = Resources.PowerOff;
            }
        } // end method SetSerialStatusIcon

        private void SerialParameterValueChanged(object sender, EventArgs e)
        {
            // the user has made a selection. The new value/values must be inserted in the FileConfig instance
            if (this.firstLoadExecuted)
            {
                var mutex = new Mutex();
                mutex.WaitOne();
                string realSerialPortName = this.DiscoverSerialPortName();
                string tmp = realSerialPortName + " " + comboBox_BaudSP.Text + " " + comboBox_ParitySP.Text + " " +
                             comboBox_NBitSP.Text + " " + comboBox_BitStopSP.Text + " " + comboBox_FlowControl.Text + " " +
                             textBox_BufferInputSP.Text + " " + textBox_BufferOutputSP.Text;
                this.SetParams(tmp.Split());
                if (!this.serialPort.IsOpen)
                {
                    MessageBox.Show("Error in setting serial port's values. ");
                }

                this.SetSerialStatusIcon();
                mutex.Close();
            }
        } // end method SerialParameterValueChanged

        /// <summary>
        /// Sets the params
        /// </summary>
        /// <param name="defaultParams">
        /// The default params.
        /// </param>
        private void SetParams(string[] defaultParams)
        {
            if (defaultParams != null)
            {
                // before setting parameters, the serial port must be (temporaly) closed
                try
                {
                    if (this.serialPort != null)
                    {
                        this.serialPort.Close();
                        this.serialPort.PortName = defaultParams[0];
                        this.serialPort.BaudRate = int.Parse(defaultParams[1]);

                        var parity = (Parity)Enum.Parse(typeof(Parity), defaultParams[2]);
                        this.serialPort.Parity = parity;

                        this.serialPort.DataBits = int.Parse(defaultParams[3]);

                        var stop = (StopBits)Enum.Parse(typeof(StopBits), defaultParams[4]);
                        this.serialPort.StopBits = stop;

                        var hand = (Handshake)Enum.Parse(typeof(Handshake), defaultParams[5]);
                        this.serialPort.Handshake = hand;

                        this.serialPort.ReadBufferSize = int.Parse(defaultParams[6]);
                        this.serialPort.WriteBufferSize = int.Parse(defaultParams[7]);

                        // now i can re-open the serial port
                        this.serialPort.Open();
                    }
                }
                catch (IOException)
                {
                    MessageBox.Show("Error in setting serial port's values");
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("Error in setting serial port's values");
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("Error in setting serial port's values");
                }
            }
        } // end method SetParams

        private void OnButtonCloseSpClick(object sender, EventArgs e)
        {
            try
            {
                this.serialPort.Close();
                this.SetSerialStatusIcon();
            }
            catch (IOException)
            {
                MessageBox.Show("Error in closing the serial port");
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Error in closing the serial port");
            }
        }

        private void OnButtonResetSpClick(object sender, EventArgs e)
        {
            var mutex = new Mutex();
            mutex.WaitOne();
            string serialPortName = this.DiscoverSerialPortName();
            string tmp = serialPortName + " " + this.initialValues[1] + " " + this.initialValues[2] + " "
                         + this.initialValues[3] + " " + this.initialValues[4] + " " + this.initialValues[5] + " "
                         + this.initialValues[6] + " " + this.initialValues[7];
            comboBox_NameSP.Text = serialPortName;
            this.comboBox_BaudSP.Text = this.initialValues[1];
            this.comboBox_ParitySP.Text = this.initialValues[2];
            this.comboBox_NBitSP.Text = this.initialValues[3];
            this.comboBox_BitStopSP.Text = this.initialValues[4];
            this.comboBox_FlowControl.Text = this.initialValues[5];
            this.textBox_BufferInputSP.Text = this.initialValues[6];
            this.textBox_BufferOutputSP.Text = this.initialValues[7];
            this.SetParams(tmp.Split());
            mutex.Close();
        } // end method button_ResetSP_Click

        private void OnButtonOpenSpClick(object sender, EventArgs e)
        {
            try
            {
                this.serialPort.Open();
                this.SetSerialStatusIcon();
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Serial Port already occupied.");
            }
            catch (IOException)
            {
                MessageBox.Show("Serial Port already used.");
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Serial Port already used from other process.");
            }
        } // end method button_OpenSP_Click

        private void OnSerialPortStatusPictureBoxClick(object sender, EventArgs e)
        {
            if (this.serialPort.IsOpen)
            {
                this.OnButtonCloseSpClick(sender, e);
            }
            else
            {
                this.OnButtonOpenSpClick(sender, e);
            }
        } // end method button_ResetSP_Click

        private void OnButtonSendClick(object sender, EventArgs e)
        {
            if (!this.serialPort.IsOpen)
            {
                return;
            }

            string text = this.textBox_packetToWrite.Text;
            this.WriteData(text, this.checkBox_ascii.Checked);
        }

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // read data till no more bytes to read
            int bytesToRead;
            while ((bytesToRead = this.serialPort.BytesToRead) > 0)
            {
                var tmpBuffer = new byte[bytesToRead];
                this.serialPort.Read(tmpBuffer, 0, bytesToRead);
                string line = string.Empty;
                for (int i = 0; i < tmpBuffer.Length; i++)
                {
                    line += tmpBuffer[i].ToString("X2");
                    if (i + 1 < tmpBuffer.Length)
                    {
                        line += " ";
                    }
                }

                if (this.checkBox_readAreaEnable.Checked)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        if (this.textBox_writeArea.Text.Length + line.Length >= this.textBox_writeArea.MaxLength)
                        {
                            this.textBox_writeArea.Clear();
                        }

                        line += Environment.NewLine;
                        this.textBox_writeArea.Text += line;
                    });
                }

                // this.textBox_writeArea.Invoke(SetTextDelegHandler, new object[] { line });
            }

            if (this.checkBox_readAreaEnable.Checked)
            {
                this.Invoke(
                    (MethodInvoker)delegate
                        {
                            this.textBox_writeArea.SelectionStart = textBox_writeArea.Text.Length;
                            this.textBox_writeArea.ScrollToCaret();
                            this.textBox_writeArea.Update();
                        });
            }

            // this.textBox_writeArea.Invoke(UpdatePositionDelegHandler);
        }

        private byte[] HexStringToByteArray(string s)
        {
            var buf = new byte[s.Length / 2];
            try
            {
                s = s.Replace(" ", string.Empty);
                for (int i = 0; i < s.Length; i += 2)
                {
                    buf[i / 2] = Convert.ToByte(s.Substring(i, 2), 16);
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

        private void OnButtonClearDataReadClick(object sender, EventArgs e)
        {
            this.textBox_writeArea.Text = string.Empty;
        }

        private void OnExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.Exit();
        }

        private void OnForm1FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Exit();
        }

        private void Exit()
        {
            if (this.serialPort != null)
            {
                this.serialPort.Close();
            }

            this.serialPort = null;

            if (this.mutexWriteArea != null)
            {
                this.mutexWriteArea.Close();
            }

            this.mutexWriteArea = null;

            if (this.writeSchedulerUi != null)
            {
                this.writeSchedulerUi.StopThread();
                this.writeSchedulerUi.Dispose();
                this.writeSchedulerUi.Close();
            }

            this.writeSchedulerUi = null;

            if (this.sendFileUi != null)
            {
                this.sendFileUi.Stop();
                this.sendFileUi.Dispose();
                this.sendFileUi.Close();
            }

            this.sendFileUi = null;
            Application.Exit();
        }

        private void OnAutoWriteToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.writeSchedulerUi == null)
            {
                this.writeSchedulerUi = new WriteSchedulerUI();
                this.writeSchedulerUi.WriteOperationScheduledAllarmer += this.WriteOperationScheduledAllarmer;
            }
            else
            {
                this.writeSchedulerUi.Dispose();
                this.writeSchedulerUi.Close();
                this.writeSchedulerUi = null;
                this.writeSchedulerUi = new WriteSchedulerUI();
                this.writeSchedulerUi.WriteOperationScheduledAllarmer += this.WriteOperationScheduledAllarmer;
            }

            this.writeSchedulerUi.Show(); // <== Non blocking call
        }

        private void OnSendAFileToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.sendFileUi == null)
            {
                this.sendFileUi = new SendFileUI();
                this.sendFileUi.SendFileWriteOperationAllarmer += this.SendFileWriteOperationAllarmer;
            }
            else
            {
                this.sendFileUi.Stop();
                this.sendFileUi.Dispose();
                this.sendFileUi.Close();
                this.sendFileUi = null;
                this.sendFileUi = new SendFileUI();
                this.sendFileUi.SendFileWriteOperationAllarmer += this.SendFileWriteOperationAllarmer;
            }

            this.sendFileUi.Show();
        }

        private void SendFileWriteOperationAllarmer(object sender, SendFileEvent e)
        {
            if (e.Buffer == null || e.BufferLength <= 0)
            {
                return;
            }

            this.WriteData(e.Buffer, e.BufferLength);
        }

        private void WriteOperationScheduledAllarmer(object sender, WriteOperationEvent e)
        {
            if (string.IsNullOrEmpty(e.Text))
            {
                return;
            }

            this.WriteData(e.Text, e.IsAscii);
        }

        private void WriteData(string text, bool isAscii)
        {
            if (this.serialPort == null || !this.serialPort.IsOpen)
            {
                return;
            }

            byte[] buffer;
            if (isAscii)
            {
                // the user want to send an ascii string
                buffer = Encoding.ASCII.GetBytes(text);
            }
            else
            {
                // the user want to send a hex string
                text = text.Replace(" ", string.Empty);
                text = text.Trim();
                buffer = this.HexStringToByteArray(text);
            }

            if (buffer == null || buffer.Length == 0)
            {
                MessageBox.Show("Error. Verify your data.");
                return;
            }

            this.WriteData(buffer, buffer.Length);
        }

        private void WriteData(byte[] buffer, int length)
        {
            this.serialPort.Write(buffer, 0, length);
            this.serialPort.BaseStream.Flush();
        }

        private void OnButtonUpdateClick(object sender, EventArgs e)
        {
            this.OnButtonCloseSpClick(this, null);
            this.comboBox_NameSP.Items.Clear();

            // this.comPortInThisComputer = SerialPort.GetPortNames();
            this.comPortInThisComputer = this.DiscoverSerialPortsWmi();
            for (int i = 0; i < this.comPortInThisComputer.Length; i++)
            {
                if (!this.comboBox_NameSP.Items.Contains(this.comPortInThisComputer[i]))
                {
                    this.comboBox_NameSP.Items.Add(this.comPortInThisComputer[i]);
                }
            }
        }

        private void OnIBisCreatorToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.ibisUi == null)
            {
                this.ibisUi = new IbisUi();
            }
            else
            {
                this.ibisUi.Dispose();
                this.ibisUi.Close();
                this.ibisUi = null;
                this.ibisUi = new IbisUi();
            }

            this.ibisUi.IbisTelegramCreated += this.OnIbisUiIbisTelegramCreated;
            this.ibisUi.Show();
        }

        private void OnIbisUiIbisTelegramCreated(object sender, DataEventArgs args)
        {
            this.WriteData(args.Data, args.Data.Length);
        }
    }
}
