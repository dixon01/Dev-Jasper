// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConstantEvaluatorTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConstantEvaluatorTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Tests.Presentation.Evaluation
{
    using Gorba.Common.Configuration.Infomedia.Eval;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Motion.Infomedia.Core.Presentation.Evaluation;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test for <see cref="ConstantEvaluator"/>.
    /// </summary>
    [TestClass]
    public class ConstantEvaluatorTest
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
        /// Tests with an empty <see cref="ConstantEval"/>.
        /// </summary>
        [TestMethod]
        public void EmptyEvalTest()
        {
            var context = new PresentationContextMock();

            var eval = new ConstantEval();

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as ConstantEvaluator;
            Assert.IsNotNull(target);

            using (target)
            {
                Assert.IsFalse(target.BoolValue);
                Assert.AreEqual(0, target.IntValue);
                Assert.AreEqual(string.Empty, target.StringValue);
            }
        }

        /// <summary>
        /// Tests that all *Values properties are working properly.
        /// </summary>
        [TestMethod]
        public void ValuesTest()
        {
            var context = new PresentationContextMock();

            var eval = new ConstantEval("2");

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as ConstantEvaluator;
            Assert.IsNotNull(target);

            using (target)
            {
                Assert.IsTrue(target.BoolValue);
                Assert.AreEqual(2, target.IntValue);
                Assert.AreEqual("2", target.StringValue);
            }
        }
    }
}
