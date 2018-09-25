// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectionBaseTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SectionBaseTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Tests.Presentation.Cycles
{
    using System;

    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Eval;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Motion.Infomedia.Core.Presentation.Cycles;
    using Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test for <see cref="SectionBase{T}"/>.
    /// </summary>
    [TestClass]
    public class SectionBaseTest
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
        }

        /// <summary>
        /// A test for Activate
        /// </summary>
        [TestMethod]
        public void Activate_Disabled_Test()
        {
            var config = new StandardSectionConfig
            {
                Duration = TimeSpan.FromSeconds(10),
                Enabled = false,
                Layout = "Layout"
            };

            var context = new PresentationContextMock(new LayoutConfig { Name = "Layout" });
            var target = new StandardSection(config, context);

            bool actual = target.Activate();

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// A test for Activate
        /// </summary>
        [TestMethod]
        public void Activate_GenericEnabled_Test()
        {
            var generic = new GenericEval { Language = 0, Table = 1, Column = 2, Row = 3, };
            var config = new StandardSectionConfig
            {
                Duration = TimeSpan.FromSeconds(10),
                Enabled = true,
                Layout = "Layout",
                EnabledProperty = new DynamicProperty(generic)
            };
            var context = new PresentationContextMock(new LayoutConfig { Name = "Layout" });
            context.SetCellValue(generic.ToCoordinate(), "true");
            var target = new StandardSection(config, context);

            bool actual = target.Activate();

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// A test for Activate
        /// </summary>
        [TestMethod]
        public void Activate_GenericDisabled_Test()
        {
            var generic = new GenericEval { Language = 0, Table = 1, Column = 2, Row = 3, };
            var config = new StandardSectionConfig
            {
                Duration = TimeSpan.FromSeconds(10),
                Enabled = true,
                Layout = "Layout",
                EnabledProperty = new DynamicProperty(generic)
            };
            var context = new PresentationContextMock(new LayoutConfig { Name = "Layout" });
            context.SetCellValue(generic.ToCoordinate(), "false");
            var target = new StandardSection(config, context);

            bool actual = target.Activate();

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// A test for EnabledChanged
        /// </summary>
        [TestMethod]
        public void EnabledChangedTest()
        {
            var generic = new GenericEval { Language = 0, Table = 1, Column = 2, Row = 3, };
            var config = new StandardSectionConfig
            {
                Duration = TimeSpan.FromSeconds(10),
                Enabled = true,
                Layout = "Layout",
                EnabledProperty = new DynamicProperty(generic)
            };
            var context = new PresentationContextMock(new LayoutConfig { Name = "Layout" });
            context.SetCellValue(generic.ToCoordinate(), "false");
            var enabledChanged = 0;
            var target = new StandardSection(config, context);
            target.EnabledChanged += (s, e) => enabledChanged++;

            Assert.IsFalse(target.IsEnabled());
            Assert.AreEqual(0, enabledChanged);

            Assert.IsFalse(target.Activate());
            context.SetCellValue(generic.ToCoordinate(), "true");
            Assert.IsTrue(target.Activate());

            Assert.IsTrue(target.IsEnabled());
            Assert.AreEqual(0, enabledChanged);

            context.SetCellValue(generic.ToCoordinate(), "0");

            Assert.IsFalse(target.IsEnabled());
            Assert.AreEqual(1, enabledChanged);
        }

        // ReSharper restore InconsistentNaming
    }
}
