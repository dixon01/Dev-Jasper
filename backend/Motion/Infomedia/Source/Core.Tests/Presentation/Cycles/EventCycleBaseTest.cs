// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventCycleBaseTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EventCycleBaseTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Tests.Presentation.Cycles
{
    using System;

    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Eval;
    using Gorba.Common.Configuration.Infomedia.Presentation.Cycle;
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Motion.Infomedia.Core.Presentation.Cycles;
    using Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test for <see cref="EventCycleBase{T}"/>.
    /// </summary>
    [TestClass]
    public class EventCycleBaseTest
    {
        /// <summary>
        /// Initializes MessageDispatcher before the tests
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            MessageDispatcher.Instance.Configure(new ObjectConfigurator(new MediConfig(), "U", "A"));
        }

        /// <summary>
        /// Tests the <see cref="EventCycleBase{T}.Triggered"/> event.
        /// </summary>
        [TestMethod]
        public void TestTriggered()
        {
            var triggerCounter = 0;
            var config = new EventCycleConfig
            {
                Trigger = new GenericTriggerConfig(new GenericEval(0, 1, 2, 3)),
                Sections =
                        {
                            new StandardSectionConfig
                                {
                                    Duration = TimeSpan.FromSeconds(10),
                                    Layout = "Standard1"
                                }
                        }
            };
            var context = new PresentationContextMock();
            var target = new EventCycle(config, null, context);
            target.Triggered += (s, e) => triggerCounter++;

            Assert.AreEqual(0, triggerCounter);
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "1");
            Assert.AreEqual(1, triggerCounter);
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "1");
            Assert.AreEqual(1, triggerCounter);
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "2");
            Assert.AreEqual(2, triggerCounter);
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "1");
            Assert.AreEqual(3, triggerCounter);
        }

        /// <summary>
        /// Tests the <see cref="EventCycleBase{T}.Triggered"/> event.
        /// </summary>
        [TestMethod]
        public void TestTriggeredWithEnabled()
        {
            var triggerCounter = 0;
            var config = new EventCycleConfig
            {
                EnabledProperty = new DynamicProperty(
                    new IntegerCompareEval
                        {
                            Evaluation = new GenericEval(0, 1, 2, 3),
                            Begin = 10,
                            End = 19
                        }),
                Trigger = new GenericTriggerConfig(new GenericEval(0, 1, 2, 3)),
                Sections =
                        {
                            new StandardSectionConfig
                                {
                                    Duration = TimeSpan.FromSeconds(10),
                                    Layout = "Standard1"
                                }
                        }
            };
            var context = new PresentationContextMock();
            var target = new EventCycle(config, null, context);
            target.Triggered += (s, e) => triggerCounter++;

            Assert.AreEqual(0, triggerCounter);
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "1");
            Assert.AreEqual(0, triggerCounter);
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "2");
            Assert.AreEqual(0, triggerCounter);
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "10");
            Assert.AreEqual(1, triggerCounter);
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "11");
            Assert.AreEqual(2, triggerCounter);
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "20");
            Assert.AreEqual(2, triggerCounter);
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "21");
            Assert.AreEqual(2, triggerCounter);
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "12");
            Assert.AreEqual(3, triggerCounter);
        }
    }
}
