// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringCompareEvaluatorTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StringCompareEvaluatorTest type.
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
    /// Test for <see cref="StringCompareEvaluator"/>.
    /// </summary>
    [TestClass]
    public class StringCompareEvaluatorTest
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
        /// Tests with an <see cref="StringCompareEval"/> that is true in the beginning.
        /// </summary>
        [TestMethod]
        public void StartTrueTest()
        {
            var context = new PresentationContextMock();
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "Hello");

            var eval = new StringCompareEval
                           {
                               Evaluation = new GenericEval(0, 1, 2, 3),
                               Value = "Hello"
                           };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as StringCompareEvaluator;
            Assert.IsNotNull(target);

            using (target)
            {
                Assert.IsTrue(target.BoolValue);
            }
        }

        /// <summary>
        /// Tests with an <see cref="StringCompareEval"/> that is false in the beginning.
        /// </summary>
        [TestMethod]
        public void StartFalseTest()
        {
            var context = new PresentationContextMock();
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "Hello");

            var eval = new StringCompareEval
                            {
                                Evaluation = new GenericEval(0, 1, 2, 3),
                                Value = "World"
                            };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as StringCompareEvaluator;
            Assert.IsNotNull(target);

            using (target)
            {
                Assert.IsFalse(target.BoolValue);
            }
        }

        /// <summary>
        /// Tests that <see cref="StringCompareEval.IgnoreCase"/> works.
        /// </summary>
        [TestMethod]
        public void IgnoreCaseTest()
        {
            var context = new PresentationContextMock();
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "Hello");

            var eval = new StringCompareEval
            {
                Evaluation = new GenericEval(0, 1, 2, 3),
                Value = "heLLo",
                IgnoreCase = true
            };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as StringCompareEvaluator;
            Assert.IsNotNull(target);

            using (target)
            {
                Assert.IsTrue(target.BoolValue);
            }
        }

        /// <summary>
        /// Tests that having no <see cref="StringCompareEval.IgnoreCase"/> works.
        /// </summary>
        [TestMethod]
        public void NoIgnoreCaseTest()
        {
            var context = new PresentationContextMock();
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "Hello");

            var eval = new StringCompareEval
            {
                Evaluation = new GenericEval(0, 1, 2, 3),
                Value = "heLLo",
                IgnoreCase = false
            };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as StringCompareEvaluator;
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

            var eval = new StringCompareEval
                            {
                                Evaluation = new GenericEval(0, 1, 2, 3),
                                Value = "Hello",
                                IgnoreCase = true
                            };

            var valueChanged = 0;
            var target = EvaluatorFactory.CreateEvaluator(eval, context) as StringCompareEvaluator;
            Assert.IsNotNull(target);
            target.ValueChanged += (sender, args) => valueChanged++;

            using (target)
            {
                Assert.IsFalse(target.BoolValue);
                Assert.AreEqual(0, valueChanged);

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "test");

                Assert.IsFalse(target.BoolValue);
                Assert.AreEqual(0, valueChanged);

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "hello");

                Assert.IsTrue(target.BoolValue);
                Assert.AreEqual(1, valueChanged);

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "Hello");

                Assert.IsTrue(target.BoolValue);
                Assert.AreEqual(1, valueChanged);

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "World");

                Assert.IsFalse(target.BoolValue);
                Assert.AreEqual(2, valueChanged);
            }
        }
    }
}
