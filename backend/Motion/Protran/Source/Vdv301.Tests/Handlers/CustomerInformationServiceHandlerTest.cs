// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomerInformationServiceHandlerTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CustomerInformationServiceHandlerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Vdv301.Tests.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Protran.VDV301;
    using Gorba.Common.Protocols.Vdv301.Messages;
    using Gorba.Common.Protocols.Vdv301.Services;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.Utility.Compatibility.Container;
    using Gorba.Motion.Protran.Vdv301.Handlers;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test for the <see cref="CustomerInformationServiceHandler"/>.
    /// </summary>
    [TestClass]
    public class CustomerInformationServiceHandlerTest
    {
        /// <summary>
        /// The test initialization for each test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            var container = new ServiceContainer();
            ServiceLocator.SetLocatorProvider(() => new ServiceContainerLocator(container));
            container.RegisterInstance<IElementHandlerFactory>(new ElementHandlerFactory());
        }

        /// <summary>
        /// Test that <see cref="CustomerInformationServiceAllData.DoorState"/> is properly handled.
        /// </summary>
        [TestMethod]
        [DeploymentItem("vdv301-test.xml")]
        [DeploymentItem("dictionary.xml")]
        public void TestDoorState()
        {
            var configMgr = new ConfigManager<Vdv301ProtocolConfig> { FileName = "vdv301-test.xml" };
            var dictMgr = new ConfigManager<Dictionary> { FileName = "dictionary.xml" };
            var service = new TestCustomerInformationService();
            var context = new TestHandlerContext
                {
                    Config = configMgr.Config,
                    Dictionary = dictMgr.Config,
                    CustomerInformationService = service
                };
            var ximples = new Queue<Ximple>();

            var target = new CustomerInformationServiceHandler();
            target.XimpleCreated += (s, e) => ximples.Enqueue(e.Ximple);
            target.Configure(context);
            target.Start();

            Assert.AreEqual(0, ximples.Count);

            service.SetAllData(
                new CustomerInformationServiceAllData
                    {
                        DoorState = DoorOpenStateEnumeration.DoorsOpen,
                        DoorStateSpecified = true
                    });

            Assert.AreEqual(1, ximples.Count);
            var ximple = ximples.Dequeue();
            Assert.AreEqual(1, ximple.Cells.Count);
            var cell = ximple.Cells[0];
            Assert.AreEqual(0, cell.LanguageNumber);
            Assert.AreEqual(0, cell.TableNumber);
            Assert.AreEqual(4, cell.ColumnNumber);
            Assert.AreEqual(0, cell.RowNumber);
            Assert.AreEqual("1", cell.Value);

            service.SetAllData(
                new CustomerInformationServiceAllData
                    {
                        DoorState = DoorOpenStateEnumeration.AllDoorsClosed,
                        DoorStateSpecified = true
                    });

            Assert.AreEqual(1, ximples.Count);
            ximple = ximples.Dequeue();
            Assert.AreEqual(1, ximple.Cells.Count);
            cell = ximple.Cells[0];
            Assert.AreEqual(0, cell.LanguageNumber);
            Assert.AreEqual(0, cell.TableNumber);
            Assert.AreEqual(4, cell.ColumnNumber);
            Assert.AreEqual(0, cell.RowNumber);
            Assert.AreEqual("0", cell.Value);

            target.Stop();
        }

        /// <summary>
        /// Test that <see cref="CustomerInformationServiceAllData.VehicleStopRequested"/> is properly handled.
        /// </summary>
        [TestMethod]
        [DeploymentItem("vdv301-test.xml")]
        [DeploymentItem("dictionary.xml")]
        public void TestVehicleStopRequested()
        {
            var configMgr = new ConfigManager<Vdv301ProtocolConfig> { FileName = "vdv301-test.xml" };
            var dictMgr = new ConfigManager<Dictionary> { FileName = "dictionary.xml" };
            var service = new TestCustomerInformationService();
            var context = new TestHandlerContext
            {
                Config = configMgr.Config,
                Dictionary = dictMgr.Config,
                CustomerInformationService = service
            };
            var ximples = new Queue<Ximple>();

            var target = new CustomerInformationServiceHandler();
            target.XimpleCreated += (s, e) => ximples.Enqueue(e.Ximple);
            target.Configure(context);
            target.Start();

            Assert.AreEqual(0, ximples.Count);

            service.SetAllData(
                new CustomerInformationServiceAllData
                {
                    VehicleStopRequested = new IBISIPboolean(false)
                });

            Assert.AreEqual(1, ximples.Count);
            var ximple = ximples.Dequeue();
            Assert.AreEqual(1, ximple.Cells.Count);
            var cell = ximple.Cells[0];
            Assert.AreEqual(0, cell.LanguageNumber);
            Assert.AreEqual(0, cell.TableNumber);
            Assert.AreEqual(3, cell.ColumnNumber);
            Assert.AreEqual(0, cell.RowNumber);
            Assert.AreEqual("False", cell.Value);

            service.SetAllData(
                new CustomerInformationServiceAllData
                {
                    VehicleStopRequested = new IBISIPboolean(true)
                });

            Assert.AreEqual(1, ximples.Count);
            ximple = ximples.Dequeue();
            Assert.AreEqual(1, ximple.Cells.Count);
            cell = ximple.Cells[0];
            Assert.AreEqual(0, cell.LanguageNumber);
            Assert.AreEqual(0, cell.TableNumber);
            Assert.AreEqual(3, cell.ColumnNumber);
            Assert.AreEqual(0, cell.RowNumber);
            Assert.AreEqual("True", cell.Value);

            target.Stop();
        }

        /// <summary>
        /// Test that <see cref="TripInformationStructure.StopSequence"/> is properly handled.
        /// </summary>
        [TestMethod]
        [DeploymentItem("vdv301-test.xml")]
        [DeploymentItem("dictionary.xml")]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test code")]
        public void TestStopList()
        {
            var configMgr = new ConfigManager<Vdv301ProtocolConfig> { FileName = "vdv301-test.xml" };
            var dictMgr = new ConfigManager<Dictionary> { FileName = "dictionary.xml" };
            var service = new TestCustomerInformationService();
            var context = new TestHandlerContext
            {
                Config = configMgr.Config,
                Dictionary = dictMgr.Config,
                CustomerInformationService = service
            };
            var ximples = new Queue<Ximple>();

            var target = new CustomerInformationServiceHandler();
            target.XimpleCreated += (s, e) => ximples.Enqueue(e.Ximple);
            target.Configure(context);
            target.Start();

            Assert.AreEqual(0, ximples.Count);

            var allData = CreateAllData(1);
            service.SetAllData(allData);

            Assert.AreEqual(1, ximples.Count);
            var ximple = ximples.Dequeue();
            Assert.AreEqual(22, ximple.Cells.Count);
            Assert.AreEqual(
                10, ximple.Cells.Count(c => c.LanguageNumber == 0 && c.TableNumber == 12 && c.ColumnNumber == 0));
            Assert.AreEqual(
                10, ximple.Cells.Count(c => c.LanguageNumber == 3 && c.TableNumber == 12 && c.ColumnNumber == 0));
            Assert.AreEqual(
                1, ximple.Cells.Count(c => c.LanguageNumber == 0 && c.TableNumber == 11 && c.ColumnNumber == 0));
            Assert.AreEqual(
                1, ximple.Cells.Count(c => c.LanguageNumber == 3 && c.TableNumber == 11 && c.ColumnNumber == 0));

            var cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 12 && c.ColumnNumber == 0 && c.RowNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Haltestelle 1", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 12 && c.ColumnNumber == 0 && c.RowNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Haltestelle 2", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 12 && c.ColumnNumber == 0 && c.RowNumber == 9);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Haltestelle 10", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 11 && c.ColumnNumber == 0 && c.RowNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Ziel 1", cell.Value);

            //////////////////// Next Stop (2) //////////////////////
            allData = CreateAllData(2);
            service.SetAllData(allData);

            Assert.AreEqual(1, ximples.Count);
            ximple = ximples.Dequeue();
            Assert.AreEqual(22, ximple.Cells.Count);
            Assert.AreEqual(
                10, ximple.Cells.Count(c => c.LanguageNumber == 0 && c.TableNumber == 12 && c.ColumnNumber == 0));
            Assert.AreEqual(
                10, ximple.Cells.Count(c => c.LanguageNumber == 3 && c.TableNumber == 12 && c.ColumnNumber == 0));
            Assert.AreEqual(
                1, ximple.Cells.Count(c => c.LanguageNumber == 0 && c.TableNumber == 11 && c.ColumnNumber == 0));
            Assert.AreEqual(
                1, ximple.Cells.Count(c => c.LanguageNumber == 3 && c.TableNumber == 11 && c.ColumnNumber == 0));

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 12 && c.ColumnNumber == 0 && c.RowNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Haltestelle 2", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 12 && c.ColumnNumber == 0 && c.RowNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Haltestelle 3", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 12 && c.ColumnNumber == 0 && c.RowNumber == 9);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 11 && c.ColumnNumber == 0 && c.RowNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Ziel 2", cell.Value);

            target.Stop();
        }

        /// <summary>
        /// Test that <see cref="ConnectionStructure"/> is properly handled.
        /// </summary>
        [TestMethod]
        [DeploymentItem("vdv301-test.xml")]
        [DeploymentItem("dictionary.xml")]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test code")]
        public void TestConnectionList()
        {
            var configMgr = new ConfigManager<Vdv301ProtocolConfig> { FileName = "vdv301-test.xml" };
            var dictMgr = new ConfigManager<Dictionary> { FileName = "dictionary.xml" };
            var service = new TestCustomerInformationService();
            var context = new TestHandlerContext
            {
                Config = configMgr.Config,
                Dictionary = dictMgr.Config,
                CustomerInformationService = service
            };
            var ximples = new Queue<Ximple>();

            var target = new CustomerInformationServiceHandler();
            target.XimpleCreated += (s, e) => ximples.Enqueue(e.Ximple);
            target.Configure(context);
            target.Start();

            Assert.AreEqual(0, ximples.Count);

            var allData = CreateAllDataWithConnections(1);
            service.SetAllData(allData);

            Assert.AreEqual(1, ximples.Count);
            var ximple = ximples.Dequeue();
            Assert.AreEqual(
                2, ximple.Cells.Count(c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 0));
            Assert.AreEqual(
                2, ximple.Cells.Count(c => c.LanguageNumber == 3 && c.TableNumber == 13 && c.ColumnNumber == 0));
            Assert.AreEqual(
                2, ximple.Cells.Count(c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 4));
            Assert.AreEqual(
                2, ximple.Cells.Count(c => c.LanguageNumber == 3 && c.TableNumber == 13 && c.ColumnNumber == 4));
            Assert.AreEqual(
                2, ximple.Cells.Count(c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 7));

            // Platform is not translated!
            Assert.AreEqual(
                0, ximple.Cells.Count(c => c.LanguageNumber == 3 && c.TableNumber == 13 && c.ColumnNumber == 7));

            var cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 0 && c.RowNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Schiff", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 3 && c.TableNumber == 13 && c.ColumnNumber == 0 && c.RowNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Bateau", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 4 && c.RowNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Z7", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 3 && c.TableNumber == 13 && c.ColumnNumber == 4 && c.RowNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("D7", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 7 && c.RowNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Platform H", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 0 && c.RowNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Schiff", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 3 && c.TableNumber == 13 && c.ColumnNumber == 0 && c.RowNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Bateau", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 4 && c.RowNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Z9", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 3 && c.TableNumber == 13 && c.ColumnNumber == 4 && c.RowNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual("D9", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 7 && c.RowNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Platform J", cell.Value);

            //////////////////// Next Stop (2) //////////////////////
            allData = CreateAllDataWithConnections(2);
            service.SetAllData(allData);

            Assert.AreEqual(1, ximples.Count);
            ximple = ximples.Dequeue();
            Assert.AreEqual(
                2, ximple.Cells.Count(c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 0));
            Assert.AreEqual(
                2, ximple.Cells.Count(c => c.LanguageNumber == 3 && c.TableNumber == 13 && c.ColumnNumber == 0));
            Assert.AreEqual(
                2, ximple.Cells.Count(c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 4));
            Assert.AreEqual(
                2, ximple.Cells.Count(c => c.LanguageNumber == 3 && c.TableNumber == 13 && c.ColumnNumber == 4));
            Assert.AreEqual(
                2, ximple.Cells.Count(c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 7));

            // Platform is not translated!
            Assert.AreEqual(
                0, ximple.Cells.Count(c => c.LanguageNumber == 3 && c.TableNumber == 13 && c.ColumnNumber == 7));

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 0 && c.RowNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Schiff", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 3 && c.TableNumber == 13 && c.ColumnNumber == 0 && c.RowNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Bateau", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 4 && c.RowNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Z3", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 3 && c.TableNumber == 13 && c.ColumnNumber == 4 && c.RowNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("D3", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 7 && c.RowNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Platform D", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 0 && c.RowNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 3 && c.TableNumber == 13 && c.ColumnNumber == 0 && c.RowNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 4 && c.RowNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 3 && c.TableNumber == 13 && c.ColumnNumber == 4 && c.RowNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 7 && c.RowNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            //////////////////// Next Stop (3) //////////////////////
            allData = CreateAllDataWithConnections(3);
            service.SetAllData(allData);

            Assert.AreEqual(1, ximples.Count);
            ximple = ximples.Dequeue();
            Assert.AreEqual(
                1, ximple.Cells.Count(c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 0));
            Assert.AreEqual(
                1, ximple.Cells.Count(c => c.LanguageNumber == 3 && c.TableNumber == 13 && c.ColumnNumber == 0));
            Assert.AreEqual(
                1, ximple.Cells.Count(c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 4));
            Assert.AreEqual(
                1, ximple.Cells.Count(c => c.LanguageNumber == 3 && c.TableNumber == 13 && c.ColumnNumber == 4));
            Assert.AreEqual(
                1, ximple.Cells.Count(c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 7));

            // Platform is not translated!
            Assert.AreEqual(
                0, ximple.Cells.Count(c => c.LanguageNumber == 3 && c.TableNumber == 13 && c.ColumnNumber == 7));

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 0 && c.RowNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 3 && c.TableNumber == 13 && c.ColumnNumber == 0 && c.RowNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 4 && c.RowNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 3 && c.TableNumber == 13 && c.ColumnNumber == 4 && c.RowNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 7 && c.RowNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            //////////////////// Next Stop (4) //////////////////////
            allData = CreateAllDataWithConnections(4);
            service.SetAllData(allData);

            Assert.AreEqual(1, ximples.Count);
            ximple = ximples.Dequeue();
            Assert.AreEqual(
                3, ximple.Cells.Count(c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 0));
            Assert.AreEqual(
                3, ximple.Cells.Count(c => c.LanguageNumber == 3 && c.TableNumber == 13 && c.ColumnNumber == 0));
            Assert.AreEqual(
                3, ximple.Cells.Count(c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 4));
            Assert.AreEqual(
                3, ximple.Cells.Count(c => c.LanguageNumber == 3 && c.TableNumber == 13 && c.ColumnNumber == 4));
            Assert.AreEqual(
                3, ximple.Cells.Count(c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 7));

            // Platform is not translated!
            Assert.AreEqual(
                0, ximple.Cells.Count(c => c.LanguageNumber == 3 && c.TableNumber == 13 && c.ColumnNumber == 7));

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 0 && c.RowNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Schiff", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 3 && c.TableNumber == 13 && c.ColumnNumber == 0 && c.RowNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Bateau", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 4 && c.RowNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Z6", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 3 && c.TableNumber == 13 && c.ColumnNumber == 4 && c.RowNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("D6", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 7 && c.RowNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Platform G", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 0 && c.RowNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Schiff", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 3 && c.TableNumber == 13 && c.ColumnNumber == 0 && c.RowNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Bateau", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 4 && c.RowNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Z8", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 3 && c.TableNumber == 13 && c.ColumnNumber == 4 && c.RowNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual("D8", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 7 && c.RowNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Platform I", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 0 && c.RowNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Schiff", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 3 && c.TableNumber == 13 && c.ColumnNumber == 0 && c.RowNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Bateau", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 4 && c.RowNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Z10", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 3 && c.TableNumber == 13 && c.ColumnNumber == 4 && c.RowNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("D10", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 13 && c.ColumnNumber == 7 && c.RowNumber == 2);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Platform K", cell.Value);

            target.Stop();
        }

        /// <summary>
        /// Test that <see cref="TripInformationStructure.StopSequence"/> is properly handled when it comes from
        /// <see cref="ICustomerInformationService.GetTripData"/>.
        /// </summary>
        [TestMethod]
        [DeploymentItem("vdv301-TripData.xml")]
        [DeploymentItem("dictionary.xml")]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test code")]
        public void TestStopListFromTripData()
        {
            var configMgr = new ConfigManager<Vdv301ProtocolConfig> { FileName = "vdv301-TripData.xml" };
            var dictMgr = new ConfigManager<Dictionary> { FileName = "dictionary.xml" };
            var service = new TestCustomerInformationService();
            var context = new TestHandlerContext
            {
                Config = configMgr.Config,
                Dictionary = dictMgr.Config,
                CustomerInformationService = service
            };
            var ximples = new Queue<Ximple>();

            var target = new CustomerInformationServiceHandler();
            target.XimpleCreated += (s, e) => ximples.Enqueue(e.Ximple);
            target.Configure(context);
            target.Start();

            Assert.AreEqual(0, ximples.Count);

            var tripData = CreateTripData(1);
            service.SetTripData(tripData);

            Assert.AreEqual(1, ximples.Count);
            var ximple = ximples.Dequeue();
            Assert.AreEqual(22, ximple.Cells.Count);
            Assert.AreEqual(
                10, ximple.Cells.Count(c => c.LanguageNumber == 0 && c.TableNumber == 12 && c.ColumnNumber == 0));
            Assert.AreEqual(
                10, ximple.Cells.Count(c => c.LanguageNumber == 3 && c.TableNumber == 12 && c.ColumnNumber == 0));
            Assert.AreEqual(
                1, ximple.Cells.Count(c => c.LanguageNumber == 0 && c.TableNumber == 11 && c.ColumnNumber == 0));
            Assert.AreEqual(
                1, ximple.Cells.Count(c => c.LanguageNumber == 3 && c.TableNumber == 11 && c.ColumnNumber == 0));

            var cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 12 && c.ColumnNumber == 0 && c.RowNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Haltestelle 1", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 12 && c.ColumnNumber == 0 && c.RowNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Haltestelle 2", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 12 && c.ColumnNumber == 0 && c.RowNumber == 9);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Haltestelle 10", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 11 && c.ColumnNumber == 0 && c.RowNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Ziel 1", cell.Value);

            //////////////////// Next Stop (2) //////////////////////
            tripData = CreateTripData(2);
            service.SetTripData(tripData);

            Assert.AreEqual(1, ximples.Count);
            ximple = ximples.Dequeue();
            Assert.AreEqual(22, ximple.Cells.Count);
            Assert.AreEqual(
                10, ximple.Cells.Count(c => c.LanguageNumber == 0 && c.TableNumber == 12 && c.ColumnNumber == 0));
            Assert.AreEqual(
                10, ximple.Cells.Count(c => c.LanguageNumber == 3 && c.TableNumber == 12 && c.ColumnNumber == 0));
            Assert.AreEqual(
                1, ximple.Cells.Count(c => c.LanguageNumber == 0 && c.TableNumber == 11 && c.ColumnNumber == 0));
            Assert.AreEqual(
                1, ximple.Cells.Count(c => c.LanguageNumber == 3 && c.TableNumber == 11 && c.ColumnNumber == 0));

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 12 && c.ColumnNumber == 0 && c.RowNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Haltestelle 2", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 12 && c.ColumnNumber == 0 && c.RowNumber == 1);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Haltestelle 3", cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 12 && c.ColumnNumber == 0 && c.RowNumber == 9);
            Assert.IsNotNull(cell);
            Assert.AreEqual(string.Empty, cell.Value);

            cell =
                ximple.Cells.SingleOrDefault(
                    c => c.LanguageNumber == 0 && c.TableNumber == 11 && c.ColumnNumber == 0 && c.RowNumber == 0);
            Assert.IsNotNull(cell);
            Assert.AreEqual("Ziel 2", cell.Value);

            target.Stop();
        }

        /// <summary>
        /// Tests the <see cref="ICustomerInformationService.GetCurrentAnnouncement"/>.
        /// </summary>
        [TestMethod]
        [DeploymentItem("vdv301-test.xml")]
        [DeploymentItem("dictionary.xml")]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test code")]
        public void TestAnnouncment()
        {
            var configMgr = new ConfigManager<Vdv301ProtocolConfig> { FileName = "vdv301-test.xml" };
            var dictMgr = new ConfigManager<Dictionary> { FileName = "dictionary.xml" };
            var service = new TestCustomerInformationService();
            var context = new TestHandlerContext
                              {
                                  Config = configMgr.Config,
                                  Dictionary = dictMgr.Config,
                                  CustomerInformationService = service
                              };
            var ximples = new Queue<Ximple>();

            var target = new CustomerInformationServiceHandler();
            target.XimpleCreated += (s, e) => ximples.Enqueue(e.Ximple);
            target.Configure(context);
            target.Start();

            Assert.AreEqual(0, ximples.Count);

            // English is not configured, therefore it is ignored
            service.SetCurrentAnnouncement(
                new CustomerInformationServiceCurrentAnnouncementData
                    {
                        CurrentAnnouncement =
                            new AnnouncementStructure
                                {
                                    AnnouncementRef = new IBISIPNMTOKEN("ANN-0001"),
                                    AnnouncementText = new[]
                                                           {
                                                               new InternationalTextType
                                                                   {
                                                                       Language = "de-DE",
                                                                       Value = "Nächster Halt: Hauptbahnhof"
                                                                   },
                                                               new InternationalTextType
                                                                   {
                                                                       Language = "fr-FR",
                                                                       Value = "Prochain arrêt: gare principale"
                                                                   },
                                                               new InternationalTextType
                                                                   {
                                                                       Language = "en-GB",
                                                                       Value = "Next stop: main station"
                                                                   }
                                                           }
                                }
                    });

            Assert.AreEqual(1, ximples.Count);
            var ximple = ximples.Dequeue();
            Assert.AreEqual(2, ximple.Cells.Count);

            var cell = ximple.Cells.First(c => c.LanguageNumber == 0);
            Assert.AreEqual(0, cell.LanguageNumber);
            Assert.AreEqual(20, cell.TableNumber);
            Assert.AreEqual(2, cell.ColumnNumber);
            Assert.AreEqual(0, cell.RowNumber);
            Assert.AreEqual("Nächster Halt: Hauptbahnhof", cell.Value);

            cell = ximple.Cells.First(c => c.LanguageNumber == 3);
            Assert.AreEqual(3, cell.LanguageNumber);
            Assert.AreEqual(20, cell.TableNumber);
            Assert.AreEqual(2, cell.ColumnNumber);
            Assert.AreEqual(0, cell.RowNumber);
            Assert.AreEqual("Prochain arrêt: gare principale", cell.Value);

            // English is not configured, therefore it is ignored
            service.SetCurrentAnnouncement(
                new CustomerInformationServiceCurrentAnnouncementData
                    {
                        CurrentAnnouncement =
                            new AnnouncementStructure
                                {
                                    AnnouncementRef = new IBISIPNMTOKEN("ANN-0002"),
                                    AnnouncementText = new[]
                                        {
                                            new InternationalTextType
                                                {
                                                    Language = "de-DE",
                                                    Value = "Endstation, bitte alle aussteigen"
                                                },
                                            new InternationalTextType
                                                {
                                                    Language = "fr-FR",
                                                    Value = "Terminus, tout le monde descendre, s'il vous plaît"
                                                },
                                            new InternationalTextType
                                                {
                                                    Language = "en-GB",
                                                    Value = "Terminal stop, please exit"
                                                }
                                        }
                                }
                    });

            Assert.AreEqual(1, ximples.Count);
            ximple = ximples.Dequeue();
            Assert.AreEqual(2, ximple.Cells.Count);

            cell = ximple.Cells.First(c => c.LanguageNumber == 0);
            Assert.AreEqual(0, cell.LanguageNumber);
            Assert.AreEqual(20, cell.TableNumber);
            Assert.AreEqual(2, cell.ColumnNumber);
            Assert.AreEqual(0, cell.RowNumber);
            Assert.AreEqual("Endstation, bitte alle aussteigen", cell.Value);

            cell = ximple.Cells.First(c => c.LanguageNumber == 3);
            Assert.AreEqual(3, cell.LanguageNumber);
            Assert.AreEqual(20, cell.TableNumber);
            Assert.AreEqual(2, cell.ColumnNumber);
            Assert.AreEqual(0, cell.RowNumber);
            Assert.AreEqual("Terminus, tout le monde descendre, s'il vous plaît", cell.Value);

            target.Stop();
        }

        /// <summary>
        /// Test that <see cref="CustomerInformationServiceVehicleData"/> is properly handled.
        /// </summary>
        [TestMethod]
        [DeploymentItem("vdv301-VehicleData.xml")]
        [DeploymentItem("dictionary.xml")]
        public void TestVehicleData()
        {
            var configMgr = new ConfigManager<Vdv301ProtocolConfig> { FileName = "vdv301-VehicleData.xml" };
            var dictMgr = new ConfigManager<Dictionary> { FileName = "dictionary.xml" };
            var service = new TestCustomerInformationService();
            var context = new TestHandlerContext
            {
                Config = configMgr.Config,
                Dictionary = dictMgr.Config,
                CustomerInformationService = service
            };
            var ximples = new Queue<Ximple>();

            var target = new CustomerInformationServiceHandler();
            target.XimpleCreated += (s, e) => ximples.Enqueue(e.Ximple);
            target.Configure(context);
            target.Start();

            Assert.AreEqual(0, ximples.Count);

            service.SetVehicleData(
                new CustomerInformationServiceVehicleData
                {
                    VehicleStopRequested = new IBISIPboolean(false),
                    DoorState = DoorOpenStateEnumeration.AllDoorsClosed,
                    DoorStateSpecified = true,
                    ExitSide = ExitSideEnumeration.right,
                    ExitSideSpecified = true
                });

            Assert.AreEqual(1, ximples.Count);
            var ximple = ximples.Dequeue();
            Assert.AreEqual(3, ximple.Cells.Count);
            var cell = ximple.Cells.Single(c => c.LanguageNumber == 0 && c.TableNumber == 0 && c.ColumnNumber == 3);
            Assert.AreEqual(0, cell.RowNumber);
            Assert.AreEqual("0", cell.Value);
            cell = ximple.Cells.Single(c => c.LanguageNumber == 0 && c.TableNumber == 0 && c.ColumnNumber == 4);
            Assert.AreEqual(0, cell.RowNumber);
            Assert.AreEqual("0", cell.Value);
            cell = ximple.Cells.Single(c => c.LanguageNumber == 0 && c.TableNumber == 0 && c.ColumnNumber == 6);
            Assert.AreEqual(0, cell.RowNumber);
            Assert.AreEqual("1", cell.Value);

            service.SetVehicleData(
                new CustomerInformationServiceVehicleData
                {
                    VehicleStopRequested = new IBISIPboolean(true),
                    DoorState = DoorOpenStateEnumeration.SingleDoorOpen,
                    DoorStateSpecified = true,
                    ExitSide = ExitSideEnumeration.unknown,
                    ExitSideSpecified = true
                });

            Assert.AreEqual(1, ximples.Count);
            ximple = ximples.Dequeue();
            Assert.AreEqual(3, ximple.Cells.Count);
            cell = ximple.Cells.Single(c => c.LanguageNumber == 0 && c.TableNumber == 0 && c.ColumnNumber == 3);
            Assert.AreEqual(0, cell.RowNumber);
            Assert.AreEqual("1", cell.Value);
            cell = ximple.Cells.Single(c => c.LanguageNumber == 0 && c.TableNumber == 0 && c.ColumnNumber == 4);
            Assert.AreEqual(0, cell.RowNumber);
            Assert.AreEqual("1", cell.Value);
            cell = ximple.Cells.Single(c => c.LanguageNumber == 0 && c.TableNumber == 0 && c.ColumnNumber == 6);
            Assert.AreEqual(0, cell.RowNumber);
            Assert.AreEqual("0", cell.Value);

            target.Stop();
        }

        /// <summary>
        /// Test that <see cref="CustomerInformationServiceVehicleData"/> is properly handled.
        /// </summary>
        [TestMethod]
        [DeploymentItem("vdv301-MultiOutput.xml")]
        [DeploymentItem("dictionary.xml")]
        public void TestMultiOutput()
        {
            var configMgr = new ConfigManager<Vdv301ProtocolConfig> { FileName = "vdv301-MultiOutput.xml" };
            var dictMgr = new ConfigManager<Dictionary> { FileName = "dictionary.xml" };
            var service = new TestCustomerInformationService();
            var context = new TestHandlerContext
            {
                Config = configMgr.Config,
                Dictionary = dictMgr.Config,
                CustomerInformationService = service
            };
            var ximples = new Queue<Ximple>();

            var target = new CustomerInformationServiceHandler();
            target.XimpleCreated += (s, e) => ximples.Enqueue(e.Ximple);
            target.Configure(context);
            target.Start();

            Assert.AreEqual(0, ximples.Count);

            service.SetVehicleData(
                new CustomerInformationServiceVehicleData
                {
                    VehicleStopRequested = new IBISIPboolean(false),
                    DoorState = DoorOpenStateEnumeration.AllDoorsClosed,
                    DoorStateSpecified = true,
                    ExitSide = ExitSideEnumeration.right,
                    ExitSideSpecified = true
                });

            Assert.AreEqual(1, ximples.Count);
            var ximple = ximples.Dequeue();
            Assert.AreEqual(4, ximple.Cells.Count);
            var cell = ximple.Cells.Single(c => c.LanguageNumber == 0 && c.TableNumber == 0 && c.ColumnNumber == 3);
            Assert.AreEqual(0, cell.RowNumber);
            Assert.AreEqual("0", cell.Value);
            cell = ximple.Cells.Single(c => c.LanguageNumber == 0 && c.TableNumber == 0 && c.ColumnNumber == 4);
            Assert.AreEqual(0, cell.RowNumber);
            Assert.AreEqual("0", cell.Value);
            cell = ximple.Cells.Single(c => c.LanguageNumber == 0 && c.TableNumber == 0 && c.ColumnNumber == 6);
            Assert.AreEqual(0, cell.RowNumber);
            Assert.AreEqual("1", cell.Value);
            cell = ximple.Cells.Single(c => c.LanguageNumber == 0 && c.TableNumber == 12 && c.ColumnNumber == 2);
            Assert.AreEqual(0, cell.RowNumber);
            Assert.AreEqual("Exit on the right", cell.Value);

            service.SetVehicleData(
                new CustomerInformationServiceVehicleData
                {
                    VehicleStopRequested = new IBISIPboolean(true),
                    DoorState = DoorOpenStateEnumeration.SingleDoorOpen,
                    DoorStateSpecified = true,
                    ExitSide = ExitSideEnumeration.unknown,
                    ExitSideSpecified = true
                });

            Assert.AreEqual(1, ximples.Count);
            ximple = ximples.Dequeue();
            Assert.AreEqual(4, ximple.Cells.Count);
            cell = ximple.Cells.Single(c => c.LanguageNumber == 0 && c.TableNumber == 0 && c.ColumnNumber == 3);
            Assert.AreEqual(0, cell.RowNumber);
            Assert.AreEqual("1", cell.Value);
            cell = ximple.Cells.Single(c => c.LanguageNumber == 0 && c.TableNumber == 0 && c.ColumnNumber == 4);
            Assert.AreEqual(0, cell.RowNumber);
            Assert.AreEqual("1", cell.Value);
            cell = ximple.Cells.Single(c => c.LanguageNumber == 0 && c.TableNumber == 0 && c.ColumnNumber == 6);
            Assert.AreEqual(0, cell.RowNumber);
            Assert.AreEqual("0", cell.Value);
            cell = ximple.Cells.Single(c => c.LanguageNumber == 0 && c.TableNumber == 12 && c.ColumnNumber == 2);
            Assert.AreEqual(0, cell.RowNumber);
            Assert.AreEqual(string.Empty, cell.Value);

            target.Stop();
        }

        private static CustomerInformationServiceAllData CreateAllData(int stopIndex)
        {
            var stopSequence = new StopInformationStructure[10];
            var time = new DateTime(2013, 12, 12, 17, 35, 00);
            for (int i = 0; i < stopSequence.Length; i++)
            {
                stopSequence[i] = new StopInformationStructure
                    {
                        StopIndex = new IBISIPint(i + 1),
                        ArrivalScheduled = new IBISIPdateTime(time.AddMinutes(i * 2)),
                        DepartureScheduled = new IBISIPdateTime(time.AddMinutes((i * 2) + 1)),
                        Platform = new IBISIPstring(GetAlphaString(i)),
                        StopName = CreateInternationalTexts(
                            "Haltestelle " + (i + 1), "Arrêt " + (i + 1), "Stop " + (i + 1)),
                        DisplayContent = new[]
                            {
                                new DisplayContentStructure
                                    {
                                        Destination = new DestinationStructure
                                            {
                                                DestinationName = CreateInternationalTexts(
                                                    "Ziel " + (i + 1),
                                                    "Destination " + (i + 1),
                                                    "Destination " + (i + 1))
                                            }
                                    }
                            }
                    };
            }

            var allData = new CustomerInformationServiceAllData
                {
                    TripInformation =
                        new[]
                            {
                                new TripInformationStructure
                                    {
                                        LocationState = LocationStateEnumeration.AtStop,
                                        LocationStateSpecified = true,
                                        StopSequence = stopSequence
                                    }
                            },
                    CurrentStopIndex = new IBISIPint(stopIndex)
                };
            return allData;
        }

        private static CustomerInformationServiceAllData CreateAllDataWithConnections(int stopIndex)
        {
            var data = CreateAllData(stopIndex);
            var time = new DateTime(2013, 12, 12, 17, 55, 00);
            foreach (var stop in data.TripInformation[0].StopSequence)
            {
                var index = stop.StopIndex.Value;
                stop.Connection = new ConnectionStructure[3 - (index % 4)];
                for (int i = 0; i < stop.Connection.Length; i++)
                {
                    var nonRandom = ((index * 7) + (i * 13)) % 11;
                    stop.Connection[i] = new ConnectionStructure
                        {
                            ConnectionState = (ConnectionStateEnumeration)(nonRandom % 3),
                            ConnectionStateSpecified = true,
                            ConnectionType = (ConnectionTypeEnumeration)(nonRandom % 2),
                            Platform = new IBISIPstring("Platform " + GetAlphaString(nonRandom)),
                            TransportMode = new VehicleStructure
                                {
                                    Name = CreateInternationalTexts("Schiff", "Bateau", "Ship")
                                },
                            DisplayContent = new DisplayContentStructure
                                {
                                    Destination = new DestinationStructure
                                        {
                                            DestinationName = CreateInternationalTexts(
                                                "Z" + nonRandom, "D" + nonRandom, "D" + nonRandom)
                                        }
                                },
                            ExpectedDepatureTime = new IBISIPdateTime(time.AddMinutes(nonRandom))
                        };
                }
            }

            return data;
        }

        private static CustomerInformationServiceTripData CreateTripData(int stopIndex)
        {
            var all = CreateAllData(stopIndex);
            var data = new CustomerInformationServiceTripData
                           {
                               CurrentStopIndex = all.CurrentStopIndex,
                               DefaultLanguage = all.DefaultLanguage,
                               TimeStamp = all.TimeStamp,
                               TripInformation = all.TripInformation[0],
                               VehicleRef = all.VehicleRef
                           };
            return data;
        }

        private static InternationalTextType[] CreateInternationalTexts(string german, string french, string english)
        {
            return new[]
                       {
                           new InternationalTextType { Language = "de-DE", Value = german },
                           new InternationalTextType { Language = "fr-FR", Value = french },
                           new InternationalTextType { Language = "en-GB", Value = english }
                       };
        }

        private static string GetAlphaString(int offset)
        {
            return ((char)('A' + offset)).ToString(CultureInfo.InvariantCulture);
        }
    }
}
