// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LawoStringTransformerTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LawoStringTransformerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Tests.Transformations
{
    using System.Collections.Generic;
    using System.Text;

    using Gorba.Common.Configuration.Protran.Transformations;
    using Gorba.Motion.Protran.Core.Transformations;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// This is a test class for LawoStringTransformer and is intended
    /// to contain all LawoStringTransformer Unit Tests
    /// </summary>
    [TestClass]
    public class LawoStringTransformerTest
    {
        /// <summary>
        /// A test for Transform
        /// </summary>
        [TestMethod]
        public void TransformTest()
        {
            string result = null;
            var nextMock = new Mock<ITransformationSink<string>>();
            nextMock.Setup(t => t.Transform(It.IsAny<string>())).Callback<string>(value => result = value);

            var transformer = new LawoStringTransformer();
            ((ITransformer)transformer).Configure(new LawoString());
            ((ITransformer)transformer).Next = nextMock.Object;

            var bytes = Encoding.ASCII.GetBytes("HELLO WORLD");
            transformer.Transform(bytes);
            Assert.AreEqual("HELLO WORLD", result);

            bytes = Encoding.ASCII.GetBytes("H\u0006ELLO \u0006W\u0006ORLD\u0006");
            transformer.Transform(bytes);
            Assert.AreEqual("Hello World", result);

            bytes = Encoding.ASCII.GetBytes("Hello World");
            transformer.Transform(bytes);
            Assert.AreEqual("Hello World", result);

            bytes = Encoding.ASCII.GetBytes("H\u0006ello \u0006W\u0006orld\u0006");
            transformer.Transform(bytes);
            Assert.AreEqual("Hello World", result);

            bytes = Encoding.ASCII.GetBytes("\u0006E = MC\u0001R");
            transformer.Transform(bytes);
            Assert.AreEqual("e = mc²", result);

            bytes = Encoding.ASCII.GetBytes("Some \u000CDV\\\u000C and then \u0002D, \u0002V, \u0002\\");
            transformer.Transform(bytes);
            Assert.AreEqual("Some äöü and then ä, ö, ü", result);
        }

        /// <summary>
        /// A test for Transform that uses (almost) real telegram contents
        /// </summary>
        [TestMethod]
        public void TransformRealTelegramTest()
        {
            string result = null;
            var nextMock = new Mock<ITransformationSink<string>>();
            nextMock.Setup(t => t.Transform(It.IsAny<string>())).Callback<string>(value => result = value);

            var transformer = new LawoStringTransformer();
            ((ITransformer)transformer).Configure(new LawoString());
            ((ITransformer)transformer).Next = nextMock.Object;

            var bytes = Encoding.ASCII.GetBytes("aX80\u0003001\u0004O\u0006BERRIET \u0006Z\u0006OLLBR\u0002\\CKE");
            transformer.Transform(bytes);
            Assert.AreEqual("aX80\u0003001\u0004Oberriet Zollbrücke", result);

            bytes = Encoding.ASCII.GetBytes("aX81\u0003002\u0004O\u0006BERRIET \u0006S\u0006EKUNDARSCHULE");
            transformer.Transform(bytes);
            Assert.AreEqual("aX81\u0003002\u0004Oberriet Sekundarschule", result);

            bytes = Encoding.ASCII.GetBytes("aX81\u0003007\u0004E\u0006ICHBERG \u0006H\u0002D\u0006RDLI");
            transformer.Transform(bytes);
            Assert.AreEqual("aX81\u0003007\u0004Eichberg Härdli", result);

            bytes = Encoding.ASCII.GetBytes("aX81\u0003008\u0004E\u0006ICHBERG \u0006S\u0002\\\u0006ESSWINKEL");
            transformer.Transform(bytes);
            Assert.AreEqual("aX81\u0003008\u0004Eichberg Süesswinkel", result);

            bytes = Encoding.ASCII.GetBytes("aX81\u0003010\u0004H\u0006INTERFORST \u0006O\u0006BERR\u0002\\TI");
            transformer.Transform(bytes);
            Assert.AreEqual("aX81\u0003010\u0004Hinterforst Oberrüti", result);

            bytes = Encoding.ASCII.GetBytes("aX81\u0003013\u0004A\u0006LTST\u0002DTTEN \u0006F\u0006ORST");
            transformer.Transform(bytes);
            Assert.AreEqual("aX81\u0003013\u0004Altstätten Forst", result);
        }

        /// <summary>
        /// A test for Transform that uses real LAWO telegrams (using code page 858).
        /// </summary>
        [TestMethod]
        public void TransformLawoTelegramTest()
        {
            string result = null;
            var nextMock = new Mock<ITransformationSink<string>>();
            nextMock.Setup(t => t.Transform(It.IsAny<string>())).Callback<string>(value => result = value);

            var transformer = new LawoStringTransformer();
            ((ITransformer)transformer).Configure(new LawoString { CodePage = 858 });
            ((ITransformer)transformer).Next = nextMock.Object;

            var bytes = ParseLawoLog("aX80<003>001<004>O<006>BERRIET <006>Z<006>OLLBR<001>!CKE");
            transformer.Transform(bytes);
            Assert.AreEqual("aX80\u0003001\u0004Oberriet Zollbrücke", result);

            bytes = ParseLawoLog("aX81<003>002<004>O<006>BERRIET <006>S<006>EKUNDARSCHULE");
            transformer.Transform(bytes);
            Assert.AreEqual("aX81\u0003002\u0004Oberriet Sekundarschule", result);

            bytes = ParseLawoLog("aX81<003>003<004>O<006>BERRIET <006>F<006>RANZISKUSSTR.");
            transformer.Transform(bytes);
            Assert.AreEqual("aX81\u0003003\u0004Oberriet Franziskusstr.", result);

            bytes = ParseLawoLog("aX81<003>004<004>O<006>BERRIET <006>M<006>ETTLEN");
            transformer.Transform(bytes);
            Assert.AreEqual("aX81\u0003004\u0004Oberriet Mettlen", result);

            bytes = ParseLawoLog("aX81<003>005<004>O<006>BERRIET <006>H<006>UEB");
            transformer.Transform(bytes);
            Assert.AreEqual("aX81\u0003005\u0004Oberriet Hueb", result);

            bytes = ParseLawoLog("aX81<003>006<004>E<006>ICHBERG <006>P<006>OST");
            transformer.Transform(bytes);
            Assert.AreEqual("aX81\u0003006\u0004Eichberg Post", result);

            bytes = ParseLawoLog("aX81<003>007<004>E<006>ICHBERG <006>H<001>$<006>RDLI<");
            transformer.Transform(bytes);
            Assert.AreEqual("aX81\u0003007\u0004Eichberg Härdli", result);

            bytes = ParseLawoLog("aX81<003>008<004>E<006>ICHBERG <006>S<001>!<006>ESSWINKEL");
            transformer.Transform(bytes);
            Assert.AreEqual("aX81\u0003008\u0004Eichberg Süesswinkel", result);

            bytes = ParseLawoLog("aX81<003>009<004>E<006>ICHBERG <006>D<006>ORF");
            transformer.Transform(bytes);
            Assert.AreEqual("aX81\u0003009\u0004Eichberg Dorf", result);

            bytes = ParseLawoLog("aX81<003>010<004>H<006>INTERFORST <006>O<006>BERR<001>!TI");
            transformer.Transform(bytes);
            Assert.AreEqual("aX81\u0003010\u0004Hinterforst Oberrüti", result);

            bytes = ParseLawoLog("aX81<003>013<004>A<006>LTST<001>$TTEN <006>F<006>ORST");
            transformer.Transform(bytes);
            Assert.AreEqual("aX81\u0003013\u0004Altstätten Forst", result);
        }

        private static byte[] ParseLawoLog(string input)
        {
            var bytes = new List<byte>(input.Length);
            for (int i = 0; i < input.Length; i++)
            {
                var c = input[i];
                if (c != '<')
                {
                    bytes.Add((byte)c);
                    continue;
                }

                int end = input.IndexOf('>', i);
                if (end < 0)
                {
                    break;
                }

                i++;
                byte b;
                if (!byte.TryParse(input.Substring(i, end - i), out b))
                {
                    break;
                }

                bytes.Add(b);
                i = end;
            }

            return bytes.ToArray();
        }
    }
}
