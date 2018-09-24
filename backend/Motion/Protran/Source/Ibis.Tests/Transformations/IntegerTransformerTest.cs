// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegerTransformerTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for IntegerTransformerTest and is intended
//   to contain all IntegerTransformerTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Tests.Transformations
{
    using System.Globalization;

    using Gorba.Common.Configuration.Protran.Transformations;
    using Gorba.Motion.Protran.Core.Transformations;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// This is a test class for IntegerTransformerTest and is intended
    /// to contain all IntegerTransformerTest Unit Tests
    /// </summary>
    [TestClass]
    public class IntegerTransformerTest
    {
        /// <summary>
        /// A test for Transform
        /// </summary>
        [TestMethod]
        [DeploymentItem("Gorba.Motion.Protran.Protocols.dll")]
        public void TransformTest()
        {
            // ARRANGE
            var nextMock = new Mock<ITransformationSink<int>>();
            int result = int.MinValue;
            nextMock.Setup(t => t.Transform(It.IsAny<int>())).Callback<int>(value => result = value);

            var transformer = new IntegerTransformer();
            ((ITransformer)transformer).Configure(new Integer());
            ((ITransformationSource)transformer).Next = nextMock.Object;

            // ACT
            for (int i = -1000; i != 1000; i++)
            {
                transformer.Transform(i.ToString(CultureInfo.InvariantCulture));

                // ASSERT
                Assert.AreEqual(i, result);
            }
        }
    }
}
