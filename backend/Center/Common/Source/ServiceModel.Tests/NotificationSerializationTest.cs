// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationSerializationTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NotificationSerializationTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Tests
{
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for serialization (and deserialization) of notifications.
    /// </summary>
    [TestClass]
    public class NotificationSerializationTest
    {
        /// <summary>
        /// Test for serialization with XML serializer.
        /// </summary>
        [TestMethod]
        public void XmlSerializationTest()
        {
            var notification = new ProductTypeDeltaNotification { Delta = new ProductTypeDeltaMessage() };
            var serialized = Serialize(notification);
            var nullOrEmpty = string.IsNullOrEmpty(serialized);
            Assert.IsFalse(nullOrEmpty);
            var deserializedNotification = Deserialize<ProductTypeDeltaNotification>(serialized);
            Assert.IsNotNull(deserializedNotification.Delta);
        }

        private static string Serialize<T>(T notification)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter))
                {
                    serializer.Serialize(xmlWriter, notification);
                }

                return stringWriter.ToString();
            }
        }

        private static T Deserialize<T>(string value)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var stringReader = new StringReader(value))
            {
                using (var xmlReader = XmlReader.Create(stringReader))
                {
                    return (T)serializer.Deserialize(xmlReader);
                }
            }
        }
    }
}