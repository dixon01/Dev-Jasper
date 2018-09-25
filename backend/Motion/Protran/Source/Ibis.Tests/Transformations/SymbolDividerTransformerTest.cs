// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SymbolDividerTransformerTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for SymbolDividerTransformerTest and is intended
//   to contain all SymbolDividerTransformerTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Tests.Transformations
{
    using Gorba.Common.Configuration.Protran.Transformations;
    using Gorba.Motion.Protran.Core.Transformations;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// This is a test class for SymbolDividerTransformerTest and is intended
    /// to contain all SymbolDividerTransformerTest Unit Tests
    /// </summary>
    [TestClass]
    public class SymbolDividerTransformerTest
    {
        /// <summary>
        /// A test for Transform
        /// </summary>
        [TestMethod]
        [DeploymentItem("Gorba.Motion.Protran.Protocols.dll")]
        public void TransformTest()
        {
            string[] result = null;
            var nextMock = new Mock<ITransformationSink<string[]>>();
            nextMock.Setup(t => t.Transform(It.IsAny<string[]>())).Callback<string[]>(value => result = value);

            var transformer = new SymbolDividerTransformer();
            var divider = new SymbolDivider { Symbol = "#" };
            ((ITransformer)transformer).Configure(divider);
            ((ITransformationSource)transformer).Next = nextMock.Object;

            transformer.Transform("Hello#World!");
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("Hello", result[0]);
            Assert.AreEqual("World!", result[1]);
        }
    }
}
