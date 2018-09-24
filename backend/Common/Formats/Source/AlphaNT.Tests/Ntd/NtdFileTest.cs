// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NtdFileTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NtdFileTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Tests.Ntd
{
    using System.Collections.Generic;

    using Gorba.Common.Formats.AlphaNT.Ntd;
    using Gorba.Common.Formats.AlphaNT.Ntd.Primitives;
    using Gorba.Common.Formats.AlphaNT.Ntd.Telegrams;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="NtdFile"/>.
    /// </summary>
    [TestClass]
    public class NtdFileTest
    {
        /// <summary>
        /// Tests the constructor.
        /// </summary>
        [TestMethod]
        [DeploymentItem("Ntd\\TCBR3.ntd")]
        public void TestConstructor()
        {
            var file = new NtdFile("TCBR3.ntd");
            Assert.IsNotNull(file.Signs);
        }

        /// <summary>
        /// Tests that all signs are loaded properly and a few other things work as well.
        /// This file uses <see cref="SimpleTelegramInfo"/>.
        /// </summary>
        [TestMethod]
        [DeploymentItem("Ntd\\16x_112_112_28.ntd")]
        public void TestSignsSimple()
        {
            var file = new NtdFile("16x_112_112_28.ntd");
            Assert.AreEqual(3, file.Signs.Length);

            Assert.AreEqual(1, file.Signs[0].Address);
            Assert.AreEqual(2, file.Signs[1].Address);
            Assert.AreEqual(3, file.Signs[2].Address);

            var sign = file.Signs[1];
            Assert.AreEqual(112, sign.Width);
            Assert.AreEqual(16, sign.Height);

            var telegrams = new List<ITelegramInfo>(sign.GetLineTelegrams());
            Assert.AreEqual(777, telegrams.Count);

            foreach (var telegramInfo in telegrams)
            {
                foreach (var primitive in telegramInfo.GetPrimitives())
                {
                    Assert.IsInstanceOfType(primitive, typeof(TextPrimitive));
                }
            }

            telegrams = new List<ITelegramInfo>(sign.GetDestinationTelegrams());
            Assert.AreEqual(92, telegrams.Count);

            var telegram = telegrams[2];
            Assert.IsNotNull(telegram);
            Assert.AreEqual(2, telegram.PrimitiveCount);

            var primitives = new List<GraphicPrimitiveBase>(telegram.GetPrimitives());
            Assert.AreEqual(2, primitives.Count);

            var text = primitives[0] as TextPrimitive;
            Assert.IsNotNull(text);

            var value = file.GetString(text.TextAddress);
            Assert.AreEqual("ziel2", value);

            var font = file.GetFont(text.FontIndex);
            Assert.IsNotNull(font);

            text = primitives[1] as TextPrimitive;
            Assert.IsNotNull(text);

            value = file.GetString(text.TextAddress);
            Assert.AreEqual("Ziel 2.2", value);

            font = file.GetFont(text.FontIndex);
            Assert.IsNotNull(font);

            foreach (var telegramInfo in telegrams)
            {
                foreach (var primitive in telegramInfo.GetPrimitives())
                {
                    Assert.IsInstanceOfType(primitive, typeof(TextPrimitive));
                }
            }
        }

        /// <summary>
        /// Tests that all signs are loaded properly and a few other things work as well.
        /// This file uses <see cref="ExtendedTelegramInfo"/>.
        /// </summary>
        [TestMethod]
        [DeploymentItem("Ntd\\16x112_32x384_16x28.ntd")]
        public void TestSignsExtended()
        {
            var file = new NtdFile("16x112_32x384_16x28.ntd");
            Assert.AreEqual(3, file.Signs.Length);

            Assert.AreEqual(1, file.Signs[0].Address);
            Assert.AreEqual(2, file.Signs[1].Address);
            Assert.AreEqual(3, file.Signs[2].Address);

            var sign = file.Signs[2];
            Assert.AreEqual(28, sign.Width);
            Assert.AreEqual(16, sign.Height);

            var telegrams = new List<ITelegramInfo>(sign.GetLineTelegrams());
            Assert.AreEqual(999, telegrams.Count);

            foreach (var telegramInfo in telegrams)
            {
                foreach (var primitive in telegramInfo.GetPrimitives())
                {
                    Assert.IsInstanceOfType(primitive, typeof(TextPrimitive));
                }
            }

            telegrams = new List<ITelegramInfo>(sign.GetDestinationTelegrams());
            Assert.AreEqual(1, telegrams.Count);

            var telegram = telegrams[0];
            Assert.IsNotNull(telegram);
            Assert.AreEqual(1, telegram.PrimitiveCount);

            var primitives = new List<GraphicPrimitiveBase>(telegram.GetPrimitives());
            Assert.AreEqual(1, primitives.Count);

            var text = primitives[0] as TextPrimitive;
            Assert.IsNotNull(text);

            var value = file.GetString(text.TextAddress);
            Assert.AreEqual("123", value);

            var font = file.GetFont(text.FontIndex);
            Assert.IsNotNull(font);
        }

        /// <summary>
        /// Tests that all signs are loaded properly and a few other things work as well.
        /// This file uses <see cref="ColorTelegramInfo"/>.
        /// </summary>
        [TestMethod]
        [DeploymentItem("Ntd\\TCBR3.ntd")]
        public void TestSignsColor()
        {
            var file = new NtdFile("TCBR3.ntd");
            Assert.AreEqual(6, file.Signs.Length);

            Assert.AreEqual(1, file.Signs[0].Address);
            Assert.AreEqual(2, file.Signs[1].Address);
            Assert.AreEqual(3, file.Signs[2].Address);
            Assert.AreEqual(4, file.Signs[3].Address);
            Assert.AreEqual(5, file.Signs[4].Address);
            Assert.AreEqual(6, file.Signs[5].Address);

            var sign = file.Signs[3];
            Assert.AreEqual(120, sign.Width);
            Assert.AreEqual(24, sign.Height);

            var telegrams = new List<ITelegramInfo>(sign.GetLineTelegrams());
            Assert.AreEqual(0, telegrams.Count);

            telegrams = new List<ITelegramInfo>(sign.GetDestinationTelegrams());
            Assert.AreEqual(136, telegrams.Count);

            var telegram = telegrams[2];
            Assert.IsNotNull(telegram);
            Assert.AreEqual(1, telegram.PrimitiveCount);

            var primitives = new List<GraphicPrimitiveBase>(telegram.GetPrimitives());
            Assert.AreEqual(1, primitives.Count);

            var text = primitives[0] as TextPrimitive;
            Assert.IsNotNull(text);

            var value = file.GetString(text.TextAddress);
            Assert.IsNotNull(value);

            var font = file.GetFont(text.FontIndex);
            Assert.IsNotNull(font);

            foreach (var telegramInfo in telegrams)
            {
                foreach (var primitive in telegramInfo.GetPrimitives())
                {
                    Assert.IsInstanceOfType(primitive, typeof(TextPrimitive));
                }
            }
        }
    }
}
