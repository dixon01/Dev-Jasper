// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderHelperTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RenderHelperTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Tests
{
    using System.Drawing;

    using Gorba.Common.Configuration.Infomedia.Layout;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test for <see cref="RenderHelper"/>.
    /// </summary>
    [TestClass]
    public class RenderHelperTest
    {
        /// <summary>
        /// Tests <see cref="RenderHelper.ApplyScaling"/> using <see cref="ElementScaling.Fixed"/>.
        /// </summary>
        [TestMethod]
        public void TestApplyScalingFixed()
        {
            var bounds = new Rectangle(100, 120, 20, 25);
            var size = new Size(17, 31);
            var result = RenderHelper.ApplyScaling(bounds, size, ElementScaling.Fixed);
            Assert.AreEqual(101, result.X);
            Assert.AreEqual(117, result.Y);
            Assert.AreEqual(17, result.Width);
            Assert.AreEqual(31, result.Height);

            size = new Size(17, 13);
            result = RenderHelper.ApplyScaling(bounds, size, ElementScaling.Fixed);
            Assert.AreEqual(101, result.X);
            Assert.AreEqual(126, result.Y);
            Assert.AreEqual(17, result.Width);
            Assert.AreEqual(13, result.Height);

            size = new Size(29, 43);
            result = RenderHelper.ApplyScaling(bounds, size, ElementScaling.Fixed);
            Assert.AreEqual(96, result.X);
            Assert.AreEqual(111, result.Y);
            Assert.AreEqual(29, result.Width);
            Assert.AreEqual(43, result.Height);
        }

        /// <summary>
        /// Tests <see cref="RenderHelper.ApplyScaling"/> using <see cref="ElementScaling.Stretch"/>.
        /// </summary>
        [TestMethod]
        public void TestApplyScalingStretch()
        {
            var bounds = new Rectangle(100, 120, 20, 25);
            var size = new Size(17, 31);
            var result = RenderHelper.ApplyScaling(bounds, size, ElementScaling.Stretch);
            Assert.AreEqual(100, result.X);
            Assert.AreEqual(120, result.Y);
            Assert.AreEqual(20, result.Width);
            Assert.AreEqual(25, result.Height);

            size = new Size(17, 13);
            result = RenderHelper.ApplyScaling(bounds, size, ElementScaling.Stretch);
            Assert.AreEqual(100, result.X);
            Assert.AreEqual(120, result.Y);
            Assert.AreEqual(20, result.Width);
            Assert.AreEqual(25, result.Height);

            size = new Size(29, 43);
            result = RenderHelper.ApplyScaling(bounds, size, ElementScaling.Stretch);
            Assert.AreEqual(100, result.X);
            Assert.AreEqual(120, result.Y);
            Assert.AreEqual(20, result.Width);
            Assert.AreEqual(25, result.Height);
        }

        /// <summary>
        /// Tests <see cref="RenderHelper.ApplyScaling"/> using <see cref="ElementScaling.Scale"/>.
        /// </summary>
        [TestMethod]
        public void TestApplyScalingScale()
        {
            var bounds = new Rectangle(100, 120, 20, 25);
            var size = new Size(17, 31);
            var result = RenderHelper.ApplyScaling(bounds, size, ElementScaling.Scale);
            Assert.AreEqual(103, result.X);
            Assert.AreEqual(120, result.Y);
            Assert.AreEqual(13, result.Width);
            Assert.AreEqual(25, result.Height);

            size = new Size(17, 13);
            result = RenderHelper.ApplyScaling(bounds, size, ElementScaling.Scale);
            Assert.AreEqual(100, result.X);
            Assert.AreEqual(125, result.Y);
            Assert.AreEqual(20, result.Width);
            Assert.AreEqual(15, result.Height);

            size = new Size(29, 43);
            result = RenderHelper.ApplyScaling(bounds, size, ElementScaling.Scale);
            Assert.AreEqual(102, result.X);
            Assert.AreEqual(120, result.Y);
            Assert.AreEqual(16, result.Width);
            Assert.AreEqual(25, result.Height);
        }
    }
}
