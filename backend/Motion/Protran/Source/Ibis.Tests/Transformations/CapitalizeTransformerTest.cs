// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CapitalizeTransformerTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for CapitalizeTransformerTest and is intended
//   to contain all CapitalizeTransformerTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Tests.Transformations
{
    using Gorba.Common.Configuration.Protran.Transformations;
    using Gorba.Motion.Protran.Core.Transformations;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// This is a test class for CapitalizeTransformerTest and is intended
    /// to contain all CapitalizeTransformerTest Unit Tests
    /// </summary>
    [TestClass]
    public class CapitalizeTransformerTest
    {
        /// <summary>
        /// A test for Transform
        /// </summary>
        [TestMethod]
        [DeploymentItem("Gorba.Motion.Protran.Protocols.dll")]
        public void TransformTest()
        {
            // ARRANGE
            string result = null;
            var nextMock = new Mock<ITransformationSink<string>>();
            nextMock.Setup(t => t.Transform(It.IsAny<string>())).Callback<string>(value => result = value);

            var transformer = new CapitalizeTransformer();
            ((ITransformer)transformer).Configure(new Capitalize());
            ((ITransformationSource)transformer).Next = nextMock.Object;

            // ACT
            transformer.Transform("hello World big H and big W!");

            // ASSERT
            Assert.AreEqual("Hello world big h and big w!", result);
        }

        /// <summary>
        /// A test for Transform
        /// </summary>
        [TestMethod]
        [DeploymentItem("Gorba.Motion.Protran.Protocols.dll")]
        public void TransformEmptyTest()
        {
            // ARRANGE
            string result = null;
            var nextMock = new Mock<ITransformationSink<string>>();
            nextMock.Setup(t => t.Transform(It.IsAny<string>())).Callback<string>(value => result = value);

            var transformer = new CapitalizeTransformer();
            ((ITransformer)transformer).Configure(new Capitalize());
            ((ITransformationSource)transformer).Next = nextMock.Object;

            // ACT
            transformer.Transform(string.Empty);

            // ASSERT
            Assert.AreEqual(string.Empty, result);
        }

        /// <summary>
        /// A test for Transform
        /// </summary>
        [TestMethod]
        [DeploymentItem("Gorba.Motion.Protran.Protocols.dll")]
        public void TransformUpperOnlyTest()
        {
            // ARRANGE
            string result = null;
            var nextMock = new Mock<ITransformationSink<string>>();
            nextMock.Setup(t => t.Transform(It.IsAny<string>())).Callback<string>(value => result = value);

            var transformer = new CapitalizeTransformer();
            ((ITransformer)transformer).Configure(new Capitalize { Mode = CapitalizeMode.UpperOnly });
            ((ITransformationSource)transformer).Next = nextMock.Object;

            // ACT
            transformer.Transform("hello World big H and big W!");

            // ASSERT
            Assert.AreEqual("Hello World big H and big W!", result);
        }

        /// <summary>
        /// A test for Transform
        /// </summary>
        [TestMethod]
        [DeploymentItem("Gorba.Motion.Protran.Protocols.dll")]
        public void TransformLowerOnlyTest()
        {
            // ARRANGE
            string result = null;
            var nextMock = new Mock<ITransformationSink<string>>();
            nextMock.Setup(t => t.Transform(It.IsAny<string>())).Callback<string>(value => result = value);

            var transformer = new CapitalizeTransformer();
            ((ITransformer)transformer).Configure(new Capitalize { Mode = CapitalizeMode.LowerOnly });
            ((ITransformationSource)transformer).Next = nextMock.Object;

            transformer.Transform("hello World big H and big W!");
            Assert.AreEqual("hello world big h and big w!", result);

            transformer.Transform("Hello World big H and big W!");
            Assert.AreEqual("Hello world big h and big w!", result);
        }

        /// <summary>
        /// A test for Transform
        /// </summary>
        [TestMethod]
        [DeploymentItem("Gorba.Motion.Protran.Protocols.dll")]
        public void TransformExceptionsTest()
        {
            // ARRANGE
            string result = null;
            var nextMock = new Mock<ITransformationSink<string>>();
            nextMock.Setup(t => t.Transform(It.IsAny<string>())).Callback<string>(value => result = value);

            var transformer = new CapitalizeTransformer();
            ((ITransformer)transformer).Configure(new Capitalize { Exceptions = new[] { "via", "IKEA" } });
            ((ITransformationSource)transformer).Next = nextMock.Object;

            transformer.Transform("VIA");
            Assert.AreEqual("via", result);

            transformer.Transform("IKEA");
            Assert.AreEqual("IKEA", result);

            transformer.Transform("STATION");
            Assert.AreEqual("Station", result);
        }
    }
}
