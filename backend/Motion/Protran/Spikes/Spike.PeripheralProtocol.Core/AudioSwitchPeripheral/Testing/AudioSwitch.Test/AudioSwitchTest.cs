// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="AudioSwitchTest.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace AudioSwitch.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Ports;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Messages;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Motion.Infomedia.Entities.Messages;

    using Luminator.PeripheralProtocol.Core;
    using Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral;
    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Models;
    using Luminator.PeripheralProtocol.Core.Types;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using NLog;

    /// <summary>The audio switch test.</summary>
    [TestClass]
    [DeploymentItem(@"Config\medi.config")]
    [DeploymentItem(@"Config\" + AudioSwitchPeripheralConfig.DefaultAudioSwitchConfigFileName)]
    [DeploymentItem(@"Config\" + PeripheralAudioConfig.DefaultPeripheralAudioConfigXmlFile)]
    [DeploymentItem("../App.config")]

    // [DeploymentItem(@"Config\dictionary.xml")] if needed
    public class AudioSwitchTest
    {
        #region Constants

        private const string AudioSwitchConfigXmlFileName = AudioSwitchPeripheralConfig.DefaultAudioSwitchConfigFileName;

        #endregion

        #region Static Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static FileConfigurator fileConfigurator;

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

            // Note Some unit test require virtual Serial Ports setup to emulate hardware ie COM 1 <==> COM2
            // Third party tools can setup Window to doe this and is required to execute test fully!
            Assert.IsTrue(File.Exists("App.config"));

            // var nlogTraceListner = new NLogTraceListener();
            // nlogTraceListner.TraceOutputOptions = TraceOptions.Timestamp | TraceOptions.ThreadId;
            // Trace.Listeners.Add(nlogTraceListner);
            // Debug.Listeners.Add(nlogTraceListner);
            Directory.CreateDirectory(@"C:\Temp");
            Debug.WriteLine("AudioSwitchTest.ClassInit()");
            Logger.Trace("AudioSwitchTest.ClassInit() - NLogger Output");

            var portNames = SerialPort.GetPortNames();
            Assert.IsTrue(portNames.Any(), "No Serial Ports Found for testing");
        }

        /// <summary>The get bytes.</summary>
        /// <param name="str">The str.</param>
        /// <param name="expectedSize">The expected size.</param>
        /// <returns>The <see cref="byte[]"/>.</returns>
        public static byte[] GetBytes(string str, int expectedSize = 0)
        {
            int len = Math.Min(str?.Length ?? 0, expectedSize);
            if (len == 0)
            {
                len = expectedSize;
            }

            var bytes = Encoding.ASCII.GetBytes(str.ToCharArray(), 0, len);
            if (bytes.Length < expectedSize)
            {
                Array.Resize(ref bytes, expectedSize);
            }

            return bytes;
        }

        /// <summary>The serialize to xml.</summary>
        /// <param name="model">The model.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="string"/>.</returns>
        public static string SerializeToXml<T>(T model = default(T))
        {
            try
            {
                if (model == null)
                {
                    model = (T)Activator.CreateInstance(typeof(T));
                }

                var s = new StringBuilder();

                using (var memoryStream = new MemoryStream())
                {
                    var xs = new XmlSerializer(typeof(T));
                    var xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                    xs.Serialize(xmlTextWriter, model);
                    return Encoding.UTF8.GetString(memoryStream.ToArray());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        /// <summary>The create audio switch client.</summary>
        [TestMethod]
        public void AudioSwitchSerialClient_Construct_From_AudioSwitchConfigXmlFileName()
        {
            lock (AudioSwitchClientLock)
            {
                var config = AudioSwitchPeripheralConfig.ReadAudioSwitchConfig(AudioSwitchConfigXmlFileName);
                var expectedPortName = config.SerialPortSettings.ComPort;
                //var port = SerialPort.GetPortNames().First();
                //var expectedPortName = port;
                // config.SerialPortSettings.ComPort = port;

                using (var client = new AudioSwitchSerialClient(config))
                {
                    Assert.IsTrue(client.IsComPortOpen, "Comport Not Opened");

                    // we have no comport defined in the text XML so don't expect open to pass
                    Assert.IsNotNull(client.PeripheralHandler);
                    Assert.AreEqual(expectedPortName, client.PortName);
                }
            }
        }

        /// <summary>The create audio switch client.</summary>
        [TestMethod]
        public void AudioSwitchSerialClient_Construct2()
        {
            lock (AudioSwitchClientLock)
            {
                // use the last port found at random
                // If the port is in use by some other application this test will fail
                var port = SerialPort.GetPortNames().Last();
                var s = new SerialPortSettings(port);
                try
                {
                    using (var client = new AudioSwitchSerialClient(s))
                    {
                        Assert.IsTrue(client.IsComPortOpen); // we have comport
                        Assert.IsNotNull(client.PeripheralHandler);
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    Assert.Inconclusive("Port in use " + ex.Message);
                }
            }
        }

        /// <summary>The create audio switch client expected exception.</summary>
        /// <exception cref="ArgumentNullException"><paramref name="" /> is <see langword="null" />.</exception>
        [TestMethod]
        [ExpectedException(typeof(IOException))]
        public void AudioSwitchSerialClient_ExpectedIOException()
        {
            lock (AudioSwitchClientLock)
            {
                var config = AudioSwitchPeripheralConfig.ReadAudioSwitchConfig(AudioSwitchConfigXmlFileName);
                config.SerialPortSettings.ComPort = "COM123";
                Debug.WriteLine("Opening Serial Port " + config.SerialPortSettings.ComPort);
                using (var client = new AudioSwitchSerialClient(config))
                {
                    Assert.IsFalse(client.IsComPortOpen);
                }
            }
        }
     

        /// <summary>The audio switch serial client_ set volumes.</summary>
        [TestMethod]
        public void AudioSwitchSerialClient_SetVolumes()
        {
            lock (AudioSwitchClientLock)
            {
                var responseBytes = new PeripheralAudioSwitchAck().ToBytes();
                var model = new PeripheralSetVolume(10, 20);
                var sourceBytes = model.ToBytes();
                var signaled = AudioSwitchSerialClientSerialTest<PeripheralSetVolume>(sourceBytes, responseBytes);
                Assert.IsTrue(signaled);
            }
        }

        /// <summary>The create audio switch client write peripheral audio config.</summary>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        [TestMethod]
        public void AudioSwitchSerialClient_WritePeripheralAudioConfig()
        {
            lock (AudioSwitchClientLock)
            {
                var config = AudioSwitchPeripheralConfig.ReadAudioSwitchConfig(AudioSwitchConfigXmlFileName);
                config.SerialPortSettings.ComPort = "COM1";
                AssertNoComport(config.SerialPortSettings.ComPort);
                using (var client = new AudioSwitchSerialClient(config))
                {
                    Assert.IsTrue(client.IsComPortOpen);
                    var peripheralAudioConfig = new PeripheralAudioConfig { AudioStatusDelay = 1000 };

                    // refresh audio status rate
                    int bytesWritten = client.Write(peripheralAudioConfig);
                    var totalFrameBytes = client.PeripheralFramingBytesCount;
                    Assert.AreEqual(PeripheralAudioConfig.Size + totalFrameBytes, bytesWritten); // Plus framing byte
                }
            }
        }

        /// <summary>The audio switch serial client_ write peripheral audio enable.</summary>
        [TestMethod]
        public void AudioSwitchSerialClient_WritePeripheralAudioEnable()
        {
            lock (AudioSwitchClientLock)
            {
                var model = new PeripheralAudioEnable(ActiveSpeakerZone.Both);
                var sourceBytes = model.ToBytes();
                var responseBytes = new PeripheralAudioSwitchAck().ToBytes();
                var signaled = AudioSwitchSerialClientSerialTest<PeripheralAudioEnable>(sourceBytes, responseBytes);
                Assert.IsTrue(signaled);
            }
        }

        /// <summary>The audio switch state_ multiple message test.</summary>
        /// <exception cref="ArgumentException">Condition.</exception>
        [TestMethod]
        public void AudioSwitchState_MultipleMessageTest()
        {
            var state = new PeripheralState<PeripheralAudioSwitchMessageType>(CreateAudioSwitchPeripheralContext());

            var msg1 = new PeripheralAudioGpioStatus().ToBytes();
            var msg2 = new PeripheralAudioStatus().ToBytes();
            var msg3 = new PeripheralAudioVersions().ToBytes();

            state.AppendStream(msg1);
            state.AppendStream(msg2);
            state.AppendStream(msg3);
            var totalBytesInStream = state.StreamLength;
            Debug.WriteLine("Total bytes in Stream = " + totalBytesInStream);

            Assert.AreEqual(61, state.StreamLength);
            state.EmptyStream();
            Assert.AreEqual(0, state.StreamLength);

            state.AppendStream(msg1);
            state.AppendStream(msg2);
            state.AppendStream(msg3);
            Assert.AreEqual(61, state.StreamLength);

            state.RemoveBytesFromStream(msg1.Length);
            var bytes = state.MemoryStreamBytes;
            Assert.AreEqual(48, bytes.Length);

            var audioStatusModel = bytes.FromBytes<PeripheralAudioGpioStatus>();
            Assert.IsNotNull(audioStatusModel);
        }

        static PeripheralState<PeripheralAudioSwitchMessageType> CreateState()
        {
            return new PeripheralState<PeripheralAudioSwitchMessageType>(CreateAudioSwitchPeripheralContext());
        }

        /// <summary>The audio switch state_ remove gpio status messages.</summary>
        [TestMethod]
        public void AudioSwitchState_RemoveGpioStatusMessages()
        {
            /* The packet length field indicates the octet length of the packet header plus the packet payload.  The checksum octet is not included in the length.
             *   Packets with no payload will have a fixed length of 0x0006 (the length of the header).  
             * */
            var state = CreateState();
            var peripheralAudioGpioStatus = new PeripheralAudioGpioStatus();
            peripheralAudioGpioStatus.Header.Length--; // less one byte for checksum to emulate how we will RX data from hardware
            var msg1 = peripheralAudioGpioStatus.ToBytes();
            Assert.AreEqual(PeripheralAudioGpioStatus.Size - 1, peripheralAudioGpioStatus.Header.Length);

            var frameByte = new[] { Constants.PeripheralFramingByte };
            state.AppendStream(frameByte);
            state.AppendStream(msg1);
            var peripheralContext = CreateAudioSwitchPeripheralContext(string.Empty); // use memory stream

            lock (AudioSwitchClientLock)
            {
                using (var client = new AudioSwitchSerialClient(peripheralContext.Config))
                {
                    // change this since we wrote our bytes normally and on reading we don't want the byte order changed on the header
                    client.PeripheralHandler.IsHeaderNetworkByteOrder = false;
                    var message1 = client.RemoveNextMessage(state);
                    if (message1 is PeripheralAudioGpioStatus)
                    {
                        Debug.WriteLine("Remove Message Success for PeripheralAudioGpioStatus");
                    }
                    else
                    {
                        Assert.Fail("Failed to find PeripheralAudioGpioStatus");
                    }

                    Assert.AreEqual(0, state.StreamLength, "Expected stream to be empty");
                }
            }
        }    

        /// <summary>The audio switch state_ multiple message test 2.</summary>
        [TestMethod]
        public void AudioSwitchState_RemoveMultipleMessages()
        {
            var state = new PeripheralState<PeripheralAudioSwitchMessageType>(CreateAudioSwitchPeripheralContext());

            var peripheralAudioGpioStatus = new PeripheralAudioGpioStatus();
            peripheralAudioGpioStatus.Header.Length--; // Less one byte for checksum to emulate equipment.
            var msg1 = peripheralAudioGpioStatus.ToBytes();
            Assert.AreEqual(PeripheralAudioGpioStatus.Size - 1, peripheralAudioGpioStatus.Header.Length);

            var peripheralAudioStatus = new PeripheralAudioStatus();
            peripheralAudioStatus.Header.Length--;
            var msg2 = peripheralAudioStatus.ToBytes();
            Assert.AreEqual(PeripheralAudioStatus.Size - 1, peripheralAudioStatus.Header.Length);

            var peripheralAudioVersions = new PeripheralAudioVersions();
            peripheralAudioVersions.Header.Length--;
            var msg3 = peripheralAudioVersions.ToBytes();
            Assert.AreEqual(PeripheralAudioVersions.Size - 1, peripheralAudioVersions.Header.Length);

            var frameByte = new[] { Constants.PeripheralFramingByte };
            var frameByteLen = frameByte.Length;

            // add three back to back message separated by the frame byte
            state.AppendStream(frameByte);
            state.AppendStream(msg1);
            Assert.AreEqual(msg1.Length + frameByteLen, state.StreamLength, "Msg 1 Length missmatch");
            state.AppendStream(frameByte);
            state.AppendStream(msg2);
            Assert.AreEqual(msg1.Length + msg2.Length + frameByteLen + frameByteLen, state.StreamLength, "Msg 2 Length missmatch");
            state.AppendStream(frameByte);
            state.AppendStream(msg3);
            var originalSteamLength = state.StreamLength;
            Assert.AreEqual(
                msg1.Length + msg2.Length + msg3.Length + frameByteLen + frameByteLen + frameByteLen, 
                state.StreamLength, 
                "Msg 3 Length missmatch");
            Debug.WriteLine("Total bytes in Stream = " + originalSteamLength);

            lock (AudioSwitchClientLock)
            {
                var peripheralContext = CreateAudioSwitchPeripheralContext(string.Empty); // use memory stream
                using (var client = new AudioSwitchSerialClient(peripheralContext.Config))
                {
                    // change this since we wrote our bytes normally and on reading we don't want the byte order changed on the header
                    client.PeripheralHandler.IsHeaderNetworkByteOrder = false;
                    var message1 = client.RemoveNextMessage(state);
                    if (message1 is PeripheralAudioGpioStatus)
                    {
                        Debug.WriteLine("Remove Message Success for PeripheralAudioGpioStatus");
                    }
                    else
                    {
                        Assert.Fail("Failed to find PeripheralAudioGpioStatus");
                    }

                    // Recall the Length does not include the Checksum byte
                    Assert.AreEqual(originalSteamLength - (msg1.Length + frameByte.Length), state.StreamLength, "Message 1");

                    // does the start of the stream now set at the Frame Byte as part of the second message ?
                    var firstByte = state.MemoryStreamBytes.First();
                    Assert.AreEqual(Constants.PeripheralFramingByte, firstByte);

                    var message2 = client.RemoveNextMessage(state);
                    if (message2 is PeripheralAudioStatus)
                    {
                        Debug.WriteLine("Remove Message Success for PeripheralAudioStatus");
                    }
                    else
                    {
                        Assert.Fail("Failed to find PeripheralAudioStatus");
                    }

                    Assert.AreEqual(originalSteamLength - ((msg1.Length + msg2.Length) + (2 * frameByte.Length)), state.StreamLength, "Message 2");
                    firstByte = state.MemoryStreamBytes.First();
                    Assert.AreEqual(Constants.PeripheralFramingByte, firstByte, "Message 2");

                    var message3 = client.RemoveNextMessage(state);
                    if (message3 is PeripheralAudioVersions)
                    {
                        Debug.WriteLine("Remove Message Success for PeripheralAudioVersions");
                    }
                    else
                    {
                        Assert.Fail("Failed to find PeripheralAudioVersions");
                    }

                    Assert.AreEqual(0, state.StreamLength, "Expected empty stream buffer");
                    Assert.AreEqual(0, state.MemoryStreamBytes.Length);

                    var lastMsg = client.RemoveNextMessage(state);
                    Assert.IsNull(lastMsg);
                }
            }
        }

        /// <summary>The audio switch state_ remove partial messages.</summary>
        [TestMethod]
        public void AudioSwitchState_RemovePartialMessages()
        {
            var state = CreateState();

            var msg1 = new PeripheralAudioGpioStatus().ToBytes();
            var msg2 = new PeripheralAudioStatus().ToBytes();

            var frameByte = new[] { Constants.PeripheralFramingByte };

            // add back to back messages separated by the frame byte
            var r = new Random();
            var msg2Size = r.Next(PeripheralHeaderAudioSwitch.HeaderSize, msg2.Length);
            state.AppendStream(frameByte);
            state.AppendStream(msg1);
            Assert.AreEqual(msg1.Length + 1, state.StreamLength);
            state.AppendStream(frameByte);

            // truncate the msg2 at random
            state.AppendStream(msg2.Take(msg2Size).ToArray());
            var totalBytesInStream = state.StreamLength;
            Debug.WriteLine("Total bytes in Stream = " + totalBytesInStream);

            lock (AudioSwitchClientLock)
            {
                var peripheralContext = CreateAudioSwitchPeripheralContext(string.Empty); // use memory stream
                using (var client = new AudioSwitchSerialClient(peripheralContext.Config))
                {
                    client.PeripheralHandler.IsHeaderNetworkByteOrder = false;
                    var message1 = client.RemoveNextMessage(state);
                    if (message1 is PeripheralAudioGpioStatus)
                    {
                        Debug.WriteLine("Remove Message Success for PeripheralAudioGpioStatus");
                    }
                    else
                    {
                        Assert.Fail("Failed to find PeripheralAudioGpioStatus");
                    }

                    var message2 = client.RemoveNextMessage(state);
                    if (message2 is PeripheralAudioStatus)
                    {
                        Debug.WriteLine("Remove Message Success for PeripheralAudioStatus");

                        // test if the entire stream buffer is empty
                        Assert.AreEqual(0, state.StreamLength);
                    }
                    else if (message2 != null)
                    {
                        Assert.Fail("Failed to find PeripheralAudioStatus");
                    }
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CheckSumPeripherialBaseMessage()
        {
            // because of the generic we cannot use Marshal.Sizeof
            var type = typeof(PeripheralBaseMessage<PeripheralMessageType>);
            Debug.WriteLine("MarshalSize = " + Marshal.SizeOf(type));
        }

        public static int SizeOf<T>() where T : struct
        {
            return Marshal.SizeOf(default(T));
        }

        [TestMethod]
        public void SizeOfMessages()
        {
            Assert.Inconclusive("TODO ");
            // This will fail as the Marshal.Sizeof cannot be used on classes that have a generic
        }

        /// <summary>The check sum peripheral ack.</summary>
        [TestMethod]
        public void CheckSumPeripheralAudioSwitchAck()
        {            
            var model = new PeripheralAudioSwitchAck();
            Assert.AreEqual(0x7, model.Header.Length);
            var bytes = model.ToBytes();
            Assert.AreEqual(PeripheralAck.Size, bytes.Length);
            Assert.AreEqual(0x7, bytes.Length);
            const int ExpetedCheksum = 0xFE;
            Assert.AreEqual(ExpetedCheksum, model.Checksum);
            Debug.WriteLine("checksum = 0x{0:X}", model.Checksum);
            var props = model.GetOrderedPropertiesForObject();
            Assert.AreEqual("Header", props.First().Name);
            // alter the checksum and validate
            model.Checksum = CheckSumUtil.CalculateCheckSum(model);
            Assert.AreEqual(ExpetedCheksum, model.Checksum);
            bytes = model.ToBytes();
            var valid = CheckSumUtil.IsValidChecksum(bytes);
            Assert.IsTrue(valid, "Checksum is invalid");
        }

        /// <summary>The check sum peripheral audio config.</summary>
        [TestMethod]
        public void CheckSumPeripheralAudioConfig()
        {
            var model = new PeripheralAudioConfig();
            Assert.AreEqual(PeripheralAudioConfig.Size, model.Header.Length, "Header LEngth !");
            var bytes = model.ToBytes();
            const int ExpetedCheksum = 0x1;
            Assert.AreEqual(PeripheralAudioConfig.Size, bytes.Length, "PeripheralAudioConfig Size != bytes.Length");
            Assert.AreEqual(ExpetedCheksum, model.Checksum, "Checksum != 0x1");
            Debug.WriteLine("checksum = 0x{0:X}", model.Checksum);
            var checksum = CheckSumUtil.CheckSum(bytes);
            Assert.AreEqual(ExpetedCheksum, checksum);
            Assert.AreEqual(model.Checksum, checksum);
        }

        /// <summary>The check sum peripheral audio gpio update.</summary>
        [TestMethod]
        public void CheckSumPeripheralAudioGpioUpdate()
        {
            var model = new PeripheralAudioGpioStatus();
            Assert.AreEqual(PeripheralAudioGpioStatus.Size, model.Header.Length);
            Assert.AreEqual(0, model.Checksum); // this will change if the class defaults are altered!
            Debug.WriteLine("checksum = 0x{0:X}", model.Checksum);
            model.ChangeMask = 0x123; // fake value
            model.GpioStatus = 0x456;

            // alter the checksum and validate
            model.Checksum = CheckSumUtil.CalculateCheckSum(model);
            var bytes = model.ToBytes();

            var valid = CheckSumUtil.IsValidChecksum(bytes);
            Assert.IsTrue(valid, "Checksum is invalid");
        }

        /// <summary>The check sum peripheral audio status.</summary>
        [TestMethod]
        public void CheckSumPeripheralAudioStatus()
        {
            var model = new PeripheralAudioStatus();
            Assert.AreEqual(PeripheralAudioStatus.Size, model.Header.Length);
            var bytes = model.ToBytes();
            Assert.AreEqual(0x0, model.Checksum); // this will change if the class defaults are altered!
            Debug.WriteLine("checksum = 0x{0:X}", model.Checksum);
        }

        /// <summary>The check sum peripheral audio versions.</summary>
        [TestMethod]
        public void CheckSumPeripheralAudioVersions()
        {
            var model = new PeripheralAudioVersions();
            var bytes = model.ToBytes();
            Assert.AreEqual(PeripheralAudioVersions.Size, bytes.Length, "Expected Sizeof != bytes.Length");
            var props = typeof(PeripheralAudioVersions).GetProperties();
            foreach (var prop in props)
            {
                Debug.WriteLine("Property {0} Type {1}", prop.Name, prop.PropertyType);
            }
        }

        /// <summary>The check sum peripheral gpio notification.</summary>
        [TestMethod]
        public void CheckSumPeripheralGpioNotification()
        {
            var model = new PeripheralAudioGpioStatus();
            Assert.AreEqual(PeripheralAudioGpioStatus.Size, model.Header.Length);            
            Assert.AreEqual(13, model.Header.Length);
            var bytes = model.ToBytes();
            Assert.AreEqual(PeripheralAudioGpioStatus.Size, bytes.Length, "Expected bytes length off");
            Debug.WriteLine("checksum = 0x{0:X}", model.Checksum);
            Assert.AreEqual(0, model.Checksum, "Checksum Off"); // this will change if the class defaults are altered!
        }

        /// <summary>The check sum peripheral nak.</summary>
        [TestMethod]
        public void CheckSumPeripheralAudioSwitchNak()
        {
            var model = new PeripheralAudioSwitchNak();
            var bytes = model.ToBytes();
            Assert.AreEqual(0x7, model.Header.Length);
            Assert.AreEqual(PeripheralNak.Size, bytes.Length, "Expected bytes length off");
            Assert.AreEqual(0x7, bytes.Length);            
            const int ExpetedCheksum = 0xFD;
            Assert.AreEqual(ExpetedCheksum, model.Checksum);
            Debug.WriteLine("checksum = 0x{0:X}", model.Checksum);
            model.Checksum = CheckSumUtil.CalculateCheckSum(model);
            bytes = model.ToBytes();
            var valid = CheckSumUtil.IsValidChecksum(bytes);
            Assert.IsTrue(valid, "Checksum is invalid");
        }

        /// <summary>The check sum peripheral poll audio status.</summary>
        [TestMethod]
        public void CheckSumPeripheralPollAudioStatus()
        {
            var model = new PeripheralPoll(Constants.DefaultPeripheralAudioSwitchAddress, PeripheralSystemMessageType.AudioGeneration3);
            Assert.AreEqual(PeripheralPoll.Size, model.Header.Length);
            var bytes = model.ToBytes();
            Assert.AreEqual(PeripheralPoll.Size, bytes.Length, "Expected bytes length off");
            Assert.AreEqual(0x7, model.Header.Length, "Expected bytes length off");
            Assert.AreEqual(0xF1, model.Checksum); // this will change if the class defaults are altered!
            Debug.WriteLine("checksum = 0x{0:X}", model.Checksum);
            model.Checksum = CheckSumUtil.CalculateCheckSum(model);
            bytes = model.ToBytes();
            var valid = CheckSumUtil.IsValidChecksum(bytes);
            Assert.IsTrue(valid);
        }

        /// <summary>The check sum PeripheralSetVolume.</summary>
        [TestMethod]
        public void CheckSumPeripheralSetVolume()
        {
            var model = new PeripheralSetVolume(10, 20);
            Assert.AreEqual(10, model.InteriorVolume);
            Assert.AreEqual(20, model.ExteriorVolume);

            var bytes = model.ToBytes();
            var chksum = CheckSumUtil.CheckSum(bytes);
            Assert.AreEqual(0XD0, chksum);

            Assert.AreEqual(PeripheralSetVolume.Size, bytes.Length, "Expected bytes length off");
            Assert.AreEqual(PeripheralSetVolume.Size, model.Header.Length);
            Assert.AreEqual(0XD0, model.Checksum);

            Debug.WriteLine("checksum = 0x{0:X}", model.Checksum);
            model.Checksum = CheckSumUtil.CalculateCheckSum(model);
            bytes = model.ToBytes();
            var valid = CheckSumUtil.IsValidChecksum(bytes);
            Assert.IsTrue(valid, "Checksum is invalid");
        }

        /// <summary>The create_ audio switch handler.</summary>
        [TestMethod]
        public void Create_AudioSwitchHandler()
        {
            var handler = new AudioSwitchHandler();
            Assert.IsNull(handler.ContextStream);
            Assert.IsNull(handler.PeripheralContext);
            Assert.IsTrue(handler.IsMediInitialized);
            handler.Dispose();
            // Dictionary.xml is required as a Deployed file used at runtime.
        }

        /// <summary>The create audio switch client expected not supported exception.</summary>
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void CreateAudioSwitchClientExpectedNotSupportedException()
        {
            lock (AudioSwitchClientLock)
            {
                var config = AudioSwitchPeripheralConfig.ReadAudioSwitchConfig(AudioSwitchConfigXmlFileName);
                config.SerialPortSettings.ComPort = "COM3";
                AssertNoComport(config.SerialPortSettings.ComPort);
                Debug.WriteLine("Opening Serial Port " + config.SerialPortSettings.ComPort);
                using (var client = new AudioSwitchSerialClient(config))
                {
                    if (client.IsComPortOpen)
                    {
                        // cannot use the SerialPort.BaseStream and ask for Length or bytes in the stream like a memory stream.
                        // Thinking out loud to solve this 
                        client.PeripheralHandler.Read(1);
                    }
                    else
                    {
                        Assert.Inconclusive("Serial port not found for test " + config.SerialPortSettings.ComPort);
                    }
                }
            }
        }

        /// <summary>The create audio switch config test.</summary>
        [TestMethod]
        public void CreateAudioSwitchConfigTestContext()
        {
            var audioSwitchContext = CreateAudioSwitchPeripheralContext();
            Assert.IsNotNull(audioSwitchContext);
            Assert.IsNotNull(audioSwitchContext.Stream);
            Assert.IsNotNull(audioSwitchContext.Config);
        }

        /// <summary>The create audio switch handler.</summary>
        [TestMethod]
        public void CreateAudioSwitchHandler()
        {
            var audioSwitchContext = CreateAudioSwitchPeripheralContext();

            var handler = PeripheralAudioSwitchFactory.Instance;
            handler.PeripheralContext = audioSwitchContext;

            // Other option   var handler = new AudioSwitchHandler(audioSwitchContext);
            Assert.IsNotNull(handler.PeripheralContext);
            Assert.IsNotNull(handler.PeripheralContext.Config);
            Assert.IsNotNull(handler.PeripheralContext.Stream);
            var signaled = new ManualResetEvent(false);

            handler.AudioStatusRequested += (sender, message) =>
                {
                    Assert.IsNotNull(message);
                    Debug.WriteLine("Success AudioStatusChanged event " + message);
                    signaled.Set();
                };
            Debug.WriteLine("Medi Broadcast " + nameof(AudioStatusRequest));

            // emulate an incoming medi message like Protran would
            MessageDispatcher.Instance.Broadcast(new AudioStatusRequest());
            Assert.IsTrue(signaled.WaitOne(5000), "AudioStatusChanged event failed.");

            var volumeChangeRequest = new VolumeChangeRequest { InteriorVolume = 75, ExteriorVolume = 85, AudioRefreshInterval = 0 };

            Debug.WriteLine(volumeChangeRequest);
            signaled.Reset();

            handler.VolumeChangeRequestedEvent += (sender, message) =>
                {
                    Assert.IsNotNull(message);
                    Debug.WriteLine("VolumeSettingsChanged event " + message);
                    signaled.Set();
                };

            Debug.WriteLine("Medi Broadcast " + nameof(volumeChangeRequest));
            MessageDispatcher.Instance.Broadcast(volumeChangeRequest);

            Assert.IsTrue(signaled.WaitOne(5000), "VolumeSettingsChanged event failed.");
        }

        /// <summary>The create audio switch handler from factory.</summary>
        [TestMethod]
        public void CreateAudioSwitchHandlerFromFactory()
        {
            var handler = PeripheralAudioSwitchFactory.Instance;
            Assert.IsNotNull(handler);
            var defaultContext = CreateAudioSwitchPeripheralContext();
            handler.PeripheralContext = defaultContext;
            Assert.IsNotNull(handler.PeripheralContext);
        }

        /// <summary>The create default config.</summary>
        [TestMethod]
        public void CreateDefaultAudioSwitchConfig()
        {
            var model = new AudioSwitchPeripheralConfig();
            var xml = SerializeToXml(model);
            Assert.IsTrue(String.IsNullOrEmpty(xml) == false);
            var outputConfigFile = Path.Combine(@"C:\Temp", AudioSwitchConfigXmlFileName);
            File.Delete(outputConfigFile);
            var configMgr = new ConfigManager<AudioSwitchPeripheralConfig> { FileName = outputConfigFile };
            configMgr.CreateConfig();
            configMgr.SaveConfig();
            Assert.IsTrue(File.Exists(outputConfigFile));

            var configMgr2 = new ConfigManager<AudioSwitchPeripheralConfig> { FileName = outputConfigFile };
            var cfg = configMgr2.Config;
            Assert.IsNotNull(cfg);
            Assert.AreEqual(cfg.SerialPortSettings.Parity, Parity.None);
            Assert.AreEqual(cfg.SerialPortSettings.StopBits, StopBits.One);
        }

        /// <summary>The extensions to find the .Net Class type from the given header system and message type values</summary>
        [TestMethod]
        public void PeripheralAudioSwitchAck_FromMessageType()
        {
            var type = PeripheralAudioSwitchMessageType.Ack;
            var modelType = type.FindPeripheralMessageClassType(PeripheralSystemMessageType.AudioGeneration3);
            Assert.AreEqual(typeof(PeripheralAudioSwitchAck), modelType);
        }

        [TestMethod]
        public void PeripheralAudioSwitchNak_FromMessageType()
        {
            var type = PeripheralAudioSwitchMessageType.Nak;
            var modelType = type.FindPeripheralMessageClassType(PeripheralSystemMessageType.AudioGeneration3);
            Assert.AreEqual(typeof(PeripheralAudioSwitchNak), modelType);
        }

        /// <summary>The find peripheral ack_ from message type.</summary>
        [TestMethod]
        public void FindPeripheralAck_FromMessageType()
        {
            const PeripheralMessageType MsgType = PeripheralMessageType.Ack;
            Type messageType = MsgType.FindPeripheralMessageClassType(PeripheralSystemMessageType.Unknown);
            Assert.AreEqual(typeof(PeripheralAck), messageType);
        }

        [TestMethod]
        public void FindPeripheralNak_FromMessageType()
        {
            var msgType = PeripheralMessageType.Nak;
            Type messageType = msgType.FindPeripheralMessageClassType(PeripheralSystemMessageType.Unknown);
            Assert.AreEqual(typeof(PeripheralNak), messageType);
        }

        /// <summary>The find peripheral audio config_ from message type.</summary>
        [TestMethod]
        public void FindPeripheralAudioConfig_FromMessageType()
        {
            const PeripheralAudioSwitchMessageType MsgType = PeripheralAudioSwitchMessageType.AudioConfig;
            Type messageType = MsgType.FindPeripheralMessageClassType(PeripheralSystemMessageType.AudioGeneration3);
            Assert.AreEqual(typeof(PeripheralAudioConfig), messageType);
        }

        /// <summary>The find peripheral audio gpio status_ from message type.</summary>
        [TestMethod]
        public void FindPeripheralAudioGpioStatus_FromMessageType()
        {
            const PeripheralAudioSwitchMessageType MsgType = PeripheralAudioSwitchMessageType.GpioStatusResponse;
            Type messageType = MsgType.FindPeripheralMessageClassType(PeripheralSystemMessageType.AudioGeneration3);
            Assert.AreEqual(typeof(PeripheralAudioGpioStatus), messageType);
        }

        /// <summary>The find peripheral audio versions_ from message type.</summary>
        [TestMethod]
        public void FindPeripheralAudioVersions_FromMessageType()
        {
            const PeripheralAudioSwitchMessageType MsgType = PeripheralAudioSwitchMessageType.AudioVersionResponse;
            var messageType = MsgType.FindPeripheralMessageClassType(PeripheralSystemMessageType.AudioGeneration3);
            Assert.AreEqual(typeof(PeripheralAudioVersions), messageType);
        }

        /// <summary>The from bytes bogus header.</summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void FromBytesBogusHeader()
        {
            var header = new BogusPeripheralBaseMessage();
            var bogusBytes = header.ToBytes();
            Assert.AreEqual(7, bogusBytes.Length);
            bogusBytes[4] = 123; // bad enum value
            var model = FromBytesToObjectTest<BogusPeripheralBaseMessage>(bogusBytes);
            Assert.IsNull(model);
        }

        /// <summary>Test serialization to and from bytes to object test.</summary>
        /// <param name="modelBytes">The model Bytes.</param>
        /// <param name="sourceModel">The source Model.</param>
        /// <exception cref="TargetInvocationException">The constructor being called throws an exception. </exception>
        /// <exception cref="MethodAccessException">NoteIn the .NET for Windows Store apps or the Portable Class Library, catch the
        ///     base class exception, <see cref="T:System.MemberAccessException"/>, instead.The caller does not have permission to
        ///     call this constructor.</exception>
        /// <returns>The <see cref="T"/>.</returns>
        public T FromBytesToObjectTest<T>(byte[] modelBytes = null, T sourceModel = null) where T : class, IPeripheralBaseMessageType<PeripheralAudioSwitchMessageType>
        {
            T createdModel = default(T);
            using (var audioSwitchHandler = new AudioSwitchHandler(CreateAudioSwitchPeripheralContext()))
            {
                // disable since we are not Writing in our test and just going from bytes to Reading
                audioSwitchHandler.IsHeaderNetworkByteOrder = false;
                if (sourceModel == null)
                {
                    sourceModel = (T)Activator.CreateInstance(typeof(T)); // create a model for testing
                }
                // verify the class size matches the returned bytes
                var expectedSize = sourceModel.Header.Length; // fails on generics Marshal.SizeOf(typeof(T));
                var bytes = modelBytes ?? sourceModel.ToBytes();
                var s = string.Format("Expected bytes length is wrong for type {0} where {1} != {2}", typeof(T), expectedSize, bytes.Length);

                Assert.AreEqual(expectedSize, bytes.Length, s);

                Debug.WriteLine("\r\nGetting Bytes for Type " + typeof(T));

                var checkSum = CheckSumUtil.CheckSum(bytes);
                Debug.WriteLine("Checksum = " + checkSum);

                // last byte is checksum by definition, recalc it here for unit testing
                bytes[bytes.Length - 1] = checkSum;
                sourceModel.Checksum = checkSum;

                var memoryStream = new MemoryStream(bytes);
                Debug.WriteLine("\r\nReading in from MemoryStream Bytes to Type " + typeof(T));
                createdModel = audioSwitchHandler.Read<T>(memoryStream);

                Assert.IsNotNull(createdModel, "Failed to create object type " + typeof(T) + "Read Failed!");

                Assert.AreEqual(sourceModel.Header.Address, createdModel.Header.Address);
                Assert.AreEqual(sourceModel.Header.Length, createdModel.Header.Length);
                Assert.AreEqual(sourceModel.Header.SystemType, createdModel.Header.SystemType);
                Assert.AreEqual(sourceModel.Header.MessageType, createdModel.Header.MessageType);

                // verify
                Debug.WriteLine("\r\nVerify properties match values");
                var sourceProps = Extensions.GetOrderedPropertiesFor<T>();
                foreach (var prop in sourceProps)
                {
                    Debug.WriteLine("Verify property " + prop.Name);
                    var val1 = prop.GetValue(sourceModel);
                    var val2 = prop.GetValue(createdModel);
                    var valid = false;
                    if (prop.PropertyType().IsArray)
                    {
                        var enumerable1 = (IEnumerable<byte>)val1;
                        var enumerable2 = (IEnumerable<byte>)val2;

                        valid = enumerable1.SequenceEqual(enumerable2);
                        Debug.WriteLine("Compare array Property {0} == {1}, Result {2}", prop.Name, prop.Name, valid);
                    }
                    else
                    {
                        valid = val1.Equals(val2);
                        if (prop.Name == "Checksum")
                        {
                            Debug.WriteLine("Comparing Checksum {0} = {1}", val1, val2);
                        }
                    }

                    Assert.IsTrue(valid, string.Format("! Property miss-match {0} {1} != {2}", prop.Name, val1, val2));
                    Debug.WriteLine("Property  {0} {1} == {2}", prop.Name, val1, val2);
                }
            }

            return createdModel;
        }

        /// <summary>The ximple cell extension string empty.</summary>
        [TestMethod]
        public void FromBytesToPeripheralAck()
        {
            var model = this.FromBytesToObjectTest<PeripheralAudioSwitchAck>();
            Assert.IsNotNull(model);
            Assert.AreEqual(typeof(PeripheralAudioSwitchAck), model.GetType());
        }

        [TestMethod]
        public void ToBytes_PeripheralAudioVersions()
        {
            var m = new PeripheralAudioVersions();
            m.Checksum = 0xff;  // fake test data

            var modelBytes = m.ToBytes();
            Assert.AreEqual(29, modelBytes.Length);
            Assert.AreEqual(m.Checksum, modelBytes.Last());


            var m2 = modelBytes.FromBytes<PeripheralAudioVersions>();
            Assert.AreEqual(m.Checksum, m2.Checksum);
        }

        /// <summary>The from bytes to peripheral audio versions.</summary>
        [TestMethod]
        public void FromBytesToPeripheralAudioVersions()
        {
            Assert.AreEqual(29, PeripheralAudioVersions.Size);
            

            var model = this.FromBytesToObjectTest<PeripheralAudioVersions>();
            Assert.IsNotNull(model);

            var p = new PeripheralAudioVersions();
            Assert.IsNotNull(p.VersionsInfo);

            p.HardwareVersion = "1.2.3.4";
            p.SerialNumber = "11223344";
            p.SoftwareVersion = "1.0.0.2";

            var bytesHardwareVersion = new byte[] { 1, 2, 3, 4 };
            var bytesSerialNumber = new byte[] { 49, 49, 50, 50, 51, 51, 52, 52, 0, 0, 0, 0, 0, 0 };
            var bytesSoftwareVersion = new byte[] { 1, 0, 0, 2 };

            Assert.IsTrue(bytesHardwareVersion.SequenceEqual(p.VersionsInfo.HardwareVersion), "!HardwareVersion");
            Assert.IsTrue(bytesSoftwareVersion.SequenceEqual(p.VersionsInfo.SoftwareVersion), "!SoftwareVersion");
            Assert.IsTrue(bytesSerialNumber.SequenceEqual(p.VersionsInfo.SerialNumber), "!SerialNumber");

            var sourceBytes = p.ToBytes();
            var m = FromBytesToObjectTest(sourceBytes, p);
            Assert.AreEqual(typeof(PeripheralAudioVersions), m.GetType());
            Assert.IsNotNull(m.VersionsInfo);
            Assert.AreEqual("11223344", m.SerialNumber, "SerialNumber !=");
            Assert.AreEqual("1.2.3.4", m.HardwareVersion, "HardwareVersion !=");
            Assert.AreEqual("1.0.0.2", m.SoftwareVersion, "SoftwareVersion !=");
            p.SerialNumber = "11223344556677889900";
            Assert.AreEqual(14, p.SerialNumber.Length);

            Array.Copy(new byte[] { 5, 6, 7, 8 }, p.VersionsInfo.HardwareVersion, 4);
            Array.Copy(new byte[] { 2, 3, 4, 5 }, p.VersionsInfo.SoftwareVersion, 4);
            Assert.AreEqual(p.HardwareVersion, "5.6.7.8");
            Assert.AreEqual(p.SoftwareVersion, "2.3.4.5");
        }

        /// <summary>The from bytes to peripheral nak.</summary>
        [TestMethod]
        public void FromBytesToPeripheralAudioSwitchNak()
        {
            var model = FromBytesToObjectTest<PeripheralAudioSwitchNak>();
            Assert.IsNotNull(model);
            Assert.AreEqual(typeof(PeripheralAudioSwitchNak), model.GetType());
        }

        /// <summary>The from i peripheral image status.</summary>
        [TestMethod]
        public void FromIPeripheralImageStatus()
        {
            var model = FromBytesToObjectTest<PeripheralImageStatus>();
            Assert.IsNotNull(model);
            Assert.AreEqual(typeof(PeripheralImageStatus), model.GetType());
        }

        /// <summary>The from peripheral audio config.</summary>
        [TestMethod]
        public void FromPeripheralAudioConfig()
        {
            var sourceModel = new PeripheralAudioConfig();
            Assert.AreEqual(PeripheralAudioConfig.InteriorDefaultVolumeLevel, sourceModel.InteriorDefaultVolume);

            // Add some mock data
            for (byte i = 0; i < sourceModel.PinMeaning.Length; i++)
            {
                sourceModel.PinMeaning[i] = i;
            }

            Assert.AreEqual(0x1, sourceModel.Checksum); // set in constructor

            CheckSumUtil.CalculateCheckSum(sourceModel);
            var bytes = sourceModel.ToBytes();
            Assert.AreEqual(PeripheralAudioConfig.Size, bytes.Length);

            // for testing with the bytes we get from our model pass it in and use to compare and construct a model back
            var createdModel = FromBytesToObjectTest(null, sourceModel);
            Assert.IsNotNull(createdModel);
        }

        /// <summary>The from peripheral audio gpio update.</summary>
        [TestMethod]
        public void FromPeripheralAudioGpioUpdate()
        {
            var model = FromBytesToObjectTest<PeripheralAudioGpioStatus>();
            Assert.IsNotNull(model);
            Assert.AreEqual(typeof(PeripheralAudioGpioStatus), model.GetType());
        }

        /// <summary>The from peripheral audio status.</summary>
        [TestMethod]
        public void FromPeripheralAudioStatus()
        {
            var model = FromBytesToObjectTest<PeripheralAudioStatus>();
            Assert.IsNotNull(model);
            Assert.AreEqual(typeof(PeripheralAudioStatus), model.GetType());
        }

        /// <summary>The from peripheral header.</summary>
        [TestMethod]
        public void FromPeripheralHeader()
        {
            var m = new PeripheralHeader<PeripheralAudioSwitchMessageType>();
            var bytes = m.ToBytes();
            
            Assert.AreEqual(PeripheralHeader<PeripheralAudioSwitchMessageType>.HeaderSize, bytes.Length);
            Assert.AreEqual(PeripheralHeader<PeripheralMessageType>.HeaderSize, bytes.Length);
            Assert.AreEqual(Constants.PeripheralHeaderSize, bytes.Length);
        }

        /// <summary>The from peripheral image update cancel.</summary>
        [TestMethod]
        public void FromPeripheralImageUpdateCancel()
        {
            var model = FromBytesToObjectTest<PeripheralImageUpdateCancel>();
            Assert.IsNotNull(model);
            Assert.AreEqual(typeof(PeripheralImageUpdateCancel), model.GetType());
        }

        /// <summary>The from peripheral image update start.</summary>
        [TestMethod]
        public void FromPeripheralImageUpdateStart()
        {
            var model = FromBytesToObjectTest<PeripheralImageUpdateStart>();
            Assert.IsNotNull(model);
            Assert.AreEqual(typeof(PeripheralImageUpdateStart), model.GetType());
        }

        /// <summary>The from peripheral gpio notification.</summary>
        [TestMethod]
        public void FromPeripheralPeripheralAudioGpioUpdate()
        {
            var model = FromBytesToObjectTest<PeripheralAudioGpioStatus>();
            Assert.IsNotNull(model);
            Assert.AreEqual(typeof(PeripheralAudioGpioStatus), model.GetType());
        }

        /// <summary>The from peripheral set volume.</summary>
        [TestMethod]
        public void FromPeripheralSetVolume()
        {
            var model = FromBytesToObjectTest<PeripheralSetVolume>();
            Assert.IsTrue(model.Checksum > 0);
            Assert.IsNotNull(model);
            Assert.AreEqual(typeof(PeripheralSetVolume), model.GetType());
        }

        /// <summary>The get bytes test.</summary>
        [TestMethod]
        public void GetBytesTestFromString()
        {
            var bytes = GetBytes("test1234", 4);
            Assert.AreEqual(4, bytes.Length);
            Assert.AreEqual("test", Encoding.ASCII.GetString(bytes));

            bytes = GetBytes("test1234", 10);
            Assert.AreEqual(10, bytes.Length);
            var s = Encoding.ASCII.GetString(bytes).TrimEnd('\0');
            Assert.AreEqual("test1234", s);
        }

        /// <summary>The get peripheral bytes write buffer_ write audio poll status.</summary>
        [TestMethod]
        public void GetPeripheralBytesWriteBuffer_WriteAudioPollStatus()
        {
            var model = new PeripheralPoll(Constants.DefaultPeripheralAudioSwitchAddress, PeripheralSystemMessageType.AudioGeneration3);
            var bytes = model.ToBytes();
            var buffer = AudioSwitchHandler.GetPeripheralBytesWriteBuffer(bytes);
            Assert.AreEqual(Constants.PeripheralFramingByte, buffer[0]);
            Assert.AreEqual(8, buffer.Length);
        }

        /// <summary>The property info peripheral header.</summary>
        [TestMethod]
        public void GetPropertyInfoFromPeripheralAudioSwitchHeader()
        {
            const int ExpectedPeripheralProps = 4;
            var propInfo = typeof(PeripheralHeader<PeripheralAudioSwitchMessageType>).GetProperties();
            foreach (var p in propInfo)
            {
                Debug.WriteLine("{0} Type {1}", p.Name, p.PropertyType);
            }

            Assert.AreEqual(ExpectedPeripheralProps, propInfo.Length);
        }

        [TestMethod]
        public void GetPropertyInfoFromPeripheralHeader()
        {
            const int ExpectedPeripheralProps = 4;
            var propInfo = typeof(PeripheralHeader<PeripheralMessageType>).GetProperties();
            foreach (var p in propInfo)
            {
                Debug.WriteLine("{0} Type {1}", p.Name, p.PropertyType);
            }

            Assert.AreEqual(ExpectedPeripheralProps, propInfo.Length);
        }

        /// <summary>The get test ordered attribute as defailt ordering class props.</summary>
        [TestMethod]
        public void GetTestOrderedAttributeAsDefaultOrderingClassProps()
        {
            var props = Extensions.GetOrderedPropertiesFor<TestOrderedAttributeAsDefailtOrderingClass>();
            foreach (var p in props)
            {
                Debug.WriteLine(p.Name);
            }

            var expectedProps = typeof(TestOrderedAttributeAsDefailtOrderingClass).GetProperties().Count();
            for (int i = 0; i < expectedProps; i++)
            {
                Assert.AreEqual("Field" + i, props[i].Name, "Expected Field" + i);
            }
        }

        /// <summary>The get test ordered attribute class no attributes props.</summary>
        [TestMethod]
        public void GetTestOrderedAttributeClassNoAttributesProps()
        {
            var props = Extensions.GetOrderedPropertiesFor<TestOrderedAttributeClassNoAttributes>();
            var expectedProps = typeof(TestOrderedAttributeClass).GetProperties().Count();

            int i = expectedProps - 1;
            foreach (var p in props)
            {
                Debug.WriteLine(p.Name);
                Assert.AreEqual("Field" + i, p.Name, "Expected Field" + i);
                i--;
            }
        }

        /// <summary>The get test ordered attribute class props.</summary>
        [TestMethod]
        public void GetTestOrderedAttributeClassProps()
        {
            var props = Extensions.GetOrderedPropertiesFor<TestOrderedAttributeClass>();
            foreach (var p in props)
            {
                Debug.WriteLine(p.Name);
            }

            var expectedProps = typeof(TestOrderedAttributeClass).GetProperties().Count();
            for (int i = 0; i < expectedProps; i++)
            {
                Assert.AreEqual("Field" + i, props[i].Name, "Expected Field" + i);
            }
        }

        /// <summary>The get test ordered attribute peripheral audio config.</summary>
        [TestMethod]
        public void GetTestOrderedAttributePeripheralAudioConfig()
        {
            var props = Extensions.GetOrderedPropertiesFor<PeripheralAudioConfig>();
            Assert.AreEqual(20, props.Count, "Total Props incorrect");

            string[] expectedPropNames =
                {
                    "Header", "PinMeaning", "PinSense", "NoiseLevel", "InteriorGain", "ExteriorGain", "PriorityTable", 
                    "InteriorDefaultVolume", "ExteriorDefaultVolume", "InteriorMinVolume", "InteriorMaxVolume", 
                    "InteriorMaxAllowedVolume", "ExteriorMinVolume", "ExteriorMaxVolume", "ExteriorMaxAllowedVolume", 
                    "PushToTalkTimeout", "PushToTalkLockoutTime", "AudioStatusDelay","LineInEnable", "Checksum"
                };

            for (int i = 0; i < expectedPropNames.Length; i++)
            {
                Assert.AreEqual(expectedPropNames[i], props[i].Name, "Property order Wrong");
            }
        }

        /// <summary>The peripheral audio config_ expect exception.</summary>
        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void PeripheralAudioConfig_ExpectException()
        {
            var peripheralAudioConfig = PeripheralAudioConfig.ReadPeripheralAudioConfig("Foobar.xml");
        }

        /// <summary>The Peripheral audio config_ from memory byte stream.</summary>
        [TestMethod]
        [DeploymentItem("Config/PeripheralAudioConfig2.xml")]
        public void PeripheralAudioConfig_FromMemoryByteStream()
        {
            lock (AudioSwitchClientLock)
            {
                var configFile = PeripheralAudioConfig.DefaultPeripheralAudioConfigXmlFile;
                Assert.IsTrue(File.Exists(configFile));
                var peripheralAudioConfig = PeripheralAudioConfig.ReadPeripheralAudioConfig("PeripheralAudioConfig2.xml");
                Assert.IsNotNull(peripheralAudioConfig);
                Assert.IsNotNull(peripheralAudioConfig.Header);
                Assert.AreEqual(PeripheralAudioConfig.Size, peripheralAudioConfig.Header.Length);
                Assert.IsTrue(peripheralAudioConfig.Checksum > 0);
                var bytes = peripheralAudioConfig.ToBytes();

                var memStream = new MemoryStream();

                // now back the other way from the bytes to the model
                var a = new AudioSwitchHandler(new PeripheralContext<PeripheralAudioSwitchMessageType>(new AudioSwitchPeripheralConfig(), memStream));
                a.Write(bytes, memStream);

                a.ContextStream.Seek(0, SeekOrigin.Begin);
                var model = a.Read();
                Assert.IsNotNull(model);
                Type messageType = model.GetType();
                Assert.AreEqual(typeof(PeripheralAudioConfig), messageType);

                // prep the bytes as if it would be written to the stream or serial port
                var buffer = AudioSwitchHandler.GetPeripheralBytesWriteBuffer(bytes);
                a.ContextStream.Seek(0, SeekOrigin.Begin);
                a.Write(buffer, memStream);
                a.ContextStream.Seek(0, SeekOrigin.Begin);
                var model2 = a.Read();
                Assert.IsNotNull(model2);
                Type messageType2 = model2.GetType();
                Assert.AreEqual(typeof(PeripheralAudioConfig), messageType2);
            }
        }

        /// <summary>The from peripheral audio config.</summary>
        [TestMethod]
        public void PeripheralAudioConfig_UpdateChecksum()
        {
            var sourceModel = new PeripheralAudioConfig();
            Assert.AreEqual(0, sourceModel.AudioStatusDelay);
            Assert.AreEqual(1, sourceModel.Checksum, "Check Constructor"); // set in constructor
            sourceModel.AudioStatusDelay = 1000;
            var checkSum = CheckSumUtil.CalculateCheckSum(sourceModel);
            Assert.AreEqual(22, checkSum); // setter should recal the checksum
        }

        /// <summary>The peripheral audio versions_ construct.</summary>
        [TestMethod]
        public void PeripheralAudioVersions_Construct()
        {
            var p = new PeripheralAudioVersions();
            Assert.IsNotNull(p.VersionsInfo);

            p.HardwareVersion = "1.2.3.4";
            p.SerialNumber = "11223344";
            p.SoftwareVersion = "1.0.0.2";

            var bytesHardwareVersion = new byte[] { 1, 2, 3, 4 };
            var bytesSerialNumber = new byte[] { 49, 49, 50, 50, 51, 51, 52, 52, 0, 0, 0, 0, 0, 0 };
            var bytesSoftwareVersion = new byte[] { 1, 0, 0, 2 };

            Assert.IsTrue(bytesHardwareVersion.SequenceEqual(p.VersionsInfo.HardwareVersion), "!HardwareVersion");
            Assert.IsTrue(bytesSoftwareVersion.SequenceEqual(p.VersionsInfo.SoftwareVersion), "!SoftwareVersion");
            Assert.IsTrue(bytesSerialNumber.SequenceEqual(p.VersionsInfo.SerialNumber), "!SerialNumber");
        }

        /// <summary>The peripheral audio versions_ construct.</summary>
        [TestMethod]
        public void PeripheralAudioVersionsInfo_Construct()
        {
            var p = new PeripheralVersionsInfo { HardwareVersionText = "1.2.3.4", SerialNumberText = "11223344", SoftwareVersionText = "1.0.0.2" };

            var bytesHardwareVersion = new byte[] { 1, 2, 3, 4 };
            var bytesSerialNumber = new byte[] { 49, 49, 50, 50, 51, 51, 52, 52, 0, 0, 0, 0, 0, 0 };
            var bytesSoftwareVersion = new byte[] { 1, 0, 0, 2 };

            Assert.IsTrue(bytesHardwareVersion.SequenceEqual(p.HardwareVersion), "!HardwareVersion");
            Assert.IsTrue(bytesSoftwareVersion.SequenceEqual(p.SoftwareVersion), "!SoftwareVersion");
            Assert.IsTrue(bytesSerialNumber.SequenceEqual(p.SerialNumber), "!SerialNumber");
        }

        /// <summary>The calculate check sum.</summary>
        [TestMethod]
        public void PeripheralHeaderToBytes()
        {
            var header = new PeripheralHeader<PeripheralMessageType>();
            var bytes = header.ToBytes();
            Assert.AreEqual(PeripheralHeader<PeripheralMessageType>.HeaderSize, bytes.Length, "Expected length incorrect");

            // the header has no checksum prop, here we just check the bytes and it's checksum            
        }

        [TestMethod]
        public void PeripheralAudioSwitchHeaderToBytes()
        {
            var header = new PeripheralHeader<PeripheralAudioSwitchMessageType>();
            var bytes = header.ToBytes();
            Assert.AreEqual(PeripheralHeader<PeripheralAudioSwitchMessageType>.HeaderSize, bytes.Length, "Expected length incorrect");

            // the header has no checksum prop, here we just check the bytes and it's checksum            
        }

        /// <summary>The peripheral header to network byte order.</summary>
        [TestMethod]
        public void PeripheralHeaderToNetworkByteOrder()
        {
            var peripheralHeader = new PeripheralHeader<PeripheralMessageType>();
            var modelDest = peripheralHeader.HostToNetworkByteOrder();
            Assert.AreEqual(0x0600, modelDest.Length);
            Assert.AreEqual(0x00F0, modelDest.Address);
            Assert.AreEqual(peripheralHeader.MessageType, modelDest.MessageType);
            Assert.AreEqual(peripheralHeader.SystemType, modelDest.SystemType);

            var expected = modelDest.HostToNetworkByteOrder();
            Assert.AreEqual(peripheralHeader.Length, expected.Length);
            Assert.AreEqual(peripheralHeader.Address, expected.Address);
            Assert.AreEqual(peripheralHeader.MessageType, expected.MessageType);
            Assert.AreEqual(peripheralHeader.SystemType, expected.SystemType);
        }

        /// <summary>The read ack from stream.</summary>
        [TestMethod]
        public void ReadAck_FromMemoryStream()
        {
            Logger.Trace("Enter Test ReadAck_FromMemoryStream");
            var model = new PeripheralAck();
            var bytes = model.ToBytes();
            var stream = new MemoryStream(bytes);
            var context = CreateAudioSwitchPeripheralContext();
            var audioSwitchHandler = new AudioSwitchHandler(context);
            var message = audioSwitchHandler.Read(stream);
            AssertValidateMessage<PeripheralAudioSwitchAck>(message);
        }

        /// <summary>The read nak from stream.</summary>
        [TestMethod]
        public void ReadNak_FromMemoryStream()
        {
            var model = new PeripheralNak();
            var bytes = model.ToBytes();
            var stream = new MemoryStream(bytes);
            var context = CreateAudioSwitchPeripheralContext();
            var audioSwitchHandler = new AudioSwitchHandler(context);
            var message = audioSwitchHandler.Read(stream);
            AssertValidateMessage<PeripheralNak>(message);
        }

        /// <summary>The read peripheral audio config file.</summary>
        [TestMethod]
        public void ReadPeripheralAudioConfigFile()
        {
            lock (AudioSwitchClientLock)
            {
                var peripheralAudioConfig = PeripheralAudioConfig.ReadPeripheralAudioConfig("PeripheralAudioConfig.xml");
                Assert.IsNotNull(peripheralAudioConfig);
                Assert.IsNotNull(peripheralAudioConfig.Header);
                Assert.AreEqual(PeripheralAudioConfig.Size, peripheralAudioConfig.Header.Length);
                Assert.IsTrue(peripheralAudioConfig.Checksum > 0);
                Assert.AreEqual(100, peripheralAudioConfig.InteriorMaxVolume);
                Assert.AreEqual(100, peripheralAudioConfig.ExteriorMaxVolume);
            }
        }

        /// <summary>The read write audio config as xml.</summary>
        [TestMethod]
        public void ReadWriteAudioConfigAsXml()
        {
            var model = new PeripheralAudioConfig();
            const string FileName = @"C:\temp\PeripheralAudioConfig.xml";
            File.Delete(FileName);
            PeripheralAudioConfig.WritePeripheralAudioConfig(model, FileName);
            Assert.IsTrue(File.Exists(FileName));

            var model2 = PeripheralAudioConfig.ReadPeripheralAudioConfig(FileName);
            Assert.IsNotNull(model2);
        }

        /// <summary>The read write audio switch config.</summary>
        [TestMethod]
        public void ReadWriteAudioSwitchConfig()
        {
            var model = new AudioSwitchPeripheralConfig { Enabled = false };
            Assert.IsNotNull(model.SerialPortSettings);
            Assert.AreEqual(model.PeripheralHeaderInNetworkByteOrder, true);
            Assert.IsTrue(model.SerialPortSettings.BaudRate != 0);

            var outputConfigFile = Path.Combine(@"C:\Temp", "AudioSwtichConfig2.xml");
            AudioSwitchPeripheralConfig.WriteAudioSwitchConfig(model, outputConfigFile);

            var config = AudioSwitchPeripheralConfig.ReadAudioSwitchConfig(outputConfigFile);
            Assert.AreEqual(model.PeripheralHeaderInNetworkByteOrder, config.PeripheralHeaderInNetworkByteOrder);
            Assert.AreEqual(model.Enabled, config.Enabled);
        }

        /// <summary>The read write nak.</summary>
        [TestMethod]
        public void ReadWriteNak()
        {
            Assert.Inconclusive();
            
            var context = CreateAudioSwitchPeripheralContext();
            var audioSwitchHandler = new AudioSwitchHandler(context);
            var bytes = audioSwitchHandler.WriteNak();
            Assert.AreEqual(PeripheralNak.Size + sizeof(byte), bytes); // Extra byte for Framing byte written to stream      

            // rewind the stream
            var stream = audioSwitchHandler.PeripheralContext.Stream;
            stream.Position = 0;
    // TODO        var model = audioSwitchHandler.Read<PeripheralNak>();
    //        Assert.IsNotNull(model);
        }

        /// <summary>The read write peripheral audio config.</summary>
        [TestMethod]
        public void ReadWritePeripheralAudioConfigToFile()
        {
            var model = new PeripheralAudioConfig { AudioStatusDelay = 1234, NoiseLevel = 0xFF };

            // AudioStatusDelay to be ignored in write
            Assert.IsNotNull(model.Header);

            var outputConfigFile = Path.Combine(@"C:\Temp", "PeripheralAudioConfig_ReadWritePeripheralAudioConfigToFile.xml");

            PeripheralAudioConfig.WritePeripheralAudioConfig(model, outputConfigFile);
            var config = PeripheralAudioConfig.ReadPeripheralAudioConfig(outputConfigFile);
            Assert.IsNotNull(config);
            Assert.IsTrue(File.Exists(outputConfigFile));

            var config2 = PeripheralAudioConfig.ReadPeripheralAudioConfig(outputConfigFile);
            Assert.AreEqual(0, config2.AudioStatusDelay);
            Assert.AreEqual(0xFF, config2.NoiseLevel);
        }

        /// <summary>The serialize audio status message.</summary>
        [TestMethod]
        public void SerializeAudioStatusMessage()
        {
            var audioStatusMessage = new AudioStatusMessage();
            var xml = SerializeToXml<AudioStatusMessage>();
            Debug.WriteLine(xml);
            Debug.WriteLine("\r\nToString() = {0}", audioStatusMessage);
        }

        /// <summary>The serialize audio switch config.</summary>
        [TestMethod]
        public void SerializeAudioSwitchConfig()
        {
            try
            {
                var configManager = new ConfigManager<AudioSwitchPeripheralConfig>(AudioSwitchConfigXmlFileName);
                var audioSwitchConfig = configManager.Config;
                Assert.IsNotNull(audioSwitchConfig);
                Assert.AreEqual(true, audioSwitchConfig.Enabled);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Assert.Fail(ex.ToString());
            }
        }

        /// <summary>The serialize periphal header.</summary>
        [TestMethod]
        public void SerializePeripheralHeader()
        {
            var header = new PeripheralHeader<PeripheralAudioSwitchMessageType>
                             {
                                 Address = 0x0, 
                                 Length = 6, 
                                 MessageType = PeripheralAudioSwitchMessageType.Unknown, 
                                 SystemType = PeripheralSystemMessageType.Unknown
                             };
            var xml = SerializeToXml<PeripheralHeader<PeripheralAudioSwitchMessageType>>();
            Debug.WriteLine(xml);
            Debug.WriteLine("\r\nToString() = {0}", header);
        }

        /// <summary>The serialize serial port settings.</summary>
        [TestMethod]
        public void SerializeSerialPortSettings()
        {
            var model = new SerialPortSettings();
            var xml = SerializeToXml(model);
            Debug.WriteLine(xml);
        }

        /// <summary>The serialize volume change request.</summary>
        [TestMethod]
        public void SerializeVolumeChangeRequest()
        {
            var model = new VolumeChangeRequest();
            var xml = SerializeToXml(model);
            Debug.WriteLine(xml);
        }

        /// <summary>The serialize volume settings.</summary>
        [TestMethod]
        public void SerializeVolumeSettings()
        {
            var model = new VolumeSettings();
            var xml = SerializeToXml(model);
            Debug.WriteLine(xml);
        }

        /// <summary>The serialize volume settings message.</summary>
        [TestMethod]
        public void SerializeVolumeSettingsMessage()
        {
            var volumeSettingsMessage = new VolumeSettingsMessage();
            Assert.AreEqual(VolumeSettingsMessage.DefaultRefreshInterval, volumeSettingsMessage.RefreshIntervalMiliSeconds);
            Assert.IsNotNull(volumeSettingsMessage.Interior);
            Assert.IsNotNull(volumeSettingsMessage.Exterior);

            var xml = SerializeToXml<VolumeSettingsMessage>();
            Debug.WriteLine(xml);
            Debug.WriteLine("\r\nToString() = {0}", volumeSettingsMessage);
        }

        /// <summary>Serialize VolumeSettingsMessageResponse.</summary>
        [TestMethod]
        public void SerializeVolumeSettingsResponseMessage()
        {
            var response = new AudioStatusResponse();
            Assert.AreEqual(MessageActions.None, response.MessageAction);
            Assert.IsNotNull(response.Interior);
            Assert.IsNotNull(response.Exterior);

            var xml = SerializeToXml<AudioStatusResponse>();
            Debug.WriteLine(xml);
            Debug.WriteLine("\r\nToString() = {0}", response);
        }

        /// <summary>The test to version string.</summary>
        [TestMethod]
        public void TestToVersionString()
        {
            var bytes = new byte[] { 1, 2, 3, 4 };
            var s = PeripheralVersionsInfo.ToVersionString(bytes);
            Debug.WriteLine("Version = " + s);
            Assert.AreEqual("1.2.3.4", s);

            bytes = new byte[] { 1, 2, 3 };
            s = PeripheralVersionsInfo.ToVersionString(bytes);
            Debug.WriteLine("Version = " + s);
            Assert.AreEqual("1.2.3", s);

            bytes = new byte[4];
            s = PeripheralVersionsInfo.ToVersionString(bytes);
            Debug.WriteLine("Version = " + s);
            Assert.AreEqual("0.0.0.0", s);
        }

        /// <summary>The to byte array on string.</summary>
        [TestMethod]
        public void ToByteArrayOnString()
        {
            var s = "Hello";
            var bytes = s.ToByteArray();
            Assert.IsTrue(bytes.SequenceEqual(new byte[] { 72, 101, 108, 108, 111 }));
        }

        /// <summary>The to byte array peripheral ack.</summary>
        [TestMethod]
        public void ToByteArrayPeripheralAck()
        {
            var model = new PeripheralAudioSwitchAck();
            var bytes = model.ToByteArray();
            Assert.AreEqual(PeripheralAck.Size, bytes.Length);
        }

        /// <summary>The to byte array peripheral audio config.</summary>
        [TestMethod]
        public void ToByteArrayPeripheralAudioConfig()
        {
            var model = new PeripheralAudioConfig();
            var bytes = model.ToByteArray();
            Assert.AreEqual(PeripheralAudioConfig.Size, bytes.Length);

            // How about the bytes placed in the correct order
        }

        /// <summary>The to bytes peripheral audio config.</summary>
        [TestMethod]
        public void ToBytesPeripheralAudioConfig()
        {
            var model = new PeripheralAudioConfig();
            var bytes = model.ToBytes();
            Assert.AreEqual(PeripheralAudioConfig.Size, bytes.Length);
        }

        /// <summary>The validate bytes peripheral audio status.</summary>
        [TestMethod]
        public void ValidateBytesPeripheralAudioStatus()
        {
            var model = ValidateSize<PeripheralAudioStatus>(PeripheralAudioStatus.Size);
        }

        /// <summary>The validate enum size active speaker type.</summary>
        [TestMethod]
        public void ValidateEnumSizeActiveSpeakerType()
        {
            ValidateSize<ActiveSpeakerZone>(1);
        }

        /// <summary>The validate enum size peripheral gpio type.</summary>
        [TestMethod]
        public void ValidateEnumSizePeripheralGpioType()
        {
            ValidateSize<PeripheralGpioType>(1);
        }

        /// <summary>The validate enum size peripheral image status type.</summary>
        [TestMethod]
        public void ValidateEnumSizePeripheralImageStatusType()
        {
            // enum one byte
            ValidateSize<PeripheralImageStatusType>(1);
        }

        /// <summary>The validate enum size peripheral message type.</summary>
        [TestMethod]
        public void ValidateEnumSizePeripheralMessageType()
        {
            ValidateSize<PeripheralAudioSwitchMessageType>(1);
        }

        /// <summary>The validate enum size peripheral priority type.</summary>
        [TestMethod]
        public void ValidateEnumSizePeripheralPriorityType()
        {
            // enum one byte
            ValidateSize<AudioSourcePriority>(1);
        }

        /// <summary>The validate enum size peripheral system message type.</summary>
        [TestMethod]
        public void ValidateEnumSizePeripheralSystemMessageType()
        {
            // enum one byte
            ValidateSize<PeripheralSystemMessageType>(1);
        }

        /// <summary>The validate peripheral image update start.</summary>
        [TestMethod]
        public void ValidatePeripheralImageUpdateStart()
        {
            var model = ValidateSize<PeripheralImageUpdateStart>(PeripheralImageUpdateStart.Size);
            Assert.AreEqual(PeripheralSystemMessageType.AudioGeneration3, model.Header.SystemType);
            Assert.AreEqual(PeripheralAudioSwitchMessageType.ImageUpdateStart, model.Header.MessageType);
        }

        /// <summary>The validate poll audio status.</summary>
        [TestMethod]
        public void ValidatePollAudioStatus()
        {
            var message = this.ValidateSize<PeripheralPoll>(7);
            Assert.AreEqual(PeripheralSystemMessageType.Unknown, message.Header.SystemType);
        }

        /// <summary>The validate message ack.</summary>
        [TestMethod]
        public void ValidateSize_PeripheralAck()
        {
            var model = ValidateSize<PeripheralAck>(PeripheralAck.Size);
            Assert.AreEqual(PeripheralSystemMessageType.Unknown, model.Header.SystemType);
        }

        /// <summary>The validate peripheral audio config.</summary>
        [TestMethod]
        public void ValidateSize_PeripheralAudioConfig()
        {
            var model = ValidateSize<PeripheralAudioConfig>(PeripheralAudioConfig.Size);
            Assert.AreEqual(PeripheralSystemMessageType.AudioGeneration3, model.Header.SystemType);
            Assert.AreEqual(PeripheralAudioSwitchMessageType.AudioConfig, model.Header.MessageType);
        }

        /// <summary>The validate peripheral audio enable.</summary>
        [TestMethod]
        public void ValidateSize_PeripheralAudioEnable()
        {
            var model = ValidateSize<PeripheralAudioEnable>(PeripheralAudioEnable.Size);
            Assert.AreEqual(PeripheralSystemMessageType.AudioGeneration3, model.Header.SystemType);
            Assert.AreEqual(PeripheralAudioSwitchMessageType.AudioOutputEnable, model.Header.MessageType);
        }

        /// <summary>The validate peripheral audio gpio update.</summary>
        [TestMethod]
        public void ValidateSize_PeripheralAudioGpioUpdate()
        {
            ValidateSize<PeripheralAudioGpioStatus>(PeripheralAudioGpioStatus.Size);
        }

        /// <summary>The validate peripheral audio versions.</summary>
        [TestMethod]
        public void ValidateSize_PeripheralAudioVersions()
        {
            var model = ValidateSize<PeripheralAudioVersions>(PeripheralAudioVersions.Size);
            Assert.IsNotNull(model.VersionsInfo);
            Assert.IsNotNull(model.VersionsInfo.HardwareVersion);
            Assert.IsNotNull(model.VersionsInfo.SerialNumber);
            Assert.IsNotNull(model.VersionsInfo.SoftwareVersion);

            string s1 = model.HardwareVersion;
            Assert.IsNotNull(s1);
            string s2 = model.SerialNumber;
            Assert.IsNotNull(s2);
            string s3 = model.SoftwareVersion;
            Assert.IsNotNull(s3);
        }

        /// <summary>The validate peripheral audio versions info.</summary>
        [TestMethod]
        public void ValidateSize_PeripheralAudioVersionsInfo()
        {
            var model = ValidateSize<PeripheralVersionsInfo>(PeripheralVersionsInfo.Size);

            model.HardwareVersion = new byte[PeripheralVersionsInfo.HardwareVersionSize];
            model.SerialNumber = new byte[PeripheralVersionsInfo.SerialNumberSize];
            model.SoftwareVersion = new byte[] { 0x1, 0x2 }; // make short on purpose to expect the correct byte size back

            var bytes = model.FieldsToBytes();
            Assert.AreEqual(PeripheralVersionsInfo.Size, bytes.Length);
        }

        /// <summary>The validate Peripheral Base message.</summary>
        [TestMethod]
        public void ValidateSize_PeripheralBaseMessage()
        {
            Assert.AreEqual(Constants.PeripheralHeaderSize, PeripheralBaseMessageType<PeripheralMessageType>.Size);
            ValidateSize<PeripheralBaseMessageType<PeripheralMessageType>>(PeripheralBaseMessage<PeripheralMessageType>.Size);
            var size = PeripheralBaseMessage<PeripheralMessageType>.Size;
            Assert.AreEqual(PeripheralBaseMessage<PeripheralMessageType>.Size, size);
        }

        /// <summary>The validate peripheral gpio notification.</summary>
        [TestMethod]
        public void ValidateSize_PeripheralGpioNotification()
        {
            var model = this.ValidateSize<PeripheralAudioGpioStatus>(PeripheralAudioGpioStatus.Size);
            Assert.AreEqual(PeripheralSystemMessageType.AudioGeneration3, model.Header.SystemType);
            Assert.AreEqual(PeripheralAudioSwitchMessageType.GpioStatusResponse, model.Header.MessageType);
            Assert.AreEqual(PeripheralAudioGpioStatus.Size, model.Header.Length);
        }

        /// <summary>The validate header.</summary>
        [TestMethod]
        public void ValidateSize_PeripheralHeader()
        {
            var model = ValidateSize<PeripheralHeader<PeripheralMessageType>>(PeripheralHeader<PeripheralMessageType>.HeaderSize);
            Assert.AreEqual(Constants.DefaultPeripheralAudioSwitchAddress, model.Address, "Address wrong default");
            Assert.AreEqual(PeripheralHeader<PeripheralMessageType>.DefaultSystemType, model.SystemType, "SystemType wrong default");
            Assert.AreEqual(PeripheralMessageType.Unknown, model.MessageType, "MessageType wrong default");
        }

        [TestMethod]
        public void ValidateSize_PeripheralAudioHeader()
        {
            var model = ValidateSize<PeripheralHeader<PeripheralAudioSwitchMessageType>>(PeripheralHeader<PeripheralAudioSwitchMessageType>.HeaderSize);
            Assert.AreEqual(Constants.DefaultPeripheralAudioSwitchAddress, model.Address, "Address wrong default");
            Assert.AreEqual(PeripheralHeader<PeripheralAudioSwitchMessageType>.DefaultSystemType, model.SystemType, "SystemType wrong default");
            Assert.AreEqual(PeripheralAudioSwitchMessageType.Unknown, model.MessageType, "MessageType wrong default");
        }


        /// <summary>The validate size_ peripheral image record.</summary>
        [TestMethod]
        public void ValidateSize_PeripheralImageRecord()
        {
            ValidateSize<PeripheralImageRecordMessage>(PeripheralImageRecordMessage.Size);
        }

        /// <summary>The validate size_ peripheral image status.</summary>
        [TestMethod]
        public void ValidateSize_PeripheralImageStatus()
        {
            ValidateSize<PeripheralImageStatus>(PeripheralImageStatus.Size);
        }

        /// <summary>The validate size_ peripheral image update cancel.</summary>
        [TestMethod]
        public void ValidateSize_PeripheralImageUpdateCancel()
        {
            ValidateSize<PeripheralImageUpdateCancel>(PeripheralImageUpdateCancel.Size);
        }

        /// <summary>The validate size_ peripheral image update start.</summary>
        [TestMethod]
        public void ValidateSize_PeripheralImageUpdateStart()
        {
            ValidateSize<PeripheralImageUpdateStart>(PeripheralImageUpdateStart.Size);
        }

        /// <summary>The validate peripheral nak.</summary>
        [TestMethod]
        public void ValidateSize_PeripheralNak()
        {
            var model = ValidateSize<PeripheralNak>(PeripheralNak.Size);
            Assert.AreEqual(PeripheralSystemMessageType.Unknown, model.Header.SystemType);
        }

        /// <summary>The validate peripheral poll audio status.</summary>
        [TestMethod]
        public void ValidateSize_PeripheralPollAudioStatus()
        {
            var model = ValidateSize<PeripheralPoll>(PeripheralPoll.Size);
            Assert.AreEqual(PeripheralSystemMessageType.Unknown, model.Header.SystemType);
            Assert.AreEqual(PeripheralAudioSwitchMessageType.Poll, model.Header.MessageType);
        }

        /// <summary>The validate nak message.</summary>
        [TestMethod]
        public void ValidateSize_PeripheralSetVolume()
        {
            var model = ValidateSize<PeripheralSetVolume>(PeripheralSetVolume.Size);
            Assert.AreEqual(PeripheralSystemMessageType.AudioGeneration3, model.Header.SystemType);
            Assert.AreEqual(PeripheralAudioSwitchMessageType.SetVolume, model.Header.MessageType);
        }

        /// <summary>The set check sum test.</summary>
        [TestMethod]
        public void VerifyCheckSumPropExistsPeripheralBaseMessage()
        {
            var checkSumPropInfo = typeof(PeripheralBaseMessageType<PeripheralMessageType>).GetProperties().SingleOrDefault(m => m.Name.Contains("Checksum"));
            Assert.IsNotNull(checkSumPropInfo);

            checkSumPropInfo = typeof(PeripheralBaseMessageType<PeripheralAudioSwitchMessageType>).GetProperties().SingleOrDefault(m => m.Name.Contains("Checksum"));
            Assert.IsNotNull(checkSumPropInfo);
        }

        /// <summary>The audio switch handler write ack.</summary>
        [TestMethod]
        public void WriteAck_ToMemoryStream()
        {
            var context = CreateAudioSwitchPeripheralContext();
            var audioSwitchHandler = new AudioSwitchHandler(context);
            var bytes = audioSwitchHandler.WriteAck();
            Assert.AreEqual(PeripheralAck.Size + sizeof(byte), bytes); // Extra byte for Framing byte written to stream              
        }

        /// <summary>The write audio config settings from file.</summary>
        [TestMethod]
        public void WriteAudioConfigSettingsFromFile()
        {
            AssertNoConsoleTestApp();

            lock (AudioSwitchClientLock)
            {
                // This test depends on our console test app
                // run the separate console test app to echo an ack back
                var config = AudioSwitchPeripheralConfig.ReadAudioSwitchConfig(AudioSwitchConfigXmlFileName);
                config.SerialPortSettings.ComPort = "COM1";
                AssertNoComport("COM1", "COM2");
                var signaledEvent = new ManualResetEvent(false);
                var signaled = false;
                using (var client = new AudioSwitchSerialClient(config))
                {
                    Assert.IsTrue(client.IsComPortOpen);
                    client.PeripheralDataReceived += (sender, args) =>
                        {
                            var message = args.Message as IPeripheralBaseMessageType<PeripheralAudioSwitchMessageType>;
                            
                            // i am testing an explicitly return or reply from my test client to simulate the real world
                            if (message.Header.MessageType == PeripheralAudioSwitchMessageType.Ack)
                            {
                                Debug.WriteLine("Success got signaled Type=" + message.GetType());
                                signaledEvent.Set();
                            }
                            else
                            {
                                Debug.WriteLine("Failed to get Ack message signaled Type=" + message.GetType());
                            }
                        };

                    var audioConfigSettingsFile = PeripheralAudioConfig.DefaultPeripheralAudioConfigXmlFile;
                    int bytesWritten = client.WriteAudioConfig(audioConfigSettingsFile);
                    Assert.IsTrue(bytesWritten > 0);
                    signaled = signaledEvent.WaitOne(Debugger.IsAttached ? 30000 : 10000);
                }

                Assert.IsTrue(signaled);
            }
        }

        /// <summary>The write audio config to binary.</summary>
        [TestMethod]
        public void WriteAudioConfigToBinary()
        {
            var model = new PeripheralAudioConfig();
            const string FileName = @"C:\temp\PeripheralAudioConfig.bin";
            File.Delete(FileName);
            using (var binWriter = new BinaryWriter(File.Open(FileName, FileMode.Create)))
            {
                var context = CreateAudioSwitchPeripheralContext();
                context.Stream = binWriter.BaseStream;
                var audioSwitchHandler = new AudioSwitchHandler(context);
                var bytes = audioSwitchHandler.Write(model);
                Assert.AreEqual(PeripheralAudioConfig.Size + sizeof(byte), bytes); // Extra byte for Framing byte written to stream          
                Assert.IsTrue(File.Exists(FileName));
            }
        }

        /// <summary>The ximple cell extension Boolean.</summary>
        [TestMethod]
        public void XimpleCellExtensionBool()
        {
            var cell = new XimpleCell("1", 1, 0, 0);
            var result = cell.TryGetValue<bool>();
            Assert.AreEqual(true, result);
        }

        /// <summary>The ximple cell extension Boolean.</summary>
        [TestMethod]
        public void XimpleCellExtensionBoolean()
        {
            var cell = new XimpleCell("true", 1, 0, 0);
            var result = cell.TryGetValue<bool>();
            Assert.AreEqual(true, result);
        }

        /// <summary>The ximple cell extension empty string.</summary>
        [TestMethod]
        public void XimpleCellExtensionEmptyString()
        {
            var cell = new XimpleCell(null, 1, 0, 0);
            var result = cell.TryGetValue<string>();
            Assert.AreEqual(string.Empty, result);
        }

        /// <summary>The ximple cell extension int.</summary>
        [TestMethod]
        public void XimpleCellExtensionInt()
        {
            var cell = new XimpleCell("1", 1, 0, 0);
            var result = cell.TryGetValue<int>();
            Assert.AreEqual(1, result);
        }

        /// <summary>The ximple cell extension int default value.</summary>
        [TestMethod]
        public void XimpleCellExtensionIntDefaultValue()
        {
            var cell = new XimpleCell(string.Empty, 1, 0, 0);
            var result = cell.TryGetValue(-1);
            Assert.AreEqual(-1, result);
        }

        /// <summary>The ximple cell extension string.</summary>
        [TestMethod]
        public void XimpleCellExtensionString()
        {
            var cell = new XimpleCell("test", 1, 0, 0);
            var result = cell.TryGetValue<string>();
            Assert.AreEqual("test", result);
        }

        /// <summary>The ximple cell extension string empty.</summary>
        [TestMethod]
        public void XimpleCellExtensionStringEmpty()
        {
            var cell = new XimpleCell(string.Empty, 1, 0, 0);
            var result = cell.TryGetValue<string>();
            Assert.AreEqual(string.Empty, result);
        }

        #endregion

        #region Methods

        /// <summary>The audio switch serial client serial test.</summary>
        /// <param name="sourceBytes">The test data.</param>
        /// <param name="replyBytes">The reply Bytes.</param>
        /// <param name="comportNames">The comport names.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="bool"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name=""/> is <see langword="null"/>.</exception>
        /// <exception cref="XmlException">If the file content could not be loaded</exception>
        internal bool AudioSwitchSerialClientSerialTest<T>(byte[] sourceBytes, byte[] replyBytes = null, string[] comportNames = null)
            where T : class, IPeripheralBaseMessageType<PeripheralAudioSwitchMessageType>
        {
            // This unit  test uses a tool to emulate Serial Ports and may fail if incomplete
            // http://www.virtual-serial-port.org/products/vspdxp/?gclid=CjwKEAjws5zABRDqkoOniLqfywESJACjdoiGZhwzLPRrcsIbpckZP7TgrCYI7y2_yz3F2XXntyRSFBoCDB3w_wcB
            lock (AudioSwitchClientLock)
            {
                bool isPeripheralData = false;

                // setup to send our peripheral message entities or other bogus data
                T model = default(T);
                if (sourceBytes == null)
                {
                    model = (T)Activator.CreateInstance(typeof(T));
                    isPeripheralData = model is IPeripheralBaseMessage;
                    Debug.WriteLine("Test Model isPeripheralData=" + isPeripheralData);
                    sourceBytes = model.ToBytes();
                }

                // test if know response,
                var responeHeader = PeripheralHeader<PeripheralAudioSwitchMessageType>.Create(replyBytes);

                Assert.IsNotNull(sourceBytes);

                // Used http://www.hhdsoftware.com/virtual-serial-ports to setup virtual test serial ports to time them together for testing
                if (comportNames == null)
                {
                    comportNames = new[] { "COM1", "COM2" };
                }

                AssertNoComport(comportNames);

                // Note this unit test is relying on bridging two serial ports. Without this configuration we cannot test
                var dataEvent = new ManualResetEvent(false);
                var terminatedEvent = new ManualResetEvent(false);
                var startTestEvent = new ManualResetEvent(false);
                var portNames = SerialPort.GetPortNames();
                var firstPort = comportNames[0];
                var secondPort = comportNames[1];
                bool signaledData = false;

                // check if our virtual serial ports exists
                if (portNames.Contains(firstPort) && portNames.Contains(secondPort))
                {
                    // test we can open our test ports
                    Debug.WriteLine("Opening " + firstPort);

                    Task.Factory.StartNew(
                        () =>
                            {
                                // open the serial port to listen on 
                                var secondSerialPort = AudioSwitchSerialClient.OpenSerialPort(secondPort);
                                if (secondSerialPort.IsOpen == false)
                                {
                                    Assert.Fail("Failed to open Reader Serial Port " + secondSerialPort.PortName);
                                    return;
                                }

                                try
                                {
                                    var dataReady = new ManualResetEvent(false);
                                    secondSerialPort.DataReceived += (sender, args) =>
                                        {
                                            Debug.WriteLine("RX Serial Data Received " + secondSerialPort.PortName);
                                            dataReady.Set();
                                        };

                                    // indicate read, wait for data RX event
                                    startTestEvent.Set();
                                    var idx = WaitHandle.WaitAny(new WaitHandle[] { dataReady, terminatedEvent });
                                    if (idx == 0)
                                    {
                                        try
                                        {
                                            var buffer = new byte[4096];
                                            Debug.WriteLine("Unit Test thread waiting for RX " + secondSerialPort.PortName);
                                            int bytesRead = secondSerialPort.Read(buffer, 0, buffer.Length);
                                            if (bytesRead > 0)
                                            {
                                                var bytesReceived = buffer.Take(bytesRead).ToArray();
                                                Debug.Write("Unit Test thread RX Byte read = " + bytesReceived.Length + " ");
                                                foreach (var b in bytesReceived)
                                                {
                                                    Debug.Write(string.Format("0x{0:X},", b));
                                                }

                                                Debug.WriteLine("-------------------------------");

                                                // echo it back to trigger the background reader thread to work in the client
                                                // if the data is binary and is our peripheral data and not a random string like hello world
                                                // we need to write the header in bytes in NetworkByte order. A bit more complicated than a simple echo.
                                                // we also need to emulate writing a framing octent(byte) as the first byte as that is what the peripheral will do for TX to us.

                                                // Echo the bytes we read or some other optional set
                                                if (replyBytes == null || replyBytes.Length == 0)
                                                {
                                                    secondSerialPort.Write(new[] { Constants.PeripheralFramingByte }, 0, 1);
                                                    secondSerialPort.Write(bytesReceived, 0, bytesReceived.Length);
                                                    Debug.WriteLine("\nTX received bytes completed Length = " + replyBytes.Length);
                                                }
                                                else
                                                {
                                                    // reply simulating the hardware. Header is in Network Byte Order proceeded by the framing octet
                                                    Debug.WriteLine("\nTX writing bytes Length = " + replyBytes.Length);

                                                    // Write Frame Byte
                                                    secondSerialPort.Write(new[] { Constants.PeripheralFramingByte }, 0, 1);
                                                    secondSerialPort.Write(replyBytes, 0, replyBytes.Length);
                                                    Thread.Sleep(Debugger.IsAttached ? 60000 : 500);
                                                }
                                            }
                                        }
                                        catch (TimeoutException)
                                        {
                                            Debug.WriteLine("TimeoutException in unit test Thread");
                                        }
                                        catch (IOException ioException)
                                        {
                                            // The Virtual Free Serial Ports may need the Com2=>Com3 Bridge re-created or fixed
                                            Debug.WriteLine(ioException.Message);
                                        }
                                    }
                                    else
                                    {
                                        Debug.WriteLine("Unit Test did not receive Signal for data ready, write something.");
                                    }
                                }
                                finally
                                {
                                    Debug.WriteLine("Unit test Task Exited Close Port " + secondSerialPort.PortName);
                                    secondSerialPort.Close();
                                    terminatedEvent.Set();
                                }
                            });

                    // allow our reader task to get going and listen before we TX 
                    if (!startTestEvent.WaitOne(10000))
                    {
                        return false;
                    }

                    // read in our settings file, open a serial port in the client - Be sure Serial settings match!
                    // Test reading data on our background client thread and turning a series of bytes into a peripheral message/class.
                    // We expect a generated event when we get a message as a success case, on negative data or unknown we will
                    // come back with a event but the payload will not be a know peripheral class entity but null.
                    var config = AudioSwitchPeripheralConfig.ReadAudioSwitchConfig(AudioSwitchConfigXmlFileName);
                    Debug.WriteLine("Opening Serial " + firstPort);
                    config.SerialPortSettings.ComPort = firstPort;

                    using (var client = new AudioSwitchSerialClient(config))
                    {
                        // event when we get a peripheral message
                        client.PeripheralDataReceived += (sender, args) =>
                            {
                                Debug.WriteLine("Success, Signal Data Received on SerialPort COM2");

                                var message = args.Message as IPeripheralBaseMessageType<PeripheralAudioSwitchMessageType>;
                                if (responeHeader != null)
                                {
                                    // based on a known test reply verify we got this back here
                                    
                                    if (message.Header.Equals(responeHeader))
                                    {
                                        dataEvent.Set();
                                    }
                                    else
                                    {
                                        Debug.WriteLine("Got message but Unexpected header");
                                    }
                                }
                                else
                                {
                                    dataEvent.Set();
                                }
                            };
                        Assert.IsTrue(client.IsComPortOpen, "Com Port Not opened!  " + firstPort);
                        Debug.WriteLine("Writing test data on " + client.PortName);

                        if (isPeripheralData)
                        {
                            client.PeripheralHandler.Write(model);
                        }
                        else
                        {
                            client.WriteRaw(sourceBytes);
                        }

                        Debug.WriteLine("COM2 waiting for data event signal...");
                        var idx = WaitHandle.WaitAny(new WaitHandle[] { dataEvent, terminatedEvent }, Debugger.IsAttached ? 60000 : 10000);
                        signaledData = idx == 0;

                        Debug.WriteIf(signaledData, "Test Success = Got Signaled Event!");
                        Debug.WriteIf(!signaledData, "Test Failed  = Timeout No Event!");
                    }
                }
                else
                {
                    Assert.Inconclusive("The test serial ports Com2 & Com3 are not available for testing");
                }

                return signaledData;
            }
        }

        private static void AssertNoComport(params string[] comports)
        {
            if (comports == null)
            {
                throw new ArgumentNullException();
            }

            var existingPorts = SerialPort.GetPortNames();
            var missing = comports.Except(existingPorts).Any();
            if (missing)
            {
                Assert.Inconclusive("Serial ComPort not found test skipped Port=" + string.Join(",", comports));
            }
        }

        private static void AssertNoConsoleTestApp()
        {
            var found = Process.GetProcessesByName("AudioSwitchEchoConsoleApp").Any();
            if (!found)
            {
                Assert.Inconclusive("Unit test skipped, Console test process not running - AudioSwitchEchoConsoleApp");
            }
        }

        private static void AssertValidateMessage<T>(object message) where T : class
        {
            Assert.IsNotNull(message);
            Assert.AreEqual(message.GetType(), typeof(T));
            var baseMessage = message as IPeripheralBaseMessageType<T>;
            Assert.IsNotNull(baseMessage, "Model object is not from IPeripheralBaseMessageType<TMessageType> Type="+typeof(T) );
            Assert.IsNotNull(baseMessage.Header);
            Assert.AreNotEqual(PeripheralMessageType.Unknown.ToString(), baseMessage.Header.MessageType.ToString());
            Assert.AreNotEqual(PeripheralMessageType.Unknown, baseMessage.Header.MessageType);
            Assert.IsTrue(baseMessage.Header.Length > 0);
            Assert.IsTrue(baseMessage.Checksum > 0);
        }

        private static PeripheralContextAudioSwtich CreateAudioSwitchPeripheralContext(
            string comPort = "", 
            string configFileName = AudioSwitchPeripheralConfig.DefaultAudioSwitchConfigFileName)
        {
            var audioSwitchConfig = AudioSwitchPeripheralConfig.ReadAudioSwitchConfig(configFileName);
            Assert.IsNotNull(audioSwitchConfig);
            if (string.IsNullOrEmpty(comPort) == false)
            {
                audioSwitchConfig.SerialPortSettings.ComPort = comPort;
            }

            return new PeripheralContextAudioSwtich(audioSwitchConfig, new MemoryStream());
        }

        private static void InitMedi()
        {
            // use the same mechanics as the host apps so we can run them standalone and debug messages to/from them
            var configFileName = PathManager.Instance.GetPath(FileType.Config, "medi.config");
            fileConfigurator = new FileConfigurator(configFileName, Environment.MachineName, Environment.MachineName);
            MessageDispatcher.Instance.Configure(fileConfigurator);
        }

        private T ValidateSize<T>(int expectedSize)
        {
            Type type;
            int size = 0;
            var model = Activator.CreateInstance<T>();
            Assert.IsNotNull(model);

            if (typeof(T).IsEnum)
            {
                type = Enum.GetUnderlyingType(typeof(T));
                Debug.WriteLine("Enum MarshalSize = " + Marshal.SizeOf(Activator.CreateInstance(type)));
            }
            else
            {
                type = typeof(T);
                Debug.WriteLine(typeof(T) + ".Size = " + expectedSize);
                try
                {
                    Debug.WriteLine("Class MarshalSize = " + Marshal.SizeOf(typeof(T)));
                }
                catch (System.ArgumentException)
                {
                    var fieldInfo = typeof(T).GetField("Size");
                    if (fieldInfo != null)
                    {
                        Debug.WriteLine("Field Size = " + fieldInfo);
                        size = (int)fieldInfo.GetValue(model);
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            if (size==0)
                size = Marshal.SizeOf(type);
            Assert.AreEqual(expectedSize, size, "Expected size Not Equal");
            if (type.IsAbstract)
            {
                return default(T);
            }
            
            var propInfo = typeof(T).GetProperty("Header");
            if (propInfo != null)
            {                
                var val = propInfo.GetValue(model);
                IPeripheralBaseMessageType<PeripheralMessageType> peripheralBaseMessage = model as IPeripheralBaseMessageType<PeripheralMessageType>;
                if (peripheralBaseMessage == null)
                {
                    var peripheralAudioSwtichBaseMessage = model as IPeripheralBaseMessageType<PeripheralAudioSwitchMessageType>;

                }

                if (peripheralBaseMessage != null)
                {
                    Assert.IsNotNull(peripheralBaseMessage.Header);
                    var fieldInfo =
                        typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                            .FirstOrDefault(fi => fi.IsLiteral && !fi.IsInitOnly && fi.Name == "Size");
                    val = fieldInfo.GetValue(model);
                    int entitySize = val != null ? Convert.ToInt32(val) : 0;
                    if (entitySize > 0)
                    {
                        Assert.AreEqual(entitySize, peripheralBaseMessage.Header.Length, "Expected Length != " + entitySize);
                    }
                }
            }

            return model;
        }

        #endregion

        // }
        // Assert.IsTrue(valid);
        // var valid = signaled.WaitOne(1000);

        ////           handler.RaiseXimpleCreated(gipoXimple);
        // });
        // signaled.Set();
        // Debug.WriteLine("Received Medi Ximple Message");
        // {
        // (sender, args) =>
        // MessageDispatcher.Instance.Subscribe<Ximple>(

        // // for test
        // ManualResetEvent signaled = new ManualResetEvent(false);
        // //         var gipoXimple = handler.CreateGipoXimple(peripheralGpioEventArg);
        // peripheralGpioEventArg.GpioInfo.Add(new GpioInfo(PeripheralGpioType.Door1, true));

        // var peripheralGpioEventArg = new PeripheralGpioEventArg();
        // var handler = new AudioSwitchHandler();
        // {
        // public void RaiseXimpleCreated()
        // [DeploymentItem(@"Config\medi.config")]
        // [DeploymentItem(@"Config\dictionary.xml")]

        // [TestMethod]
    }
}