// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaProjectDataViewModelTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediaProjectDataViewModelTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests.DataViewModels
{
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Media.Core.DataViewModels.Dictionary;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Common.Protocols.Ximple.Generic;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for the <see cref="MediaProjectDataViewModel"/> class.
    /// </summary>
    [TestClass]
    public class MediaProjectDataViewModelTest
    {
        /// <summary>
        /// Gets or sets the test context.
        /// </summary>
        /// <value>
        /// The test context.
        /// </value>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Verifies the handling of the aggregated <see cref="MediaProjectDataViewModel.IsDirty"/> flag.
        /// </summary>
        [TestMethod]
        public void IsDirtyTest()
        {
            var shellMock = this.CreateMediaShell();
            var commandRegistryMock = new Mock<ICommandRegistry>();
            var mediaProjectDataModel = new MediaProjectDataModel();
            var mediaProjectDataViewModel = new MediaProjectDataViewModel(
                shellMock.Object, commandRegistryMock.Object, mediaProjectDataModel);
            Assert.IsFalse(mediaProjectDataViewModel.IsDirty);
            mediaProjectDataViewModel.Authors.Add("author");
            Assert.IsTrue(mediaProjectDataViewModel.IsDirty);
        }

        /// <summary>
        /// Verifies that the references of cycles are set up correctly after loading
        /// </summary>
        [TestMethod]
        public void CycleReferencesLoadTest()
        {
            ResetServiceLocator();
            var applicationStateMock = new Mock<IMediaApplicationState>();
            var container = ServiceLocator.Current.GetInstance<IUnityContainer>();
            container.RegisterInstance(applicationStateMock.Object);
            var shellMock = this.CreateMediaShell();
            var commandRegistyMock = new Mock<ICommandRegistry>();
            var mediaProjectDataModel = new MediaProjectDataModel
                                            {
                                                InfomediaConfig =
                                                    Helpers.CreateTestInfomediaConfigDataModel()
                                            };
            var mediaProjectDataViewModel = new MediaProjectDataViewModel(
                shellMock.Object, commandRegistyMock.Object, mediaProjectDataModel);
            var infomediaConfigDataViewModel = mediaProjectDataViewModel.InfomediaConfig;
            applicationStateMock.Setup(a => a.CurrentProject).Returns(mediaProjectDataViewModel);

            Assert.AreEqual(1, infomediaConfigDataViewModel.CyclePackages[0].StandardCycles.Count);
            Assert.AreEqual(1, infomediaConfigDataViewModel.CyclePackages[0].EventCycles.Count);

            Assert.AreEqual(1, infomediaConfigDataViewModel.CyclePackages[0].StandardCycles.Count);
            Assert.AreEqual(1, infomediaConfigDataViewModel.CyclePackages[0].EventCycles.Count);

            Assert.IsNotNull(infomediaConfigDataViewModel.CyclePackages[0].StandardCycles[0].Reference);
            Assert.IsNotNull(infomediaConfigDataViewModel.CyclePackages[0].EventCycles[0].Reference);
        }

        /// <summary>
        /// Creates an IMediaShell mock which contains a dictionary.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        /// <returns>
        /// The media shell mock.
        /// </returns>
        protected Mock<IMediaShell> CreateMediaShell(DictionaryDataViewModel dictionary = null)
        {
            if (dictionary == null)
            {
                dictionary = new DictionaryDataViewModel(new Dictionary());
            }

            var mock = new Mock<IMediaShell>();
            mock.SetupGet(s => s.Dictionary).Returns(dictionary);
            return mock;
        }

        private static void ResetServiceLocator()
        {
            var unityContainer = new UnityContainer();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(unityContainer));
        }
    }
}