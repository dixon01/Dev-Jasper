// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitConfiguratorControllerTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Tests for the <see cref="UnitConfiguratorController" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Tests.Controllers.UnitConfig
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Gorba.Center.Admin.Core.Controllers.Entities;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Hardware;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.MainUnit;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.ThorebC90;
    using Gorba.Center.Admin.Core.DataViewModels.Configurations;
    using Gorba.Center.Admin.Core.Models;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.Configurations;
    using Gorba.Center.Common.ServiceModel.Documents;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.ServiceModel.Units;
    using Gorba.Common.Configuration.HardwareDescription;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for the <see cref="UnitConfiguratorController"/>.
    /// </summary>
    [TestClass]
    public class UnitConfiguratorControllerTest
    {
        /// <summary>
        /// The initialize.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            Helpers.ResetServiceLocator();
        }

        /// <summary>
        /// The cleanup.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            Helpers.ResetServiceLocator();
        }

        /// <summary>
        /// Tests the initialization of the <see cref="UnitConfiguratorController"/>.
        /// </summary>
        [TestMethod]
        [DeploymentItem("dictionary.xml")]
        public void InitializationTest()
        {
            var testSetup = SetupTest();
            var unitConfigController = testSetup.UnitConfiguratorController;
            Assert.AreEqual(12, unitConfigController.GetExportControllers().Count());
            Assert.AreEqual(18, unitConfigController.ViewModel.Categories.Count);
        }

        /// <summary>
        /// Tests getting an existing part controller.
        /// </summary>
        [TestMethod]
        [DeploymentItem("dictionary.xml")]
        public void GetPartTest()
        {
            var testSetup = SetupTest();
            var unitConfigController = testSetup.UnitConfiguratorController;
            var part = unitConfigController.GetPart<InputsPartController>(UnitConfigKeys.Hardware.Inputs);
            Assert.IsNotNull(part);
        }

        /// <summary>
        /// Tests getting a part controller from OBU category and checks the visibility of the ViewModel
        /// depending on the unit type.
        /// </summary>
        [TestMethod]
        [DeploymentItem("dictionary.xml")]
        public void ObuVisibilityTest()
        {
            var testSetup = SetupTest(UnitTypes.Obu);
            var unitConfigController = testSetup.UnitConfiguratorController;
            var part = unitConfigController.GetPart<BusPartController>(UnitConfigKeys.ThorebC90C74.Bus);
            Assert.IsNotNull(part);
            Assert.IsTrue(part.ViewModel.IsVisible);
            testSetup = SetupTest();
            unitConfigController = testSetup.UnitConfiguratorController;
            part = unitConfigController.GetPart<BusPartController>(UnitConfigKeys.ThorebC90C74.Bus);
            Assert.IsNotNull(part);
            Assert.IsFalse(part.ViewModel.IsVisible);
        }

        /// <summary>
        /// Tests the initialization of the <see cref="UnitConfiguratorController"/>.
        /// </summary>
        [TestMethod]
        [DeploymentItem("dictionary.xml")]
        public void MainUnitInitializationTest()
        {
            var testSetup = SetupTest(UnitTypes.EPaper, "PU.night-2");
            var unitConfigController = testSetup.UnitConfiguratorController;
            Assert.AreEqual(1, unitConfigController.GetExportControllers().Count());
            Assert.AreEqual(3, unitConfigController.ViewModel.Categories.Count);
        }

        /// <summary>
        /// Tests if a main unit with one display unit does not contain a second part
        /// </summary>
        [TestMethod]
        [DeploymentItem("dictionary.xml")]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void MainUnitSingleDisplayVisibilityTest()
        {
            var testSetup = SetupTest(UnitTypes.EPaper, "PU.night-1");
            var unitConfigController = testSetup.UnitConfiguratorController;
            var part =
                unitConfigController.GetPart<DisplayUnitPartController>(UnitConfigKeys.MainUnit.DisplayUnit + "1");
            Assert.IsNotNull(part);
            Assert.IsTrue(part.ViewModel.IsVisible);

            unitConfigController.GetPart<DisplayUnitPartController>(UnitConfigKeys.MainUnit.DisplayUnit + "2");
        }

        /// <summary>
        /// Tests if a main unit with two display units contain the required parts.
        /// </summary>
        [TestMethod]
        [DeploymentItem("dictionary.xml")]
        public void MainUnitDualDisplayVisibilityTest()
        {
            var testSetup = SetupTest(UnitTypes.EPaper, "PU.night-2");
            var unitConfigController = testSetup.UnitConfiguratorController;
            var part =
                unitConfigController.GetPart<DisplayUnitPartController>(UnitConfigKeys.MainUnit.DisplayUnit + "1");
            Assert.IsNotNull(part);
            Assert.IsTrue(part.ViewModel.IsVisible);

            part =
                unitConfigController.GetPart<DisplayUnitPartController>(UnitConfigKeys.MainUnit.DisplayUnit + "2");
            Assert.IsNotNull(part);
            Assert.IsTrue(part.ViewModel.IsVisible);
        }

        /// <summary>
        /// Test that the display units are not created for other products.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        [DeploymentItem("dictionary.xml")]
        public void MainUnitPartNotCreatedTest()
        {
            // not visible on other unit configurations
            var testSetup = SetupTest();
            var unitConfigController = testSetup.UnitConfiguratorController;
            unitConfigController.GetPart<DisplayUnitPartController>(UnitConfigKeys.MainUnit.DisplayUnit + "1");
        }

        /// <summary>
        /// Tests that an exception is thrown if a part controller could not be found.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        [DeploymentItem("dictionary.xml")]
        public void GetNonExistingPartTest()
        {
            var testSetup = SetupTest();
            var unitConfigController = testSetup.UnitConfiguratorController;

            // ReSharper disable once UnusedVariable
            var part = unitConfigController.GetPart<InputsPartController>(UnitConfigKeys.Software.Category);
        }

        /// <summary>
        /// Tests the NavigateToPart command.
        /// </summary>
        [TestMethod]
        [DeploymentItem("dictionary.xml")]
        public void NavigateToPartTest()
        {
            var testSetup = SetupTest();
            var unitConfigController = testSetup.UnitConfiguratorController;
            Assert.IsNull(unitConfigController.ViewModel.SelectedItem);
            var part = unitConfigController.GetPart<InputsPartController>(UnitConfigKeys.Hardware.Inputs);
            unitConfigController.CommandRegistry.GetCommand(CommandCompositionKeys.UnitConfig.NavigateToPart)
                .Execute(part.ViewModel);
            Assert.AreEqual(part.ViewModel, unitConfigController.ViewModel.SelectedItem);
            Assert.IsTrue(part.ViewModel.Category.IsExpanded);
        }

        /// <summary>
        /// Tests the previous navigate within one category.
        /// </summary>
        [TestMethod]
        [DeploymentItem("dictionary.xml")]
        public void NavigateToPreviousPartTest()
        {
            var testSetup = SetupTest();
            var unitConfigController = testSetup.UnitConfiguratorController;
            var currentCategory = unitConfigController.ViewModel.Categories[2];
            var currentPart = currentCategory.Parts[1];
            unitConfigController.ViewModel.SelectedItem = currentPart;
            unitConfigController.CommandRegistry.GetCommand(CommandCompositionKeys.UnitConfig.NavigateToPrevious)
                .Execute(null);
            Assert.AreEqual(currentCategory.Parts.First(), unitConfigController.ViewModel.SelectedItem);
        }

        /// <summary>
        /// Tests the previous navigation if the current selected item is the first part of a category.
        /// </summary>
        [TestMethod]
        [DeploymentItem("dictionary.xml")]
        public void NavigateToPreviousFromFirstPartToCategoryTest()
        {
            var testSetup = SetupTest();
            var unitConfigController = testSetup.UnitConfiguratorController;
            var currentCategory = unitConfigController.ViewModel.Categories[1];
            var currentPart = currentCategory.Parts.First();
            unitConfigController.ViewModel.SelectedItem = currentPart;
            unitConfigController.CommandRegistry.GetCommand(CommandCompositionKeys.UnitConfig.NavigateToPrevious)
                .Execute(null);
            Assert.AreEqual(currentCategory, unitConfigController.ViewModel.SelectedItem);
        }

        /// <summary>
        /// Tests the previous navigation if the current selected item is a category other than the first one.
        /// </summary>
        [TestMethod]
        [DeploymentItem("dictionary.xml")]
        public void NavigateToPreviousFromCategoryToPartTest()
        {
            var testSetup = SetupTest();
            var unitConfigController = testSetup.UnitConfiguratorController;
            Assert.IsNull(unitConfigController.ViewModel.SelectedItem);
            var category = unitConfigController.ViewModel.Categories[1];
            unitConfigController.ViewModel.SelectedItem = category;
            var nextCategory = unitConfigController.ViewModel.Categories[2];
            unitConfigController.ViewModel.SelectedItem = nextCategory;
            unitConfigController.CommandRegistry.GetCommand(CommandCompositionKeys.UnitConfig.NavigateToPrevious)
                .Execute(null);
            Assert.AreEqual(category.Parts.Last(), unitConfigController.ViewModel.SelectedItem);
        }

        /// <summary>
        /// Tests the next navigation if the current selected item and the next are parts within a category.
        /// </summary>
        [TestMethod]
        [DeploymentItem("dictionary.xml")]
        public void NavigateToNextPartTest()
        {
            var testSetup = SetupTest();
            var unitConfigController = testSetup.UnitConfiguratorController;
            var currentCategory = unitConfigController.ViewModel.Categories[2];
            var currentpart = currentCategory.Parts.First();
            unitConfigController.ViewModel.SelectedItem = currentpart;
            unitConfigController.CommandRegistry.GetCommand(CommandCompositionKeys.UnitConfig.NavigateToNext)
                .Execute(null);
            Assert.AreEqual(currentCategory.Parts[1], unitConfigController.ViewModel.SelectedItem);
        }

        /// <summary>
        /// Tests the next navigation if the current selected item is the last part of the first category.
        /// </summary>
        [TestMethod]
        [DeploymentItem("dictionary.xml")]
        public void NavigateToNextCategoryFromLastPartTest()
        {
            var testSetup = SetupTest();
            var unitConfigController = testSetup.UnitConfiguratorController;
            var currentCategory = unitConfigController.ViewModel.Categories.Skip(1).First();
            var currentpart = currentCategory.Parts.Last();
            unitConfigController.ViewModel.SelectedItem = currentpart;
            unitConfigController.CommandRegistry.GetCommand(CommandCompositionKeys.UnitConfig.NavigateToNext)
                .Execute(null);
            var nextCategory = unitConfigController.ViewModel.Categories[2];
            Assert.AreEqual(nextCategory, unitConfigController.ViewModel.SelectedItem);
        }

        /// <summary>
        /// Tests the can navigate to the next part. Once where it is possible and when we are already on the last
        /// part.
        /// </summary>
        [TestMethod]
        [DeploymentItem("dictionary.xml")]
        public void CanNavigateToNextTest()
        {
            var testSetup = SetupTest();
            var viewModel = testSetup.UnitConfiguratorController.ViewModel;
            viewModel.SelectedItem = viewModel.Categories.Skip(1).First().Parts.First();
            Assert.IsTrue(
                testSetup.UnitConfiguratorController.CommandRegistry.GetCommand(
                    CommandCompositionKeys.UnitConfig.NavigateToNext).CanExecute(null));
            viewModel.SelectedItem = viewModel.Categories.Last().FilteredParts.OfType<PartViewModelBase>().Last();
            Assert.IsFalse(
                testSetup.UnitConfiguratorController.CommandRegistry.GetCommand(
                    CommandCompositionKeys.UnitConfig.NavigateToNext).CanExecute(null));
        }

        private static SetupTestResult SetupTest(UnitTypes unitType = UnitTypes.Tft, string productName = "TestProduct")
        {
            var container = Helpers.InitializeServiceLocator();
            var currentTenant = new Tenant { Id = 1, Name = "TestTenant" };
            Helpers.CreateApplicationStateMock(container, currentTenant);
            var connectionControllerMockSetup = Helpers.CreateConnectionControllerMock();
            var resourceServiceMock = new Mock<IResourceService>();
            var resourceChannel = ChannelScope<IResourceService>.Create(resourceServiceMock.Object);
            connectionControllerMockSetup.ConnectionControllerMock.Setup(
                controller => controller.CreateChannelScope<IResourceService>())
                .Returns(resourceChannel);
            var dataController = Helpers.SetupDataController(connectionControllerMockSetup.ConnectionControllerMock);
            var unitConfig = CreateUnitConfigurationDataViewModel(dataController, unitType, productName);

            var unitConfigController = new UnitConfiguratorController(unitConfig, dataController);
            foreach (var category in unitConfigController.ViewModel.Categories)
            {
                category.CanBeVisible = true;
            }

            return new SetupTestResult
                       {
                           ConnectionControllerMock = connectionControllerMockSetup,
                           DataController = dataController,
                           UnitConfiguratorController = unitConfigController
                       };
        }

        private static UnitConfigurationReadOnlyDataViewModel CreateUnitConfigurationDataViewModel(
            DataController dataController, UnitTypes unitType, string productName)
        {
            var documentVersion = new DocumentVersion
                                      {
                                          Id = 100,
                                          CreatingUser = new User { Id = 5, FirstName = "User" },
                                          Minor = 1,
                                          Major = 2,
                                          Content =
                                              new XmlData(
                                              new UnitConfigData
                                                  {
                                                      Categories =
                                                          {
                                                              new UnitConfigCategory { Key = "A" },
                                                              new UnitConfigCategory { Key = "B" }
                                                          }
                                                  })
                                      };
            var document = new Document
                               {
                                   Id = 10,
                                   Name = "UnitDocument",
                                   Tenant =
                                       ServiceLocator.Current.GetInstance<IAdminApplicationState>()
                                       .CurrentTenant.ToDto(),
                                   Versions = new Collection<DocumentVersion> { documentVersion }
                               };
            var productType = new ProductType
                                  {
                                      HardwareDescriptor = CreateHardwareDescriptor(),
                                      Name = productName,
                                      Id = 50,
                                      UnitType = unitType,
                                  };
            var unitConfig = new UnitConfiguration { Id = 20, Document = document, ProductType = productType };
            var readable = new Helpers.UnitConfigurationReadableModelMock(unitConfig);
            return dataController.Factory.CreateReadOnly(readable);
        }

        private static XmlData CreateHardwareDescriptor()
        {
            var descriptor = new HardwareDescriptor
                {
                    Name = "TestProduct",
                    Platform =
                        new InfoVisionPlatformDescriptor
                        {
                            DisplayAdapters =
                                                       {
                                                           new DisplayAdapterDescriptor(0, DisplayConnectionType.Dvi),
                                                           new DisplayAdapterDescriptor(1, DisplayConnectionType.Dvi)
                                                       },
                            HasSharedRs485Port = false,
                            Transceivers =
                                                       {
                                                           new MultiProtocolTransceiverDescriptor(0),
                                                           new MultiProtocolTransceiverDescriptor(1)
                                                       },
                            HasGenericButton = true,
                            HasGenericLed = true,
                            Inputs =
                                                       {
                                                           new InputDescriptor(0, "IN0 (Pins 1/2)"),
                                                           new InputDescriptor(1, "IN1 (Pins 3/4)"),
                                                           new InputDescriptor(2, "IN2 (Pins 5/6)"),
                                                           new InputDescriptor(3, "IN3 (Pins 7/8)")
                                                       },
                            Outputs =
                                                       {
                                                           new OutputDescriptor(4, "OUT0 (Pins 9/10)"),
                                                           new OutputDescriptor(5, "OUT1 (Pins 11/12)"),
                                                           new OutputDescriptor(6, "OUT2 (Pins 13/14)"),
                                                           new OutputDescriptor(7, "OUT3 (Pins 15/16)")
                                                       },
                            SerialPorts =
                                                       {
                                                           new SerialPortDescriptor("COM3", true),
                                                           new SerialPortDescriptor("COM4")
                                                       }
                        },
                    OperatingSystem =
                        new WindowsEmbeddedDescriptor
                        {
                            Version = OperatingSystemVersion.WindowsEmbedded8Standard
                        }
                };
            return new XmlData(descriptor);
        }

        private class SetupTestResult
        {
            public UnitConfiguratorController UnitConfiguratorController { get; set; }

            public Helpers.ConnectionControllerResult ConnectionControllerMock { get; set; }

            public DataController DataController { get; set; }
        }
    }
}
