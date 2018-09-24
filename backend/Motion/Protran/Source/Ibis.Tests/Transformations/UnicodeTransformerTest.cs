// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnicodeTransformerTest.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for UnicodeTransformerTest and is intended
//   to contain all UnicodeTransformerTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Tests.Transformations
{
    using System.Text;

    using Gorba.Motion.Protran.Core.Transformations;
    using Gorba.Motion.Protran.Ibis.Transformations;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// This is a test class for UnicodeTransformerTest and is intended
    /// to contain all UnicodeTransformerTest Unit Tests
    /// </summary>
    [TestClass]
    public class UnicodeTransformerTest
    {
        /// <summary>
        /// A test for Transform
        /// </summary>
        [TestMethod]
        [DeploymentItem("Gorba.Motion.Protran.Protocols.dll")]
        public void TransformTest()
        {
            string result = null;
            var nextMock = new Mock<ITransformationSink<string>>();
            nextMock.Setup(t => t.Transform(It.IsAny<string>())).Callback<string>(value => result = value);

            var transformer = new UnicodeTransformer();
            ((ITransformationSource)transformer).Next = nextMock.Object;
            byte[] buffer = Encoding.BigEndianUnicode.GetBytes("Hello World!");
            transformer.Transform(buffer);

            Assert.AreEqual("Hello World!", result);
        }
    }
}
