// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchEvaluatorTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SwitchEvaluatorTest type.
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
    /// Test for <see cref="SwitchEvaluator"/>.
    /// </summary>
    [TestClass]
    public class SwitchEvaluatorTest
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
        /// Tests the <see cref="SwitchEval.Cases"/> condition.
        /// </summary>
        [TestMethod]
        public void CasesValueTest()
        {
            var context = new PresentationContextMock();
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "1");

            var eval = new SwitchEval
                           {
                               Value = new DynamicProperty(new GenericEval(0, 1, 2, 3)),
                               Cases =
                                   {
                                       new CaseDynamicProperty
                                           {
                                               Value = "0",
                                               Evaluation = new ConstantEval("Foo")
                                           },
                                       new CaseDynamicProperty
                                           {
                                               Value = "1",
                                               Evaluation = new ConstantEval("Bar")
                                           }
                                   },
                               Default = new DynamicProperty(new ConstantEval("Default"))
                           };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as SwitchEvaluator;
            Assert.IsNotNull(target);

            using (target)
            {
                Assert.AreEqual("Bar", target.StringValue);
            }
        }

        /// <summary>
        /// Tests the <see cref="SwitchEval.Default"/> condition.
        /// </summary>
        [TestMethod]
        public void DefaultValueTest()
        {
            var context = new PresentationContextMock();
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "3");

            var eval = new SwitchEval
                           {
                               Value = new DynamicProperty(new GenericEval(0, 1, 2, 3)),
                               Cases =
                                   {
                                       new CaseDynamicProperty
                                           {
                                               Value = "0",
                                               Evaluation = new ConstantEval("Foo")
                                           },
                                       new CaseDynamicProperty
                                           {
                                               Value = "1",
                                               Evaluation = new ConstantEval("Bar")
                                           }
                                   },
                               Default = new DynamicProperty(new ConstantEval("Default"))
                           };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as SwitchEvaluator;
            Assert.IsNotNull(target);

            using (target)
            {
                Assert.AreEqual("Default", target.StringValue);
            }
        }

        /// <summary>
        /// Tests the <see cref="SwitchEval.Default"/> condition in case it was not defined.
        /// </summary>
        [TestMethod]
        public void DefaultEmptyValueTest()
        {
            var context = new PresentationContextMock();
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "3");

            var eval = new SwitchEval
                           {
                               Value = new DynamicProperty(new GenericEval(0, 1, 2, 3)),
                               Cases =
                                   {
                                       new CaseDynamicProperty
                                           {
                                               Value = "0",
                                               Evaluation = new ConstantEval("Foo")
                                           },
                                       new CaseDynamicProperty
                                           {
                                               Value = "1",
                                               Evaluation = new ConstantEval("Bar")
                                           }
                                   }
                           };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as SwitchEvaluator;
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

            var eval = new SwitchEval
                           {
                               Value = new DynamicProperty(new GenericEval(0, 1, 2, 3)),
                               Cases =
                                   {
                                       new CaseDynamicProperty
                                           {
                                               Value = "0",
                                               Evaluation = new ConstantEval("Foo")
                                           },
                                       new CaseDynamicProperty
                                           {
                                               Value = "1",
                                               Evaluation = new ConstantEval("Bar")
                                           }
                                   },
                               Default = new DynamicProperty(new ConstantEval("Default"))
                           };

            var valueChanged = 0;
            var target = EvaluatorFactory.CreateEvaluator(eval, context) as SwitchEvaluator;
            Assert.IsNotNull(target);
            target.ValueChanged += (sender, args) => valueChanged++;

            using (target)
            {
                Assert.AreEqual("Default", target.StringValue);
                Assert.AreEqual(0, valueChanged);

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "4");

                Assert.AreEqual("Default", target.StringValue);
                Assert.AreEqual(0, valueChanged);

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "0");

                Assert.AreEqual("Foo", target.StringValue);
                Assert.AreEqual(1, valueChanged);

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "1");

                Assert.AreEqual("Bar", target.StringValue);
                Assert.AreEqual(2, valueChanged);

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "3");

                Assert.AreEqual("Default", target.StringValue);
                Assert.AreEqual(3, valueChanged);
            }
        }
    }
}
