// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegexDividerTransformerTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for RegexDividerTransformerTest and is intended
//   to contain all RegexDividerTransformerTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Tests.Transformations
{
    using Gorba.Common.Configuration.Protran.Transformations;
    using Gorba.Motion.Protran.Core.Transformations;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// This is a test class for RegexDividerTransformerTest and is intended
    /// to contain all RegexDividerTransformerTest Unit Tests
    /// </summary>
    [TestClass]
    public class RegexDividerTransformerTest
    {
        /// <summary>
        /// A test for DoTransform
        /// </summary>
        [TestMethod]
        [DeploymentItem("Gorba.Motion.Protran.Protocols.dll")]
        public void TransformTest()
        {
            string[] result = null;
            var nextMock = new Mock<ITransformationSink<string[]>>();
            nextMock.Setup(t => t.Transform(It.IsAny<string[]>())).Callback<string[]>(value => result = value);

            var transformer = new RegexDividerTransformer();
            var divider = new RegexDivider();
            ((ITransformationSource)transformer).Next = nextMock.Object;

            // "divide at newline characters"
            divider.Regex = "\u000A";
            ((ITransformer)transformer).Configure(divider);
            transformer.Transform("Hello\nWorld !");
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("Hello", result[0]);
            Assert.AreEqual("World !", result[1]);
        }
    }
}
