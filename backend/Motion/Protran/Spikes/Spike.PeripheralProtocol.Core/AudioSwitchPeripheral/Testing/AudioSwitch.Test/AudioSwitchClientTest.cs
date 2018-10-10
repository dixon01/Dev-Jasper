// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="AudioSwitchClientTest.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace AudioSwitch.Test
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Ports;
    using System.Linq;
    using System.Threading;

    using Luminator.PeripheralProtocol.Core;
    using Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral;
    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Models;
    using Luminator.PeripheralProtocol.Core.Types;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using NLog;

    /// <summary>The audio switch client test.</summary>
    [TestClass]
    [DeploymentItem(@"Config\medi.config")]
    [DeploymentItem(@"Config\" + AudioSwitchPeripheralConfig.DefaultAudioSwitchConfigFileName)]
    [DeploymentItem(@"Config\" + PeripheralAudioConfig.DefaultPeripheralAudioConfigXmlFile)]
    [DeploymentItem("../App.config")]
    public class AudioSwitchClientTest
    {
        #region Constants

        // set the comport to the true hardware
        /// <summary>The default audio switch com port.</summary>
        public const string DefaultAudioSwitchComPort = "COM3";

        private const string AudioSwitchConfigXmlFileName = AudioSwitchPeripheralConfig.DefaultAudioSwitchConfigFileName;

        #endregion

        #region Static Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static int DefaultTimeout = 1000;

        private static TestContext testContext;

        #endregion

        #region Public Properties

        /// <summary>Gets the audio switch client lock.</summary>
        public static object AudioSwitchClientLock { get; } = new object();

        #endregion

        #region Public Methods and Operators

        /// <summary>The class init.</summary>
        /// <param name="context">The context.</param>
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            testContext = context;

            if (Debugger.IsAttached)
            {
                DefaultTimeout = 10000;
            }

            Assert.IsTrue(File.Exists("App.config"));
            Assert.IsTrue(File.Exists("medi.config"));
            Assert.IsTrue(File.Exists("AudioSwitchConfig.xml"));
            Directory.CreateDirectory(@"C:\Temp");
            Debug.WriteLine("AudioSwitchClientTest.ClassInit()");
            Logger.Trace("AudioSwitchClientTest.ClassInit() - NLogger Output");

            var portNames = SerialPort.GetPortNames();
            Assert.IsTrue(portNames.Any(), "No Serial Ports Found for testing");
            Assert.IsTrue(portNames.Contains(DefaultAudioSwitchComPort), "No Serial Ports Found " + DefaultAudioSwitchComPort);

            // init the dictionary here so our debug trace output for the other test are smaller and easier to read
            var type = PeripheralAudioSwitchMessageType.Ack;
            type.FindPeripheralMessageClassType(PeripheralSystemMessageType.AudioGeneration3);
        }

        /// <summary>The audio switch serial client disable the status interval.</summary>
        [TestMethod]
        public void AudioSwitchSerialClientRS485_DisableAudioStatusInterval()
        {
            lock (AudioSwitchClientLock)
            {
                var config = ReadPeripheralAudioConfig();
                var manualResetEvent = new ManualResetEvent(false);
                var signaled = false;
                using (var client = new AudioSwitchSerialClient(config))
                {
                    client.PeripheralDataReceived += (sender, args) =>
                        {
                            var match = args.IsMessage(PeripheralAudioSwitchMessageType.AudioVersionResponse);
                            if (match)
                            {
                                Debug.WriteLine("Matching expected message found");
                                var baseMessage = args.GetPeripheralMessage<PeripheralAudioSwitchMessageType>();
                                Assert.IsNotNull(baseMessage);

                                Debug.WriteLine("Got Peripheral response " + baseMessage.Header.MessageType);

                                // we expect the version as a response                                
                                if (baseMessage.IsMessage(PeripheralAudioSwitchMessageType.AudioVersionResponse))
                                {
                                    manualResetEvent.Set();
                                }
                            }
                        };

                    // Enabling this will result in the Audio Mux sending audio status to us at the set refresh interval! Zero to disable
                    client.WriteAudioConfig(string.Empty, 0);
                    signaled = manualResetEvent.WaitOne(DefaultTimeout);
                }

                Assert.IsTrue(signaled);
            }
        }

        [TestMethod]
        public void AudioSwitchSerialClientRS485_RequestVersion()
        {
            lock (AudioSwitchClientLock)
            {
                var manualResetEvent = new ManualResetEvent(false);
                var signaled = false;
                using (var client = new AudioSwitchSerialClient(ReadPeripheralAudioConfig()))
                {
                    Assert.IsTrue(client.IsComPortOpen);
                    //client.PeripheralAudioSwitchDataReceived += (sender, args) =>
                    //    {                            
                    //        var isAudioVersionResponse = args.AudioSwitchMessage.IsMessage(PeripheralAudioSwitchMessageType.AudioVersionResponse);
                    //        var baseMessage = args.AudioSwitchMessage;
                    //        Assert.IsNotNull(baseMessage);
                    //        Debug.WriteLine("Got Peripheral response " + baseMessage.Header.MessageType);

                    //        // we expect the version as a response                            
                    //        if (baseMessage.IsMessage(PeripheralAudioSwitchMessageType.AudioVersionResponse))
                    //        {
                    //            manualResetEvent.Set();
                    //        }
                    //    };

                    client.PeripheralDataReceived += (sender, args) =>
                        {
                            var baseMessage = args.GetPeripheralMessage<PeripheralAudioSwitchMessageType>();
                            Assert.IsNotNull(baseMessage);
                            var isAck = baseMessage.Header.IsAck;
                            Debug.WriteLine("Got Peripheral response " + baseMessage.Header.MessageType);

                            // we expect the version as a response                            
                            if (baseMessage.IsMessage(PeripheralAudioSwitchMessageType.AudioVersionResponse))
                            {
                                manualResetEvent.Set();
                            }
                        };

                    var peripheralVersionsInfo = client.WriteVersionRequest();

                    signaled = manualResetEvent.WaitOne(Debugger.IsAttached  ? 60000 : DefaultTimeout);

                    Assert.IsNotNull(peripheralVersionsInfo, "Expected Version info returned");     
                    Assert.IsTrue(client.IsVersionInfoReceived, "IsVersionInfoReceived != true");

                    Assert.IsTrue(client.IsPeripheralConnected, "IsPeripheralConnected != true");
                }

                Assert.IsTrue(signaled);
            }
        }

        /// <summary>The audio switch serial client r s 485_ open close.</summary>
        /// <exception cref="ArgumentNullException"><paramref name="audioSwitchConfig" /> is <see langword="null" />.</exception>
        [TestMethod]
        public void AudioSwitchSerialClientRS485_OpenClose()
        {
            lock (AudioSwitchClientLock)
            {
                var client = new AudioSwitchSerialClient(ReadPeripheralAudioConfig());
                Assert.IsTrue(client.IsComPortOpen);
                Thread.Sleep(100);
                client.Close();
                Assert.IsFalse(client.IsComPortOpen);
            }
        }

        /// <summary>The audio switch serial client r s 485_ write audio config.</summary>
        [TestMethod]
        public void AudioSwitchSerialClientRS485_WriteAudioConfig()
        {
            lock (AudioSwitchClientLock)
            {
                var countdownEvent = new CountdownEvent(2);
                var signaled = false;

                // we expect to events or callbacks, Version Response and GPIO status
                using (var client = new AudioSwitchSerialClient(ReadPeripheralAudioConfig()))
                {
                    client.PeripheralDataReceived += (sender, args) =>
                        {
                            var baseMessage = args.GetPeripheralMessage<PeripheralAudioSwitchMessageType>();
                            Assert.IsNotNull(baseMessage);
                            Debug.WriteLine("Got Peripheral response " + baseMessage.Header.MessageType);

                            // we expect the version as a response
                            if (baseMessage.IsMessage(PeripheralAudioSwitchMessageType.AudioVersionResponse))
                            {
                                countdownEvent.Signal();
                            }

                            // Note The RX reply from our first WRite of Audio Config is the switch version info that we save off
                        };

                    client.GpioChanged += (sender, arg) =>
                        {
                            Debug.WriteLine("Got GpioChanged response ");
                            countdownEvent.Signal();
                        };

                    Assert.IsTrue(client.IsComPortOpen);
                    string defaultFile = PeripheralAudioConfig.DefaultPeripheralAudioConfigXmlFile;
                    Assert.IsTrue(File.Exists(defaultFile), "File not found File=" + defaultFile);
                    client.WriteAudioConfig();

                    signaled = countdownEvent.Wait(DefaultTimeout);

                    // this gets set automatically be the peripheral sending us the data any time we send it a AudioConfig
                    var peripheralVersionInfo = client.PeripheralVersionInfo;

                    Assert.IsNotNull(peripheralVersionInfo, "Version not set");
                    Debug.WriteLine(peripheralVersionInfo.ToString());

                    // expected true after we send the audio config and capture/save the version off
                    Assert.IsTrue(client.IsPeripheralConnected, "Not Connected");
                    Assert.AreEqual(0, client.State.StreamLength);
                    var buffer = client.State.MemoryStreamBytes;
                    Assert.IsNotNull(buffer);
                    Assert.AreEqual(0, buffer.Length);
                }

                Assert.IsTrue(signaled);
            }
        }

        /// <summary>The audio switch serial client rs485_ write audio config second version 2.</summary>
        [TestMethod]
        public void AudioSwitchSerialClientRS485_WriteAudioConfig2()
        {
            Assert.Inconclusive("TBD");
            lock (AudioSwitchClientLock)
            {
                var resetEvent = new ManualResetEvent(false);
                var gpioStatusReceivedEvent = new ManualResetEvent(false);

                var signaled = false;
                using (var client = new AudioSwitchSerialClient(ReadPeripheralAudioConfig()))
                {
                    client.PeripheralDataReceived += (sender, args) =>
                        {
                            var baseMessage = args.GetPeripheralMessage<PeripheralAudioSwitchMessageType>();
                            Assert.IsNotNull(baseMessage);
                            Debug.WriteLine("Got Peripheral response " + baseMessage.Header.MessageType);

                            // we expect the version as a response
                            
                            if (baseMessage.IsMessage(PeripheralAudioSwitchMessageType.AudioVersionResponse))
                            {
                                resetEvent.Set();
                            }

                            if (baseMessage.IsMessage(PeripheralAudioSwitchMessageType.GpioStatusResponse))
                            {
                                gpioStatusReceivedEvent.Set();
                            }
                        };

                    Assert.IsTrue(client.IsComPortOpen);
                    string defaultFile = PeripheralAudioConfig.DefaultPeripheralAudioConfigXmlFile;
                    Assert.IsTrue(File.Exists(defaultFile));
                    var peripheralAudioConfig = client.ReadPeripheralAudioConfigFile(defaultFile);
                    Assert.IsNotNull(peripheralAudioConfig);

                    client.WriteAudioConfig(peripheralAudioConfig);

                    signaled = resetEvent.WaitOne(DefaultTimeout);

                    // gpioStatusReceivedEvent.WaitOne(5000);
                    Assert.IsTrue(signaled);

                    // this gets set automatically be the peripheral sending us the data any time we send it a AudioConfig
                    var version = client.PeripheralVersionInfo;

                    Assert.IsNotNull(version, "Version not set");
                    Debug.WriteLine(version.ToString());

                    // after we write the config this should be true
                    Assert.IsTrue(client.IsPeripheralConnected);
                }
            }
        }

        /// <summary>The audio switch serial client to poll for audio status</summary>
        [TestMethod]
        public void AudioSwitchSerialClientRS485_WriteAudioStausPollRequest()
        {
            lock (AudioSwitchClientLock)
            {
                var resetEvent = new ManualResetEvent(false);
                var signaled = false;
                using (var client = new AudioSwitchSerialClient(ReadPeripheralAudioConfig()))
                {
                    client.SerialErrorReceived += (sender, args) => { Assert.Fail("Serial Error encountered " + args.EventType); };
                    client.PeripheralDataReceived += (sender, args) =>
                        {
                            var baseMessage = args.GetPeripheralMessage<PeripheralAudioSwitchMessageType>();
                            Assert.IsNotNull(baseMessage);

                            Debug.WriteLine("Got response " + baseMessage.Header.MessageType);
                            if (baseMessage.IsMessage(PeripheralAudioSwitchMessageType.AudioStatusResponse))
                            {
                                resetEvent.Set();
                            }
                        };
                    Assert.IsTrue(client.IsComPortOpen);

                    client.WriteAudioStausRequest();
                    signaled = resetEvent.WaitOne(Debugger.IsAttached ? 30000 : DefaultTimeout);
                    Assert.AreEqual(0, client.State.StreamLength, "Expected Empty buffer");
                    Thread.Sleep(10000);
                }

                Assert.IsTrue(signaled);
            }
        }

        /// <summary>The audio switch serial client r s 485_ write enable audio both speakers.</summary>
        [TestMethod]
        public void AudioSwitchSerialClientRS485_WriteEnableAudioBothSpeakers()
        {
            lock (AudioSwitchClientLock)
            {
                var resetEvent = new ManualResetEvent(false);
                var signaled = false;
                using (var client = new AudioSwitchSerialClient(ReadPeripheralAudioConfig()))
                {
                    client.PeripheralDataReceived += (sender, args) =>
                        {
                            // we expect an Ack back version as a response
                            var baseMessage = args.GetPeripheralMessage<PeripheralAudioSwitchMessageType>();
                            Assert.IsNotNull(baseMessage);
                            Debug.WriteLine("Got response " + baseMessage.Header.MessageType);
                            if (baseMessage.Header.IsAck)
                            {
                                resetEvent.Set();
                            }
                        };
                    Assert.IsTrue(client.IsComPortOpen);

                    // write and wait for ACK from audio hardware
                    client.WriteAudioEnabled(ActiveSpeakerZone.Both);
                    signaled = resetEvent.WaitOne(Debugger.IsAttached ? 30000 : DefaultTimeout);
                }

                Assert.IsTrue(signaled);
            }
        }

        /// <summary>The audio switch serial client r s 485_ write enable audio none.</summary>
        [TestMethod]
        public void AudioSwitchSerialClientRS485_WriteEnableAudioNone()
        {
            lock (AudioSwitchClientLock)
            {
                var resetEvent = new ManualResetEvent(false);
                var signaled = false;
                using (var client = new AudioSwitchSerialClient(ReadPeripheralAudioConfig()))
                {
                    client.PeripheralDataReceived += (sender, args) =>
                        {
                            // we expect an Ack back version as a response
                            //bool ackMessage = args.Message.IsAck;
                            var baseMessage = args.GetPeripheralMessage<PeripheralAudioSwitchMessageType>();
                            Assert.IsNotNull(baseMessage);
                            Debug.WriteLine("Got response " + baseMessage.Header.MessageType);                            
                            if (baseMessage.Header.IsAck)
                            {
                                resetEvent.Set();
                            }
                        };
                    Assert.IsTrue(client.IsComPortOpen);

                    // write and wait for ACK from audio hardware. This disables audio out on the lines
                    client.WriteAudioEnabled(ActiveSpeakerZone.None);
                    signaled = resetEvent.WaitOne(Debugger.IsAttached ? 30000 : DefaultTimeout);
                    Assert.AreEqual(0, client.State.StreamLength);
                }

                Assert.IsTrue(signaled);
            }
        }

        /// <summary>The audio switch serial client to poll for audio status</summary>
        [TestMethod]
        public void AudioSwitchSerialClientRS485_WriteMultipleAudioStausRequest()
        {
            lock (AudioSwitchClientLock)
            {
                var countdownEvent = new CountdownEvent(2);
                var signaled = false;
                using (var client = new AudioSwitchSerialClient(ReadPeripheralAudioConfig()))
                {
                    client.PeripheralDataReceived += (sender, args) =>
                        {
                            var baseMessage = args.GetPeripheralMessage<PeripheralAudioSwitchMessageType>();
                            Assert.IsNotNull(baseMessage);
                            Debug.WriteLine("*** UNIT TEST Got  Audio Switch response " + baseMessage.Header.MessageType);
                            if (baseMessage.IsMessage(PeripheralAudioSwitchMessageType.AudioStatusResponse) || baseMessage.Header.IsAck)
                            {
                                countdownEvent.Signal();
                            }
                        };

                    Assert.IsTrue(client.IsComPortOpen);

                    // write volumes and wait for ACK from hardware 
                    client.WriteSetVolume(1, 2);

                    client.WriteAudioStausRequest();

                    signaled = countdownEvent.Wait(Debugger.IsAttached ? 30000 : 5000);
                    Assert.AreEqual(0, client.State.StreamLength);
                }

                Assert.IsTrue(signaled);
            }
        }

        /// <summary>The audio switch serial client rs485_ write set volume.</summary>
        [TestMethod]
        public void AudioSwitchSerialClientRS485_WriteRequestVersion()
        {
            lock (AudioSwitchClientLock)
            {
                var resetEvent = new ManualResetEvent(false);
                var signaled = false;
                var oldVersionFound = false;
                using (var client = new AudioSwitchSerialClient(ReadPeripheralAudioConfig()))
                {
                    client.PeripheralDataReceived += (sender, args) =>
                        {
                            var baseMessage = args.GetPeripheralMessage<PeripheralAudioSwitchMessageType>();
                            Assert.IsNotNull(baseMessage);
                            Debug.WriteLine("Got response " + baseMessage.Header.MessageType);
                            
                            if (baseMessage.Header.IsAck || baseMessage.Header.IsNak)
                            {
                                // older audio hardware did not return the version info but instead an Ack
                                oldVersionFound = true;
                                resetEvent.Set();

                                // Note The older audio mux hardware never responds to the request for version
                            }
                            else if (baseMessage.IsMessage(PeripheralAudioSwitchMessageType.AudioVersionResponse))
                            {
                                resetEvent.Set();
                            }
                        };

                    Assert.IsTrue(client.IsComPortOpen);
                    client.WriteVersionRequest();
                    signaled = resetEvent.WaitOne(Debugger.IsAttached ? 30000 : DefaultTimeout);
                }

                if (oldVersionFound)
                {
                    Assert.Inconclusive("Older Audio hardware being testing and does not respond to this message");
                }

                if (!signaled)
                {
                    Assert.Inconclusive("Older Hardware does not respond");
                }

                Assert.IsTrue(signaled);
            }
        }

        /// <summary>The audio switch serial client r s 485_ write set volume.</summary>
        [TestMethod]
        public void AudioSwitchSerialClientRS485_WriteSetVolume()
        {
            lock (AudioSwitchClientLock)
            {
                var m = new ManualResetEvent(false);
                var signaled = false;
                using (var client = new AudioSwitchSerialClient(ReadPeripheralAudioConfig()))
                {
                    client.PeripheralDataReceived += (sender, args) =>
                        {
                            var baseMessage = args.GetPeripheralMessage<PeripheralAudioSwitchMessageType>();
                            Assert.IsNotNull(baseMessage);
                            Debug.WriteLine("Test Got response " + baseMessage.Header.MessageType);
                            if (baseMessage.Header.IsAck)
                            {
                                m.Set();
                            }
                        };
                    Assert.IsTrue(client.IsComPortOpen);
                    client.WriteSetVolume(50, 60); // made these are made up volume values

                    signaled = m.WaitOne(Debugger.IsAttached ? 30000 : DefaultTimeout);
                }

                Assert.IsTrue(signaled);
            }
        }

        /// <summary>The create_ peripheral gpio event arg.</summary>
        [TestMethod]
        public void Create_PeripheralGpioEventArg()
        {
            var peripheralAudioGpioStatus = new PeripheralAudioGpioStatus { ChangeMask = 0x3, GpioStatus = 0x3, RawPinStatus = 0x1 };
            var audioConfig = PeripheralAudioConfig.ReadPeripheralAudioConfig();
            Assert.IsNotNull(audioConfig);
            PeripheralGpioEventArg args = new PeripheralGpioEventArg(peripheralAudioGpioStatus, audioConfig);
            Assert.IsNotNull(args.GpioInfo);
            Assert.AreEqual(2, args.GpioInfo.Count);
            GpioInfo gpioInfo = args.GpioInfo.FirstOrDefault();
            Assert.IsNotNull(gpioInfo);
            Assert.AreEqual(PeripheralGpioType.Door1, gpioInfo.Gpio);
            Assert.AreEqual(true, gpioInfo.Active);

            gpioInfo = args.GpioInfo[1];
            Assert.AreEqual(PeripheralGpioType.Door2, gpioInfo.Gpio);
            Assert.AreEqual(true, gpioInfo.Active);
        }

        /*
         * 5.2.1	Length
        The packet length field indicates the octet length of the packet header plus the packet payload.  The checksum octet is not included in the length.  Packets with no payload will have a fixed length of 0x0006 (the length of the header).  The length field byte order is Most significant byte followed by Least significant byte.  The maximum packet length supported is 0x6FFF (28,671) octets.

         */

        /// <summary>The serial port write_ audio disabled.</summary>
        [TestMethod]
        public void SimpleSerialPortWrite_AudioDisabled()
        {
            bool signaled = false;
            int bytesRead = 0;
            lock (AudioSwitchClientLock)
            {
                // lock use of serial port one test at a time
                var config = ReadPeripheralAudioConfig();
                var serialPortSettings = config.SerialPortSettings;
                var serialPort = new SerialPort(
                    serialPortSettings.ComPort,
                    serialPortSettings.BaudRate,
                    serialPortSettings.Parity,
                    serialPortSettings.DataBits,
                    serialPortSettings.StopBits);
                serialPort.Open();
                var m = new ManualResetEvent(false);
                serialPort.DataReceived += (sender, args) =>
                    {
                        Debug.WriteLine("Data Received");
                        var buffer = new byte[1024];
                        bytesRead = serialPort.Read(buffer, 0, buffer.Length);
                        m.Set();
                    };

                var modelSize = PeripheralAudioEnable.Size;
                Debug.WriteLine("PeripheralAudioEnable.Size=" + PeripheralAudioEnable.Size);
                // The packet length field indicates the octet length of the packet header plus the packet payload.  The checksum octet is not included in the length.  

                byte MessageLen = (byte)(modelSize - 1);
                byte AudioOutputEnable = (byte)PeripheralAudioSwitchMessageType.AudioOutputEnable;
                const byte SystemMessageType = 0x7;
                const byte CheckSum = 0xF1;
                Debug.WriteLine("Using Header.Length = " + MessageLen);
                byte[] bytes = { Constants.PeripheralFramingByte, 0x0, MessageLen, 0xF0, 0x00, SystemMessageType, AudioOutputEnable, 0x00, CheckSum };
                Assert.AreEqual(9, bytes.Length, "incorrect number of bytes for message POLL");

                // HACK
                //      serialPort.Write(new[] { Constants.PeripheralFramingByte }, 0, 1); // HACK Write extra 0x7E frame byte twice
                // End HACK

                var s = bytes.Aggregate(string.Empty, (current, b) => current + string.Format("0x{0:X},", b));
                Debug.WriteLine("TX " + s);
                serialPort.Write(bytes, 0, bytes.Length);


                // wait for RX of data, we expect an ACK to be sent
                signaled = m.WaitOne(5000);
                serialPort.Close();
            }

            Assert.IsTrue(signaled, "No serial data received");
            Assert.AreEqual(PeripheralAck.Size + 1, bytesRead, "Ack No received"); // extra byte for checksum byte that is not part of Length!
        }

        #endregion

        #region Methods

        private static PeripheralConfig ReadPeripheralAudioConfig(string comport = DefaultAudioSwitchComPort)
        {
            var config = AudioSwitchPeripheralConfig.ReadAudioSwitchConfig(AudioSwitchConfigXmlFileName);
            Assert.IsNotNull(config);
            config.SerialPortSettings.ComPort = comport;

            return config;
        }

        #endregion
    }
}