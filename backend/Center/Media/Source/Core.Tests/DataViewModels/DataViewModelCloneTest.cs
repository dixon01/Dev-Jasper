// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataViewModelCloneTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataViewModelCloneTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests.DataViewModels
{
    using System.Collections.Generic;

    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Models.Layout;
    using Gorba.Center.Media.Core.Models.Presentation;
    using Gorba.Center.Media.Core.ViewModels;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests the Clone function of DataViewModels.
    /// </summary>
    [TestClass]
    public class DataViewModelCloneTest
    {
        /// <summary>
        /// Tests that the Clone() of <see cref="LayoutConfigDataViewModel"/> creates a deep copy of all properties.
        /// </summary>
        [TestMethod]
        public void LayoutConfigDataViewModelCloneTest()
        {
            var mediaShellMock = new Mock<IMediaShell>();
            var textElement = new TextElementDataModel { Value = "TextElement" };

            var resolutions = new List<ResolutionConfigDataModel>
                                  {
                                      new ResolutionConfigDataModel
                                          {
                                              Elements = new List<LayoutElementDataModelBase> { textElement }
                                          }
                                  };
            var model = new LayoutConfigDataModel
                            {
                                BaseLayoutName = "BaseLayout",
                                Name = "LayoutName",
                                Resolutions = resolutions
                            };
            var original = new LayoutConfigDataViewModel(mediaShellMock.Object, model);
            Assert.IsNotNull(original.Resolutions);
            Assert.AreEqual(1, original.Resolutions.Count);
            var originalResolution = original.Resolutions[0];
            Assert.IsNotNull(originalResolution.Elements);
            Assert.AreEqual(1, originalResolution.Elements.Count);
            var originalText = (TextElementDataViewModel)originalResolution.Elements[0];
            Assert.IsNotNull(originalText);
            var clone = (LayoutConfigDataViewModel)original.Clone();
            Assert.AreNotSame(original, clone);
            Assert.AreEqual(original.Name.Value, clone.Name.Value);
            Assert.AreEqual(original.BaseLayoutNameName, clone.BaseLayoutNameName);
            Assert.IsNotNull(clone.Resolutions);
            Assert.AreEqual(1, clone.Resolutions.Count);
            var cloneResolution = clone.Resolutions[0];
            Assert.AreNotSame(originalResolution, cloneResolution);
            Assert.IsNotNull(cloneResolution.Elements);
            Assert.AreEqual(1, cloneResolution.Elements.Count);
            var cloneText = (TextElementDataViewModel)cloneResolution.Elements[0];
            Assert.IsNotNull(cloneText);
            Assert.AreEqual(originalText.Value.Value, cloneText.Value.Value);
            Assert.AreNotSame(originalText.Value, cloneText.Value);
        }
    }
}
