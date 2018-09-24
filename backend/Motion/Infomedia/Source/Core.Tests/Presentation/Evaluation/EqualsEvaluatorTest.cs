// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EqualsEvaluatorTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EqualsEvaluatorTest type.
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
    /// Test for <see cref="EqualsEvaluator"/>.
    /// </summary>
    [TestClass]
    public class EqualsEvaluatorTest
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

            var eval = new EqualsEval
                           {
                               Left = new DynamicProperty(new ConstantEval("A")),
                               Right = new DynamicProperty(new ConstantEval("A"))
                           };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as EqualsEvaluator;
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

            var eval = new EqualsEval
                           {
                               Left = new DynamicProperty(new ConstantEval("A")),
                               Right = new DynamicProperty(new ConstantEval("B"))
                           };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as EqualsEvaluator;
            Assert.IsNotNull(target);

            using (target)
            {
                Assert.IsFalse(target.BoolValue);
            }
        }

        /// <summary>
        /// Tests with a static evaluation that returns false with two case sensitively unequal strings.
        /// </summary>
        [TestMethod]
        public void StaticEvalFalseCaseSensitiveTest()
        {
            var context = new PresentationContextMock();

            var eval = new EqualsEval
                           {
                               Left = new DynamicProperty(new ConstantEval("A")),
                               Right = new DynamicProperty(new ConstantEval("a"))
                           };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as EqualsEvaluator;
            Assert.IsNotNull(target);

            using (target)
            {
                Assert.IsFalse(target.BoolValue);
            }
        }

        /// <summary>
        /// Tests the <see cref="EvaluatorBase.ValueChanged"/> event.
        /// </summary>
        [TestMethod]
        public void ValueChangedTest()
        {
            var context = new PresentationContextMock();

            var eval = new EqualsEval
                           {
                               Left = new DynamicProperty(new GenericEval(0, 1, 2, 3)),
                               Right = new DynamicProperty(new GenericEval(1, 2, 3, 4))
                           };

            var valueChanged = 0;
            var target = EvaluatorFactory.CreateEvaluator(eval, context) as EqualsEvaluator;
            Assert.IsNotNull(target);
            target.ValueChanged += (sender, args) => valueChanged++;

            using (target)
            {
                Assert.IsTrue(target.BoolValue);
                Assert.AreEqual(0, valueChanged);

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "Hello");
                Assert.IsFalse(target.BoolValue);
                Assert.AreEqual(1, valueChanged);

                context.SetCellValue(new GenericCoordinate(1, 2, 3, 4), "Hello");
                Assert.IsTrue(target.BoolValue);
                Assert.AreEqual(2, valueChanged);

                context.SetCellValue(new GenericCoordinate(1, 2, 3, 4), "World");
                Assert.IsFalse(target.BoolValue);
                Assert.AreEqual(3, valueChanged);

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "other");
                Assert.IsFalse(target.BoolValue);
                Assert.AreEqual(3, valueChanged);
            }
        }
    }
}