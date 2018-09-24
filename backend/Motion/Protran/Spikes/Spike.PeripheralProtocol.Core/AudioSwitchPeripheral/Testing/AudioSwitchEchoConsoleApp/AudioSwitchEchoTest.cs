// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="AudioSwitchEchoTest.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace AudioSwitchEchoConsoleApp
{
    using System;
    using System.IO;
    using System.IO.Ports;
    using System.Linq;
    using System.Runtime.Remoting.Messaging;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Luminator.PeripheralProtocol.Core;
    using Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral;
    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Models;
    using Luminator.PeripheralProtocol.Core.Types;

    /// <summary>The audio switch echo.</summary>
    public class AudioSwitchEchoTest
    {
        #region Fields

        private readonly ManualResetEvent runningEvent = new ManualResetEvent(false);

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="AudioSwitchEchoTest"/> class.</summary>
        /// <param name="comPort">The com port.</param>
        /// <param name="baudRate">The baud rate.</param>
        public AudioSwitchEchoTest(string comPort, int baudRate)
        {
            this.ComPort = comPort;
            this.BaudRate = baudRate;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the baud rate.</summary>
        public int BaudRate { get; set; }

        /// <summary>Gets or sets the com port.</summary>
        public string ComPort { get; set; }

        /// <summary>Gets or sets a value indicating whether replay with ack always.</summary>
        public bool ReplayWithAck { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The open serial port.</summary>
        /// <param name="comPort">The com Port.</param>
        /// <param name="baudRate">The baud Rate.</param>
        /// <param name="dataBits">The data Bits.</param>
        /// <param name="parity">The parity.</param>
        /// <param name="stopBits">The stop Bits.</param>
        /// <param name="bufferSize">The buffer Size.</param>
        /// <exception cref="IOException">The specified port could not be found or opened.</exception>
        /// <returns>The <see cref="SerialPort"/>.</returns>
        public static SerialPort OpenSerialPort(
            string comPort, 
            int baudRate = SerialPortSettings.DefaultBaudRate, 
            int dataBits = 8, 
            Parity parity = Parity.None, 
            StopBits stopBits = StopBits.One, 
            int bufferSize = 4096)
        {
            var port = new SerialPort(comPort, baudRate, parity, dataBits, stopBits)
                           {
                               Encoding = Encoding.UTF8, 
                               ReadBufferSize = bufferSize, 
                               WriteBufferSize = bufferSize
                           };
            port.Open();
            return port;
        }

        /// <summary>The run.</summary>
        public void Run()
        {
            this.runningEvent.Reset();
            Task.Factory.StartNew(
                () =>
                    {
                        SerialPort serialPort = null;
                        try
                        {
                            var buffer = new byte[4096];
                            var bufferStream = new MemoryStream(buffer.Length);
                            Console.WriteLine("Opening Serial " + this.ComPort);
                            serialPort = OpenSerialPort(this.ComPort, this.BaudRate);

                            int bufferPosition = 0;

                            if (serialPort.IsOpen)
                            {
                                Console.WriteLine("Ready to receive Peripheral test messages on Serial " + this.ComPort);
                                serialPort.DataReceived += (sender, args) =>
                                    {
                                        if (args.EventType == SerialData.Chars)
                                        {
                                            Console.WriteLine(
                                                "\r\n{0} - Ready waiting for messages on {1}", 
                                                DateTime.Now.ToShortTimeString(), 
                                                serialPort.PortName);
                                            int length = buffer.Length - bufferPosition;
                                            if (length >= buffer.Length)
                                            {
                                                bufferPosition = 0;
                                                length = buffer.Length;
                                            }
                                            Console.WriteLine(" SerialPort.Read Position=" + bufferPosition + " Length="+length);
                                            int bytesRead = serialPort.Read(buffer, bufferPosition, length);
                                            if (bytesRead > 0)
                                            {
                                                var receivedBytes = buffer.Take(bytesRead).ToArray();                                
                                                Console.WriteLine("Received Bytes " + receivedBytes.Length + " finding message type...");

                                                // try to determine from the buffer what message type this is
                                                var model = this.FindAudioSwtichMessage(receivedBytes);
                                                if (model == null)
                                                {
                                                    Console.WriteLine("! Message Not Found in buffer");
                                                }

                                                var messageTypeFromBytes = model?.Header.MessageType;
                                                bool validMessage = false;
                                                if (messageTypeFromBytes != null && model != null)
                                                {
                                                    var bytesInBuffer = receivedBytes.Length;
                                                    if (bytesInBuffer >= model.Header.Length)
                                                    {
                                                        Console.WriteLine("Buffer Count = " + bytesInBuffer);
                                                        Console.WriteLine("Success - RX Peripheral Message Type " + messageTypeFromBytes);
                                                        validMessage = true;
                                                        bufferPosition = 0;
                                                    }
                                                }

                                                if (this.ReplayWithAck)
                                                {
                                                    // did we get from them the full message ?
                                                    if (validMessage)
                                                    {
                                                        var bytes = this.GetAckBytes();
                                                        Console.WriteLine("TX Peripheral Ack Message...");
                                                        serialPort.Write(bytes, 0, bytes.Length);
                                                    }
                                                    else
                                                    {
                                                        bufferPosition = bytesRead;
                                                        Console.WriteLine(" ! OOPs waiting for full message from sender. No Reply");
                                                    }
                                                }
                                                else
                                                {
                                                    // write back what we read as an echo test
                                                    serialPort.Write(receivedBytes, 0, receivedBytes.Length);
                                                    Console.WriteLine("Echoed {0} bytes to {1}", receivedBytes.Length, serialPort.PortName);
                                                }
                                            }
                                        }
                                    };
                                this.runningEvent.WaitOne();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Exception Error ! " + ex.Message);
                            Console.WriteLine("Enter Key to Continue");
                            Console.ReadLine();
                        }
                        finally
                        {
                            if (serialPort != null && serialPort.IsOpen)
                            {
                                Console.WriteLine("Closing " + serialPort.PortName);
                                serialPort.Close();
                            }

                            Console.WriteLine("Exited");
                        }
                    });
        }

        /// <summary>The stop.</summary>
        public void Stop()
        {
            this.runningEvent.Set();
            Thread.Sleep(500);
        }

        #endregion

        #region Methods

        private IPeripheralBaseMessage FindAudioSwtichMessage(byte[] bytes)
        {
            try
            {
                var model = AudioSwitchHandler.FindPeripheralMessageType(bytes);
                return model as IPeripheralBaseMessage;
            }
            catch (ArgumentException exception)
            {
                Console.WriteLine(exception.Message);
                return null;
            }
        }

        private byte[] GetAckBytes()
        {
            var bytes = new PeripheralAudioSwitchAck().ToBytes();
            var writeBytes = AudioSwitchHandler.GetPeripheralBytesWriteBuffer(bytes);
            return writeBytes;
        }


        #endregion
    }
}