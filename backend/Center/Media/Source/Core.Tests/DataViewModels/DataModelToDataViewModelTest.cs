// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataModelToDataViewModelTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataModelToDataViewModelTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests.DataViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.Models.Layout;
    using Gorba.Center.Media.Core.Models.Presentation;
    using Gorba.Common.Configuration.Infomedia.Common;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Defines all tests for the conversion from DataModel to DataViewModel.
    /// </summary>
    [TestClass]
    public class DataModelToDataViewModelTest
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
        /// Cleanup the test.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            Helpers.ResetServiceLocator();
        }

        /// <summary>
        /// Tests the conversion of a DataModel that contains a list typed by a base class.
        /// </summary>
        [TestMethod]
        public void CreateViewModelContainingAbstractListTest()
        {
            var dataModel = new ResolutionConfigDataModel { DisplayText = "DisplayText", Height = 100, Width = 50 };
            var font = new FontDataModel { Color = "Red", Face = "TimesRoman" };
            var textElement = new TextElementDataModel { Value = "TestText", Font = font };
            dataModel.Elements.Add(textElement);
            var mediaShellMock = Helpers.CreateMediaShell();
            var viewModel = new ResolutionConfigDataViewModel(mediaShellMock.Object, dataModel);
            Assert.AreEqual(dataModel.DisplayText, viewModel.DisplayText);
            Assert.AreEqual(dataModel.Height, viewModel.Height.Value);
            Assert.AreEqual(dataModel.Width, viewModel.Width.Value);
            Assert.AreEqual(1, viewModel.Elements.Count);
            Assert.AreEqual(typeof(TextElementDataViewModel), viewModel.Elements[0].GetType());
            var textViewModel = (TextElementDataViewModel)viewModel.Elements[0];
            Assert.AreEqual("TestText", textViewModel.Value.Value);
            Assert.AreEqual(font.Color, textViewModel.Font.Color.Value);
        }

        /// <summary>
        /// Verifies that referenced objects are also set when creating a view model.
        /// </summary>
        [TestMethod]
        public void CreateViewModelContainingReferencesTest()
        {
            var dataModel = new VirtualDisplayConfigDataModel
                                {
                                    CyclePackage = "ReferenceName",
                                    Name = "VirtualDisplay1"
                                };
            var mediaShellMock = Helpers.CreateMediaShell();
            var infomediaConfig = new InfomediaConfigDataViewModel(mediaShellMock.Object);
            var cyclePackage = new CyclePackageConfigDataViewModel(mediaShellMock.Object)
                                   {
                                       Name = new DataValue<string>("ReferenceName"),
                                       DisplayText = "CyclePackage"
                                   };
            infomediaConfig.CyclePackages.Add(cyclePackage);
            var applicationState = new Mock<IMediaApplicationState>();
            applicationState.Setup(state => state.CurrentProject.InfomediaConfig).Returns(infomediaConfig);
            var unityContainer = ServiceLocator.Current.GetInstance<IUnityContainer>();
            unityContainer.RegisterInstance(applicationState.Object);
            var viewModel = new VirtualDisplayConfigDataViewModel(mediaShellMock.Object, dataModel);
            Assert.IsNotNull(viewModel.CyclePackage);
            Assert.AreEqual("ReferenceName", viewModel.CyclePackage.Name.Value);
        }

        /// <summary>
        /// Verifies that also the properties of the base class are set.
        /// </summary>
        [TestMethod]
        public void CreateLayoutConfigViewModelTest()
        {
            var dataModel = new LayoutConfigDataModel
                                {
                                    Name = "TestLayout",
                                    Resolutions =
                                        new List<ResolutionConfigDataModel>
                                            {
                                                new ResolutionConfigDataModel
                                                    {
                                                        Height = 10,
                                                        Width = 100
                                                    }
                                            }
                                };
            var mediaShellMock = Helpers.CreateMediaShell();
            var viewModel = new LayoutConfigDataViewModel(mediaShellMock.Object, dataModel);
            Assert.AreEqual(dataModel.Name, viewModel.Name.Value);
            Assert.AreEqual(1, viewModel.Resolutions.Count);
        }

        /// <summary>
        /// Verifies that an animated dynamic property is converted properly.
        /// </summary>
        [TestMethod]
        public void CreateDataViewModelWithAnimationTest()
        {
            var dataModel = new TextElementDataModel
            {
                Value = "StaticText",
                ValueProperty = new AnimatedDynamicPropertyDataModel
                                    {
                                        Animation = new PropertyChangeAnimationDataModel
                                                        {
                                                            Duration = TimeSpan.FromSeconds(10),
                                                            Type = PropertyChangeAnimationType.Linear
                                                        }
                                    },
                ElementName = "StaticTextName"
            };
            var mediaShellMock = Helpers.CreateMediaShell();
            var viewModel = new TextElementDataViewModel(mediaShellMock.Object, dataModel);
            Assert.AreEqual(dataModel.ElementName, viewModel.ElementName.Value);
            Assert.IsNull(viewModel.Value.Formula);
            Assert.IsNotNull(viewModel.Value.Animation);
            Assert.AreEqual(
                TimeSpan.FromSeconds(10), ((AnimationDataViewModel)viewModel.Value.Animation).Duration.Value);
            Assert.AreEqual(
                PropertyChangeAnimationType.Linear, ((AnimationDataViewModel)viewModel.Value.Animation).Type.Value);
        }

        /// <summary>
        /// Verifies that a complete tree is initialized correctly.
        /// </summary>
        [TestMethod]
        public void CreateInfomediaConfigDataViewModelTest()
        {
            var dataModel = Helpers.CreateTestInfomediaConfigDataModel();
            var mediaShellMock = Helpers.CreateMediaShell();
            var viewModel = new InfomediaConfigDataViewModel(mediaShellMock.Object, dataModel);
            var applicationState = new Mock<IMediaApplicationState>();
            applicationState.Setup(state => state.CurrentProject.InfomediaConfig).Returns(viewModel);
            var unityContainer = ServiceLocator.Current.GetInstance<IUnityContainer>();
            unityContainer.RegisterInstance(applicationState.Object);
            Assert.IsNotNull(viewModel.CyclePackages);
            Assert.AreEqual(1, viewModel.CyclePackages.Count);
            var cyclePackage = viewModel.CyclePackages[0];
            Assert.AreEqual(1, cyclePackage.StandardCycles.Count);
            var cycleRef = cyclePackage.StandardCycles.FirstOrDefault();
            Assert.IsNotNull(cycleRef);
            Assert.IsNotNull(cycleRef.Reference);
            var cycle = cycleRef.Reference;
            Assert.AreEqual("Test cycle", cycle.Name.Value);
            Assert.IsNotNull(cycle.Sections);
            var layout = cycle.Sections[0].Layout;
            Assert.IsNotNull(layout);
            Assert.AreEqual("Test Layout 1", layout.Name.Value);
            Assert.AreEqual(2, viewModel.Layouts.Count);
            var testLayout2 = viewModel.Layouts[1];
            Assert.AreEqual(1, testLayout2.Resolutions.Count);
            var resolution = testLayout2.Resolutions.First();
            Assert.AreEqual(3, resolution.Elements.Count);
            var dynamicText =
                resolution.Elements.First(element => element.GetType() == typeof(DynamicTextElementDataViewModel));
            Assert.IsTrue(((DynamicTextElementDataViewModel)dynamicText).Visible.Value);
            var staticText = resolution.Elements.First(e => e.GetType() == typeof(StaticTextElementDataViewModel));
            Assert.IsFalse(((StaticTextElementDataViewModel)staticText).Visible.Value);
        }
    }
}
