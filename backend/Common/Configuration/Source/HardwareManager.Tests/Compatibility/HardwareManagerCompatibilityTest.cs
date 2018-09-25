// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HardwareManagerCompatibilityTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HardwareManagerCompatibilityTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareManager.Tests.Compatibility
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.IO.Ports;
    using System.Runtime.Serialization.Formatters.Binary;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.HardwareManager.Gps;
    using Gorba.Common.Configuration.HardwareManager.Mgi;
    using Gorba.Common.Configuration.HardwareManager.Vdv301;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The System Manager configuration file compatibility test.
    /// </summary>
    [TestClass]
    public class HardwareManagerCompatibilityTest
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Tests <c>HardwareManager.xml</c> version 1.2.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\HardwareManager_v1.2.xml")]
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test code")]
        public void TestV1_2()
        {
            var configManager = new ConfigManager<HardwareManagerConfig>();
            configManager.FileName = "HardwareManager_v1.2.xml";
            configManager.XmlSchema = HardwareManagerConfig.Schema;
            var config = configManager.Config;

            Assert.IsNotNull(config);
            Assert.IsTrue(config.BroadcastTimeChanges);

            Assert.IsNotNull(config.Mgi);
            Assert.IsTrue(config.Mgi.Enabled);
            Assert.AreEqual(TimeSpan.FromSeconds(0.1), config.Mgi.PollingInterval);
            Assert.AreEqual(8, config.Mgi.Gpio.Pins.Count);
            Assert.AreEqual(0, config.Mgi.Gpio.Pins[0].Index);
            Assert.AreEqual(1, config.Mgi.Gpio.Pins[1].Index);
            Assert.AreEqual(2, config.Mgi.Gpio.Pins[2].Index);
            Assert.AreEqual(3, config.Mgi.Gpio.Pins[3].Index);
            Assert.AreEqual(4, config.Mgi.Gpio.Pins[4].Index);
            Assert.AreEqual(5, config.Mgi.Gpio.Pins[5].Index);
            Assert.AreEqual(6, config.Mgi.Gpio.Pins[6].Index);
            Assert.AreEqual(7, config.Mgi.Gpio.Pins[7].Index);
            Assert.AreEqual("Pin0", config.Mgi.Gpio.Pins[0].Name);
            Assert.AreEqual("Pin1", config.Mgi.Gpio.Pins[1].Name);
            Assert.AreEqual("Pin2", config.Mgi.Gpio.Pins[2].Name);
            Assert.AreEqual("Pin3", config.Mgi.Gpio.Pins[3].Name);
            Assert.AreEqual("Pin4", config.Mgi.Gpio.Pins[4].Name);
            Assert.AreEqual("Pin5", config.Mgi.Gpio.Pins[5].Name);
            Assert.AreEqual("Pin6", config.Mgi.Gpio.Pins[6].Name);
            Assert.AreEqual("Pin7", config.Mgi.Gpio.Pins[7].Name);
            Assert.AreEqual("Button", config.Mgi.Button);
            Assert.AreEqual("UpdateLed", config.Mgi.UpdateLed);
            Assert.AreEqual(CompactRs485Switch.At91, config.Mgi.Rs485Interface);

            Assert.AreEqual(2, config.Mgi.DviLevelShifters.Count);
            var dviLevelShifter = config.Mgi.DviLevelShifters[0];
            Assert.AreEqual(1, dviLevelShifter.Index);
            Assert.AreEqual(TrimOptions.StandardCurrent, dviLevelShifter.Trim);
            Assert.AreEqual(0, dviLevelShifter.OutputLevel);

            dviLevelShifter = config.Mgi.DviLevelShifters[1];
            Assert.AreEqual(2, dviLevelShifter.Index);
            Assert.AreEqual(TrimOptions.StandardCurrent, dviLevelShifter.Trim);
            Assert.AreEqual(0, dviLevelShifter.OutputLevel);

            Assert.AreEqual(2, config.Mgi.Transceivers.Count);
            var transceiver = config.Mgi.Transceivers[0];
            Assert.AreEqual(1, transceiver.Index);
            Assert.AreEqual(TransceiverType.RS485, transceiver.Type);
            Assert.IsFalse(transceiver.Termination);
            Assert.AreEqual(TransceiverMode.HalfDuplex, transceiver.Mode);

            transceiver = config.Mgi.Transceivers[1];
            Assert.AreEqual(2, transceiver.Index);
            Assert.AreEqual(TransceiverType.RS485, transceiver.Type);
            Assert.IsFalse(transceiver.Termination);
            Assert.AreEqual(TransceiverMode.HalfDuplex, transceiver.Mode);

            Assert.IsFalse(config.Sntp.Enabled);
            Assert.AreEqual("2.europe.pool.ntp.org", config.Sntp.Host);
            Assert.AreEqual(123, config.Sntp.Port);
            Assert.AreEqual(SntpVersionNumber.Version4, config.Sntp.VersionNumber);

            Assert.IsFalse(config.Vdv301.Enabled);
            Assert.AreEqual(DeviceClass.InteriorDisplay, config.Vdv301.DeviceClass);
            Assert.IsFalse(config.Vdv301.TimeSync.Enabled);
            Assert.AreEqual(SntpVersionNumber.Version3, config.Vdv301.TimeSync.VersionNumber);

            Assert.AreEqual(1, config.Settings.Count);
            var setting = config.Settings[0];
            Assert.AreEqual(0, setting.Conditions.Count);
            Assert.AreEqual(HostnameSource.MacAddress, setting.HostnameSource);
            Assert.AreEqual("UTC", setting.TimeZone);
        }

        /// <summary>
        /// Tests <c>HardwareManager.xml</c> version 1.4.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\HardwareManager_v1.4.xml")]
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test code")]
        public void TestV1_4()
        {
            var configManager = new ConfigManager<HardwareManagerConfig>();
            configManager.FileName = "HardwareManager_v1.4.xml";
            configManager.XmlSchema = HardwareManagerConfig.Schema;
            var config = configManager.Config;

            Assert.IsNotNull(config);
            Assert.IsTrue(config.BroadcastTimeChanges);

            Assert.IsNotNull(config.Mgi);
            Assert.IsTrue(config.Mgi.Enabled);
            Assert.AreEqual(TimeSpan.FromSeconds(0.1), config.Mgi.PollingInterval);
            Assert.AreEqual(8, config.Mgi.Gpio.Pins.Count);
            Assert.AreEqual(0, config.Mgi.Gpio.Pins[0].Index);
            Assert.AreEqual(1, config.Mgi.Gpio.Pins[1].Index);
            Assert.AreEqual(2, config.Mgi.Gpio.Pins[2].Index);
            Assert.AreEqual(3, config.Mgi.Gpio.Pins[3].Index);
            Assert.AreEqual(4, config.Mgi.Gpio.Pins[4].Index);
            Assert.AreEqual(5, config.Mgi.Gpio.Pins[5].Index);
            Assert.AreEqual(6, config.Mgi.Gpio.Pins[6].Index);
            Assert.AreEqual(7, config.Mgi.Gpio.Pins[7].Index);
            Assert.AreEqual("Pin0", config.Mgi.Gpio.Pins[0].Name);
            Assert.AreEqual("Pin1", config.Mgi.Gpio.Pins[1].Name);
            Assert.AreEqual("Pin2", config.Mgi.Gpio.Pins[2].Name);
            Assert.AreEqual("Pin3", config.Mgi.Gpio.Pins[3].Name);
            Assert.AreEqual("Pin4", config.Mgi.Gpio.Pins[4].Name);
            Assert.AreEqual("Pin5", config.Mgi.Gpio.Pins[5].Name);
            Assert.AreEqual("Pin6", config.Mgi.Gpio.Pins[6].Name);
            Assert.AreEqual("Pin7", config.Mgi.Gpio.Pins[7].Name);
            Assert.AreEqual("Button", config.Mgi.Button);
            Assert.AreEqual("UpdateLed", config.Mgi.UpdateLed);
            Assert.AreEqual(CompactRs485Switch.At91, config.Mgi.Rs485Interface);

            Assert.AreEqual(1, config.Mgi.DviLevelShifters.Count);
            var dviLevelShifter = config.Mgi.DviLevelShifters[0];
            Assert.AreEqual(2, dviLevelShifter.Index);
            Assert.AreEqual(TrimOptions.StandardCurrent, dviLevelShifter.Trim);
            Assert.AreEqual(0, dviLevelShifter.OutputLevel);

            Assert.AreEqual(0, config.Mgi.Transceivers.Count);

            Assert.IsFalse(config.Sntp.Enabled);
            Assert.AreEqual("2.europe.pool.ntp.org", config.Sntp.Host);
            Assert.AreEqual(123, config.Sntp.Port);
            Assert.AreEqual(SntpVersionNumber.Version4, config.Sntp.VersionNumber);

            Assert.IsFalse(config.Vdv301.Enabled);
            Assert.AreEqual(DeviceClass.InteriorDisplay, config.Vdv301.DeviceClass);
            Assert.IsFalse(config.Vdv301.TimeSync.Enabled);
            Assert.AreEqual(SntpVersionNumber.Version3, config.Vdv301.TimeSync.VersionNumber);

            Assert.AreEqual(1, config.Settings.Count);
            var setting = config.Settings[0];
            Assert.AreEqual(0, setting.Conditions.Count);
            Assert.AreEqual(HostnameSource.MacAddress, setting.HostnameSource);
            Assert.AreEqual("UTC", setting.TimeZone);
        }

        /// <summary>
        /// Tests <c>HardwareManager.xml</c> version 1.6.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\HardwareManager_v1.6.xml")]
        public void TestV1_6()
        {
            var configManager = new ConfigManager<HardwareManagerConfig>();
            configManager.FileName = "HardwareManager_v1.6.xml";
            configManager.XmlSchema = HardwareManagerConfig.Schema;
            var config = configManager.Config;

            Assert.IsTrue(config.BroadcastTimeChanges);

            Assert.IsNotNull(config.Mgi);
            Assert.IsTrue(config.Mgi.Enabled);
            Assert.AreEqual(TimeSpan.FromSeconds(0.1), config.Mgi.PollingInterval);
            Assert.AreEqual(8, config.Mgi.Gpio.Pins.Count);
            Assert.AreEqual(0, config.Mgi.Gpio.Pins[0].Index);
            Assert.AreEqual(1, config.Mgi.Gpio.Pins[1].Index);
            Assert.AreEqual(2, config.Mgi.Gpio.Pins[2].Index);
            Assert.AreEqual(3, config.Mgi.Gpio.Pins[3].Index);
            Assert.AreEqual(4, config.Mgi.Gpio.Pins[4].Index);
            Assert.AreEqual(5, config.Mgi.Gpio.Pins[5].Index);
            Assert.AreEqual(6, config.Mgi.Gpio.Pins[6].Index);
            Assert.AreEqual(7, config.Mgi.Gpio.Pins[7].Index);
            Assert.AreEqual("Pin0", config.Mgi.Gpio.Pins[0].Name);
            Assert.AreEqual("Pin1", config.Mgi.Gpio.Pins[1].Name);
            Assert.AreEqual("Pin2", config.Mgi.Gpio.Pins[2].Name);
            Assert.AreEqual("Pin3", config.Mgi.Gpio.Pins[3].Name);
            Assert.AreEqual("Pin4", config.Mgi.Gpio.Pins[4].Name);
            Assert.AreEqual("Pin5", config.Mgi.Gpio.Pins[5].Name);
            Assert.AreEqual("Pin6", config.Mgi.Gpio.Pins[6].Name);
            Assert.AreEqual("Pin7", config.Mgi.Gpio.Pins[7].Name);
            Assert.AreEqual("Button", config.Mgi.Button);
            Assert.AreEqual("UpdateLed", config.Mgi.UpdateLed);
            Assert.AreEqual(CompactRs485Switch.At91, config.Mgi.Rs485Interface);

            Assert.AreEqual(1, config.Mgi.DviLevelShifters.Count);
            var dviLevelShifter = config.Mgi.DviLevelShifters[0];
            Assert.AreEqual(2, dviLevelShifter.Index);
            Assert.AreEqual(TrimOptions.StandardCurrent, dviLevelShifter.Trim);
            Assert.AreEqual(0, dviLevelShifter.OutputLevel);

            Assert.AreEqual(0, config.Mgi.Transceivers.Count);

            Assert.AreEqual(100, config.Mgi.BacklightControlRate.Minimum);
            Assert.AreEqual(150, config.Mgi.BacklightControlRate.Maximum);
            Assert.AreEqual(8, config.Mgi.BacklightControlRate.Speed);

            Assert.IsFalse(config.Sntp.Enabled);
            Assert.AreEqual("2.europe.pool.ntp.org", config.Sntp.Host);
            Assert.AreEqual(123, config.Sntp.Port);
            Assert.AreEqual(SntpVersionNumber.Version4, config.Sntp.VersionNumber);

            Assert.IsFalse(config.Vdv301.Enabled);
            Assert.AreEqual(DeviceClass.InteriorDisplay, config.Vdv301.DeviceClass);
            Assert.IsFalse(config.Vdv301.TimeSync.Enabled);
            Assert.AreEqual(SntpVersionNumber.Version3, config.Vdv301.TimeSync.VersionNumber);

            Assert.AreEqual(1, config.Settings.Count);
            var setting = config.Settings[0];
            Assert.AreEqual(0, setting.Conditions.Count);
            Assert.AreEqual(HostnameSource.MacAddress, setting.HostnameSource);
            Assert.AreEqual("UTC", setting.TimeZone);

            Assert.IsNotNull(config.Gps);
            Assert.AreEqual(GpsConnectionType.GpsSerial, config.Gps.ConnectionType);
            Assert.IsTrue(config.Gps.Client.Enabled);
            Assert.AreEqual("127.0.0.1", ((GpsPilotConfig)config.Gps.Client).IpAddress);
            Assert.AreEqual(1599, ((GpsPilotConfig)config.Gps.Client).Port);
            Assert.IsTrue(config.Gps.GpsSerialClient.Enabled);
            Assert.AreEqual("COM1", config.Gps.GpsSerialClient.GpsSerialPort.ComPort);
            Assert.AreEqual(115200, config.Gps.GpsSerialClient.GpsSerialPort.BaudRate);
            Assert.AreEqual(8, config.Gps.GpsSerialClient.GpsSerialPort.DataBits);
            Assert.IsTrue(config.Gps.GpsSerialClient.GpsSerialPort.FParity);
            Assert.AreEqual(StopBits.Two, config.Gps.GpsSerialClient.GpsSerialPort.StopBits);
            Assert.AreEqual(Parity.Mark, config.Gps.GpsSerialClient.GpsSerialPort.Parity);
            Assert.IsTrue(config.Gps.GpsSerialClient.GpsSerialPort.DtrControl);
            Assert.IsTrue(config.Gps.GpsSerialClient.GpsSerialPort.RtsControl);
            Assert.AreEqual<uint>(100, config.Gps.GpsSerialClient.GpsSerialPort.ReadIntervalTimeout);
            Assert.AreEqual<uint>(10, config.Gps.GpsSerialClient.GpsSerialPort.ReadTotalTimeout);
            Assert.AreEqual<uint>(101, config.Gps.GpsSerialClient.GpsSerialPort.ReadTotalMultiplierTimeout);
            Assert.AreEqual<uint>(11, config.Gps.GpsSerialClient.GpsSerialPort.WriteTotalTimeout);
            Assert.AreEqual<uint>(20, config.Gps.GpsSerialClient.GpsSerialPort.WriteMultiplierTimeout);
        }

        /// <summary>
        /// Tests that <c>SystemManager.xml</c> can be saved with binary serialization (used for .cache file).
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\HardwareManager_v1.6.xml")]
        public void TestBinarySerialization()
        {
            var configManager = new ConfigManager<HardwareManagerConfig>();
            configManager.FileName = "HardwareManager_v1.6.xml";
            configManager.XmlSchema = HardwareManagerConfig.Schema;
            var config = configManager.Config;

            Assert.IsNotNull(config);
            var memory = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(memory, config);
            Assert.IsTrue(memory.Position > 0);
        }

        // ReSharper restore InconsistentNaming
    }
}
