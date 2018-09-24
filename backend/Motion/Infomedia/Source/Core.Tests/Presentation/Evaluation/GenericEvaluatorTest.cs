// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericEvaluatorTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GenericEvaluatorTest type.
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
    /// Test for <see cref="GenericEvaluator"/>.
    /// </summary>
    [TestClass]
    public class GenericEvaluatorTest
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
        /// Tests all value properties for a string input.
        /// </summary>
        [TestMethod]
        public void StringValueTest()
        {
            var context = new PresentationContextMock();
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "Foo");

            var eval = new GenericEval
                           {
                               Language = 0,
                               Table = 1,
                               Column = 2,
                               Row = 3
                           };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as GenericEvaluator;
            Assert.IsNotNull(target);

            using (target)
            {
                Assert.IsFalse(target.BoolValue);
                Assert.AreEqual(0, target.IntValue);
                Assert.AreEqual("Foo", target.StringValue);
            }
        }

        /// <summary>
        /// Tests all value properties for an integer input.
        /// </summary>
        [TestMethod]
        public void IntegerValueTest()
        {
            var context = new PresentationContextMock();
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "15");

            var eval = new GenericEval
            {
                Language = 0,
                Table = 1,
                Column = 2,
                Row = 3
            };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as GenericEvaluator;
            Assert.IsNotNull(target);

            using (target)
            {
                Assert.IsTrue(target.BoolValue);
                Assert.AreEqual(15, target.IntValue);
                Assert.AreEqual("15", target.StringValue);
            }
        }

        /// <summary>
        /// Tests all value properties for a boolean input (as a string).
        /// </summary>
        [TestMethod]
        public void BooleanValueTest()
        {
            var context = new PresentationContextMock();
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "true");

            var eval = new GenericEval
            {
                Language = 0,
                Table = 1,
                Column = 2,
                Row = 3
            };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as GenericEvaluator;
            Assert.IsNotNull(target);

            using (target)
            {
                Assert.IsTrue(target.BoolValue);
                Assert.AreEqual(1, target.IntValue);
                Assert.AreEqual("true", target.StringValue);
            }
        }

        /// <summary>
        /// Tests that the evaluator raises the <see cref="EvaluatorBase.ValueChanged"/>.
        /// </summary>
        [TestMethod]
        public void ValueChangedTest()
        {
            var context = new PresentationContextMock();

            var eval = new GenericEval
                            {
                                Language = 0,
                                Table = 1,
                                Column = 2,
                                Row = 3
                            };

            var valueChanged = 0;
            var target = EvaluatorFactory.CreateEvaluator(eval, context) as GenericEvaluator;
            Assert.IsNotNull(target);
            target.ValueChanged += (sender, args) => valueChanged++;

            using (target)
            {
                Assert.AreEqual(string.Empty, target.StringValue);
                Assert.AreEqual(0, valueChanged);

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "World");

                Assert.AreEqual("World", target.StringValue);
                Assert.AreEqual(1, valueChanged);

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "Gorba");

                Assert.AreEqual("Gorba", target.StringValue);
                Assert.AreEqual(2, valueChanged);
            }
        }
    }
}
