// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsciiTransformerTest.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for AsciiTransformerTest and is intended
//   to contain all AsciiTransformerTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Tests.Transformations
{
    using Gorba.Motion.Protran.Core.Transformations;
    using Gorba.Motion.Protran.Ibis.Transformations;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// This is a test class for AsciiTransformerTest and is intended
    /// to contain all AsciiTransformerTest Unit Tests
    /// </summary>
    [TestClass]
    public class AsciiTransformerTest
    {
        /// <summary>
        /// A test for Transform
        /// </summary>
        [TestMethod]
        [DeploymentItem("Gorba.Motion.Protran.Protocols.dll")]
        public void TransformTest()
        {
            // ARRANGE
            var nextMock = new Mock<ITransformationSink<string>>();
            string result = null;
            nextMock.Setup(t => t.Transform(It.IsAny<string>())).Callback<string>(value => result = value);

            var transformer = new AsciiTransformer();
            ((ITransformationSource)transformer).Next = nextMock.Object;

            // ACT
            transformer.Transform(new byte[] { 0x61, 0x64, 0x66 });

            // ASSERT
            Assert.AreEqual("adf", result);
        }
    }
}
