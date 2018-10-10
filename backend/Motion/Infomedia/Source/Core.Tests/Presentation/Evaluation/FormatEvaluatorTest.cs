// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormatEvaluatorTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FormatEvaluatorTest type.
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
    /// Test for <see cref="FormatEvaluator"/>.
    /// </summary>
    [TestClass]
    public class FormatEvaluatorTest
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
        /// Tests with an empty <see cref="FormatEval.Format"/>.
        /// </summary>
        [TestMethod]
        public void EmptyFormatTest()
        {
            var context = new PresentationContextMock();

            var eval = new FormatEval
                           {
                               Format = string.Empty,
                               Arguments = { new ConstantEval("Hello"), new ConstantEval("World") }
                           };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as FormatEvaluator;
            Assert.IsNotNull(target);

            using (target)
            {
                Assert.AreEqual(string.Empty, target.StringValue);
            }
        }

        /// <summary>
        /// Tests with a simple <see cref="FormatEval.Format"/>.
        /// </summary>
        [TestMethod]
        public void SimpleFormatTest()
        {
            var context = new PresentationContextMock();

            var eval = new FormatEval
            {
                Format = "{0} {1}!",
                Arguments = { new ConstantEval("Hello"), new ConstantEval("World") }
            };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as FormatEvaluator;
            Assert.IsNotNull(target);

            using (target)
            {
                Assert.AreEqual("Hello World!", target.StringValue);
            }
        }

        /// <summary>
        /// Tests with an invalid <see cref="FormatEval.Format"/>.
        /// </summary>
        [TestMethod]
        public void InvalidFormatTest()
        {
            var context = new PresentationContextMock();

            var eval = new FormatEval
            {
                Format = "{0} {1!",
                Arguments = { new ConstantEval("Hello"), new ConstantEval("World") }
            };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as FormatEvaluator;
            Assert.IsNotNull(target);

            using (target)
            {
                Assert.AreEqual(string.Empty, target.StringValue);
            }
        }

        /// <summary>
        /// Tests with a missing <see cref="FormatEval.Arguments"/>.
        /// </summary>
        [TestMethod]
        public void MissingArgumentFormatTest()
        {
            var context = new PresentationContextMock();

            var eval = new FormatEval
            {
                Format = "{0} {1}!",
                Arguments = { new ConstantEval("Hello") }
            };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as FormatEvaluator;
            Assert.IsNotNull(target);

            using (target)
            {
                Assert.AreEqual(string.Empty, target.StringValue);
            }
        }

        /// <summary>
        /// Tests that the evaluator raises the <see cref="EvaluatorBase.ValueChanged"/>.
        /// </summary>
        [TestMethod]
        public void ValueChangedTest()
        {
            var context = new PresentationContextMock();

            var eval = new FormatEval
            {
                Format = "{0} {1}!",
                Arguments = { new ConstantEval("Hello"), new GenericEval(0, 1, 2, 3) }
            };

            var valueChanged = 0;
            var target = EvaluatorFactory.CreateEvaluator(eval, context) as FormatEvaluator;
            Assert.IsNotNull(target);
            target.ValueChanged += (sender, args) => valueChanged++;

            using (target)
            {
                Assert.AreEqual("Hello !", target.StringValue);
                Assert.AreEqual(0, valueChanged);

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "World");

                Assert.AreEqual("Hello World!", target.StringValue);
                Assert.AreEqual(1, valueChanged);

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "Gorba");

                Assert.AreEqual("Hello Gorba!", target.StringValue);
                Assert.AreEqual(2, valueChanged);
            }
        }
    }
}
