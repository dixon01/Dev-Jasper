// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigSerializationTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConfigSerializationTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.IO.Tests.Config
{
    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Protran.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The config serialization test.
    /// </summary>
    [TestClass]
    public class ConfigSerializationTest
    {
        /// <summary>
        /// Tests the deserialization of io.xml.
        /// </summary>
        [TestMethod]
        [DeploymentItem("Config\\io1.xml")]
        public void TestDeserialization()
        {
            var configMgr = new ConfigManager<IOProtocolConfig> { FileName = "io1.xml" };
            var config = configMgr.Config;

            Assert.IsNotNull(config);

            Assert.IsNotNull(config.SerialPorts);
            Assert.AreEqual(2, config.SerialPorts.Count);

            var serialPort = config.SerialPorts[0];
            Assert.IsFalse(serialPort.Enabled);
            Assert.AreEqual("COM1", serialPort.Name);
            Assert.AreEqual("Speaker", serialPort.Rts);
            Assert.AreEqual("SpecialInput", serialPort.Cts);
            Assert.AreEqual("COM1.DTR", serialPort.Dtr);
            Assert.AreEqual("COM1.DSR", serialPort.Dsr);

            serialPort = config.SerialPorts[1];
            Assert.IsTrue(serialPort.Enabled);
            Assert.AreEqual("COM2", serialPort.Name);
            Assert.AreEqual("COM2.RTS", serialPort.Rts);
            Assert.AreEqual("StopRequest", serialPort.Cts);
            Assert.AreEqual("COM2.DTR", serialPort.Dtr);
            Assert.AreEqual("COM2.DSR", serialPort.Dsr);

            Assert.IsNotNull(config.Inputs);
            Assert.AreEqual(2, config.Inputs.Count);

            var input = config.Inputs[0];
            Assert.IsNull(input.Unit);
            Assert.AreEqual("Protran", input.Application);
            Assert.AreEqual("StopRequest", input.Name);
            Assert.AreEqual("Default", input.TransfRef);
            Assert.IsTrue(input.Enabled);
            var usage = input.UsedFor;
            Assert.IsNotNull(usage);
            Assert.AreEqual("SystemStatus", usage.Table);
            Assert.AreEqual("StopRequestedState", usage.Column);
            Assert.AreEqual("0", usage.Row);

            input = config.Inputs[1];
            Assert.AreEqual("TFT-2", input.Unit);
            Assert.IsNull(input.Application);
            Assert.AreEqual("SpecialInput", input.Name);
            Assert.AreEqual("Default", input.TransfRef);
            Assert.IsFalse(input.Enabled);
            usage = input.UsedFor;
            Assert.IsNotNull(usage);
            Assert.AreEqual("SystemStatus", usage.Table);
            Assert.AreEqual("SpecialInput", usage.Column);
            Assert.AreEqual("0", usage.Row);

            Assert.IsNotNull(config.Transformations);
            Assert.AreEqual(1, config.Transformations.Count);

            var chain = config.Transformations[0];
            Assert.AreEqual("Default", chain.Id);
            Assert.IsNotNull(chain.Transformations);
            Assert.AreEqual(0, chain.Transformations.Count);
        }
    }
}
