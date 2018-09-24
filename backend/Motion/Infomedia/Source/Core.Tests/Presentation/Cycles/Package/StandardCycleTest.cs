// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StandardCycleTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CycleStepTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Tests.Presentation.Cycles.Package
{
    using System;

    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Eval;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Configuration.Infomedia.Presentation.Cycle;
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Motion.Infomedia.Core.Presentation;
    using Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Test for <see cref="StandardCycle"/>.
    /// </summary>
    [TestClass]
    public class StandardCycleTest
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
        /// A test for IsEnabled
        /// </summary>
        [TestMethod]
        public void IsEnabledTest()
        {
            var config = CreateConfig();
            var context = Mock.Of<IPresentationContext>();
            var target = new StandardCycle(config, null, context);

            var actual = target.IsEnabled();

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// A test for IsEnabled
        /// </summary>
        [TestMethod]
        public void IsEnabled_Config_Test()
        {
            var config = CreateConfig();
            config.Enabled = false;
            var context = Mock.Of<IPresentationContext>();
            var target = new StandardCycle(config, null, context);

            var actual = target.IsEnabled();

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// A test for IsEnabled
        /// </summary>
        [TestMethod]
        public void IsEnabled_Condition_True_Test()
        {
            var generic = new GenericEval { Language = 0, Table = 1, Column = 2, Row = 3, };
            var config = CreateConfig();
            config.EnabledProperty = new DynamicProperty(generic);
            var context = new PresentationContextMock(CreateLayoutConfigs());
            context.SetCellValue(generic.ToCoordinate(), "true");
            var target = new StandardCycle(config, null, context);

            var actual = target.IsEnabled();

            Assert.IsTrue(actual);
        }

        /// <summary>
        /// A test for IsEnabled
        /// </summary>
        [TestMethod]
        public void IsEnabled_Condition_False_Test()
        {
            var generic = new GenericEval { Language = 0, Table = 1, Column = 2, Row = 3, };
            var config = CreateConfig();
            config.EnabledProperty = new DynamicProperty(generic);
            var context = new PresentationContextMock(CreateLayoutConfigs());
            context.SetCellValue(generic.ToCoordinate(), "false");
            var target = new StandardCycle(config, null, context);

            var actual = target.IsEnabled();

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// A test for EnabledChanged
        /// </summary>
        [TestMethod]
        public void EnabledChangedTest()
        {
            var generic = new GenericEval { Language = 0, Table = 1, Column = 2, Row = 3, };
            var config = CreateConfig();
            config.EnabledProperty = new DynamicProperty(generic);
            var context = new PresentationContextMock(CreateLayoutConfigs());
            context.SetCellValue(generic.ToCoordinate(), "false");
            var enabledChanged = 0;
            var target = new StandardCycle(config, null, context);
            target.EnabledChanged += (s, e) => enabledChanged++;

            Assert.IsFalse(target.IsEnabled());
            Assert.AreEqual(0, enabledChanged);

            context.SetCellValue(generic.ToCoordinate(), "true");

            Assert.IsTrue(target.IsEnabled());
            Assert.AreEqual(1, enabledChanged);

            context.SetCellValue(generic.ToCoordinate(), "0");

            Assert.IsFalse(target.IsEnabled());
            Assert.AreEqual(2, enabledChanged);
        }

        /// <summary>
        /// A test for Activate
        /// </summary>
        [TestMethod]
        public void ActivateTest()
        {
            var config = CreateConfig();
            var context = new PresentationContextMock(CreateLayoutConfigs());
            var target = new StandardCycle(config, null, context);

            var actual = target.Activate();

            Assert.IsTrue(actual);
            Assert.IsNotNull(target.CurrentSection);
            Assert.AreEqual("Layout1", target.CurrentSection.Config.Layout);
        }

        /// <summary>
        /// A test for Activate
        /// </summary>
        [TestMethod]
        public void Activate_Disabled_Test()
        {
            var generic = new GenericEval { Language = 0, Table = 1, Column = 2, Row = 3, };
            var config = CreateConfig();
            config.EnabledProperty = new DynamicProperty(generic);
            var context = new PresentationContextMock(CreateLayoutConfigs());
            context.SetCellValue(generic.ToCoordinate(), "false");
            var target = new StandardCycle(config, null, context);

            var actual = target.Activate();

            Assert.IsFalse(actual);
            Assert.IsNull(target.CurrentSection);
        }

        /// <summary>
        /// A test for Deactivate
        /// </summary>
        [TestMethod]
        public void DeactivateTest()
        {
            var config = CreateConfig();
            var context = new PresentationContextMock(CreateLayoutConfigs());
            var target = new StandardCycle(config, null, context);

            target.Activate();
            target.Deactivate();

            Assert.IsNotNull(target.CurrentSection);
        }

        /// <summary>
        /// A test for ShouldReset (and Reset())
        /// </summary>
        [TestMethod]
        public void ResetTest()
        {
            var config = CreateConfig();
            var context = new PresentationContextMock(CreateLayoutConfigs());
            var target = new StandardCycle(config, null, context);

            target.Activate();
            target.ShouldReset = true;
            target.Deactivate();

            Assert.IsNull(target.CurrentSection);
        }

        /// <summary>
        /// A test for Activate
        /// </summary>
        [TestMethod]
        public void ShowNextStepTest()
        {
            var config = CreateConfig();
            var stepGeneric0 = new GenericEval { Language = 0, Table = 1, Column = 1, Row = 0 };
            var stepGeneric1 = new GenericEval { Language = 0, Table = 1, Column = 1, Row = 1 };
            config.Sections[0].EnabledProperty = new DynamicProperty(stepGeneric0);
            config.Sections[1].EnabledProperty = new DynamicProperty(stepGeneric1);

            var context = new PresentationContextMock(CreateLayoutConfigs());
            context.SetCellValue(stepGeneric0.ToCoordinate(), "true");
            context.SetCellValue(stepGeneric1.ToCoordinate(), "true");

            var target = new StandardCycle(config, null, context);

            var actual = target.Activate();

            Assert.IsTrue(actual);
            Assert.IsNotNull(target.CurrentSection);
            Assert.AreEqual("Layout1", target.CurrentSection.Config.Layout);

            actual = target.ShowNextObject();

            Assert.IsTrue(actual);
            Assert.IsNotNull(target.CurrentSection);
            Assert.AreEqual("Layout2", target.CurrentSection.Config.Layout);

            actual = target.ShowNextObject();

            Assert.IsTrue(actual);
            Assert.IsNotNull(target.CurrentSection);
            Assert.AreEqual("Layout1", target.CurrentSection.Config.Layout);

            // disable the second step, so we expect to stay in the first layout
            context.SetCellValue(stepGeneric1.ToCoordinate(), "false");

            actual = target.ShowNextObject();

            Assert.IsTrue(actual);
            Assert.IsNotNull(target.CurrentSection);
            Assert.AreEqual("Layout1", target.CurrentSection.Config.Layout);

            // also disable the first step, so we expect to find no valid step
            context.SetCellValue(stepGeneric0.ToCoordinate(), "false");

            actual = target.ShowNextObject();

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// A test for Activate
        /// </summary>
        [TestMethod]
        public void DisableStepTest()
        {
            var generic = new GenericEval { Language = 0, Table = 1, Column = 2, Row = 3, };
            var config = CreateConfig();
            config.Sections[0].EnabledProperty = new DynamicProperty(generic);
            var context = new PresentationContextMock(CreateLayoutConfigs());
            context.SetCellValue(generic.ToCoordinate(), "true");
            var target = new StandardCycle(config, null, context);

            var actual = target.Activate();

            Assert.IsTrue(actual);
            Assert.IsNotNull(target.CurrentSection);
            Assert.AreEqual("Layout1", target.CurrentSection.Config.Layout);

            context.SetCellValue(generic.ToCoordinate(), "false");

            Assert.IsTrue(actual);
            Assert.IsNotNull(target.CurrentSection);
            Assert.AreEqual("Layout2", target.CurrentSection.Config.Layout);

            actual = target.ShowNextObject();

            Assert.IsTrue(actual);
            Assert.IsNotNull(target.CurrentSection);
            Assert.AreEqual("Layout2", target.CurrentSection.Config.Layout);

            context.SetCellValue(generic.ToCoordinate(), "true");

            Assert.IsTrue(actual);
            Assert.IsNotNull(target.CurrentSection);
            Assert.AreEqual("Layout2", target.CurrentSection.Config.Layout);

            actual = target.ShowNextObject();

            Assert.IsTrue(actual);
            Assert.IsNotNull(target.CurrentSection);
            Assert.AreEqual("Layout1", target.CurrentSection.Config.Layout);
        }

        private static StandardCycleConfig CreateConfig()
        {
            return new StandardCycleConfig
                       {
                           Enabled = true,
                           Name = "Cycle",
                           Sections =
                               {
                                   new StandardSectionConfig
                                       {
                                           Duration = TimeSpan.FromSeconds(10),
                                           Enabled = true,
                                           Layout = "Layout1"
                                       },
                                   new StandardSectionConfig
                                       {
                                           Duration = TimeSpan.FromSeconds(15),
                                           Enabled = true,
                                           Layout = "Layout2"
                                       },
                               }
                       };
        }

        private static LayoutConfig[] CreateLayoutConfigs()
        {
            return new[]
                {
                    new LayoutConfig { Name = "Layout1" },
                    new LayoutConfig { Name = "Layout2" }
                };
        }

        // ReSharper restore InconsistentNaming
    }
}
