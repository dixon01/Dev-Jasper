using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lightSensor
{
    public partial class Form1 : Form
    {
        private static SerialPort  com;

        public Form1 ()
        {
            InitializeComponent ();
        }

        private bool openCOM ()
        {
            string  portName;

            if (com1.Checked)
            {
                portName = "COM1";
            }
            else
            {
                if (com2.Checked)
                {
                    portName = "COM2";
                }
                else
                {
                    if (com3.Checked)
                    {
                        portName = "COM3";
                    }
                    else
                    {
                        if (com4.Checked)
                        {
                            portName = "COM4";
                        }
                        else
                        {
                            portName = "COM10";
                        }
                    }
                }
            }

            if (portName == "COM10")
            {
                com = new SerialPort (portName, 115200, Parity.None, 8);
            }
            else
            {
                com = new SerialPort (portName, 9600, Parity.None, 8);
            }

            com.DiscardNull  = false;
            com.Handshake    = Handshake.None;
            com.StopBits     = StopBits.One;
            com.ReadTimeout  = 1000;
            com.WriteTimeout = 1000;

            try
            {
                com.Open ();
            }
            catch (Exception e)
            {
                response.Text = e.ToString ();
                return false;
            }

            return true;
        }

        public char hex (int n)
        {
            return (char) ((n < 10) ? ((int)'0'+n) : ((int)'A'+n-10));
        }
        private void responseCOM ()
        {
            int     c;
            int     len;
            string  text;

            text = "";

            /* Read to the start of message byte.
             */
            do
            {
                c = com.ReadByte ();
            }
            while (c != 0x7E);

            /* Read packet length.
             */
            len = com.ReadByte ()<<8+com.ReadByte ()-2;

            /* Read packet data.
             */
            while (len > 0)
            {
                len--;

                c = com.ReadByte ();

                text = text+hex ((c>>4)&0xF)+hex (c&0xF)+' ';
            }

            /* Read packet checksum.
             */
            c = com.ReadByte ();

            /* Put the result into the response box.
             */
            response.Text = text;
        }

        private void ioCom (char[] what)
        {
/*
            int  i;
            int  sum;

            sum       = what.Length-1;
            what[sum] = 0;
            for (i = 1; i < sum; i++) what[sum] = (byte)(what[sum]-what[i]);
 */

            if (openCOM ())
            {
                try
                {
                    com.Write (what, 0, what.Length);
                    responseCOM ();
                }
                catch (Exception e)
                {
                    response.Text = e.ToString ();
                }

                com.Close ();
            }
        }

        private void pollRequest_Click (object sender, EventArgs e)
        {
            char[]  packet = { 'p', 'o', 'l', 'l', 'R', 'e', 'q', 'u', 'e', 's', 't' };

            /*
                        byte[]  packet =
                        {
                            0x7E,       // Start of message
                            0x00, 0x06, // Length of message
                            0x00, 0x00, // Address
                            0x06,       // System ID
                            0x01,       // Message ID
                            0x00        // Checksum
                        };

             */
            ioCom (packet);
        }

        private void versionRequest_Click (object sender, EventArgs e)
        {
            char[]  packet = { 'v', 'e', 'r', 's', 'i', 'o', 'n', 'R', 'e', 'q', 'u', 'e', 's', 't' };

            /*
                        byte[]  packet =
                        {
                            0x7E,       // Start of message
                            0x00, 0x06, // Length of message
                            0x00, 0x00, // Address
                            0x06,       // System ID
                            0x10,       // Message ID
                            0x00        // Checksum
                        };

             */
            ioCom (packet);
        }

        private void setSensorScale_Click (object sender, EventArgs e)
        {
            char[]  packet = { 's', 'e', 't', 'S', 'e', 'n', 's', 'o', 'r', 'S', 'c', 'a', 'l', 'e' };

            /*
                        byte[]  packet =
                        {
                            0x7E,       // Start of message
                            0x00, 0x07, // Length of message
                            0x00, 0x00, // Address
                            0x06,       // System ID
                            0x11,       // Message ID
                            0x00,       // Scale setting
                            0x00        // Checksum
                        };

                        if (scale0.Checked)
                        {
                            packet[7] = 0;
                        }
                        else
                        {
                            if (scale1.Checked)
                            {
                                packet[7] = 1;
                            }
                            else
                            {
                                if (scale2.Checked)
                                {
                                    packet[7] = 2;
                                }
                                else
                                {
                                    packet[7] = 3;
                                }
                            }
                        }

             */
            ioCom (packet);
        }

        private void setBrightnessRequest_Click (object sender, EventArgs e)
        {
            char[]  packet = { 's', 'e', 't', 'B', 'r', 'i', 'g', 'h', 't', 'n', 'e', 's', 's' };

            /*
                        byte[]  packet =
                        {
                            0x7E,       // Start of message
                            0x00, 0x07, // Length of message
                            0x00, 0x00, // Address
                            0x06,       // System ID
                            0x12,       // Message ID
                            0x00,       // Brightness setting
                            0x00        // Checksum
                        };

                        packet[7] = (byte)brightness.Value;

             */
            ioCom (packet);
        }

        private void setMonitorPower_Click (object sender, EventArgs e)
        {
            char[]  packet = { 's', 'e', 't', 'M', 'o', 'n', 'i', 't', 'o', 'r', 'P', 'o', 'w', 'e', 'r' };

            /*
                        byte[]  packet =
                        {
                            0x7E,       // Start of message
                            0x00, 0x07, // Length of message
                            0x00, 0x00, // Address
                            0x06,       // System ID
                            0x13,       // Message ID
                            0x00,       // Power setting
                            0x00        // Checksum
                        };

                        packet[7] = (byte)((power.Checked) ? 1 : 0);

             */
            ioCom (packet);
        }

        private void setMode_Click (object sender, EventArgs e)
        {
            char[]  packet = { 's', 'e', 't', 'M', 'o', 'd', 'e' };

            /*
                        byte[]  packet =
                        {
                            0x7E,       // Start of message
                            0x00, 0x07, // Length of message
                            0x00, 0x00, // Address
                            0x06,       // System ID
                            0x14,       // Message ID
                            0x00,       // Mode setting
                            0x00        // Checksum
                        };

                        packet[7] = (byte)((normal.Checked) ? 1 : 0);

             */
            ioCom (packet);
        }

        private void queryRequest_Click (object sender, EventArgs e)
        {
            char[]  packet = { 'q', 'u', 'e', 'r', 'y', 'R', 'e', 'q', 'u', 'e', 's', 't' };

            /*
                        byte[]  packet =
                        {
                            0x7E,       // Start of message
                            0x00, 0x06, // Length of message
                            0x00, 0x00, // Address
                            0x06,       // System ID
                            0x15,       // Message ID
                            0x00        // Checksum
                        };

             */
            ioCom (packet);
        }

        private void brightness_Scroll (object sender, ScrollEventArgs e)
        {
            if (brightness.Value > 255) brightness.Value = 255;

            brightnessText.Text = brightness.Value.ToString ();
        }
    }
}
