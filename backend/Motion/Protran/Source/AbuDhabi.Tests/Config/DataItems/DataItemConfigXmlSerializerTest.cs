// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataItemConfigXmlSerializerTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConfigMngTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Tests.Config.DataItems
{
    using System.IO;
    using System.Xml;

    using Gorba.Motion.Protran.AbuDhabi.Config.DataItems;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for DataItemConfigXmlSerializer and is intended
    /// to contain all DataItemConfigXmlSerializer Unit Tests
    /// </summary>
    [TestClass]
    public class DataItemConfigXmlSerializerTest
    {
        /// <summary>
        /// A test for Deserialize
        /// </summary>
        [TestMethod]
        public void DeserializeDefaultTest()
        {
            const string ItemXml = "<DestinationArabic Enabled=\"true\" TransfRef=\"TransfArabic\">"
                                   + "<UsedFor Table=\"DestinationArabic\" Column=\"DestinationName\" Row=\"0\" />"
                                   + "</DestinationArabic>";
            var target = new DataItemConfigXmlSerializer();
            var reader = XmlReader.Create(new StringReader(ItemXml));
            reader.MoveToContent();

            var config = target.Deserialize(reader);

            Assert.IsNotNull(config);
            Assert.AreEqual(typeof(DataItemConfig), config.GetType());
            Assert.AreEqual("DestinationArabic", config.Name);
            Assert.AreEqual(true, config.Enabled);
            Assert.AreEqual("TransfArabic", config.TransfRef);

            Assert.IsNotNull(config.UsedFor);
            Assert.AreEqual("DestinationName", config.UsedFor.Column);
            Assert.AreEqual("0", config.UsedFor.Row);
            Assert.AreEqual("DestinationArabic", config.UsedFor.Table);
        }

        /// <summary>
        /// A test for Deserialize
        /// </summary>
        [TestMethod]
        public void DeserializeStopTest()
        {
            const string ItemXml = "<Stop2 Enabled=\"true\" TransfRef=\"TransfStop\">"
                                   + "<FirstLineUsedFor Table=\"Stops\" Column=\"StopCity\" Row=\"2\" />"
                                   + "<SecondLineUsedFor Table=\"Stops\" Column=\"StopName\" Row=\"2\" /></Stop2>";
            var target = new DataItemConfigXmlSerializer();
            var reader = XmlReader.Create(new StringReader(ItemXml));
            reader.MoveToContent();

            var config = target.Deserialize(reader);
            Assert.IsNotNull(config);
            Assert.AreEqual(typeof(Stop2), config.GetType());

            var stop2 = (Stop2)config;
            Assert.AreEqual("Stop2", stop2.Name);
            Assert.AreEqual(true, stop2.Enabled);
            Assert.AreEqual("TransfStop", stop2.TransfRef);
            Assert.IsNull(stop2.UsedFor);

            Assert.IsNotNull(stop2.FirstLineUsedFor);
            Assert.AreEqual("StopCity", stop2.FirstLineUsedFor.Column);
            Assert.AreEqual("2", stop2.FirstLineUsedFor.Row);
            Assert.AreEqual("Stops", stop2.FirstLineUsedFor.Table);

            Assert.IsNotNull(stop2.SecondLineUsedFor);
            Assert.AreEqual("StopName", stop2.SecondLineUsedFor.Column);
            Assert.AreEqual("2", stop2.SecondLineUsedFor.Row);
            Assert.AreEqual("Stops", stop2.SecondLineUsedFor.Table);
        }

        /// <summary>
        /// A test for Deserialize
        /// </summary>
        [TestMethod]
        public void DeserializeStopMinus1Test()
        {
            const string ItemXml = "<Stop-1 Enabled=\"true\" TransfRef=\"TransfStop\">"
                                   + "<FirstLineUsedFor Table=\"Stops\" Column=\"StopCity\" Row=\"0\" />"
                                   + "<SecondLineUsedFor Table=\"Stops\" Column=\"StopName\" Row=\"0\" /></Stop-1>";
            var target = new DataItemConfigXmlSerializer();
            var reader = XmlReader.Create(new StringReader(ItemXml));
            reader.MoveToContent();

            var config = target.Deserialize(reader);
            Assert.IsNotNull(config);
            Assert.AreEqual(typeof(StopMinus1), config.GetType());

            var stopMinus1 = (StopMinus1)config;
            Assert.AreEqual("Stop-1", stopMinus1.Name);
            Assert.AreEqual(true, stopMinus1.Enabled);
            Assert.AreEqual("TransfStop", stopMinus1.TransfRef);
            Assert.IsNull(stopMinus1.UsedFor);

            Assert.IsNotNull(stopMinus1.FirstLineUsedFor);
            Assert.AreEqual("StopCity", stopMinus1.FirstLineUsedFor.Column);
            Assert.AreEqual("0", stopMinus1.FirstLineUsedFor.Row);
            Assert.AreEqual("Stops", stopMinus1.FirstLineUsedFor.Table);

            Assert.IsNotNull(stopMinus1.SecondLineUsedFor);
            Assert.AreEqual("StopName", stopMinus1.SecondLineUsedFor.Column);
            Assert.AreEqual("0", stopMinus1.SecondLineUsedFor.Row);
            Assert.AreEqual("Stops", stopMinus1.SecondLineUsedFor.Table);
        }
    }
}
