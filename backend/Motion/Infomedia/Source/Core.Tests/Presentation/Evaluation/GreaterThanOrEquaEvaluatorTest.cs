﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GreaterThanOrEquaEvaluatorTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GreaterThanOrEquaEvaluatorTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Tests.Presentation.Evaluation
{
    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Eval;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Motion.Infomedia.Core.Presentation.Evaluation;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test for <see cref="GreaterThanOrEqualEvaluator"/>.
    /// </summary>
    [TestClass]
    public class GreaterThanOrEquaEvaluatorTest
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
        /// Tests with a static evaluation that returns true.
        /// </summary>
        [TestMethod]
        public void StaticEvalTrueTest()
        {
            var context = new PresentationContextMock();

            var eval = new GreaterThanOrEqualEval
                           {
                               Left = new DynamicProperty(new ConstantEval("111")),
                               Right = new DynamicProperty(new ConstantEval("22"))
                           };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as GreaterThanOrEqualEvaluator;
            Assert.IsNotNull(target);

            using (target)
            {
                Assert.IsTrue(target.BoolValue);
            }
        }

        /// <summary>
        /// Tests with a static evaluation that returns false.
        /// </summary>
        [TestMethod]
        public void StaticEvalFalseTest()
        {
            var context = new PresentationContextMock();

            var eval = new GreaterThanOrEqualEval
                           {
                               Left = new DynamicProperty(new ConstantEval("55")),
                               Right = new DynamicProperty(new ConstantEval("148"))
                           };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as GreaterThanOrEqualEvaluator;
            Assert.IsNotNull(target);

            using (target)
            {
                Assert.IsFalse(target.BoolValue);
            }
        }

        /// <summary>
        /// Tests with a static evaluation that returns false with mixed number and text strings.
        /// </summary>
        [TestMethod]
        public void StaticEvalFalseMixed()
        {
            var context = new PresentationContextMock();

            var eval = new GreaterThanOrEqualEval
                           {
                               Left = new DynamicProperty(new ConstantEval("A87abc")),
                               Right = new DynamicProperty(new ConstantEval("A228doo"))
                           };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as GreaterThanOrEqualEvaluator;
            Assert.IsNotNull(target);

            using (target)
            {
                Assert.IsFalse(target.BoolValue);
            }
        }

        /// <summary>
        /// Tests with a static evaluation that returns true with two case sensitively equal strings.
        /// </summary>
        [TestMethod]
        public void StaticEvalTrueCaseInsensitive()
        {
            var context = new PresentationContextMock();

            var eval = new GreaterThanOrEqualEval
                           {
                               Left = new DynamicProperty(new ConstantEval("HeLLo")),
                               Right = new DynamicProperty(new ConstantEval("hello"))
                           };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as GreaterThanOrEqualEvaluator;
            Assert.IsNotNull(target);

            using (target)
            {
                Assert.IsTrue(target.BoolValue);
            }
        }

        /// <summary>
        /// Tests the <see cref="EvaluatorBase.ValueChanged"/> event.
        /// </summary>
        [TestMethod]
        public void ValueChangedTest()
        {
            var context = new PresentationContextMock();

            var eval = new GreaterThanOrEqualEval
                           {
                               Left = new DynamicProperty(new GenericEval(0, 1, 2, 3)),
                               Right = new DynamicProperty(new GenericEval(1, 2, 3, 4))
                           };

            var valueChanged = 0;
            var target = EvaluatorFactory.CreateEvaluator(eval, context) as GreaterThanOrEqualEvaluator;
            Assert.IsNotNull(target);
            target.ValueChanged += (sender, args) => valueChanged++;

            using (target)
            {
                Assert.IsTrue(target.BoolValue);
                Assert.AreEqual(0, valueChanged);

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "Hello");
                Assert.IsTrue(target.BoolValue);
                Assert.AreEqual(0, valueChanged);

                context.SetCellValue(new GenericCoordinate(1, 2, 3, 4), "zebra");
                Assert.IsFalse(target.BoolValue);
                Assert.AreEqual(1, valueChanged);

                context.SetCellValue(new GenericCoordinate(1, 2, 3, 4), "donkey");
                Assert.IsTrue(target.BoolValue);
                Assert.AreEqual(2, valueChanged);

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "cat");
                Assert.IsFalse(target.BoolValue);
                Assert.AreEqual(3, valueChanged);
            }
        }
    }
}