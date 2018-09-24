// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegexReplaceEvaluatorTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RegexReplaceEvaluatorTest type.
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
    /// Unit tests for <see cref="RegexReplaceEvaluator"/>.
    /// </summary>
    [TestClass]
    public class RegexReplaceEvaluatorTest
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
        /// Tests with a simple regular expression.
        /// </summary>
        [TestMethod]
        public void SimpleReplaceTest()
        {
            var context = new PresentationContextMock();

            var eval = new RegexReplaceEval
            {
                Evaluation = new ConstantEval("Hello World"),
                Pattern = "l+",
                Replacement = "-"
            };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as RegexReplaceEvaluator;
            Assert.IsNotNull(target);

            using (target)
            {
                Assert.AreEqual("He-o Wor-d", target.StringValue);
            }
        }

        /// <summary>
        /// Tests that the evaluator raises the <see cref="EvaluatorBase.ValueChanged"/>.
        /// </summary>
        [TestMethod]
        public void ValueChangedTest()
        {
            var context = new PresentationContextMock();

            var eval = new RegexReplaceEval
            {
                Evaluation = new GenericEval(0, 1, 2, 3),
                Pattern = "l+",
                Replacement = "-"
            };

            var valueChanged = 0;
            var target = EvaluatorFactory.CreateEvaluator(eval, context) as RegexReplaceEvaluator;
            Assert.IsNotNull(target);
            target.ValueChanged += (sender, args) => valueChanged++;

            using (target)
            {
                Assert.AreEqual(string.Empty, target.StringValue);
                Assert.AreEqual(0, valueChanged);

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "World");

                Assert.AreEqual("Wor-d", target.StringValue);
                Assert.AreEqual(1, valueChanged);

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "Gorba");

                Assert.AreEqual("Gorba", target.StringValue);
                Assert.AreEqual(2, valueChanged);

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "Go for it");

                Assert.AreEqual("Go for it", target.StringValue);
                Assert.AreEqual(3, valueChanged);
            }
        }
    }
}
