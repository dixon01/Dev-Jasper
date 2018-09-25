// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigManagerTest.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for ConfigManagerTest and is intended
//   to contain all ConfigManagerTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Core.Tests
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml.Schema;

    using Gorba.Common.Protocols.Ximple.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for ConfigManagerTest and is intended
    /// to contain all ConfigManagerTest Unit Tests
    /// </summary>
    [TestClass]
    public class ConfigManagerTest
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
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext)
        // {
        // }
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
        #region Public Methods

        /// <summary>
        /// The config manager constructor test.
        /// </summary>
        [TestMethod]
        public void ConfigManagerConstructorTest()
        {
            this.ConfigManagerConstructorTestHelper<GenericParameterHelper>();
        }

        /// <summary>
        /// A test for ConfigManager`1 Constructor
        /// </summary>
        /// <typeparam name="T">
        /// Type of the configuration manager
        /// </typeparam>
        public void ConfigManagerConstructorTestHelper<T>() where T : class, new()
        {
            var target = new ConfigManager<T>();
            Assert.IsNotNull(target);
        }

        /// <summary>
        /// The config test.
        /// </summary>
        [TestMethod]
        [DeploymentItem("Gorba.Common.Configuration.Core.dll")]
        // ReSharper disable InconsistentNaming
        public void CreateConfigTest_WithGenericParameterHelper_ReturnedConfigTypeShouldBeGenericParameterHelper()
        // ReSharper restore InconsistentNaming
        {
            this.CreateConfigTestHelper<GenericParameterHelper>();
        }

        /// <summary>
        /// A test for Config
        /// </summary>
        /// <typeparam name="T">
        /// Type of the configuration manager
        /// </typeparam>
        public void CreateConfigTestHelper<T>() where T : class, new()
        {
            var target = new ConfigManager<T>();
            var expectedConfig = new T();

            target.CreateConfig();

            var actualConfig = target.Config;

            Assert.AreEqual(expectedConfig.GetType(), actualConfig.GetType());
        }

        /// <summary>
        /// The file name test for specified FileName.
        /// </summary>
        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void FileNameTest_WithDummyDotXmlFileName_FileNameShouldBeDummyDotXml()
        // ReSharper restore InconsistentNaming
        {
            this.FileNameTestHelper<GenericParameterHelper>("Dummy.Xml", "Dummy.Xml");
        }

        /// <summary>
        /// The file name test for unspecified FileName.
        /// </summary>
        [TestMethod]
        // ReSharper disable InconsistentNaming
        public void FileNameTest_WithEmptyFileName_FileNameShouldBeGenericParameterHelperDotXml()
        // ReSharper restore InconsistentNaming
        {
            this.FileNameTestHelper<GenericParameterHelper>(string.Empty, "GenericParameterHelper.xml");
        }

        /// <summary>
        /// A test for specified FileName
        /// </summary>
        /// <param name="fileName">
        /// The file Name.
        /// </param>
        /// <param name="expectedFielName">
        /// The expected Fiel Name.
        /// </param>
        /// <typeparam name="T">
        /// Type of the configuration manager
        /// </typeparam>
        public void FileNameTestHelper<T>(string fileName, string expectedFielName) where T : class, new()
        {
            // ASSIGN
            var target = new ConfigManager<T>();
            if (!string.IsNullOrEmpty(fileName))
            {
                target.FileName = fileName;
            }

            // ACT
            var actualFileName = target.FileName;

            // ASSERT
            Assert.AreEqual(expectedFielName, actualFileName);
        }

        /// <summary>
        /// The initiated test.
        /// </summary>
        [TestMethod]
        public void InitiatedTest()
        {
            var target = new ConfigManager<PurchaseOrder>();
            var serial = new Configurator(PurchaseOrderXmlInMemoryStream.GetInMemorySerializedPurchaseOrder());

            target.Configurator = serial;

            // ACT 
            var po = target.Config;

            bool actual = target.Initiated;

            Assert.IsNotNull(po, "target.Config should return a PurchaseOrder instance.");
            Assert.IsTrue(actual);
        }

        /// <summary>
        /// The initiated test.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        // ReSharper disable InconsistentNaming
        public void SaveTest_WithNoConfigSet_RaisesNullReferenceException()
        // ReSharper restore InconsistentNaming
        {
            var target = new ConfigManager<PurchaseOrder>();
            var configurator = new Configurator(PurchaseOrderXmlInMemoryStream.GetInMemorySerializedPurchaseOrder());

            target.Configurator = configurator;

            // ACT 
            target.SaveConfig();
        }

        /// <summary>
        /// The initiated test.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        // ReSharper disable InconsistentNaming
        public void ConfiguratorTest_SpecifyNonExistigPath_ThrowFileNotFoundException()
        // ReSharper restore InconsistentNaming
        {
            var target = new ConfigManager<PurchaseOrder>();
            var configurator = new Configurator("A_path_that_doesnt_exist");

            target.Configurator = configurator;

            // ACT 
            // Access to Config to load configuration from file
            var date = target.Config.OrderDate;

            // ASSERT
            // Dummy assert !
            Assert.IsNotNull(date);
        }

        /// <summary>
        /// Simple test XmlValidationException thrown during deserialization.
        /// </summary>
        [TestMethod]
        public void ConfigDeserializeNonValidXmlTest()
        {
            var target = new ConfigManager<Dictionary>();
            var schemaStream = new MemoryStream(Encoding.UTF8.GetBytes(StringResources.Schema));
            var xmlStream = new MemoryStream(Encoding.UTF8.GetBytes(StringResources.InvalidXml));
            var xmlSchema = XmlSchema.Read(schemaStream, null);
            target.XmlSchema = xmlSchema;
            target.Configurator = new Configurator(xmlStream, xmlSchema);
            Dictionary dict = null;
            try
            {
                dict = target.Config;
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
        /// Simple test getting the absolute path of a file related to the path of the configuration file.
        /// </summary>
        [TestMethod]
        public void GetAbsolutePathRelatedToConfigTest()
        {
            var target = new ConfigManager<Dictionary>();
            var expected = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.xml");
            var actual = target.GetAbsolutePathRelatedToConfig("Config.xml");
            Assert.AreEqual(expected, actual);

            const string AbsoluteConfigPath = @"C:\Temp\Dictionary.xml";
            target.FileName = AbsoluteConfigPath;
            actual = target.FullConfigFileName;
            Assert.AreEqual(AbsoluteConfigPath, actual);

            var relativePath = "TestConfig.xml";
            expected = @"C:\Temp\TestConfig.xml";
            actual = target.GetAbsolutePathRelatedToConfig(relativePath);
            Assert.AreEqual(expected, actual);

            relativePath = @"..\TestConfig.xml";
            expected = @"C:\TestConfig.xml";
            actual = target.GetAbsolutePathRelatedToConfig(relativePath);
            Assert.AreEqual(expected, actual);

            relativePath = @"Folder\TestConfig.xml";
            expected = @"C:\Temp\Folder\TestConfig.xml";
            actual = target.GetAbsolutePathRelatedToConfig(relativePath);
            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}