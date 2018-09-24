// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutConfigDataViewModelTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Tests for the  class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests
{
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.ViewModels;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for the <see cref="LayoutConfigDataViewModel"/> class.
    /// </summary>
    [TestClass]
    public class LayoutConfigDataViewModelTest
    {
        /// <summary>
        /// Tests that the <see cref="LayoutConfigDataViewModel.ReferencesCount"/> is updated when manipulating the
        /// <see cref="LayoutConfigDataViewModel.CycleSectionReferences"/> list.
        /// </summary>
        [TestMethod]
        public void CycleSectionReferencesTest()
        {
            var shellMock = new Mock<IMediaShell>();
            var layoutDataViewModel = new LayoutConfigDataViewModel(shellMock.Object);
            Assert.AreEqual(0, layoutDataViewModel.ReferencesCount);
            Assert.IsNotNull(layoutDataViewModel.CycleSectionReferences);
            Assert.AreEqual(0, layoutDataViewModel.CycleSectionReferences.Count);
            var cycleReference = new StandardCycleConfigDataViewModel(shellMock.Object);
            var sectionReference = new StandardSectionConfigDataViewModel(shellMock.Object);
            layoutDataViewModel.CycleSectionReferences.Add(
                new LayoutCycleSectionRefDataViewModel(cycleReference, sectionReference));
            Assert.AreEqual(1, layoutDataViewModel.CycleSectionReferences.Count);
            Assert.AreEqual(1, layoutDataViewModel.ReferencesCount);
            layoutDataViewModel.CycleSectionReferences.Clear();
            Assert.AreEqual(0, layoutDataViewModel.CycleSectionReferences.Count);
            Assert.AreEqual(0, layoutDataViewModel.ReferencesCount);
        }
    }
}
