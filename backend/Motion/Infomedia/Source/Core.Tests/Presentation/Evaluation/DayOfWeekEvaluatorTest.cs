﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DayOfWeekEvaluatorTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DayOfWeekEvaluatorTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Tests.Presentation.Evaluation
{
    using System;

    using Gorba.Common.Configuration.Infomedia.Eval;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Motion.Infomedia.Core.Presentation.Evaluation;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test for <see cref="DayOfWeekEvaluator"/>
    /// </summary>
    [TestClass]
    public class DayOfWeekEvaluatorTest
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
        /// Tests that the evaluator is true when the start condition is met.
        /// </summary>
        [TestMethod]
        public void StartTrueTest()
        {
            var context = new PresentationContextMock();
            context.SetTime(new DateTime(2013, 3, 1, 10, 0, 0)); // Friday

            var eval = new DayOfWeekEval
                           {
                               Monday = true,
                               Tuesday = false,
                               Wednesday = true,
                               Thursday = false,
                               Friday = true,
                               Saturday = false,
                               Sunday = true
                           };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as DayOfWeekEvaluator;
            Assert.IsNotNull(target);

            using (target)
            {
                Assert.IsTrue(target.BoolValue);
            }
        }

        /// <summary>
        /// Tests that the evaluator is true when the start condition is met.
        /// </summary>
        [TestMethod]
        public void StartFalseTest()
        {
            var context = new PresentationContextMock();
            context.SetTime(new DateTime(2013, 1, 1, 10, 0, 0)); // Tuesday

            var eval = new DayOfWeekEval
                           {
                               Monday = true,
                               Tuesday = false,
                               Wednesday = true,
                               Thursday = false,
                               Friday = true,
                               Saturday = false,
                               Sunday = true
                           };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as DayOfWeekEvaluator;
            Assert.IsNotNull(target);

            using (target)
            {
                Assert.IsFalse(target.BoolValue);
            }
        }

        /// <summary>
        /// Tests that the evaluator raises the <see cref="EvaluatorBase.ValueChanged"/>.
        /// </summary>
        [TestMethod]
        public void ValueChangedTest()
        {
            var context = new PresentationContextMock();
            context.SetTime(new DateTime(2013, 1, 1, 10, 0, 0)); // Tuesday

            var eval = new DayOfWeekEval
                           {
                               Monday = true,
                               Tuesday = false,
                               Wednesday = true,
                               Thursday = false,
                               Friday = true,
                               Saturday = false,
                               Sunday = true
                           };

            var valueChanged = 0;
            var target = EvaluatorFactory.CreateEvaluator(eval, context) as DayOfWeekEvaluator;
            Assert.IsNotNull(target);
            target.ValueChanged += (sender, args) => valueChanged++;

            using (target)
            {
                Assert.IsFalse(target.BoolValue);
                Assert.AreEqual(0, valueChanged);

                context.AddTime(TimeSpan.FromHours(12)); // Tue, 1.1.2013 22:00
                Assert.IsFalse(target.BoolValue);
                Assert.AreEqual(0, valueChanged);

                context.AddTime(TimeSpan.FromHours(12)); // Wed, 2.1.2013 10:00
                Assert.IsTrue(target.BoolValue);
                Assert.AreEqual(1, valueChanged);

                context.AddTime(TimeSpan.FromHours(12)); // Wed, 2.1.2013 22:00
                Assert.IsTrue(target.BoolValue);
                Assert.AreEqual(1, valueChanged);

                context.AddTime(TimeSpan.FromHours(12)); // Thu, 3.1.2013 10:00
                Assert.IsFalse(target.BoolValue);
                Assert.AreEqual(2, valueChanged);

                context.AddTime(TimeSpan.FromHours(12)); // Thu, 3.1.2013 22:00
                Assert.IsFalse(target.BoolValue);
                Assert.AreEqual(2, valueChanged);
            }
        }
    }
}
