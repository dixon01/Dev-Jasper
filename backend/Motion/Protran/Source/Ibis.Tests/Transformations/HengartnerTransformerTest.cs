// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HengartnerTransformerTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HengartnerTransformerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Tests.Transformations
{
    using Gorba.Motion.Protran.Core.Transformations;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// The Hengartner transformer test.
    /// </summary>
    [TestClass]
    public class HengartnerTransformerTest
    {
        /// <summary>
        /// Test to check transformation using Hengartner transformer
        /// </summary>
        [TestMethod]
        public void TransformNonArabicTest()
        {
            // ARRANGE
            var nextMock = new Mock<ITransformationSink<string>>();
            string result = null;
            nextMock.Setup(t => t.Transform(It.IsAny<string>())).Callback<string>(value => result = value);

            var transformer = new HengartnerTransformer();
            ((ITransformationSource)transformer).Next = nextMock.Object;

            // ACT
            transformer.Transform(
                new byte[]
                {
                    0x00, 0x03, 0x00, 0x31, 0x00, 0x38, 0x00, 0x04, 0x1C, 0x00, 0x4D, 0x00, 0x61, 0x00, 0x69, 0x00,
                    0x65, 0x00, 0x72, 0x00, 0x20, 0x00, 0x2F, 0x00, 0x20, 0x00, 0x4D, 0x00, 0x61, 0x00, 0x69, 0x00,
                    0x65, 0x00, 0x72, 0x00, 0x05, 0x39, 0x00, 0x34, 0x00, 0x34, 0x00, 0x35, 0x00, 0x0D, 0x30
                });

            // ASSERT
            Assert.AreEqual("\x000318\x0004Maier / Maier\x00059445」", result);
        }
    }
}
