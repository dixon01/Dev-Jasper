// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegexMappingTransformerTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for RegexMappingTransformerTest and is intended
//   to contain all RegexMappingTransformerTest Unit Tests
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
    /// This is a test class for RegexMappingTransformerTest and is intended
    /// to contain all RegexMappingTransformerTest Unit Tests
    /// </summary>
    [TestClass]
    public class RegexMappingTransformerTest
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

            var transformer = new RegexMappingTransformer();
            var regexMap = new RegexMapping();

            var map = new Mapping { From = "^(\\d\\d)(\\d\\d)$", To = "$1:$2" };
            regexMap.Mappings = new List<Mapping>();
            regexMap.Mappings.Add(map);
            ((ITransformer)transformer).Configure(regexMap);
            ((ITransformationSource)transformer).Next = nextMock.Object;
            transformer.Transform("1011");
            Assert.AreEqual("10:11", result);

            map = new Mapping { From = "^(\\d\\d)(\\d\\d)(\\d\\d)$", To = "$1.$2.20$3" };
            regexMap.Mappings = new List<Mapping>();
            regexMap.Mappings.Add(map);
            ((ITransformer)transformer).Configure(regexMap);
            ((ITransformationSource)transformer).Next = nextMock.Object;
            transformer.Transform("101112");
            Assert.AreEqual("10.11.2012", result);
        }
    }
}
