// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EglBitmapTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EglBitmapTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Tests.Bitmaps
{
    using Gorba.Common.Formats.AlphaNT.Bitmaps;
    using Gorba.Common.Formats.AlphaNT.Common;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="EglBitmap"/>.
    /// </summary>
    [TestClass]
    public class EglBitmapTest
    {
        /// <summary>
        /// Tests the constructor.
        /// </summary>
        [TestMethod]
        [DeploymentItem("Bitmaps\\FUSSBALL.EGL")]
        public void TestConstructor()
        {
            var target = new EglBitmap("FUSSBALL.EGL");
            Assert.AreEqual(14, target.Width);
            Assert.AreEqual(16, target.Height);

            TestLine(target, 0, "        XX    ");
            TestLine(target, 1, "       XXXX   ");
            TestLine(target, 2, "       XXXX   ");
            TestLine(target, 3, "        XX    ");
            TestLine(target, 4, "              ");
            TestLine(target, 5, " XXXXXXXXXXX  ");
            TestLine(target, 6, "XXXXXXXXXXXXX ");
            TestLine(target, 7, "     XXXXXXXXX");
            TestLine(target, 8, "    XXXXXXX XX");
            TestLine(target, 9, "   X XXXXX  XX");
            TestLine(target, 10, "  XXX XXX  XX ");
            TestLine(target, 11, " XXX X X   X  ");
            TestLine(target, 12, " XX XXX       ");
            TestLine(target, 13, "   XXX   X    ");
            TestLine(target, 14, "  XXX   X X   ");
            TestLine(target, 15, "  XX     X    ");
        }

        private static void TestLine(IBitmap target, int y, string expectation)
        {
            Assert.AreEqual(target.Width, expectation.Length);
            Assert.IsTrue(target.Height > y);

            for (int x = 0; x < expectation.Length; x++)
            {
                var expectedPixel = expectation[x] == ' ' ? Colors.Black : Colors.White;
                Assert.AreEqual(expectedPixel, target.GetPixel(x, y));
            }
        }
    }
}