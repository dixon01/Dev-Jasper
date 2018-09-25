// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfiguratorTest.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Core.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Schema;

    using Gorba.Common.Protocols.Ximple.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for ConfiguratorTest and is intended
    /// to contain all ConfiguratorTest Unit Tests
    /// </summary>
    [TestClass]
    public class ConfiguratorTest
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        // You can use the following additional attributes as you write your tests:

        // Use ClassInitialize to run code before running the first test in the class
        #region Public Methods and Operators

        /// <summary>
        /// The my class initialize.
        /// </summary>
        /// <param name="testContext">
        /// The test context.
        /// </param>
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
        }

        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup()
        // {
        // }
        // Use TestInitialize to run code before running each test
        // [TestInitialize()]
        // public void MyTestInitialize()
        // {
        // }
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup()
        // {
        // }

        /// <summary>
        /// The de serialize_ deserialize generic parameter helper from memory stream_ expected serializer exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ConfiguratorException))]
// ReSharper disable InconsistentNaming
        public void DeSerialize_DeserializeGenericParameterHelperFromMemoryStream_ExpectedSerializerException()
// ReSharper restore InconsistentNaming
        {
            // ASSIGN
            var target = new Configurator(new MemoryStream());

            // ACT
            target.Deserialize<GenericParameterHelper>();

            // ASSERT
            // expected exeption
        }

        /// <summary>
        /// The de serialize_ deserialize purchase order from memory stream_ the product order supplied should be filled.
        /// </summary>
        [TestMethod]
// ReSharper disable InconsistentNaming
        public void DeSerialize_DeserializePurchaseOrderFromMemoryStream_TheProductOrderSuppliedShouldBeFilled()
// ReSharper restore InconsistentNaming
        {
            // ASSIGN
            PurchaseOrder expectedPo = this.CreatePO();

            // ACT
            var actualPo = this.DeSerializeTestHelper<PurchaseOrder>();

            // ASSERT
            Assert.IsNotNull(actualPo);

            Assert.AreEqual(expectedPo.OrderDate, actualPo.OrderDate);
            Assert.AreEqual(expectedPo.OrderedItems.Length, actualPo.OrderedItems.Length);
            Assert.AreEqual(expectedPo.SubTotal, actualPo.SubTotal);
            Assert.AreEqual(expectedPo.TotalCost, actualPo.TotalCost);

            Address expectedAddress = expectedPo.ShipTo;
            Address actualAddress = actualPo.ShipTo;
            Assert.AreEqual(expectedAddress.City, actualAddress.City);
            Assert.AreEqual(expectedAddress.Line1, actualAddress.Line1);
            Assert.AreEqual(expectedAddress.Name, actualAddress.Name);
            Assert.AreEqual(expectedAddress.State, actualAddress.State);
            Assert.AreEqual(expectedAddress.Zip, actualAddress.Zip);

            OrderedItem expectedOrderItem1 = expectedPo.OrderedItems[0];
            OrderedItem expectedOrderItem2 = expectedPo.OrderedItems[1];
            Assert.IsNotNull(expectedOrderItem1, "The order should have 2 order items for this test.");
            Assert.IsNotNull(expectedOrderItem2, "The order should have 2 order items for this test.");

            OrderedItem actualOrderItem1 = actualPo.OrderedItems[0];
            OrderedItem actualOrderItem2 = actualPo.OrderedItems[1];

            Assert.AreEqual(expectedOrderItem1.Description, actualOrderItem1.Description);
            Assert.AreEqual(expectedOrderItem1.ItemName, actualOrderItem1.ItemName);
            Assert.AreEqual(expectedOrderItem1.LineTotal, actualOrderItem1.LineTotal);
            Assert.AreEqual(expectedOrderItem1.Quantity, actualOrderItem1.Quantity);
            Assert.AreEqual(expectedOrderItem1.UnitPrice, actualOrderItem1.UnitPrice);

            Assert.AreEqual(expectedOrderItem2.Description, actualOrderItem2.Description);
            Assert.AreEqual(expectedOrderItem2.ItemName, actualOrderItem2.ItemName);
            Assert.AreEqual(expectedOrderItem2.LineTotal, actualOrderItem2.LineTotal);
            Assert.AreEqual(expectedOrderItem2.Quantity, actualOrderItem2.Quantity);
            Assert.AreEqual(expectedOrderItem2.UnitPrice, actualOrderItem2.UnitPrice);
        }

        /// <summary>
        /// Simple test deserialize a valid xml with schema validation.
        /// </summary>
        [TestMethod]
        public void DeserializeWithValidationValidXmlTest()
        {
            var schemaStream = new MemoryStream(Encoding.UTF8.GetBytes(StringResources.Schema));
            var xmlStream = new MemoryStream(Encoding.UTF8.GetBytes(StringResources.ValidXml));
            var xmlSchema = XmlSchema.Read(schemaStream, null);
            var target = new Configurator(xmlStream, xmlSchema);
            var dict = target.Deserialize<Dictionary>();

            Assert.IsNotNull(dict);
            Assert.AreEqual(2, dict.Languages.Count);
            Assert.AreEqual(2, dict.Tables.Count);
            Assert.AreEqual("German", dict.Languages[1].Name);
        }

        /// <summary>
        /// Simple test deserialize an invalid xml with schema validation.
        /// </summary>
        [TestMethod]
        public void DeserializeWithValidationInvalidXmlTest()
        {
            var schemaStream = new MemoryStream(Encoding.UTF8.GetBytes(StringResources.Schema));
            var xmlStream = new MemoryStream(Encoding.UTF8.GetBytes(StringResources.InvalidXml));
            var xmlSchema = XmlSchema.Read(schemaStream, null);
            var target = new Configurator(xmlStream, xmlSchema);
            Dictionary dict = null;
            try
            {
                dict = target.Deserialize<Dictionary>();
            }
            catch (ConfiguratorException e)
            {
                Assert.IsNull(dict);
                var ex = e.InnerException as XmlValidationException;
                Assert.IsNotNull(ex);
                Assert.AreEqual(1, ex.Exceptions.Count);
                Assert.AreEqual(52, ex.Exceptions[0].LinePosition);
            }
        }

        /// <summary>
        /// The serialize_ serialize purchase order class_ verify the content of the resulted stream.
        /// </summary>
        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void Serialize_SerializePurchaseOrderClass_VerifyTheContentOfTheResultedStream()
        // ReSharper restore InconsistentNaming
        {            
            // ASSIGN
            var memStream = new MemoryStream();
            var target = new Configurator(memStream);

            MemoryStream stream = PurchaseOrderXmlInMemoryStream.GetInMemorySerializedPurchaseOrder();

            PurchaseOrder po = this.CreatePO();

            // ACT
            target.Serialize(po);

            // ASSERT            
            string expectedStr = Encoding.UTF8.GetString(stream.GetBuffer());
            string actualStr = Encoding.UTF8.GetString(memStream.GetBuffer());
            Assert.AreEqual(expectedStr, actualStr);
        }

        /// <summary>
        /// A test for Configurator Constructor
        /// </summary>
        [TestMethod]
        public void SerializerConstructorTest()
        {
            const string Path = "Dummy path";
            var target = new Configurator(Path);
            Assert.IsNotNull(target);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create a ProductOrder with bill address and order item.
        /// </summary>
        /// <returns>
        /// Type : <see cref="PurchaseOrder"/>
        /// PurchaseOrder filled with dummy data
        /// </returns>
        private PurchaseOrder CreatePO()
        {
            // Creates an instance of the XmlSerializer class;
            // specifies the type of object to serialize.
            var po = new PurchaseOrder();

            // Creates an address to ship and bill to.
            var billAddress = new Address
                {
                   Name = "Teresa Atkinson", Line1 = "1 Main St.", City = "AnyTown", State = "WA", Zip = "41120" 
                };

            // Sets ShipTo and BillTo to the same addressee.
            po.ShipTo = billAddress;
            po.OrderDate = new DateTime(2012, 02, 10).ToShortDateString();

            // Creates an OrderedItem.
            var i1 = new OrderedItem
                {
                   ItemName = "Widget S", Description = "Small widget", UnitPrice = (decimal)5.23, Quantity = 3 
                };

            var i2 = new OrderedItem
                {
                   ItemName = "I 2", Description = "Second item", UnitPrice = (decimal)4.89, Quantity = 5 
                };

            i1.Calculate();
            i2.Calculate();

            // Inserts the item into the array.
            OrderedItem[] items = { i1, i2 };
            po.OrderedItems = items;

            // Calculate the total cost.
            decimal subTotal = items.Sum(oi => oi.LineTotal);

            po.SubTotal = subTotal;
            po.ShipCost = (decimal)12.51;
            po.TotalCost = po.SubTotal + po.ShipCost;

            // Serializes the purchase order, and closes the TextWriter.
            return po;
        }

        /// <summary>
        /// A test for Deserialize
        /// </summary>
        /// <typeparam name="T">
        /// Type of the internal configurator
        /// </typeparam>
        /// <returns>
        /// The de serialize test helper.
        /// </returns>
        private T DeSerializeTestHelper<T>() where T : class, new()
        {
            var target = new Configurator(PurchaseOrderXmlInMemoryStream.GetInMemorySerializedPurchaseOrder());

            return target.Deserialize<T>();
        }

        #endregion
    }
}
