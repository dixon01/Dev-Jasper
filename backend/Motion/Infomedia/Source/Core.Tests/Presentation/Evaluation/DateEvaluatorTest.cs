// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateEvaluatorTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DateEvaluatorTest type.
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
    /// Test for <see cref="DateEvaluator"/>
    /// </summary>
    [TestClass]
    public class DateEvaluatorTest
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
            context.SetTime(new DateTime(2013, 3, 1));

            var eval = new DateEval { Begin = new DateTime(2013, 2, 3), End = new DateTime(2013, 3, 5) };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as DateEvaluator;
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
            context.SetTime(new DateTime(2013, 1, 1));

            var eval = new DateEval { Begin = new DateTime(2013, 2, 3), End = new DateTime(2013, 3, 5) };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as DateEvaluator;
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
            context.SetTime(new DateTime(2013, 1, 1));

            var eval = new DateEval { Begin = new DateTime(2013, 2, 3), End = new DateTime(2013, 3, 5) };

            var valueChanged = 0;
            var target = EvaluatorFactory.CreateEvaluator(eval, context) as DateEvaluator;
            Assert.IsNotNull(target);
            target.ValueChanged += (sender, args) => valueChanged++;

            using (target)
            {
                Assert.IsFalse(target.BoolValue);
                Assert.AreEqual(0, valueChanged);

                context.AddTime(TimeSpan.FromDays(20)); // 21.1.2013
                Assert.IsFalse(target.BoolValue);
                Assert.AreEqual(0, valueChanged);

                context.AddTime(TimeSpan.FromDays(20)); // 10.2.2013
                Assert.IsTrue(target.BoolValue);
                Assert.AreEqual(1, valueChanged);

                context.AddTime(TimeSpan.FromDays(20)); // 2.3.2013
                Assert.IsTrue(target.BoolValue);
                Assert.AreEqual(1, valueChanged);

                context.AddTime(TimeSpan.FromDays(20)); // 22.3.2013
                Assert.IsFalse(target.BoolValue);
                Assert.AreEqual(2, valueChanged);

                context.AddTime(TimeSpan.FromDays(20)); // 11.4.2013
                Assert.IsFalse(target.BoolValue);
                Assert.AreEqual(2, valueChanged);
            }
        }
    }
}
