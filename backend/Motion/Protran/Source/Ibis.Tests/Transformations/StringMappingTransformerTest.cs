// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringMappingTransformerTest.cs" company="Gorba AG">
//   Copyright � 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for StringMappingTransformerTest and is intended
//   to contain all StringMappingTransformerTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Tests.Transformations
{
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Protran.Transformations;
    using Gorba.Motion.Protran.Core.Transformations;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// This is a test class for StringMappingTransformerTest and is intended
    /// to contain all StringMappingTransformerTest Unit Tests
    /// </summary>
    [TestClass]
    public class StringMappingTransformerTest
    {
        /// <summary>
        /// A test for DoTransform
        /// </summary>
        [TestMethod]
        [DeploymentItem("Gorba.Motion.Protran.Protocols.dll")]
        public void TransformTest()
        {
            string result = null;
            var nextMock = new Mock<ITransformationSink<string>>();
            nextMock.Setup(t => t.Transform(It.IsAny<string>())).Callback<string>(value => result = value);

            var transformer = new StringMappingTransformer();
            var stringMap = new StringMapping();

            var map = new Mapping { From = "{", To = "�" };
            stringMap.Mappings = new List<Mapping>();
            stringMap.Mappings.Add(map);
            ((ITransformer)transformer).Configure(stringMap);
            ((ITransformationSource)transformer).Next = nextMock.Object;
            transformer.Transform("Sentence with {");
            Assert.AreEqual("Sentence with �", result);

            map = new Mapping { From = "[", To = "�" };
            stringMap.Mappings = new List<Mapping>();
            stringMap.Mappings.Add(map);
            ((ITransformer)transformer).Configure(stringMap);
            ((ITransformationSource)transformer).Next = nextMock.Object;
            transformer.Transform("Sentence with [");
            Assert.AreEqual("Sentence with �", result);

            map = new Mapping { From = "|", To = "�" };
            stringMap.Mappings = new List<Mapping>();
            stringMap.Mappings.Add(map);
            ((ITransformer)transformer).Configure(stringMap);
            ((ITransformationSource)transformer).Next = nextMock.Object;
            transformer.Transform("Sentence with |");
            Assert.AreEqual("Sentence with �", result);

            map = new Mapping { From = "\\", To = "�" };
            stringMap.Mappings = new List<Mapping>();
            stringMap.Mappings.Add(map);
            ((ITransformer)transformer).Configure(stringMap);
            ((ITransformationSource)transformer).Next = nextMock.Object;
            transformer.Transform("Sentence with \\");
            Assert.AreEqual("Sentence with �", result);

            map = new Mapping { From = "}", To = "�" };
            stringMap.Mappings = new List<Mapping>();
            stringMap.Mappings.Add(map);
            ((ITransformer)transformer).Configure(stringMap);
            ((ITransformationSource)transformer).Next = nextMock.Object;
            transformer.Transform("Sentence with }");
            Assert.AreEqual("Sentence with �", result);

            map = new Mapping { From = "]", To = "�" };
            stringMap.Mappings = new List<Mapping>();
            stringMap.Mappings.Add(map);
            ((ITransformer)transformer).Configure(stringMap);
            ((ITransformationSource)transformer).Next = nextMock.Object;
            transformer.Transform("Sentence with ]");
            Assert.AreEqual("Sentence with �", result);

            map = new Mapping { From = "~", To = "�" };
            stringMap.Mappings = new List<Mapping>();
            stringMap.Mappings.Add(map);
            ((ITransformer)transformer).Configure(stringMap);
            ((ITransformationSource)transformer).Next = nextMock.Object;
            transformer.Transform("Sentence with ~");
            Assert.AreEqual("Sentence with �", result);

            stringMap.Mappings = new List<Mapping>
                {
                    new Mapping { From = "{", To = "�" },
                    new Mapping { From = "|", To = "�" },
                    new Mapping { From = "}", To = "�" }
                };
            ((ITransformer)transformer).Configure(stringMap);
            ((ITransformationSource)transformer).Next = nextMock.Object;
            transformer.Transform("Sentence with {|} {}");
            Assert.AreEqual("Sentence with ��� ��", result);
        }
    }
}
