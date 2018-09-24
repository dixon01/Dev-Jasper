// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XimpleSerializationTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IsiSerializerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ximple.Tests
{
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Protocols.Ximple;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test for <see cref="Ximple"/> serialization and deserialization
    /// </summary>
    [TestClass]
    public class XimpleSerializationTest
    {
        private const string Version2 = "2.0.0";

        /// <summary>
        /// Simple test with an empty Ximple v2.0 object.
        /// </summary>
        [TestMethod]
        public void EmptyXimple2Test()
        {
            var ximple = new Ximple(Version2);

            Assert.AreEqual(Version2, ximple.Version);
        }

        /// <summary>
        /// Simple test serializing an empty Ximple v2.0 object.
        /// </summary>
        [TestMethod]
        public void EmptyXimple2SerializationTest()
        {
            var ximple = new Ximple(Version2);

            var serializer = new XmlSerializer(ximple.GetType());
            var stream = new MemoryStream();
            var settings = new XmlWriterSettings { Indent = false, Encoding = new UTF8Encoding(false) };
            var xml = XmlWriter.Create(stream, settings);
            serializer.Serialize(xml, ximple);
            stream.Flush();

            var str = Encoding.UTF8.GetString(stream.ToArray());
            Assert.AreEqual(StringResources.EmptyXimple2, str);
        }

        /// <summary>
        /// Simple test de-serializing an empty Ximple v2.0 object.
        /// </summary>
        [TestMethod]
        public void EmptyXimple2DeserializationTest()
        {
            var serializer = new XmlSerializer(typeof(Ximple));
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(StringResources.EmptyXimple2));
            var xml = XmlReader.Create(stream);
            var ximple = serializer.Deserialize(xml) as Ximple;

            Assert.IsNotNull(ximple);
            Assert.AreEqual(Version2, ximple.Version);
            Assert.AreEqual(0, ximple.Cells.Count);
        }

        /// <summary>
        /// Simple test serializing a Ximple v2.0 object with cells.
        /// </summary>
        [TestMethod]
        public void Ximple2CellsSerializationTest()
        {
            var ximple = new Ximple(Version2);
            ximple.Cells.Add(
                new XimpleCell
                {
                    ColumnNumber = 3,
                    RowNumber = 5,
                    TableNumber = 12,
                    LanguageNumber = 0,
                    Value = "Hello world"
                });
            ximple.Cells.Add(
                new XimpleCell
                {
                    ColumnNumber = 5,
                    RowNumber = 0,
                    TableNumber = 10,
                    LanguageNumber = 5,
                    Value = "media.jpg"
                });

            var serializer = new XmlSerializer(ximple.GetType());
            var stream = new MemoryStream();
            var settings = new XmlWriterSettings { Indent = false, Encoding = new UTF8Encoding(false) };
            var xml = XmlWriter.Create(stream, settings);
            serializer.Serialize(xml, ximple);
            stream.Flush();

            var str = Encoding.UTF8.GetString(stream.ToArray());
            Assert.AreEqual(StringResources.Ximple2WithCells, str);
        }

        /// <summary>
        /// Simple test de-serializing a Ximple v2.0 object with cells.
        /// </summary>
        [TestMethod]
        public void Ximple2CellsDeserializationTest()
        {
            var serializer = new XmlSerializer(typeof(Ximple));
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(StringResources.Ximple2WithCells));
            var xml = XmlReader.Create(stream);
            var ximple = serializer.Deserialize(xml) as Ximple;

            Assert.IsNotNull(ximple);
            Assert.AreEqual(Version2, ximple.Version);
            Assert.AreEqual(2, ximple.Cells.Count);
            Assert.AreEqual(0, ximple.Cells[0].LanguageNumber);
            Assert.AreEqual(3, ximple.Cells[0].ColumnNumber);
            Assert.AreEqual(5, ximple.Cells[0].RowNumber);
            Assert.AreEqual(12, ximple.Cells[0].TableNumber);
            Assert.AreEqual("Hello world", ximple.Cells[0].Value);
            Assert.AreEqual(5, ximple.Cells[1].LanguageNumber);
            Assert.AreEqual(5, ximple.Cells[1].ColumnNumber);
            Assert.AreEqual(0, ximple.Cells[1].RowNumber);
            Assert.AreEqual(10, ximple.Cells[1].TableNumber);
            Assert.AreEqual("media.jpg", ximple.Cells[1].Value);
        }
    }
}
