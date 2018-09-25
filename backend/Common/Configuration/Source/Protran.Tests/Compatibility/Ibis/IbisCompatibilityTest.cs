// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisCompatibilityTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisCompatibilityTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Tests.Compatibility.Ibis
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Ports;
    using System.Runtime.Serialization.Formatters.Binary;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Configuration.Protran.Transformations;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The IBIS config file compatibility test.
    /// </summary>
    [TestClass]
    public class IbisCompatibilityTest
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Tests <c>ibis.xml</c> version 2.2.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\Ibis\ibis_v2.2.xml")]
        public void TestV2_2()
        {
            var configManager = new ConfigManager<IbisConfig>();
            configManager.FileName = "ibis_v2.2.xml";
            configManager.XmlSchema = IbisConfig.Schema;
            var config = configManager.Config;

            Assert.AreEqual(1, config.Behaviour.IbisAddresses.Count);
            Assert.AreEqual(8, config.Behaviour.IbisAddresses[0]);
            Assert.AreEqual(TimeSpan.FromSeconds(60), config.Behaviour.ConnectionTimeOut);
            AssertAreEqual("0", "SystemStatus", "RemotePC", "0", config.Behaviour.ConnectionStatusUsedFor);
            Assert.IsTrue(config.Behaviour.CheckCrc);
            Assert.AreEqual(ByteType.Ascii7, config.Behaviour.ByteType);
            Assert.AreEqual(ProcessPriorityClass.AboveNormal, config.Behaviour.ProcessPriority);

            Assert.AreEqual(IbisSourceType.SerialPort, config.Sources.Active);
            Assert.AreEqual("./ibisSimulation2.log", config.Sources.Simulation.SimulationFile);
            Assert.AreEqual(TimeSpan.FromSeconds(5), config.Sources.Simulation.InitialDelay);
            Assert.AreEqual(TimeSpan.FromSeconds(1), config.Sources.Simulation.IntervalBetweenTelegrams);
            Assert.AreEqual(2, config.Sources.Simulation.TimesToRepeat);

            Assert.AreEqual("COM1", config.Sources.SerialPort.ComPort);
            Assert.AreEqual(1200, config.Sources.SerialPort.BaudRate);
            Assert.AreEqual(7, config.Sources.SerialPort.DataBits);
            Assert.AreEqual(StopBits.Two, config.Sources.SerialPort.StopBits);
            Assert.AreEqual(Parity.Even, config.Sources.SerialPort.Parity);
            Assert.AreEqual(10, config.Sources.SerialPort.RetryCount);
            Assert.AreEqual(config.Sources.SerialPort.SerialPortReopen, SerialPortReopen.FrameOnly);

            Assert.AreEqual(47555, config.Sources.UdpServer.LocalPort);
            Assert.AreEqual(TelegramFormat.NoChecksum, config.Sources.UdpServer.ReceiveFormat);
            Assert.AreEqual(TelegramFormat.NoFooter, config.Sources.UdpServer.SendFormat);

            Assert.AreEqual("127.0.0.1", config.Sources.Json.IpAddress);
            Assert.AreEqual(3011, config.Sources.Json.Port);

            Assert.IsFalse(config.Recording.Active);
            Assert.AreEqual(RecordingFormat.Protran, config.Recording.Format);
            Assert.AreEqual("./ibis.log", config.Recording.FileAbsPath);

            Assert.IsTrue(config.TimeSync.Enabled);
            Assert.AreEqual(TimeSpan.FromSeconds(10), config.TimeSync.InitialDelay);
            Assert.AreEqual(3, config.TimeSync.WaitTelegrams);
            Assert.AreEqual(TimeSpan.Zero, config.TimeSync.Tolerance);

            Assert.AreEqual(26, config.Telegrams.Count);

            // we are not testing all telegrams, that would be too much effort for not much
            var telegram = config.Telegrams[17] as DS080Config;
            Assert.IsNotNull(telegram);
            Assert.IsTrue(telegram.Enabled);
            Assert.AreEqual("1", telegram.OpenValue);
            Assert.AreEqual("0", telegram.CloseValue);
            Assert.IsTrue(telegram.ResetWithDS010B);
            AssertAreEqual("0", "SystemStatus", "DoorStatus", "0", telegram.UsedFor);

            Assert.AreEqual(13, config.Transformations.Count);

            // we are not testing all transformations, that would be too much effort for not much
            var chain = config.Transformations[7];
            Assert.AreEqual(3, chain.Transformations.Count);

            var lawo = chain.Transformations[0] as LawoString;
            Assert.IsNotNull(lawo);
            Assert.AreEqual(858, lawo.CodePage);

            var regexDivider = chain.Transformations[1] as RegexDivider;
            Assert.IsNotNull(regexDivider);
            Assert.AreEqual(@"[\u0003-\u0005]", regexDivider.Regex);

            var stringMapping = chain.Transformations[2] as StringMapping;
            Assert.IsNotNull(stringMapping);
            Assert.AreEqual(7, stringMapping.Mappings.Count);
        }

        /// <summary>
        /// Tests <c>ibis.xml</c> version 2.4.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\Ibis\ibis_v2.4.xml")]
        public void TestV2_4()
        {
            var configManager = new ConfigManager<IbisConfig>();
            configManager.FileName = "ibis_v2.4.xml";
            configManager.XmlSchema = IbisConfig.Schema;
            var config = configManager.Config;

            Assert.AreEqual(1, config.Behaviour.IbisAddresses.Count);
            Assert.AreEqual(8, config.Behaviour.IbisAddresses[0]);
            Assert.AreEqual(TimeSpan.FromSeconds(60), config.Behaviour.ConnectionTimeOut);
            AssertAreEqual("0", "SystemStatus", "RemotePC", "0", config.Behaviour.ConnectionStatusUsedFor);
            Assert.IsTrue(config.Behaviour.CheckCrc);
            Assert.AreEqual(ByteType.Ascii7, config.Behaviour.ByteType);
            Assert.AreEqual(ProcessPriorityClass.AboveNormal, config.Behaviour.ProcessPriority);

            Assert.AreEqual(IbisSourceType.SerialPort, config.Sources.Active);
            Assert.AreEqual("./ibisSimulation2.log", config.Sources.Simulation.SimulationFile);
            Assert.AreEqual(TimeSpan.FromSeconds(5), config.Sources.Simulation.InitialDelay);
            Assert.AreEqual(TimeSpan.FromSeconds(1), config.Sources.Simulation.IntervalBetweenTelegrams);
            Assert.AreEqual(2, config.Sources.Simulation.TimesToRepeat);

            Assert.AreEqual("COM1", config.Sources.SerialPort.ComPort);
            Assert.AreEqual(1200, config.Sources.SerialPort.BaudRate);
            Assert.AreEqual(7, config.Sources.SerialPort.DataBits);
            Assert.AreEqual(StopBits.Two, config.Sources.SerialPort.StopBits);
            Assert.AreEqual(Parity.Even, config.Sources.SerialPort.Parity);
            Assert.AreEqual(10, config.Sources.SerialPort.RetryCount);
            Assert.AreEqual(config.Sources.SerialPort.SerialPortReopen, SerialPortReopen.FrameOnly);

            Assert.AreEqual(47555, config.Sources.UdpServer.LocalPort);
            Assert.AreEqual(TelegramFormat.NoChecksum, config.Sources.UdpServer.ReceiveFormat);
            Assert.AreEqual(TelegramFormat.NoFooter, config.Sources.UdpServer.SendFormat);

            Assert.AreEqual("127.0.0.1", config.Sources.Json.IpAddress);
            Assert.AreEqual(3011, config.Sources.Json.Port);

            Assert.IsFalse(config.Recording.Active);
            Assert.AreEqual(RecordingFormat.Protran, config.Recording.Format);
            Assert.AreEqual("./ibis.log", config.Recording.FileAbsPath);

            Assert.IsTrue(config.TimeSync.Enabled);
            Assert.AreEqual(TimeSpan.FromSeconds(10), config.TimeSync.InitialDelay);
            Assert.AreEqual(3, config.TimeSync.WaitTelegrams);
            Assert.AreEqual(TimeSpan.Zero, config.TimeSync.Tolerance);

            Assert.AreEqual(26, config.Telegrams.Count);

            // we are not testing all telegrams, that would be too much effort for not much
            var telegram = config.Telegrams[17] as DS080Config;
            Assert.IsNotNull(telegram);
            Assert.IsTrue(telegram.Enabled);
            Assert.AreEqual("1", telegram.OpenValue);
            Assert.AreEqual("0", telegram.CloseValue);
            Assert.IsTrue(telegram.ResetWithDS010B);
            AssertAreEqual("0", "SystemStatus", "DoorStatus", "0", telegram.UsedFor);

            Assert.AreEqual(13, config.Transformations.Count);

            // we are not testing all transformations, that would be too much effort for not much
            var chain = config.Transformations[7];
            Assert.AreEqual(3, chain.Transformations.Count);

            var lawo = chain.Transformations[0] as LawoString;
            Assert.IsNotNull(lawo);
            Assert.AreEqual(858, lawo.CodePage);

            var regexDivider = chain.Transformations[1] as RegexDivider;
            Assert.IsNotNull(regexDivider);
            Assert.AreEqual(@"[\u0003-\u0005]", regexDivider.Regex);

            var stringMapping = chain.Transformations[2] as StringMapping;
            Assert.IsNotNull(stringMapping);
            Assert.AreEqual(7, stringMapping.Mappings.Count);
        }

        /// <summary>
        /// Tests that <c>AhdlcRenderer.xml</c> can be saved with binary serialization (used for .cache file).
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\Ibis\ibis_v2.4.xml")]
        public void TestBinarySerialization()
        {
            var configManager = new ConfigManager<IbisConfig>();
            configManager.FileName = "ibis_v2.4.xml";
            configManager.XmlSchema = IbisConfig.Schema;
            var config = configManager.Config;

            Assert.IsNotNull(config);
            var memory = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(memory, config);
            Assert.IsTrue(memory.Position > 0);
        }

        private static void AssertAreEqual(string lang, string table, string column, string row, GenericUsage actual)
        {
            Assert.AreEqual(lang, actual.Language);
            Assert.AreEqual(table, actual.Table);
            Assert.AreEqual(column, actual.Column);
            Assert.AreEqual(row, actual.Row);
            Assert.AreEqual(0, actual.RowOffset);
        }

        // ReSharper restore InconsistentNaming
    }
}
