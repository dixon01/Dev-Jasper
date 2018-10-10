// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AndEvaluatorTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AndEvaluatorTest type.
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
    /// Test for <see cref="AndEvaluator"/>.
    /// </summary>
    [TestClass]
    public class AndEvaluatorTest
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
        /// Tests with an empty <see cref="AndEval"/>.
        /// </summary>
        [TestMethod]
        public void EmptyEvalTest()
        {
            var context = new PresentationContextMock();

            var eval = new AndEval();

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as AndEvaluator;
            Assert.IsNotNull(target);

            using (target)
            {
                Assert.IsTrue(target.BoolValue);
            }
        }

        /// <summary>
        /// Tests with a static evaluation that returns true.
        /// </summary>
        [TestMethod]
        public void StaticEvalTrueTest()
        {
            var context = new PresentationContextMock();

            var eval = new AndEval
                {
                    Conditions =
                        {
                            new ConstantEval("1"),
                            new ConstantEval("1"),
                            new ConstantEval("1")
                        }
                };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as AndEvaluator;
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
        public void StaticEvalOneFalseTest()
        {
            var context = new PresentationContextMock();

            var eval = new AndEval
                {
                    Conditions =
                        {
                            new ConstantEval("1"),
                            new ConstantEval("1"),
                            new ConstantEval("0")
                        }
                };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as AndEvaluator;
            Assert.IsNotNull(target);

            using (target)
            {
                Assert.IsFalse(target.BoolValue);
            }
        }

        /// <summary>
        /// Tests with a static evaluation that returns false.
        /// </summary>
        [TestMethod]
        public void StaticEvalAllFalseTest()
        {
            var context = new PresentationContextMock();

            var eval = new AndEval
                {
                    Conditions =
                        {
                            new ConstantEval("0"),
                            new ConstantEval("0"),
                            new ConstantEval("0")
                        }
                };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as AndEvaluator;
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

            var eval = new AndEval
                {
                    Conditions =
                        {
                            new GenericEval(0, 1, 2, 3),
                            new GenericEval(1, 2, 3, 4),
                            new GenericEval(2, 3, 4, 5)
                        }
                };

            var valueChanged = 0;
            var target = EvaluatorFactory.CreateEvaluator(eval, context) as AndEvaluator;
            Assert.IsNotNull(target);
            target.ValueChanged += (sender, args) => valueChanged++;

            using (target)
            {
                Assert.IsFalse(target.BoolValue);
                Assert.AreEqual(0, valueChanged);

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "true");
                Assert.IsFalse(target.BoolValue);
                Assert.AreEqual(0, valueChanged);

                context.SetCellValue(new GenericCoordinate(1, 2, 3, 4), "true");
                Assert.IsFalse(target.BoolValue);
                Assert.AreEqual(0, valueChanged);

                context.SetCellValue(new GenericCoordinate(2, 3, 4, 5), "true");
                Assert.IsTrue(target.BoolValue);
                Assert.AreEqual(1, valueChanged);

                context.SetCellValue(new GenericCoordinate(1, 2, 3, 4), "false");
                Assert.IsFalse(target.BoolValue);
                Assert.AreEqual(2, valueChanged);
            }
        }
    }
}
