// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditorControllerCommandsTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines tests for the commands of the .
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;

    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Common.Wpf.Framework.Startup;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Defines tests for the commands of the editor controllers/>.
    /// </summary>
    [TestClass]
    public class EditorControllerCommandsTest
    {
        private MediaApplicationState state;
        private UnityContainer unityContainer;

        /// <summary>
        /// Initializes this test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            this.unityContainer = new UnityContainer();
            var resourceManagerMock = new Mock<IResourceManager>();
                        var textElementReferenceManager = new TextElementReferenceManager();
            resourceManagerMock.SetupGet(manager => manager.TextElementManager)
                .Returns(() => textElementReferenceManager);
            this.unityContainer.RegisterInstance(resourceManagerMock.Object);
            var unityServiceLocator = new UnityServiceLocator(this.unityContainer);
            ServiceLocator.SetLocatorProvider(() => unityServiceLocator);
            this.state = Helpers.MockApplicationState(this.unityContainer);
            this.unityContainer.RegisterInstance(typeof(IApplicationState), this.state);
            var changeHistoryMock = new Mock<IChangeHistory>();
            var changeHistoryFactoryMock = new Mock<ChangeHistoryFactory>();
            changeHistoryFactoryMock.Setup(factory => factory.Create()).Returns(changeHistoryMock.Object);
            ChangeHistoryFactory.SetCurrent(changeHistoryFactoryMock.Object);
            var applicationControllerMock = new Mock<IMediaApplicationController>();
            applicationControllerMock.Setup(
                controller => controller.ShellController.ProjectController.OnProjectGotDirty())
                .Returns(Task.FromResult(0));
            this.unityContainer.RegisterInstance(typeof(IMediaApplicationController), applicationControllerMock.Object);
        }

        /// <summary>
        /// Cleans up this test.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            ChangeHistoryFactory.ResetCurrent();
            var serviceLocator = new Mock<IServiceLocator>(MockBehavior.Strict);
            ServiceLocator.SetLocatorProvider(() => serviceLocator.Object);
            this.unityContainer = null;
        }

        /// <summary>
        /// Tests the creation of a layout element.
        /// </summary>
        [TestMethod]
        public void CreateElementCommandTest()
        {
            var editor = Helpers.InitializeEditorContent(this.state);
            var createElementParameters = new CreateElementParameters
                                              {
                                                  Bounds = new Rect(10, 20, 30, 40),
                                                  Type = LayoutElementType.StaticText
                                              };

            editor.CreateLayoutElementCommand.Execute(createElementParameters);
            var actualElements = editor.Elements;
            Assert.AreEqual(2, actualElements.Count);
            var element = actualElements.First() as StaticTextElementDataViewModel;
            Assert.IsNotNull(element);
            Assert.AreEqual(MediaStrings.ElementName_StaticText + "1", element.ElementName.Value);
            Assert.AreEqual("StaticText1", element.Value.Value);
            Assert.AreEqual(40, element.Height.Value);
            Assert.AreEqual(30, element.Width.Value);
            Assert.AreEqual(20, element.Y.Value);
            Assert.AreEqual(10, element.X.Value);
            Assert.AreEqual(1, element.ZIndex.Value);
        }

        /// <summary>
        /// Tests the deletion of a layout element.
        /// </summary>
        [TestMethod]
        public void DeleteElementCommandTest()
        {
            var editor = Helpers.InitializeEditorContent(this.state);

            var viewModelToDelete = new ImageElementDataViewModel(editor.Parent);
            var parameters = new List<DrawableElementDataViewModelBase> { viewModelToDelete };
            editor.Elements.Add(viewModelToDelete);
            editor.Elements.Add(new VideoElementDataViewModel(editor.Parent));
            editor.SelectedElements.Add(viewModelToDelete);
            Assert.AreEqual(3, editor.Elements.Count);
            Assert.AreEqual(1, editor.SelectedElements.Count);
            editor.DeleteElementsCommand.Execute(parameters);
            Assert.AreEqual(2, editor.Elements.Count);
            Assert.AreEqual(0, editor.SelectedElements.Count);
        }

        /// <summary>
        /// Tests the selection of one layout element by "clicking" on it.
        /// </summary>
        [TestMethod]
        public void SelectLayoutElementCommandTest()
        {
            var editor = Helpers.InitializeEditorContent(this.state);

            var staticTextViewModel = new StaticTextElementDataViewModel(editor.Parent)
                                          {
                                              X = new DataValue<int>(5),
                                              Y = new DataValue<int>(5),
                                              Width = new DataValue<int>(100),
                                              Height = new DataValue<int>(60),
                                              ElementName = new DataValue<string>("StaticText1")
                                          };
            editor.Elements.Add(staticTextViewModel);
            var parameters = new SelectElementParameters
                                 {
                                     Modifiers = new ModifiersState(),
                                     Position = new Point(20, 20)
                                 };
            Assert.AreEqual(0, editor.SelectedElements.Count);
            editor.SelectLayoutElementCommand.Execute(parameters);
            Assert.AreEqual(1, editor.SelectedElements.Count);
            var element = editor.SelectedElements.First() as StaticTextElementDataViewModel;
            Assert.IsNotNull(element);
            Assert.AreEqual(staticTextViewModel.ElementName, element.ElementName);
        }

        /// <summary>
        /// Tests the selection of layout elements with a rectangle.
        /// </summary>
        [TestMethod]
        public void SelectElementsCommandTest()
        {
            var editor = Helpers.InitializeEditorContent(this.state);

            var staticTextViewModel1 = new StaticTextElementDataViewModel(editor.Parent)
                                           {
                                               X = new DataValue<int>(10),
                                               Y = new DataValue<int>(10),
                                               Width = new DataValue<int>(100),
                                               Height = new DataValue<int>(50),
                                               ElementName = new DataValue<string>("StaticText1")
                                           };
            editor.Elements.Add(staticTextViewModel1);
            var staticTextViewModel2 = new StaticTextElementDataViewModel(editor.Parent)
                                           {
                                               X = new DataValue<int>(150),
                                               Y = new DataValue<int>(100),
                                               Width = new DataValue<int>(100),
                                               Height = new DataValue<int>(50),
                                               ElementName = new DataValue<string>("StaticText2")
                                           };
            editor.Elements.Add(staticTextViewModel2);
            Assert.AreEqual(0, editor.SelectedElements.Count);
            Assert.AreEqual(3, editor.Elements.Count);
            var parameters = new CreateElementParameters
                                 {
                                     Bounds = new Rect(10, 10, 20, 20),
                                     Modifiers = new ModifiersState()
                                 };
            editor.SelectLayoutElementsCommand.Execute(parameters);
            Assert.AreEqual(0, editor.SelectedElements.Count);
            parameters.Bounds = new Rect(5, 5, 120, 70);
            editor.SelectLayoutElementsCommand.Execute(parameters);
            Assert.AreEqual(1, editor.SelectedElements.Count);
            Assert.AreEqual(staticTextViewModel1.ElementName, editor.SelectedElements.First().ElementName);
            parameters.Bounds = new Rect(140, 90, 200, 100);
            editor.SelectLayoutElementsCommand.Execute(parameters);
            Assert.AreEqual(1, editor.SelectedElements.Count);
            Assert.AreEqual(staticTextViewModel2.ElementName, editor.SelectedElements.First().ElementName);
            parameters.Bounds = new Rect(5, 5, 400, 300);
            editor.SelectLayoutElementsCommand.Execute(parameters);
            Assert.AreEqual(2, editor.SelectedElements.Count);
            Assert.IsTrue(editor.SelectedElements.Contains(staticTextViewModel1));
            Assert.IsTrue(editor.SelectedElements.Contains(staticTextViewModel2));
        }
    }
}