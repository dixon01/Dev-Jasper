// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeConversionEvaluatorTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CodeConversionEvaluatorTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Tests.Presentation.Evaluation
{
    using System.Diagnostics.CodeAnalysis;

    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Eval;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Motion.Infomedia.Core.Presentation.Evaluation;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="CodeConversionEvaluator"/>.
    /// </summary>
    [TestClass]
    public class CodeConversionEvaluatorTest
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
        /// Tests that code conversion works for column index 3 (text).
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Presentation\Evaluation\codeconversion.csv")]
        public void TestCodeConversionText()
        {
            var context = new PresentationContextMock();
            context.SetCellValue(new GenericCoordinate(0, 10, 0, 0), string.Empty);
            context.SetCellValue(new GenericCoordinate(0, 10, 1, 0), string.Empty);

            var eval = new CodeConversionEval { FileName = "codeconversion.csv", UseImage = false };

            var valueChanged = false;
            var target = EvaluatorFactory.CreateEvaluator(eval, context) as CodeConversionEvaluator;
            Assert.IsNotNull(target);
            target.ValueChanged += (sender, args) => valueChanged = true;

            Assert.AreEqual(string.Empty, target.StringValue);
            Assert.IsFalse(valueChanged);

            context.SetCellValue(new GenericCoordinate(0, 10, 0, 0), "5");

            Assert.AreEqual("5", target.StringValue);
            Assert.IsTrue(valueChanged);

            valueChanged = false;
            context.SetCellValue(new GenericCoordinate(0, 10, 1, 0), "72");

            Assert.AreEqual("5N", target.StringValue);
            Assert.IsTrue(valueChanged);

            valueChanged = false;
            context.SetCellValue(new GenericCoordinate(0, 10, 1, 0), "83");

            Assert.AreEqual(string.Empty, target.StringValue);
            Assert.IsTrue(valueChanged);

            valueChanged = false;
            context.SetCellValue(new GenericCoordinate(0, 10, 0, 0), "8");

            Assert.AreEqual(string.Empty, target.StringValue);
            Assert.IsFalse(valueChanged);

            context.SetCellValue(new GenericCoordinate(0, 10, 1, 0), "0");

            Assert.AreEqual("8", target.StringValue);
            Assert.IsTrue(valueChanged);

            valueChanged = false;
            context.SetCellValue(new GenericCoordinate(0, 10, 0, 0), "14");

            Assert.AreEqual("14", target.StringValue);
            Assert.IsTrue(valueChanged);

            valueChanged = false;
            context.SetCellValue(new GenericCoordinate(0, 10, 0, 0), "17");

            Assert.AreEqual(string.Empty, target.StringValue);
            Assert.IsTrue(valueChanged);
        }

        /// <summary>
        /// Tests that code conversion works for column index 2 (image).
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Presentation\Evaluation\codeconversion.csv")]
        public void TestCodeConversionImage()
        {
            var context = new PresentationContextMock();
            context.SetCellValue(new GenericCoordinate(0, 10, 0, 0), string.Empty);
            context.SetCellValue(new GenericCoordinate(0, 10, 1, 0), string.Empty);

            var eval = new CodeConversionEval { FileName = "codeconversion.csv", UseImage = true };

            var valueChanged = false;
            var target = EvaluatorFactory.CreateEvaluator(eval, context) as CodeConversionEvaluator;
            Assert.IsNotNull(target);
            target.ValueChanged += (sender, args) => valueChanged = true;

            Assert.AreEqual(string.Empty, target.StringValue);
            Assert.IsFalse(valueChanged);

            context.SetCellValue(new GenericCoordinate(0, 10, 0, 0), "5");

            Assert.AreEqual(string.Empty, target.StringValue);
            Assert.IsFalse(valueChanged);

            context.SetCellValue(new GenericCoordinate(0, 10, 1, 0), "72");

            Assert.AreEqual(string.Empty, target.StringValue);
            Assert.IsFalse(valueChanged);

            context.SetCellValue(new GenericCoordinate(0, 10, 1, 0), "83");

            Assert.AreEqual(@"Symbols\DS001a_83.jpg", target.StringValue);
            Assert.IsTrue(valueChanged);

            valueChanged = false;
            context.SetCellValue(new GenericCoordinate(0, 10, 0, 0), "8");

            Assert.AreEqual(@"Symbols\DS001a_83.jpg", target.StringValue);
            Assert.IsFalse(valueChanged);

            context.SetCellValue(new GenericCoordinate(0, 10, 1, 0), "0");

            Assert.AreEqual(string.Empty, target.StringValue);
            Assert.IsTrue(valueChanged);

            valueChanged = false;
            context.SetCellValue(new GenericCoordinate(0, 10, 0, 0), "14");

            Assert.AreEqual(string.Empty, target.StringValue);
            Assert.IsFalse(valueChanged);

            context.SetCellValue(new GenericCoordinate(0, 10, 0, 0), "17");

            Assert.AreEqual(@"Symbols\Linie17.jpg", target.StringValue);
            Assert.IsTrue(valueChanged);
        }

        /// <summary>
        /// Tests code conversion of text with placeholders.
        /// </summary>
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Unit test code, OK.")]
        [TestMethod]
        [DeploymentItem(@"Presentation\Evaluation\codeconversion-placeholders.csv")]
        public void TestCodeConversionTextPlaceholders()
        {
            var context = new PresentationContextMock();
            context.SetCellValue(new GenericCoordinate(0, 10, 0, 0), string.Empty);
            context.SetCellValue(new GenericCoordinate(0, 10, 1, 0), string.Empty);

            var eval = new CodeConversionEval { FileName = "codeconversion-placeholders.csv", UseImage = false };

            var valueChanged = false;
            var target = EvaluatorFactory.CreateEvaluator(eval, context) as CodeConversionEvaluator;
            Assert.IsNotNull(target);
            target.ValueChanged += (sender, args) => valueChanged = true;

            Assert.AreEqual(string.Empty, target.StringValue);
            Assert.IsFalse(valueChanged);

            context.SetCellValue(new GenericCoordinate(0, 10, 0, 0), "5");

            Assert.AreEqual("5", target.StringValue);
            Assert.IsTrue(valueChanged);

            valueChanged = false;
            context.SetCellValue(new GenericCoordinate(0, 10, 1, 0), "72");

            Assert.AreEqual("5N", target.StringValue);
            Assert.IsTrue(valueChanged);

            valueChanged = false;
            context.SetCellValue(new GenericCoordinate(0, 10, 0, 0), "10");

            Assert.AreEqual("10N", target.StringValue);
            Assert.IsTrue(valueChanged);

            valueChanged = false;
            context.SetCellValue(new GenericCoordinate(0, 10, 0, 0), "102");

            Assert.AreEqual("2N", target.StringValue);
            Assert.IsTrue(valueChanged);

            valueChanged = false;
            context.SetCellValue(new GenericCoordinate(0, 10, 1, 0), "83");

            Assert.AreEqual(string.Empty, target.StringValue);
            Assert.IsTrue(valueChanged);

            valueChanged = false;
            context.SetCellValue(new GenericCoordinate(0, 10, 0, 0), "8");

            Assert.AreEqual(string.Empty, target.StringValue);
            Assert.IsFalse(valueChanged);

            context.SetCellValue(new GenericCoordinate(0, 10, 1, 0), "0");

            Assert.AreEqual("8", target.StringValue);
            Assert.IsTrue(valueChanged);

            valueChanged = false;
            context.SetCellValue(new GenericCoordinate(0, 10, 0, 0), "14");

            Assert.AreEqual("14", target.StringValue);
            Assert.IsTrue(valueChanged);

            var specialLines = new[] { "71", "72", "73", "74", "75", "76", "77" };
            var expected = new[] { "N14", "14N", "E14", "14", "N14", "14N", "014" };

            for (int i = 0; i < specialLines.Length; i++)
            {
                valueChanged = false;
                context.SetCellValue(new GenericCoordinate(0, 10, 1, 0), specialLines[i]);

                Assert.AreEqual(expected[i], target.StringValue);
                Assert.IsTrue(valueChanged);
            }

            valueChanged = false;
            context.SetCellValue(new GenericCoordinate(0, 10, 0, 0), "107");

            Assert.AreEqual("107", target.StringValue);
            Assert.IsTrue(valueChanged);

            expected = new[] { "N7", "7N", "E7", "107", "N07", "07N", "107" };

            for (int i = 0; i < specialLines.Length; i++)
            {
                valueChanged = false;
                context.SetCellValue(new GenericCoordinate(0, 10, 1, 0), specialLines[i]);

                Assert.AreEqual(expected[i], target.StringValue);
                Assert.IsTrue(valueChanged);
            }

            valueChanged = false;
            context.SetCellValue(new GenericCoordinate(0, 10, 0, 0), "003");

            Assert.AreEqual("003", target.StringValue);
            Assert.IsTrue(valueChanged);

            expected = new[] { "N3", "3N", "E3", "3", "N03", "03N", "003" };

            for (int i = 0; i < specialLines.Length; i++)
            {
                valueChanged = false;
                context.SetCellValue(new GenericCoordinate(0, 10, 1, 0), specialLines[i]);

                Assert.AreEqual(expected[i], target.StringValue);
                Assert.IsTrue(valueChanged);
            }
        }
    }
}
