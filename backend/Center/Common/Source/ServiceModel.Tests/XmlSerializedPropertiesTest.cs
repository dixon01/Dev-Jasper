// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlSerializedPropertiesTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the XmlSerializedPropertiesTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Tests
{
    using System;
    using System.Xml.Serialization;

    using Gorba.Center.Common.ServiceModel.Update;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for the serialization/deserialization of complex objects in DTOs.
    /// </summary>
    [TestClass]
    public class XmlSerializedPropertiesTest
    {
        /// <summary>
        /// Tests the usage of xml serialized properties.
        /// </summary>
        [TestMethod]
        public void TestXmlSerializedProperties()
        {
            var updateCommand = new UpdateCommand { Command = new XmlData(new UpdateCommandMsg { UnitId = 1 }) };
            var xmlPropertyIsValid = !string.IsNullOrEmpty(updateCommand.CommandXml);
            Assert.IsTrue(xmlPropertyIsValid);
            var isTypeValid = !string.IsNullOrEmpty(updateCommand.CommandXmlType);
            Assert.IsTrue(isTypeValid);
            var type = Type.GetType(updateCommand.CommandXmlType);
            var isTypeCorrect = type == typeof(UpdateCommandMsg);
            Assert.IsTrue(isTypeCorrect);

            var newCommand = new UpdateCommand();

            // settings Xml first
            newCommand.CommandXml = updateCommand.CommandXml;
            newCommand.CommandXmlType = updateCommand.CommandXmlType;
            Assert.IsNotNull(newCommand.Command);
            Assert.AreEqual(1, ((UpdateCommandMsg)newCommand.Command.Deserialize()).UnitId);

            newCommand = new UpdateCommand();

            // setting XmlType first
            newCommand.CommandXmlType = updateCommand.CommandXmlType;
            newCommand.CommandXml = updateCommand.CommandXml;
            Assert.IsNotNull(newCommand.Command);
            Assert.AreEqual(1, ((UpdateCommandMsg)newCommand.Command.Deserialize()).UnitId);
        }

        /// <summary>
        /// Tests the usage of xml serialized properties with null values.
        /// </summary>
        [TestMethod]
        public void TestXmlSerializedPropertiesNullValues()
        {
            var updateCommand = new UpdateCommand();
            Assert.IsNull(updateCommand.Command.Xml);
            updateCommand.Command = new XmlData(new UpdateCommandMsg { UnitId = 1 });
            updateCommand.CommandXml = null;
            Assert.IsNull(updateCommand.Command.Xml);
        }

        /// <summary>
        /// Serializable class used for testing.
        /// </summary>
        public class UpdateCommandMsg
        {
            /// <summary>
            /// Gets or sets the id of the unit.
            /// </summary>
            [XmlAttribute]
            public int UnitId { get; set; }
        }
    }
}