// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoCompatibilityTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IoCompatibilityTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Tests.Compatibility.IO
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The I/O config file compatibility test.
    /// </summary>
    [TestClass]
    public class IoCompatibilityTest
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Tests <c>io.xml</c> version 2.2.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\IO\io_v2.2.xml")]
        public void TestV2_2()
        {
            var configManager = new ConfigManager<IOProtocolConfig>();
            configManager.FileName = "io_v2.2.xml";
            configManager.XmlSchema = IOProtocolConfig.Schema;
            var config = configManager.Config;

            Assert.AreEqual(2, config.SerialPorts.Count);

            var serialPort = config.SerialPorts[0];
            Assert.IsTrue(serialPort.Enabled);
            Assert.AreEqual("COM21", serialPort.Name);
            Assert.AreEqual("Speaker", serialPort.Rts);
            Assert.AreEqual("SpecialInput", serialPort.Cts);
            Assert.IsNull(serialPort.Dsr);
            Assert.IsNull(serialPort.Dtr);

            serialPort = config.SerialPorts[1];
            Assert.IsTrue(serialPort.Enabled);
            Assert.AreEqual("COM22", serialPort.Name);
            Assert.IsNull(serialPort.Rts);
            Assert.AreEqual("StopRequest", serialPort.Cts);
            Assert.IsNull(serialPort.Dsr);
            Assert.IsNull(serialPort.Dtr);

            Assert.AreEqual(2, config.Inputs.Count);

            var input = config.Inputs[0];
            Assert.AreEqual("StopRequest", input.Name);
            Assert.AreEqual("Default", input.TransfRef);
            Assert.IsTrue(input.Enabled);
            AssertAreEqual("0", "SystemStatus", "StopRequestedState", "0", input.UsedFor);

            input = config.Inputs[1];
            Assert.AreEqual("SpecialInput", input.Name);
            Assert.AreEqual("Default", input.TransfRef);
            Assert.IsTrue(input.Enabled);
            AssertAreEqual("0", "SystemStatus", "SpecialInput", "0", input.UsedFor);

            Assert.AreEqual(1, config.Transformations.Count);

            var chain = config.Transformations[0];
            Assert.AreEqual("Default", chain.Id);
            Assert.AreEqual(0, chain.Transformations.Count);
        }

        /// <summary>
        /// Tests <c>io.xml</c> version 2.2.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\IO\io_v2.4.xml")]
        public void TestV2_4()
        {
            var configManager = new ConfigManager<IOProtocolConfig>();
            configManager.FileName = "io_v2.4.xml";
            configManager.XmlSchema = IOProtocolConfig.Schema;
            var config = configManager.Config;

            Assert.AreEqual(2, config.SerialPorts.Count);

            var serialPort = config.SerialPorts[0];
            Assert.IsTrue(serialPort.Enabled);
            Assert.AreEqual("COM21", serialPort.Name);
            Assert.AreEqual("Speaker", serialPort.Rts);
            Assert.AreEqual("SpecialInput", serialPort.Cts);
            Assert.IsNull(serialPort.Dsr);
            Assert.IsNull(serialPort.Dtr);

            serialPort = config.SerialPorts[1];
            Assert.IsTrue(serialPort.Enabled);
            Assert.AreEqual("COM22", serialPort.Name);
            Assert.IsNull(serialPort.Rts);
            Assert.AreEqual("StopRequest", serialPort.Cts);
            Assert.IsNull(serialPort.Dsr);
            Assert.IsNull(serialPort.Dtr);

            Assert.AreEqual(2, config.Inputs.Count);

            var input = config.Inputs[0];
            Assert.AreEqual("StopRequest", input.Name);
            Assert.AreEqual("Default", input.TransfRef);
            Assert.IsTrue(input.Enabled);
            AssertAreEqual("0", "SystemStatus", "StopRequestedState", "0", input.UsedFor);

            input = config.Inputs[1];
            Assert.AreEqual("SpecialInput", input.Name);
            Assert.AreEqual("Default", input.TransfRef);
            Assert.IsTrue(input.Enabled);
            AssertAreEqual("0", "SystemStatus", "SpecialInput", "0", input.UsedFor);

            Assert.AreEqual(1, config.Transformations.Count);

            var chain = config.Transformations[0];
            Assert.AreEqual("Default", chain.Id);
            Assert.AreEqual(0, chain.Transformations.Count);
        }

        /// <summary>
        /// Tests that <c>AhdlcRenderer.xml</c> can be saved with binary serialization (used for .cache file).
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\IO\io_v2.4.xml")]
        public void TestBinarySerialization()
        {
            var configManager = new ConfigManager<IOProtocolConfig>();
            configManager.FileName = "io_v2.4.xml";
            configManager.XmlSchema = IOProtocolConfig.Schema;
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
