// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IfEvaluatorTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IfEvaluatorTest type.
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
    /// Test for <see cref="IfEvaluator"/>.
    /// </summary>
    [TestClass]
    public class IfEvaluatorTest
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
        /// Tests the <see cref="IfEval.Then"/> condition.
        /// </summary>
        [TestMethod]
        public void ThenValueTest()
        {
            var context = new PresentationContextMock();
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "1");

            var eval = new IfEval
            {
                Condition = new DynamicProperty(new GenericEval(0, 1, 2, 3)),
                Then = new DynamicProperty(new ConstantEval("Foo")),
                Else = new DynamicProperty(new ConstantEval("Bar"))
            };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as IfEvaluator;
            Assert.IsNotNull(target);

            using (target)
            {
                Assert.AreEqual("Foo", target.StringValue);
            }
        }

        /// <summary>
        /// Tests the <see cref="IfEval.Then"/> condition in case it was not defined.
        /// </summary>
        [TestMethod]
        public void ThenEmptyValueTest()
        {
            var context = new PresentationContextMock();
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "1");

            var eval = new IfEval
            {
                Condition = new DynamicProperty(new GenericEval(0, 1, 2, 3)),
                Else = new DynamicProperty(new ConstantEval("Bar"))
            };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as IfEvaluator;
            Assert.IsNotNull(target);

            using (target)
            {
                Assert.AreEqual(string.Empty, target.StringValue);
            }
        }

        /// <summary>
        /// Tests the <see cref="IfEval.Else"/> condition.
        /// </summary>
        [TestMethod]
        public void ElseValueTest()
        {
            var context = new PresentationContextMock();
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "0");

            var eval = new IfEval
            {
                Condition = new DynamicProperty(new GenericEval(0, 1, 2, 3)),
                Then = new DynamicProperty(new ConstantEval("Foo")),
                Else = new DynamicProperty(new ConstantEval("Bar"))
            };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as IfEvaluator;
            Assert.IsNotNull(target);

            using (target)
            {
                Assert.AreEqual("Bar", target.StringValue);
            }
        }

        /// <summary>
        /// Tests the <see cref="IfEval.Else"/> condition in case it was not defined.
        /// </summary>
        [TestMethod]
        public void ElseEmptyValueTest()
        {
            var context = new PresentationContextMock();
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "0");

            var eval = new IfEval
            {
                Condition = new DynamicProperty(new GenericEval(0, 1, 2, 3)),
                Then = new DynamicProperty(new ConstantEval("Foo"))
            };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as IfEvaluator;
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

            var eval = new IfEval
                           {
                               Condition = new DynamicProperty(new GenericEval(0, 1, 2, 3)),
                               Then = new DynamicProperty(new ConstantEval("Foo")),
                               Else = new DynamicProperty(new ConstantEval("Bar"))
                           };

            var valueChanged = 0;
            var target = EvaluatorFactory.CreateEvaluator(eval, context) as IfEvaluator;
            Assert.IsNotNull(target);
            target.ValueChanged += (sender, args) => valueChanged++;

            using (target)
            {
                Assert.AreEqual("Bar", target.StringValue);
                Assert.AreEqual(0, valueChanged);

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "0");

                Assert.AreEqual("Bar", target.StringValue);
                Assert.AreEqual(0, valueChanged);

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "true");

                Assert.AreEqual("Foo", target.StringValue);
                Assert.AreEqual(1, valueChanged);

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "false");

                Assert.AreEqual("Bar", target.StringValue);
                Assert.AreEqual(2, valueChanged);
            }
        }
    }
}
