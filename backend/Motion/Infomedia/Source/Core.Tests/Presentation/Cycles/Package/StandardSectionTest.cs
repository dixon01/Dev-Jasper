// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StandardSectionTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Tests.Presentation.Cycles.Package
{
    using System;

    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test for <see cref="StandardSection"/>.
    /// </summary>
    [TestClass]
    public class StandardSectionTest
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Initializes MessageDispatcher before the tests
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            MessageDispatcher.Instance.Configure(new ObjectConfigurator(new MediConfig(), "U", "A"));
        }

        /// <summary>
        /// A test for Activate
        /// </summary>
        [TestMethod]
        public void ActivateTest()
        {
            var config = new StandardSectionConfig
                             {
                                 Duration = TimeSpan.FromSeconds(10),
                                 Enabled = true,
                                 Layout = "Layout"
                             };

            var context = new PresentationContextMock(new LayoutConfig { Name = "Layout" });
            var target = new StandardSection(config, context);

            bool actual = target.Activate();

            Assert.IsTrue(actual);

            var page = target.CurrentObject;
            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Layout);
            Assert.AreEqual("Layout", page.Layout.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(10), page.Duration);
            Assert.AreEqual(0, page.PageIndex);
            Assert.AreEqual(1, page.TotalPages);
            Assert.AreEqual(0, page.RowOffset);

            target.Dispose();
        }

        /// <summary>
        /// A test for ShowNextObject
        /// </summary>
        [TestMethod]
        public void ShowNextPageTest()
        {
            var config = new StandardSectionConfig
                             {
                                 Duration = TimeSpan.FromSeconds(10),
                                 Enabled = true,
                                 Layout = "Layout"
                             };

            var context = new PresentationContextMock(new LayoutConfig { Name = "Layout" });
            var target = new StandardSection(config, context);

            bool actual = target.Activate();
            Assert.IsTrue(actual);

            actual = target.ShowNextObject();
            Assert.IsFalse(actual);

            target.Dispose();
        }

        // ReSharper restore InconsistentNaming
    }
}