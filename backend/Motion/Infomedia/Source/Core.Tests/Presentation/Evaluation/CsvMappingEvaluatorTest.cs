// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvMappingEvaluatorTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CsvMappingEvaluatorTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Tests.Presentation.Evaluation
{
    using System.IO;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Eval;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Motion.Infomedia.Core.Presentation.Evaluation;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="CsvMappingEvaluator"/>.
    /// </summary>
    [TestClass]
    public class CsvMappingEvaluatorTest
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

            var eval = new CsvMappingEval
            {
                FileName = "codeconversion.csv",
                DefaultValue = new DynamicProperty(new GenericEval(0, 10, 0, 0)),
                OutputFormat = "{3}",
                Matches =
                    {
                        new MatchDynamicProperty
                            {
                                Column = 0,
                                Evaluation = new GenericEval(0, 10, 1, 0)
                            },
                        new MatchDynamicProperty
                            {
                                Column = 1,
                                Evaluation = new GenericEval(0, 10, 0, 0)
                            }
                    }
            };

            var valueChanged = false;
            var target = EvaluatorFactory.CreateEvaluator(eval, context) as CsvMappingEvaluator;
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

            var eval = new CsvMappingEval
            {
                FileName = "codeconversion.csv",
                OutputFormat = "{2}",
                Matches =
                    {
                        new MatchDynamicProperty
                            {
                                Column = 0,
                                Evaluation = new GenericEval(0, 10, 1, 0)
                            },
                        new MatchDynamicProperty
                            {
                                Column = 1,
                                Evaluation = new GenericEval(0, 10, 0, 0)
                            }
                    }
            };

            var valueChanged = false;
            var target = EvaluatorFactory.CreateEvaluator(eval, context) as CsvMappingEvaluator;
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
        /// Tests that the XML deserialization works fine.
        /// </summary>
        [TestMethod]
        public void TestDeserialization()
        {
            const string XmlData = @"<CsvMapping FileName=""codeconversion.csv"" OutputFormat=""{3}"">
                                       <DefaultValue>
                                         <Generic Lang=""0"" Table=""10"" Column=""0"" Row=""0"" />
                                       </DefaultValue>
                                       <Match Column=""0"">
                                         <Generic Lang=""0"" Table=""10"" Column=""1"" Row=""0"" />
                                       </Match>
                                       <Match Column=""1"">
                                         <Generic Lang=""0"" Table=""10"" Column=""0"" Row=""0"" />
                                       </Match>
                                     </CsvMapping>";

            var serializer = new XmlSerializer(typeof(CsvMappingEval));
            var reader = new StringReader(XmlData);
            var eval = serializer.Deserialize(reader) as CsvMappingEval;

            Assert.IsNotNull(eval);
            Assert.AreEqual("codeconversion.csv", eval.FileName);
            Assert.AreEqual("{3}", eval.OutputFormat);

            Assert.IsNotNull(eval.DefaultValue);
            var generic = eval.DefaultValue.Evaluation as GenericEval;
            Assert.IsNotNull(generic);
            Assert.AreEqual(0, generic.Language);
            Assert.AreEqual(10, generic.Table);
            Assert.AreEqual(0, generic.Column);
            Assert.AreEqual(0, generic.Row);

            Assert.AreEqual(2, eval.Matches.Count);

            var match = eval.Matches[0];
            Assert.AreEqual(0, match.Column);
            generic = match.Evaluation as GenericEval;
            Assert.IsNotNull(generic);
            Assert.AreEqual(0, generic.Language);
            Assert.AreEqual(10, generic.Table);
            Assert.AreEqual(1, generic.Column);
            Assert.AreEqual(0, generic.Row);

            match = eval.Matches[1];
            Assert.AreEqual(1, match.Column);
            generic = match.Evaluation as GenericEval;
            Assert.IsNotNull(generic);
            Assert.AreEqual(0, generic.Language);
            Assert.AreEqual(10, generic.Table);
            Assert.AreEqual(0, generic.Column);
            Assert.AreEqual(0, generic.Row);
        }
    }
}
