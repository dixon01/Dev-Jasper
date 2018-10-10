// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegerCompareEvaluatorTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IntegerCompareEvaluatorTest type.
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
    /// Test for <see cref="IntegerCompareEvaluator"/>.
    /// </summary>
    [TestClass]
    public class IntegerCompareEvaluatorTest
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
        /// Tests with an <see cref="IntegerCompareEval"/> that is true in the beginning.
        /// </summary>
        [TestMethod]
        public void StartTrueTest()
        {
            var context = new PresentationContextMock();
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "13");

            var eval = new IntegerCompareEval
                           {
                               Evaluation = new GenericEval(0, 1, 2, 3),
                               Begin = 12,
                               End = 15
                           };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as IntegerCompareEvaluator;
            Assert.IsNotNull(target);

            using (target)
            {
                Assert.IsTrue(target.BoolValue);
            }
        }

        /// <summary>
        /// Tests with an <see cref="IntegerCompareEval"/> that is false in the beginning.
        /// </summary>
        [TestMethod]
        public void StartFalseTest()
        {
            var context = new PresentationContextMock();
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "11");

            var eval = new IntegerCompareEval
                            {
                                Evaluation = new GenericEval(0, 1, 2, 3),
                                Begin = 12,
                                End = 15
                            };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as IntegerCompareEvaluator;
            Assert.IsNotNull(target);

            using (target)
            {
                Assert.IsFalse(target.BoolValue);
            }
        }

        /// <summary>
        /// Tests with an <see cref="IntegerCompareEval"/> that has an invalid range.
        /// </summary>
        [TestMethod]
        public void InvalidRangeTest()
        {
            var context = new PresentationContextMock();
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "14");

            var eval = new IntegerCompareEval
            {
                Evaluation = new GenericEval(0, 1, 2, 3),
                Begin = 15,
                End = 12
            };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as IntegerCompareEvaluator;
            Assert.IsNotNull(target);

            using (target)
            {
                Assert.IsFalse(target.BoolValue);
            }
        }

        /// <summary>
        /// Tests with an <see cref="IntegerCompareEval"/> that has an invalid range.
        /// </summary>
        [TestMethod]
        public void NonIntegerValueTest()
        {
            var context = new PresentationContextMock();
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "Hello");

            var eval = new IntegerCompareEval
            {
                Evaluation = new GenericEval(0, 1, 2, 3),
                Begin = -10,
                End = 10
            };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as IntegerCompareEvaluator;
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

            var eval = new IntegerCompareEval
                            {
                                Evaluation = new GenericEval(0, 1, 2, 3),
                                Begin = 12,
                                End = 15
                            };

            var valueChanged = 0;
            var target = EvaluatorFactory.CreateEvaluator(eval, context) as IntegerCompareEvaluator;
            Assert.IsNotNull(target);
            target.ValueChanged += (sender, args) => valueChanged++;

            using (target)
            {
                Assert.IsFalse(target.BoolValue);
                Assert.AreEqual(0, valueChanged);

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "10");

                Assert.IsFalse(target.BoolValue);
                Assert.AreEqual(0, valueChanged);

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "12");

                Assert.IsTrue(target.BoolValue);
                Assert.AreEqual(1, valueChanged);

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "14");

                Assert.IsTrue(target.BoolValue);
                Assert.AreEqual(1, valueChanged);

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "16");

                Assert.IsFalse(target.BoolValue);
                Assert.AreEqual(2, valueChanged);
            }
        }
    }
}
