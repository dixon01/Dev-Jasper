// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddListElementHistoryEntryTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Tests for  undo and redo with specific entries for T.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests.History
{
    using System.IO;
    using System.Linq;

    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.History;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.Properties;
    using Gorba.Center.Media.Core.ViewModels;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for <see cref="AddListElementHistoryEntry{T}"/> undo and redo with specific entries for T.
    /// </summary>
    [TestClass]
    public class AddListElementHistoryEntryTest
    {
        /// <summary>
        /// Tests adding an image to the collections and undo this operation.
        /// </summary>
        [TestMethod]
        public void UndoRedoAddImageTest()
        {
            var mediaShell = new Mock<IMediaShell>();
            var infomediaConfig = new InfomediaConfigDataViewModel(null);
            var fontsList = new ExtendedObservableCollection<FontConfigDataViewModel>();
            infomediaConfig.Fonts = fontsList;
            mediaShell.Setup(m => m.MediaApplicationState.CurrentProject.InfomediaConfig).Returns(infomediaConfig);
            var resource = new ResourceInfoDataViewModel { Type = ResourceType.Image, Filename = @"c:\TestImage.jpg" };
            var resourceCollection = new ExtendedObservableCollection<ResourceInfoDataViewModel>();
            var historyEntry = new AddListElementHistoryEntry<ResourceInfoDataViewModel>(
                mediaShell.Object,
                resource,
                resourceCollection,
                "Test");
            historyEntry.Do();
            Assert.AreEqual(0, fontsList.Count);
            Assert.AreEqual(1, resourceCollection.Count);
            Assert.AreEqual(resource, resourceCollection.First());
            historyEntry.Undo();
            Assert.AreEqual(0, fontsList.Count);
            Assert.AreEqual(0, resourceCollection.Count);
        }

        /// <summary>
        /// Tests adding a font to the collections and undo this operation.
        /// </summary>
        [TestMethod]
        public void UndoRedoAddFontTest()
        {
            var mediaShell = new Mock<IMediaShell>();
            var infomediaConfig = new InfomediaConfigDataViewModel(null);
            var fontsList = new ExtendedObservableCollection<FontConfigDataViewModel>();
            infomediaConfig.Fonts = fontsList;
            mediaShell.Setup(m => m.MediaApplicationState.CurrentProject.InfomediaConfig).Returns(infomediaConfig);
            var resources = new ExtendedObservableCollection<ResourceInfoDataViewModel>();
            mediaShell.Setup(m => m.MediaApplicationState.CurrentProject.Resources).Returns(resources);
            var resource = new ResourceInfoDataViewModel
                           {
                               Facename = "TestFont",
                               Type = ResourceType.Font,
                               Filename = @"c:\TestFont.ttf"
                           };
            var resourceCollection = new ExtendedObservableCollection<ResourceInfoDataViewModel>();
            var historyEntry = new AddListElementHistoryEntry<ResourceInfoDataViewModel>(
                mediaShell.Object,
                resource,
                resourceCollection,
                "Test");
            historyEntry.Do();
            Assert.AreEqual(1, fontsList.Count);
            var filename = Path.Combine(Settings.Default.RelativeFontsFolderPath, "TestFont.ttf");
            Assert.AreEqual(filename, fontsList.First().Path.Value);
            Assert.AreEqual(1, resourceCollection.Count);
            Assert.AreEqual(resource, resourceCollection.First());
            historyEntry.Undo();
            Assert.AreEqual(0, fontsList.Count);
            Assert.AreEqual(0, resourceCollection.Count);
        }
    }
}
