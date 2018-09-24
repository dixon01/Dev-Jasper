// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="UnitTestPeripheralDimmer.cs">
//   Copyright © 2011-2017 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmerProtocol.Test
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Ports;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading;

    using Luminator.PeripheralDimmer;
    using Luminator.PeripheralDimmer.Interfaces;
    using Luminator.PeripheralDimmer.Models;
    using Luminator.PeripheralDimmer.Processor;
    using Luminator.PeripheralDimmer.Types;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using NLog;

    /// <summary>The unit test 1.</summary>
    [TestClass]
    public class UnitTestPeripheralDimmer
    {
        #region Static Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly string[] SerialPortNames = SerialPort.GetPortNames();

        private static readonly string TestComPort = DimmerConstants.DefaultComPort;

        private static SerialPortSettings serialSettings;

        #endregion

        #region Fields

        private readonly object SerialLock = new object();

        #endregion

        #region Public Methods and Operators

        /// <summary>The test class init.</summary>
        /// <param name="context">The context.</param>
        [ClassInitialize]
        public static void TestClassInit(TestContext context)
        {
            var listener = new NLog.NLogTraceListener();
            Debug.Listeners.Add(listener);

            Assert.IsTrue(string.IsNullOrEmpty(TestComPort) == false);
            serialSettings = new SerialPortSettings(TestComPort, 9600, 8, Parity.None, StopBits.One);
        }

        /// <summary>The construct_ dimmer version info.</summary>
        [TestMethod]
        public void Construct_DimmerVersionInfo()
        {
            var m = new DimmerVersionInfo("123456789", "abcdefgh");
            Assert.IsNotNull(m.HardwareVersion);
            Assert.IsNotNull(m.SoftwareVersion);
            var bytes = m.ToBytes();
            Assert.AreEqual(DimmerVersionInfo.Size, bytes.Length);
            Assert.AreEqual("123456789", m.HardwareVersionText);
            Assert.AreEqual("abcdefgh", m.SoftwareVersionText);
        }

        /// <summary>The dimmer ack_ check size.</summary>
        [TestMethod]
        public void DimmerAck_CheckSize()
        {
            var model = new DimmerAck();
            Assert.AreEqual(0x4, model.Header.MessageType);
            Assert.IsNotNull(model.Header);
            Assert.AreEqual(7, model.Header.Length);
            var size = Marshal.SizeOf(typeof(DimmerAck));
            Assert.AreEqual(7, size);
        }

        /// <summary>The dimmer nak_ check size.</summary>
        [TestMethod]
        public void DimmerNak_CheckSize()
        {
            var model = new DimmerNak();
            Assert.AreEqual(0x5, model.Header.MessageType);
            Assert.IsNotNull(model.Header);
            Assert.AreEqual(7, model.Header.Length);
            var size = Marshal.SizeOf(typeof(DimmerNak));
            Assert.AreEqual(7, size);
            Assert.AreEqual(7, DimmerNak.Size);
        }

        /// <summary>The dimmer peripheral header_ check size.</summary>
        [TestMethod]
        public void DimmerPeripheralHeader_CheckSize()
        {
            var model = new PeripheralHeader();
            Assert.AreEqual(6, model.Length);
            Assert.AreEqual(6, PeripheralHeader.HeaderSize);
            Assert.AreEqual(DimmerConstants.DimmerAddress, model.Address);
            Assert.AreEqual((byte)DimmerMessageType.Unknown, model.MessageType);
            Assert.AreEqual(DimmerConstants.DimmerSystemMessageType, model.SystemMessageType);
        }

        /// <summary>The marshal_ dimmer Poll.</summary>
        [TestMethod]
        public void DimmerPoll()
        {
            this.MarshalTest<DimmerPoll>();
        }

        /// <summary>The dimmer ack_ check size.</summary>
        [TestMethod]
        public void DimmerPoll_CheckSize()
        {
            var model = new DimmerPoll();
            Assert.IsNotNull(model.Header);
            Assert.AreEqual(7, model.Header.Length);
            var size = Marshal.SizeOf(typeof(DimmerPoll));
            Assert.AreEqual(7, size);
        }

        /// <summary>The dimmer query request_ check size.</summary>
        [TestMethod]
        public void DimmerQueryRequest_CheckSize()
        {
            var model = new DimmerQueryRequest();
            Assert.AreEqual(0x15, model.Header.MessageType);
            Assert.IsNotNull(model.Header);
            Assert.AreEqual(7, model.Header.Length);
            var size = Marshal.SizeOf(typeof(DimmerQueryRequest));
            Assert.AreEqual(7, size);
        }

        /// <summary>The dimmer query response_ check size.</summary>
        [TestMethod]
        public void DimmerQueryResponse_CheckSize()
        {
            var model = new DimmerQueryResponse();
            Assert.IsNotNull(model.Header);
            Assert.AreEqual(13, model.Header.Length);
            var size = Marshal.SizeOf(typeof(DimmerQueryResponse));
            Assert.AreEqual(13, size);
            Assert.AreEqual(13, DimmerQueryResponse.Size);
        }

        /// <summary>The dimmer set brightness_ check size.</summary>
        [TestMethod]
        public void DimmerSetBrightness_CheckSize()
        {
            var model = new DimmerSetBrightness();
            Assert.IsNotNull(model.Header);
            Assert.AreEqual(DimmerSetBrightness.Size, model.Header.Length);
            var size = Marshal.SizeOf(typeof(DimmerSetBrightness));
            Assert.AreEqual(DimmerSetBrightness.Size, size);
        }

        /// <summary>The dimmer set sensor scale_ check size.</summary>
        [TestMethod]
        public void DimmerSetSensorScale_CheckSize()
        {
            var model = new DimmerSetSensorScale();
            Assert.IsNotNull(model.Header);
            Assert.AreEqual(8, model.Header.Length);
            var size = Marshal.SizeOf(typeof(DimmerSetSensorScale));
            Assert.AreEqual(8, size);
            Assert.AreEqual(8, DimmerSetSensorScale.Size);
        }

        /// <summary>The dimmer version request_ check size.</summary>
        [TestMethod]
        public void DimmerVersionRequest_CheckSize()
        {
            var model = new DimmerAck();
            Assert.IsNotNull(model.Header);
            Assert.AreEqual(7, model.Header.Length);
            var size = Marshal.SizeOf(typeof(DimmerVersionRequest));
            Assert.AreEqual(7, size);
        }

        /// <summary>The peripheral_ types size.</summary>
        [TestMethod]
        public void DimmerVersionResponse_CheckSize()
        {
            var model = new DimmerVersionResponse();
            Assert.IsNotNull(model.Header);
            Assert.AreEqual(39, model.Header.Length);
            var size = Marshal.SizeOf(typeof(DimmerVersionResponse));
            Assert.AreEqual(39, size);
            Assert.AreEqual(39, DimmerVersionResponse.Size);
        }

        [TestMethod]
        public void DimmerVersionInfo_CheckSize()
        {
            var model = new DimmerVersionInfo();
            Assert.IsNotNull(model.HardwareVersion);
            Assert.IsNotNull(model.SoftwareVersion);
            var size = Marshal.SizeOf(typeof(DimmerVersionInfo));
            Assert.AreEqual(32, size);
            Assert.AreEqual(32, DimmerVersionInfo.Size);
        }

        /// <summary>The marshal_ dimmer ack.</summary>
        [TestMethod]
        public void Marshal_DimmerAck()
        {
            this.MarshalTest<DimmerAck>();
        }

        /// <summary>The marshal_ dimmer nak.</summary>
        [TestMethod]
        public void Marshal_DimmerNak()
        {
            this.MarshalTest<DimmerNak>();
        }

        /// <summary>The marshal_ dimmer query request.</summary>
        [TestMethod]
        public void Marshal_DimmerQueryRequest()
        {
            this.MarshalTest<DimmerQueryRequest>();
        }

        /// <summary>The marshal_ dimmer set brightness.</summary>
        [TestMethod]
        public void Marshal_DimmerSetBrightness()
        {
            this.MarshalTest<DimmerSetBrightness>();
        }

        /// <summary>The marshal_ dimmer set power on mode.</summary>
        [TestMethod]
        public void Marshal_DimmerSetPowerOnMode()
        {
            this.MarshalTest<DimmerSetPowerOnMode>();
        }

        /// <summary>The marshal_ dimmer set scale.</summary>
        [TestMethod]
        public void Marshal_DimmerSetScale()
        {
            this.MarshalTest<DimmerSetSensorScale>();
        }

        /// <summary>The marshal_ dimmer version request.</summary>
        [TestMethod]
        public void Marshal_DimmerVersionRequest()
        {
            this.MarshalTest<DimmerVersionRequest>();
        }

        /// <summary>The marshal_ dimmer version response.</summary>
        [TestMethod]
        public void Marshal_DimmerVersionResponse()
        {
            this.MarshalTest<DimmerVersionResponse>();
        }

        /// <summary>The marshal_ peripheral header.</summary>
        [TestMethod]
        public void Marshal_PeripheralHeader()
        {
            Debug.WriteLine("Marshal_PeripheralHeader");
            var header = new PeripheralHeader(1, 2, 3);
            Assert.AreEqual(6, PeripheralHeader.HeaderSize, "HeaderSize != 6");
            Assert.AreEqual(PeripheralHeader.HeaderSize, header.Length, "Header Length wrong");
            var bytes = header.ToBytes();
            Assert.AreEqual(6, bytes.Length);
            foreach (byte b in bytes)
            {
                Debug.Write("0x" + b + ",");
            }

            Assert.IsNotNull(bytes);
            var header2 = new PeripheralHeader(bytes);
            Assert.IsNotNull(header2);
            Assert.AreEqual(header.Length, header2.Length, "Header1.Length != Header2.Length");
            Assert.AreEqual(header.Address, header2.Address, "Header1.Address != Header2.Address");
            Assert.AreEqual(header.SystemMessageType, header2.SystemMessageType);
            Assert.AreEqual(header.MessageType, header2.MessageType);
        }

        /// <summary>The marshal_ peripheral header network byte order.</summary>
        [TestMethod]
        public void Marshal_PeripheralHeaderNetworkByteOrder()
        {
            var header = new PeripheralHeader(1, 2, 3);
            var length = header.Length;
            var address = header.Address;
            header.HostToNetworkByteOrder();
            Assert.AreEqual(6, PeripheralHeader.HeaderSize);
            Assert.AreEqual(0x0600, header.Length);
            Assert.AreEqual(0x0300, header.Address);
            Assert.AreEqual(1, header.MessageType);
            Assert.AreEqual(2, header.SystemMessageType);

            var bytes = header.ToBytes();
            Assert.IsNotNull(bytes);
            var header2 = new PeripheralHeader(bytes, true);
            Assert.IsNotNull(header2);
            Assert.AreEqual(length, header2.Length);
            Assert.AreEqual(address, header2.Address);
            Assert.AreEqual(header.SystemMessageType, header2.SystemMessageType);
            Assert.AreEqual(header.MessageType, header2.MessageType);
        }

        /// <summary>The marshal test.</summary>
        /// <typeparam name="T"></typeparam>
        public void MarshalTest<T>() where T : class, IPeripheralBaseMessage
        {
            Debug.WriteLine("MarshalTest for " + typeof(T));
            var model = (T)Activator.CreateInstance(typeof(T));
            model.Header.Address = DimmerConstants.DimmerAddress;
            model.Checksum = 77; // fake data
            var size = Marshal.SizeOf<T>();
            Debug.WriteLine("Marshal Size = " + size);
            Debug.WriteLine(model.ToString());
            Assert.IsTrue(model.Header.Length > 0);

            var bytes = model.ToBytes();
            foreach (byte b in bytes)
            {
                Debug.Write(string.Format("0x{0:},", b));
            }

            Debug.WriteLine("\nCalculate Checksum for bytes");
            var checksum = CheckSumUtil.CheckSum(bytes);
            Assert.IsTrue(checksum > 0);
            Debug.WriteLine("Checksum for {0} = 0x{1:X}", typeof(T), checksum);

            Assert.IsNotNull(bytes);
            var expectedSize = Marshal.SizeOf(model);
            Assert.AreEqual(expectedSize, bytes.Length);
            Assert.AreEqual(model.Header.Length, bytes.Length);

            var model2 = bytes.FromBytes<T>();
            Assert.IsNotNull(model2);
            Assert.AreEqual(model.Header.Length, model2.Header.Length);
            Assert.AreEqual(model.Header.Address, model2.Header.Address);
            Assert.AreEqual(model.Header.MessageType, model2.Header.MessageType);
            Assert.AreEqual(model.Header.SystemMessageType, model2.Header.SystemMessageType);
        }

        /// <summary>The peripheral header_ size of.</summary>
        [TestMethod]
        public void PeripheralHeader_CheckSize()
        {
            var header = new PeripheralHeader();
            Assert.AreEqual(6, header.Length);
            var size = Marshal.SizeOf(typeof(PeripheralHeader));
            Assert.AreEqual(6, size);
        }

        /// <summary>The peripheral serial client construct from settings file.</summary>
        [TestMethod]
        [DeploymentItem("DimmerSerialSettings.xml")]
        public void PeripheralSerialClientConstructFromSettingsFile()
        {
            lock (this.SerialLock)
            {
                Assert.IsTrue(File.Exists("DimmerSerialSettings.xml"));
                var p = new DimmerPeripheralSerialClient("DimmerSerialSettings.xml");
                Assert.IsNotNull(p, "Null");
                Assert.IsTrue(p.IsOpen, "Serial Port Not Opened");
                p.Close();
            }
        }

        /// <summary>The read write dimmer peripheral config.</summary>
        [TestMethod]
        public void ReadWriteDimmerPeripheralConfig()
        {
            var filepath = @"C:\Temp\DimmerPeripheralConfig.xml";

            var config = new DimmerPeripheralConfig();
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }

            DimmerPeripheralConfig.WriteDimmerPeripheralConfig(config, filepath);

            var config2 = DimmerPeripheralConfig.ReadDimmerPeripheralConfig(filepath);
            Assert.IsNotNull(config2);
            Assert.IsFalse(config2.DimmerProcessorTuningParams.InvertBrightness);
        }

        /// <summary>The read write serial port settings.</summary>
        [TestMethod]
        public void ReadWriteSerialPortSettings()
        {
            var file = @"C:\Temp\DimmerSerialSettings.xml";

            var s1 = new SerialPortSettings(TestComPort, 9600, 8, Parity.None, StopBits.One);
            File.Delete(file);
            SerialPortSettings.Write(file, s1);
            Assert.IsTrue(File.Exists(file));
            var s2 = SerialPortSettings.Read(file);
            Assert.IsNotNull(s2);
            Assert.AreEqual(s1.ComPort, s2.ComPort);
            Assert.AreEqual(s1.BaudRate, s2.BaudRate);
            Assert.AreEqual(s1.DataBits, s2.DataBits);
            Assert.AreEqual(s1.StopBits, s2.StopBits);
            Assert.AreEqual(s1.Parity, s2.Parity);
        }

        /// <summary>The serial open test.</summary>
        [TestMethod]
        public void SerialOpenTest()
        {
            lock (this.SerialLock)
            {
                this.AssertInvalidComPort(TestComPort);
                SerialPort s = new SerialPort(TestComPort, 9600, Parity.None, 8, StopBits.One);
                s.Open();
                Assert.IsTrue(s.IsOpen);
                s.Close();
            }
        }

        /// <summary>The serial port settings test.</summary>
        [TestMethod]
        public void SerialPortSettingsTest()
        {
            var s = new SerialPortSettings(TestComPort, 9600, 8, Parity.None, StopBits.One);
            Assert.AreEqual(TestComPort, s.ComPort);
            Assert.AreEqual(9600, s.BaudRate);
            Assert.AreEqual(8, s.DataBits);
            Assert.AreEqual(Parity.None, s.Parity);
            Assert.AreEqual(StopBits.One, s.StopBits);
        }

        /// <summary>The start dimmer client timmer.</summary>
        [TestMethod]
        public void StartDimmerClientTimmer()
        {
            lock (this.SerialLock)
            {
                var countdownEvent = new CountdownEvent(4);
                int count = 0;
                using (var client = new DimmerPeripheralSerialClient(serialSettings))
                {
                    Assert.IsTrue(client.IsOpen);

                    client.DimmerSensorLevelsChanged += (sender, response) =>
                        {
                            Debug.WriteLine("DimmerSensorValuesChanged ----------------------------------");
                            DumpDebug(response);
                            countdownEvent.Signal();
                            count++;
                        };
                    client.StartBackgroundProcessing(500);
                    var signaled = countdownEvent.Wait(Debugger.IsAttached ? 30000 : 5000);
                    if (signaled)
                    {
                        Assert.AreEqual(4, count);
                    }
                }
            }
        }

        /// <summary>The write dimmer query request message_ dimmer client.</summary>
        [TestMethod]
        public void WriteDimmerQueryRequestMessage_DimmerClient()
        {
            lock (this.SerialLock)
            {
                var m = new ManualResetEvent(false);
                var dimmerSensorEvent = new ManualResetEvent(false);
                using (var client = new DimmerPeripheralSerialClient(serialSettings))
                {
                    client.PeripheralDataReady += (sender, arg) =>
                        {
                            Debug.WriteLine("PeripheralDataReady ----------------------------------");
                            var peripheralMessage = arg.Message;
                            DumpDebug(peripheralMessage);
                            m.Set();
                            Assert.IsNotNull(peripheralMessage);
                            Assert.AreEqual((byte)DimmerMessageType.QueryResponse, peripheralMessage.Header.MessageType);
                        };

                    client.DimmerSensorLevelsChanged += (sender, dimmerQueryResponse) =>
                        {
                            Assert.IsNotNull(dimmerQueryResponse);
                            Debug.WriteLine("DimmerSensorValuesChanged ----------------------------------");
                            DumpDebug(dimmerQueryResponse);
                            dimmerSensorEvent.Set();
                        };

                    var result = client.WriteQueryRequest(0);
                    Assert.IsTrue(result);
                    var signaled = m.WaitOne(3000);
                    Assert.IsTrue(signaled);
                }
            }
        }

        /// <summary>The write power on mode.</summary>
        [TestMethod]
        public void WritePowerOnMode()
        {
            lock (this.SerialLock)
            {
                var m = new DimmerSetPowerOnMode(DimmerConstants.DefaultDimmerScale);
                Assert.AreEqual(PowerOnMode.Default, m.PowerOnOnMode);
                this.WriteSerialPeripheralTest<DimmerSetPowerOnMode>(DimmerMessageType.Ack);
            }
        }

        /// <summary>The write power on mode_ dimmer client.</summary>
        [TestMethod]
        public void WritePowerOnMode_DimmerClient()
        {
            lock (this.SerialLock)
            {
                using (var client = new DimmerPeripheralSerialClient(serialSettings))
                {
                    Assert.IsTrue(client.IsOpen);
                    var result = client.WritePowerOnMode(PowerOnMode.Default);
                    Assert.IsTrue(result);
                }
            }
        }

        /// <summary>The write query request message.</summary>
        [TestMethod]
        public void WriteQueryRequestMessage()
        {
            lock (this.SerialLock)
            {
                this.WriteSerialPeripheralTest<DimmerQueryRequest>(DimmerMessageType.QueryResponse);
            }
        }

        /// <summary>The write test.</summary>
        /// <param name="expectedResponseDimmerMessageType">The expected message type response in the header.</param>
        /// <param name="useMemoryStream">The use Memory Stream.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="bool"/>.</returns>
        /// <exception cref="TypeLoadException"><paramref name="type"/> is not a valid type. </exception>
        public bool WriteSerialPeripheralTest<T>(DimmerMessageType expectedResponseDimmerMessageType, bool useMemoryStream = false)
            where T : class, IPeripheralBaseMessage
        {
            lock (this.SerialLock)
            {
                Debug.WriteLine("Enter Test for TX Peripheral Message of type " + typeof(T));
                var messageToWrite = (T)Activator.CreateInstance(typeof(T));
                var responseEvent = new ManualResetEvent(false);
                bool waitForAck = expectedResponseDimmerMessageType == DimmerMessageType.Ack;
                var expectedMessageSizeToWrite = messageToWrite.Header.Length;
                Debug.WriteLine("Expected Length for {0} with the checksum {1}, waitForAck={2}", typeof(T), expectedMessageSizeToWrite, waitForAck);
                serialSettings.EnableBackgroundReader = true;

                var responseClassType = DimmerPeripheralSerialClient.FindMessageType(expectedResponseDimmerMessageType);
                var messageToRead = (IPeripheralBaseMessage)Activator.CreateInstance(responseClassType);
                var expectedResponseMessageSizeToRead = messageToRead.Header.Length - sizeof(byte);

                // less one byte for checksum which is not part of the header length by definition.
                Assert.AreEqual((byte)expectedResponseDimmerMessageType, messageToRead.Header.MessageType);

                using (var serialClient = new DimmerPeripheralSerialClient(serialSettings))
                {
                    Assert.IsNotNull(serialClient);
                    Assert.IsTrue(serialClient.IsOpen, "Unit Test Comport not opened");
                    serialClient.PeripheralDataReady += (sender, arg) =>
                        {
                            Assert.IsNotNull(arg);
                            if (arg.Message != null)
                            {
                                Debug.WriteLine("Unit Test Received Data {0}", arg.Message);

                                var peripheralMessage = arg.Message;

                                if (expectedResponseDimmerMessageType != DimmerMessageType.Unknown)
                                {
                                    if (expectedResponseDimmerMessageType == (DimmerMessageType)arg.Message.Header.MessageType)
                                    {
                                        Debug.WriteLine("Unit Test Successful Message Reply");
                                        DumpDebug(peripheralMessage);

                                        // does the Length match
                                        Assert.AreEqual(expectedResponseMessageSizeToRead, peripheralMessage.Header.Length);
                                        Debug.WriteLine(arg.Message);

                                        responseEvent.Set();
                                    }
                                }
                            }
                        };

                    // for testing grab the serial event too
                    serialClient.SerialPort.DataReceived += (sender, args) =>
                        {
                            if (args.EventType == SerialData.Chars)
                            {
                                Debug.WriteLine("Unit Test Serial Data Received");
                            }
                        };

                    var result = serialClient.Write(messageToWrite, waitForAck, useMemoryStream ? new MemoryStream() : null);

                    if (serialSettings.EnableBackgroundReader == false)
                    {
                        var bytesRead = serialClient.Read(1000);
                        Assert.IsNotNull(bytesRead);
                        Assert.AreEqual(expectedResponseMessageSizeToRead, messageToRead.Header.Length - 1);
                    }

                    Debug.WriteLine("Waiting for response... TimeStamp " + DateTime.Now);
                    var signled = responseEvent.WaitOne(Debugger.IsAttached ? 30000 : 3000);
                    Debug.WriteLine("Signaled = " + signled + " TimeStamp " + DateTime.Now);
                    Assert.IsTrue(signled, "Did not get RX replay expected type " + expectedResponseDimmerMessageType);

                    if (waitForAck)
                    {
                        Assert.AreNotEqual(-1, result, "Ack was not received");
                    }

                    return signled;
                }
            }
        }

        /// <summary>The write set brightness.</summary>
        [TestMethod]
        public void WriteSetBrightness()
        {
            lock (this.SerialLock)
            {
                this.WriteSerialPeripheralTest<DimmerSetBrightness>(DimmerMessageType.Ack);
            }
        }

        /// <summary>The write set brightness_ dimmer client.</summary>
        [TestMethod]
        public void WriteSetBrightness_DimmerClient()
        {
            lock (this.SerialLock)
            {
                using (var client = new DimmerPeripheralSerialClient(serialSettings))
                {
                    Assert.IsTrue(client.IsOpen);
                    var result = client.WriteBrightness(0);
                    Assert.IsTrue(result);
                }
            }
        }

        /// <summary>The write set dimmer scale.</summary>
        [TestMethod]
        public void WriteSetDimmerScale()
        {
            lock (this.SerialLock)
            {
                var m = new DimmerSetSensorScale(DimmerConstants.DefaultDimmerScale);
                Assert.AreEqual(DimmerConstants.DefaultDimmerScale, m.Scale);
                this.WriteSerialPeripheralTest<DimmerSetSensorScale>(DimmerMessageType.Ack);
            }
        }

        /// <summary>The write set sensor scale_ dimmer client.</summary>
        [TestMethod]
        public void WriteSetSensorScale_DimmerClient()
        {
            lock (this.SerialLock)
            {
                using (var client = new DimmerPeripheralSerialClient(serialSettings))
                {
                    Assert.IsTrue(client.IsOpen);
                    var result = client.WriteSensorScale(DimmerConstants.DefaultDimmerScale);
                    Assert.IsTrue(result);
                }
            }
        }

        [TestMethod]
        public void StartStopDimmerClientTimer()
        {
            lock (this.SerialLock)
            {
                var m = new CountdownEvent(2);
                var count = 0;
                using (var client = new DimmerPeripheralSerialClient(serialSettings))
                {
                    Assert.IsTrue(client.IsOpen);

                    client.DimmerSensorLevelsChanged += (sender, response) =>
                        {
                            Debug.WriteLine("*** Unit test got signaled ***");
                            count++;
                            m.Signal();
                        };

                    Debug.WriteLine("*** Unit test StartBackgroundProcessing() ***");
                    client.StartBackgroundProcessing(500);
                    var signaled = m.Wait(1000);
                    Assert.IsTrue(signaled);

                    // stop timer, events for DimmerSensorLevelsChanged should stop
                    Debug.WriteLine("*** Unit test StopBackgroundProcessing() ***");
                    client.StopBackgroundProcessing();
                    m.Reset();
               
                    signaled = m.Wait(1000);
                    Assert.IsFalse(signaled);
                    Assert.AreEqual(2, count);
                }
            }
        }

        [TestMethod]
        public void WriteWriteBrightnessLevels_DimmerClient()
        {
            lock (this.SerialLock)
            {
                var signaled = new ManualResetEvent(false);
                bool responseSignaled = false;
                using (var client = new DimmerPeripheralSerialClient(serialSettings))
                {
                    Assert.IsTrue(client.IsOpen);

                    const int MinimumPercentage = 10;
                    const int MaximumPercentage = 90;
                    var dimmerProcessor = new DimmerProcessor();
                    DimmerQueryResponse dimmerQueryResponse = null;

                    client.DimmerSensorLevelsChanged += (sender, response) =>
                        {
                            dimmerQueryResponse = response;
                            responseSignaled = signaled.Set();
                        };

                    var success = client.WriteQueryRequest();
                    Assert.IsTrue(success);

                    if (success && signaled.WaitOne(5000))
                    {
                        if (dimmerQueryResponse != null)
                        {
                            var calculateDimmerOutput = dimmerProcessor.CalculateDimmerOutput(
                                MinimumPercentage,
                                MaximumPercentage,
                                dimmerQueryResponse.LightLevel,
                                dimmerQueryResponse.LightSensorScale,
                                dimmerQueryResponse.Brightness);
                            Assert.IsNotNull(calculateDimmerOutput);
                            if (calculateDimmerOutput != null && calculateDimmerOutput.IsValid)
                            {
                                var dimmerBrightnessLevels = new DimmerBrightnessLevels(
                                    calculateDimmerOutput.BrightnessSetting,
                                    calculateDimmerOutput.RangeScale);
                                var intervalWriteDelay = calculateDimmerOutput.IntervalDelay;
                                var result = client.WriteBrightnessLevels(dimmerBrightnessLevels, intervalWriteDelay);
                                Assert.IsTrue(result);
                            }
                            else
                            {
                                Logger.Warn("CalculateDimmerOutput Ignored");
                            }
                        }
                    }
                    else
                    {
                        Assert.Fail("Failed to get Response");
                    }
                }
            }
        }    

        /// <summary>The write version request_ dimmer client.</summary>
        [TestMethod]
        public void WriteVersionRequest_DimmerClient()
        {
            lock (this.SerialLock)
            {
                using (var client = new DimmerPeripheralSerialClient(serialSettings))
                {
                    Assert.IsTrue(client.IsOpen);
                    var dimmerVersionInfo = client.WriteVersionRequest();
                    Assert.IsNotNull(dimmerVersionInfo, "dimmerVersionInfo == null");
                    var isConnected = client.IsConnected;
                    Assert.IsTrue(isConnected);
                    Assert.IsNotNull(client.VersionInfo);
                    Assert.IsFalse(string.IsNullOrEmpty(dimmerVersionInfo.HardwareVersion), "hardware version empty");
                    Assert.IsFalse(string.IsNullOrEmpty(dimmerVersionInfo.SoftwareVersion), "software version empty");
                    Debug.WriteLine("DimmerVersionInfo = " + dimmerVersionInfo.ToString());
                    Assert.IsNotNull(dimmerVersionInfo);
                }
            }
        }

        /// <summary>The write version request message.</summary>
        [TestMethod]
        public void WriteVersionRequestMessage()
        {
            lock (this.SerialLock)
            {
                this.WriteSerialPeripheralTest<DimmerVersionRequest>(DimmerMessageType.VersionResponse);
            }
        }

        /// <summary>The write version request simple.</summary>
        [TestMethod]
        public void WriteVersionRequestSimple()
        {
            var manualResetEvent = new ManualResetEvent(false);
            var serialDataReadyEvent = new ManualResetEvent(false);

            lock (this.SerialLock)
            {
                var serialPort = new SerialPort(TestComPort, 9600, Parity.None, 8, StopBits.One);
                serialPort.Open();
                var minMessageSize = PeripheralHeader.HeaderSize + (2 * sizeof(byte));
                serialPort.ReceivedBytesThreshold = minMessageSize;
                serialPort.DataReceived += (sender, args) =>
                    {
                        if (args.EventType == SerialData.Chars)
                        {
                            Debug.WriteLine("Data Received Event");
                            serialDataReadyEvent.Set();
                        }
                    };

                // test data to write requesting version
                var bytes = new byte[8] { 0x7E, 0x0, 0x6, 0x0, 0x0, 0x6, 0x10, 0xE4 };
                serialPort.Write(bytes, 0, bytes.Length);
                var buffer = new byte[2048];
                try
                {
                    serialPort.ReadTimeout = 1000;
                    var memoryStream = new MemoryStream();

                    if (serialDataReadyEvent.WaitOne(5000))
                    {
                        int bytesRead;
                        do
                        {
                            try
                            {
                                Debug.WriteLine("Bytes Ready to read " + serialPort.BytesToRead);
                                bytesRead = serialPort.Read(buffer, 0, buffer.Length);
                                if (bytesRead > 0)
                                {
                                    serialPort.ReadTimeout = 100;
                                    var tempBytes = buffer.Take(bytesRead).ToArray();
                                    Debug.WriteLine("Serial Read Success bytes read = " + tempBytes.Length);
                                    memoryStream.Write(tempBytes, 0, tempBytes.Length);
                                }
                            }
                            catch (TimeoutException timeout)
                            {
                                Debug.WriteLine(timeout.Message);
                                break;
                            }
                        }
                        while (bytesRead > 0);
                    }

                    var finalBytes = memoryStream.ToArray();
                    var debugText = finalBytes.Aggregate(string.Empty, (current, b) => current + string.Format("0x{0:X},", b));
                    Debug.Write(debugText);

                    manualResetEvent.Set();
                }
                catch (Exception ex)
                {
                    Debug.Fail(ex.Message);
                }
                finally
                {
                    Debug.WriteLine("\nEnd Test");
                    serialPort.Close();
                    var signaled = manualResetEvent.WaitOne(1000);
                    Assert.IsTrue(signaled);
                }
            }
        }

        #endregion

        #region Methods

        private static void DumpDebug<T>(T model)
        {
            Debug.WriteLine("Debug Model Dump of " + typeof(T));
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(model))
            {
                string name = descriptor.Name;
                object value = descriptor.GetValue(model);
                Debug.Write(string.Format("{0}={1},", name, value));
            }

            Debug.WriteLine("End Dump");
        }

        private static bool VerifyComPortExists(string comPort)
        {
            var portExists = SerialPortNames.Contains(comPort);
            return portExists;
        }

        private void AssertInvalidComPort(string portName)
        {
            Assert.IsNotNull(SerialPortNames);
            var found = SerialPortNames.Contains(portName);
            Assert.IsTrue(found, "Serial Port Not found " + portName);
        }

        #endregion
    }
}