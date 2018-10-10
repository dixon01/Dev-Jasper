// LightSensorTestForm
// LightSensorTestor
// $Rev::                                   
// 

namespace LightSensorTestor
{
    using System;
    using System.Diagnostics;
    using System.IO.Ports;
    using System.Windows.Forms;

    /// <summary>The light sensor test form.</summary>
    public partial class LightSensorTestForm : Form
    {
        /// <summary>The dimmer address.</summary>
        private const int DimmerAddress = 0x0;

        /// <summary>The peripheral start of message.</summary>
        private const byte PeripheralStartOfMessage = 0x7E;

        /// <summary>Initializes a new instance of the <see cref="LightSensorTestForm" /> class.</summary>
        public LightSensorTestForm()
        {
            this.InitializeComponent();
        }

        /// <summary>The byte array to string.</summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>The <see cref="string" />.</returns>
        public static string ByteArrayToString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", ",");
        }

        /// <summary>Get Check sum.</summary>
        /// <param name="array">The array.</param>
        /// <returns>The <see cref="byte" />.</returns>
        public static byte CheckSum(byte[] array)
        {
            byte sum = 0;
            if (array != null)
            {
                for (var i = 0; i < array.Length - 1; i++)
                {
                    sum += array[i];
                }
            }

            // Two's compliment:
            sum = (byte)(~sum + 1);
            return sum;
        }

        public char hex(int n)
        {
            return (char)(n < 10 ? (int)'0' + n : (int)'A' + n - 10);
        }

        private void brightness_Scroll(object sender, ScrollEventArgs e)
        {
            if (this.brightness.Value > 255)
            {
                this.brightness.Value = 255;
            }

            this.brightnessText.Text = this.brightness.Value.ToString();
        }

        private string GetSelectedComPort()
        {
            string portName;
            if (this.com1.Checked)
            {
                portName = "COM1";
            }
            else
            {
                if (this.com2.Checked)
                {
                    portName = "COM2"; // Default
                }
                else
                {
                    if (this.com3.Checked)
                    {
                        portName = "COM3";
                    }
                    else
                    {
                        portName = this.com4.Checked ? "COM4" : "COM10";
                    }
                }
            }

            return portName;
        }

        private SerialPort OpenSerialPort(string portName, int baudRate = 9600, int rxTimeout = 1000)
        {
            var com = portName == "COM10" ? new SerialPort(portName, 115200, Parity.None, 8) : new SerialPort(portName, baudRate, Parity.None, 8);

            try
            {
                com.DiscardNull = false;
                com.Handshake = Handshake.None;
                com.StopBits = StopBits.One;
                com.ReadTimeout = rxTimeout;
                com.WriteTimeout = 3000;
                com.DataReceived += (sender, args) => { Debug.WriteLine("Data Received"); };
                com.Open();
            }
            catch (Exception e)
            {
                this.response.Text = e.ToString();
                return null;
            }

            return com;
        }

        private void PollRequestClick(object sender, EventArgs e)
        {
            byte[] packet =
                {
                    PeripheralStartOfMessage, // Start of message
                    0x00, 0x06, // Length of message
                    0x00, 0x00, // Address
                    0x06, // System ID
                    0x01, // Message ID
                    0x00 // Checksum
                };
            this.SendCommand(packet);
        }

        private void QueryRequestClick(object sender, EventArgs e)
        {
            byte[] packet =
                {
                    PeripheralStartOfMessage, // Start of message
                    0x06, 0x00, // Length of message
                    0x00, 0x00, // Address
                    0x06, // System ID
                    0x15, // Message ID
                    0xDF // cecksum
                };

            this.SendCommand(packet);
        }

        private void ReadResponse(SerialPort serialPort)
        {
            int c;
            var text = string.Empty;

            // Read to the start of message byte.
            do
            {
                c = serialPort.ReadByte();
            }
            while (c != PeripheralStartOfMessage);

            // Read packet length. Note this will be in Network byte order, flip to Host byte order
            // we will ignore the Address field      
            byte len1 = (byte)serialPort.ReadByte();
            byte len2 = (byte)serialPort.ReadByte();
            var length = BitConverter.ToUInt16(new[] { len1, len2 }, 0);
            var len = length.NetworkToHostByteOrder() - 2;  // less two bytes for the length
            text = string.Format("0x7E, Len=0x{0:X}, ", len);

            // Read the remaining packet data. The Length and Address are in Network byte order so Order to Host
            while (len > 0)
            {
                len--;
                c = serialPort.ReadByte();
                text = text + this.hex((c >> 4) & 0xF) + this.hex(c & 0xF) + ' ';
            }

            // Read packet checksum. We will not valid this value
            byte checksum = (byte)serialPort.ReadByte();
            text += string.Format(" Checksum=0x{0:X}", checksum);

            // Put the result into the response box.
            this.response.Text = text;
        }

        private void SendCommand(byte[] buffer)
        {
            /*
            int  i;
            int  sum;

            sum       = what.Length-1;
            what[sum] = 0;
            for (i = 1; i < sum; i++) what[sum] = (byte)(what[sum]-what[i]);
            */
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                var portName = this.GetSelectedComPort();

                var serialPort = this.OpenSerialPort(portName);
                if (serialPort != null)
                    try
                    {
                        // first four bytes should be in network byte order
                        buffer = this.UpdateChecksum(buffer);

                        this.response.Text = string.Format("Writing... {0}\n", ByteArrayToString(buffer));
                        serialPort.Write(buffer, 0, buffer.Length);
                        this.ReadResponse(serialPort);
                    }
                    catch (TimeoutException timeoutException)
                    {
                        this.response.Text = timeoutException.Message;
                    }
                    catch (Exception e)
                    {
                        this.response.Text = e.ToString();
                    }
                    finally
                    {
                        serialPort.Close();
                    }
            }
            finally
            {
                // Set cursor as default arrow
                Cursor.Current = Cursors.Default;
            }
        }

        private void SetBrightnessRequestClick(object sender, EventArgs e)
        {
            // byte[]  packet = { 's', 'e', 't', 'B', 'r', 'i', 'g', 'h', 't', 'n', 'e', 's', 's' };
            byte[] packet =
                {
                    PeripheralStartOfMessage, // Start of message
                    0x00, 0x07, // Length of message
                    0x00, 0x00, // Address
                    0x06, // System ID
                    0x12, // Message ID
                    0x00, // Brightness setting
                    0x00 // Checksum
                };

            packet[7] = (byte)this.brightness.Value;

            this.SendCommand(packet);
        }

        private void SetModeClick(object sender, EventArgs e)
        {
            // byte[]  packet = { 's', 'e', 't', 'M', 'o', 'd', 'e' };
            byte[] packet =
                {
                    PeripheralStartOfMessage, // Start of message
                    0x00, 0x07, // Length of message
                    0x00, 0x00, // Address
                    0x06, // System ID
                    0x14, // Message ID
                    0x00, // Mode setting
                    0x00 // Checksum
                };

            packet[7] = (byte)(this.normal.Checked ? 1 : 0);

            this.SendCommand(packet);
        }

        private void SetMonitorPowerClick(object sender, EventArgs e)
        {
            // byte[]  packet = { 's', 'e', 't', 'M', 'o', 'n', 'i', 't', 'o', 'r', 'P', 'o', 'w', 'e', 'r' };
            byte[] packet =
                {
                    PeripheralStartOfMessage, // Start of message
                    0x00, 0x07, // Length of message
                    0x00, 0x00, // Address
                    0x06, // System ID
                    0x13, // Message ID
                    0x00, // Power setting
                    0x00 // Checksum
                };

            packet[7] = (byte)(this.power.Checked ? 1 : 0);

            this.SendCommand(packet);
        }

        private void SetSensorScaleClick(object sender, EventArgs e)
        {
            // byte[]  packet = { 's', 'e', 't', 'S', 'e', 'n', 's', 'o', 'r', 'S', 'c', 'a', 'l', 'e' };
            byte[] packet =
                {
                    PeripheralStartOfMessage, // Start of message
                    0x00, 0x07, // Length of message
                    0x00, 0x00, // Address
                    0x06, // System ID
                    0x11, // Message ID
                    0x00, // Scale setting
                    0x00 // Checksum
                };

            if (this.scale0.Checked)
            {
                packet[7] = 0;
            }
            else
            {
                if (this.scale1.Checked)
                {
                    packet[7] = 1;
                }
                else
                {
                    if (this.scale2.Checked) packet[7] = 2;
                    else packet[7] = 3;
                }
            }

            this.SendCommand(packet);
        }

        private byte[] UpdateChecksum(byte[] bytes)
        {
            bytes[bytes.Length - 1] = CheckSum(bytes);
            return bytes;
        }

        private void VersionRequestClick(object sender, EventArgs e)
        {
            // byte[]  packet = { 'v', 'e', 'r', 's', 'i', 'o', 'n', 'R', 'e', 'q', 'u', 'e', 's', 't' };
            byte[] packet =
                {
                    PeripheralStartOfMessage, // Start of message
                    0x00, 0x06, // Length of message
                    0x00, 0x00, // Address
                    0x06, // System ID
                    0x10, // Message ID
                    0x00 // Checksum
                };

            this.SendCommand(packet);
        }
    }
}