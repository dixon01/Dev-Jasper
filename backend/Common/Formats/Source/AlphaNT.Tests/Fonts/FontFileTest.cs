// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FontFileTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FontFileTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Tests.Fonts
{
    using System;

    using Gorba.Common.Formats.AlphaNT.Fonts;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="FontFile"/>
    /// </summary>
    [TestClass]
    public class FontFileTest
    {
        private const string FontDir = "Fonts\\";
        private const string Fon8 = "G08X05U1_GORBA_V2.FON";
        private const string Fon16 = "G16X07_2_GORBA_V2.FON";
        private const string Fnt26 = "G26X19_4_GORBA_V1.FNT";
        private const string Fnt52 = "G52X18_5_GORBA_V1.FNT";
        private const string Hebrew160N = "HEBREW160N.FON";
        private const string Arabic = "Arabic.FON";
        private const string CUxFont = "GEDINARONE28.CUx";
        private const string ChinesFont = "simsun_12b.fon";

        /// <summary>
        /// Tests that a .FON file can be opened.
        /// </summary>
        [TestMethod]
        [DeploymentItem(FontDir + Fon8)]
        public void TestOpenFonFile()
        {
            var target = new FontFile(Fon8);
            Assert.AreEqual("G08X05U1", target.Name);
            Assert.AreEqual(224, target.CharacterCount);
            Assert.AreEqual(8, target.Height);
        }

        /// <summary>
        /// Tests that a .FNT file can be opened.
        /// </summary>
        [TestMethod]
        [DeploymentItem(FontDir + Fnt26)]
        public void TestOpenFntFile()
        {
            var target = new FontFile(Fnt26);
            Assert.AreEqual("G26X19_4", target.Name);
            Assert.AreEqual(96, target.CharacterCount);
            Assert.AreEqual(26, target.Height);
        }

        /// <summary>
        /// Tests that an ASCII character can be retrieved from a .FON file.
        /// </summary>
        [TestMethod]
        [DeploymentItem(FontDir + Fon16)]
        public void TestGetValidAsciiCharFon()
        {
            var target = new FontFile(Fon16);
            var bitmap = target.GetCharacter('A');
            Assert.IsNotNull(bitmap);
            Assert.AreEqual(7, bitmap.Width);
            Assert.AreEqual(16, bitmap.Height);
        }

        /// <summary>
        /// Tests that an ASCII character can be retrieved from a .FNT file.
        /// </summary>
        [TestMethod]
        [DeploymentItem(FontDir + Fnt52)]
        public void TestGetValidAsciiCharFnt()
        {
            var target = new FontFile(Fnt52);
            var bitmap = target.GetCharacter('X');
            Assert.IsNotNull(bitmap);
            Assert.AreEqual(19, bitmap.Width);
            Assert.AreEqual(52, bitmap.Height);
        }

        /// <summary>
        /// Tests that a non-ASCII character can be retrieved from a .FON file.
        /// </summary>
        [TestMethod]
        [DeploymentItem(FontDir + Fon8)]
        public void TestGetNonAsciiCharFon()
        {
            var target = new FontFile(Fon8);
            var bitmap = target.GetCharacter('ô');
            Assert.IsNotNull(bitmap);
            Assert.AreEqual(5, bitmap.Width);
            Assert.AreEqual(8, bitmap.Height);
        }

        /// <summary>
        /// Tests that an exception is thrown when a non-ASCII character is retrieved from a .FNT file.
        /// </summary>
        [TestMethod]
        [DeploymentItem(FontDir + Fnt52)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGetNonAsciiCharFnt()
        {
            var target = new FontFile(Fnt52);
            var bitmap = target.GetCharacter('ü');
            Assert.IsNotNull(bitmap);
        }

        /// <summary>
        /// Tests that a Unicode character can be retrieved from hebrew a Unicode .FON file.
        /// </summary>
        [TestMethod]
        [DeploymentItem(FontDir + Hebrew160N)]
        public void TestGetHebrewUnicdeCharFon()
        {
            var target = new FontFile(Hebrew160N);
            var bitmap = target.GetCharacter((char)0x5DE);
            Assert.IsNotNull(bitmap);
            Assert.AreEqual(11, bitmap.Width);
            Assert.AreEqual(16, bitmap.Height);
        }

        /// <summary>
        /// Tests that an exception is thrown when a non existing
        /// Unicode character is retrieved from hebrew Unicode file.
        /// </summary>
        [TestMethod]
        [DeploymentItem(FontDir + Hebrew160N)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGetNotValideHebrewUnicdeFonCharacter()
        {
            var target = new FontFile(Hebrew160N);
            var bitmap = target.GetCharacter((char)0x601);
            Assert.IsNotNull(bitmap);
        }

        /// <summary>
        /// Tests that a Unicode character can be retrieved from a arabic Unicode .FON file.
        /// </summary>
        [TestMethod]
        [DeploymentItem(FontDir + Arabic)]
        public void TestGetUnicodeCharArabicFon()
        {
            var target = new FontFile(Arabic);
            var bitmap = target.GetCharacter((char)0xFFFF);
            Assert.IsNotNull(bitmap);
            Assert.AreEqual(7, bitmap.Width);
            Assert.AreEqual(13, bitmap.Height);
        }

        /// <summary>
        /// Tests that a Unicode character can be retrieved from a chines Unicode .FON file.
        /// </summary>
        [TestMethod]
        [DeploymentItem(FontDir + ChinesFont)]
        public void TestGetUnicodeCharChineseFont()
        {
            var target = new FontFile(ChinesFont);
            var bitmap = target.GetCharacter((char)0xF0E0);
            Assert.IsNotNull(bitmap);
            Assert.AreEqual(16, bitmap.Width);
            Assert.AreEqual(16, bitmap.Height);
        }

        /// <summary>
        /// Tests that a unicode character can be retrieved from a Cux file.
        /// </summary>
        [TestMethod]
        [DeploymentItem(FontDir + CUxFont)]
        public void TestGetUnicodeCharCUxFont()
        {
            var target = new FontFile(CUxFont);
            var bitmap = target.GetCharacter((char)0x0624);
            Assert.IsNotNull(bitmap);
            Assert.AreEqual(11, bitmap.Width);
            Assert.AreEqual(28, bitmap.Height);
        }

        /// <summary>
        /// Tests that an exception is thrown when a non available  Unicode character is retrieved from a Cux file.
        /// </summary>
        [TestMethod]
        [DeploymentItem(FontDir + CUxFont)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGetNotValidUnicodeCharCuxFont()
        {
            var target = new FontFile(CUxFont);
            var bitmap = target.GetCharacter((char)0xFFFF);
            Assert.IsNotNull(bitmap);
        }
    }
}
