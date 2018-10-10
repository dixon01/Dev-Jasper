// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextToImageEvaluatorTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Summary description for TextToImageEvaluatorTest
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Tests.Presentation.Evaluation
{
    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Eval;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Motion.Infomedia.BbCode;
    using Gorba.Motion.Infomedia.Core.Presentation.Evaluation;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit test for text to image evaluator.
    /// </summary>
    [TestClass]
    public class TextToImageEvaluatorTest
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
        /// The text to image conversion test.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Presentation\Evaluation\pills.jpg")]
        [DeploymentItem(@"Presentation\Evaluation\ICN.png")]
        public void ReplaceSpecialWhitespaceTest()
        {
            var context = new PresentationContextMock();
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "This\r\nis it\twith   pills");

            var eval = new TextToImageEval
            {
                Evaluation = new GenericEval(0, 1, 2, 3),
                FilePatterns = @"{0}.jpg;{0}.png"
            };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as TextToImageEvaluator;
            Assert.IsNotNull(target);
            using (target)
            {
                Assert.IsNotNull(target.StringValue);

                var parsed = new BbParser().Parse(target.StringValue);
                var children = parsed.Children.GetEnumerator();

                Assert.IsTrue(children.MoveNext());
                var text = children.Current as BbText;
                Assert.IsNotNull(text);
                Assert.AreEqual("This\r\nis it\twith   ", text.Text);

                Assert.IsTrue(children.MoveNext());
                var image = children.Current as Image;
                Assert.IsNotNull(image);
                Assert.AreEqual("pills.jpg", image.FileName);

                Assert.IsFalse(children.MoveNext());
            }
        }

        /// <summary>
        /// The text to image conversion test.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Presentation\Evaluation\pills.jpg")]
        [DeploymentItem(@"Presentation\Evaluation\ICN.png")]
        public void ReplaceMultipleWordsTest()
        {
            var context = new PresentationContextMock();
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "This is ICN with pills 123.");

            var eval = new TextToImageEval
            {
                Evaluation = new GenericEval(0, 1, 2, 3),
                FilePatterns = @"{0}.jpg;{0}.png"
            };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as TextToImageEvaluator;
            Assert.IsNotNull(target);
            using (target)
            {
                Assert.IsNotNull(target.StringValue);

                var parsed = new BbParser().Parse(target.StringValue);
                var children = parsed.Children.GetEnumerator();

                Assert.IsTrue(children.MoveNext());
                var text = children.Current as BbText;
                Assert.IsNotNull(text);
                Assert.AreEqual("This is ", text.Text);

                Assert.IsTrue(children.MoveNext());
                var image = children.Current as Image;
                Assert.IsNotNull(image);
                Assert.AreEqual("ICN.png", image.FileName);

                Assert.IsTrue(children.MoveNext());
                text = children.Current as BbText;
                Assert.IsNotNull(text);
                Assert.AreEqual(" with ", text.Text);

                Assert.IsTrue(children.MoveNext());
                image = children.Current as Image;
                Assert.IsNotNull(image);
                Assert.AreEqual("pills.jpg", image.FileName);

                Assert.IsTrue(children.MoveNext());
                text = children.Current as BbText;
                Assert.IsNotNull(text);
                Assert.AreEqual(" 123.", text.Text);

                Assert.IsFalse(children.MoveNext());
            }
        }

        /// <summary>
        /// The text to image conversion test.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Presentation\Evaluation\pills.jpg")]
        [DeploymentItem(@"Presentation\Evaluation\ICN.png")]
        public void ReplaceSingleWordTest()
        {
            var context = new PresentationContextMock();
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "pills");

            var eval = new TextToImageEval
            {
                Evaluation = new GenericEval(0, 1, 2, 3),
                FilePatterns = @"{0}.jpg;{0}.png"
            };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as TextToImageEvaluator;
            Assert.IsNotNull(target);
            using (target)
            {
                Assert.IsNotNull(target.StringValue);

                var parsed = new BbParser().Parse(target.StringValue);
                var children = parsed.Children.GetEnumerator();

                Assert.IsTrue(children.MoveNext());
                var image = children.Current as Image;
                Assert.IsNotNull(image);
                Assert.AreEqual("pills.jpg", image.FileName);

                Assert.IsFalse(children.MoveNext());
            }
        }

        /// <summary>
        /// The text to image conversion test.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Presentation\Evaluation\pills.jpg")]
        [DeploymentItem(@"Presentation\Evaluation\ICN.png")]
        public void ReplaceCaseInsensitiveTest()
        {
            var context = new PresentationContextMock();
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "This is piLLs 123");

            var eval = new TextToImageEval
            {
                Evaluation = new GenericEval(0, 1, 2, 3),
                FilePatterns = @"{0}.JPG;{0}.png"
            };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as TextToImageEvaluator;
            Assert.IsNotNull(target);
            using (target)
            {
                Assert.IsNotNull(target.StringValue);

                var parsed = new BbParser().Parse(target.StringValue);
                var children = parsed.Children.GetEnumerator();

                Assert.IsTrue(children.MoveNext());
                var text = children.Current as BbText;
                Assert.IsNotNull(text);
                Assert.AreEqual("This is ", text.Text);

                Assert.IsTrue(children.MoveNext());
                var image = children.Current as Image;
                Assert.IsNotNull(image);
                Assert.AreEqual("piLLs.JPG", image.FileName);

                Assert.IsTrue(children.MoveNext());
                text = children.Current as BbText;
                Assert.IsNotNull(text);
                Assert.AreEqual(" 123", text.Text);

                Assert.IsFalse(children.MoveNext());
            }
        }

        /// <summary>
        /// The text to image conversion test.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Presentation\Evaluation\pills.jpg")]
        [DeploymentItem(@"Presentation\Evaluation\ICN.png")]
        public void ReplaceFirstWordTest()
        {
            var context = new PresentationContextMock();
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "pills 123");

            var eval = new TextToImageEval
            {
                Evaluation = new GenericEval(0, 1, 2, 3),
                FilePatterns = @"{0}.jpg;{0}.png"
            };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as TextToImageEvaluator;
            Assert.IsNotNull(target);
            using (target)
            {
                Assert.IsNotNull(target.StringValue);

                var parsed = new BbParser().Parse(target.StringValue);
                var children = parsed.Children.GetEnumerator();

                Assert.IsTrue(children.MoveNext());
                var image = children.Current as Image;
                Assert.IsNotNull(image);
                Assert.AreEqual("pills.jpg", image.FileName);

                Assert.IsTrue(children.MoveNext());
                var text = children.Current as BbText;
                Assert.IsNotNull(text);
                Assert.AreEqual(" 123", text.Text);

                Assert.IsFalse(children.MoveNext());
            }
        }

        /// <summary>
        /// The text to image conversion test.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Presentation\Evaluation\pills.jpg")]
        [DeploymentItem(@"Presentation\Evaluation\ICN.png")]
        public void ReplaceLastWordTest()
        {
            var context = new PresentationContextMock();
            context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "123 ICN");

            var eval = new TextToImageEval
            {
                Evaluation = new GenericEval(0, 1, 2, 3),
                FilePatterns = @"{0}.jpg;{0}.png"
            };

            var target = EvaluatorFactory.CreateEvaluator(eval, context) as TextToImageEvaluator;
            Assert.IsNotNull(target);
            using (target)
            {
                Assert.IsNotNull(target.StringValue);

                var parsed = new BbParser().Parse(target.StringValue);
                var children = parsed.Children.GetEnumerator();

                Assert.IsTrue(children.MoveNext());
                var text = children.Current as BbText;
                Assert.IsNotNull(text);
                Assert.AreEqual("123 ", text.Text);

                Assert.IsTrue(children.MoveNext());
                var image = children.Current as Image;
                Assert.IsNotNull(image);
                Assert.AreEqual("ICN.png", image.FileName);

                Assert.IsFalse(children.MoveNext());
            }
        }

        /// <summary>
        /// The text to image conversion test.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Presentation\Evaluation\pills.jpg")]
        [DeploymentItem(@"Presentation\Evaluation\ICN.png")]
        public void ValueChangedTest()
        {
            var context = new PresentationContextMock();

            var eval = new TextToImageEval
                           {
                               Evaluation = new GenericEval(0, 1, 2, 3),
                               FilePatterns = @"{0}.jpg;{0}.png"
                           };

            var valueChanged = 0;
            var target = EvaluatorFactory.CreateEvaluator(eval, context) as TextToImageEvaluator;
            Assert.IsNotNull(target);
            target.ValueChanged += (sender, args) => valueChanged++;
            using (target)
            {
                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "pills 123");
                Assert.AreEqual("[img=pills.jpg] 123", target.StringValue);
                Assert.AreEqual(1, valueChanged);

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "ICN 123");
                Assert.AreEqual("[img=ICN.png] 123", target.StringValue);
                Assert.AreEqual(2, valueChanged);

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "ICN123");
                Assert.AreEqual("ICN123", target.StringValue);
                Assert.AreEqual(3, valueChanged);

                context.SetCellValue(new GenericCoordinate(0, 1, 2, 3), "S 123");
                Assert.AreEqual("S 123", target.StringValue);
                Assert.AreEqual(4, valueChanged);
            }
        }
    }
}
