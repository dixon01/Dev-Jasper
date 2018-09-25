// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BbTagParsingTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BbTagParsingTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.BbCode.Tests
{
    using System.IO;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for <see cref="BbParser"/> to see that (all) tags are parsed properly.
    /// </summary>
    [TestClass]
    public class BbTagParsingTest
    {
        /// <summary>
        /// Tests the image tag.
        /// </summary>
        [TestMethod]
        public void ImageTagTest()
        {
            var target = new BbParser();
            string input = @"Text with [img=images\gorba.jpg] shown";

            var root = target.Parse(input);
            Assert.AreEqual(input, target.Serialize(root));

            var children = root.Children.ToList();
            Assert.AreEqual(3, children.Count);
            Assert.AreEqual("Text with ", ((BbText)children[0]).Text);
            Assert.AreEqual(@"images\gorba.jpg", ((Image)children[1]).FileName);
            Assert.AreEqual(" shown", ((BbText)children[2]).Text);
        }

        /// <summary>
        /// Tests the image tag.
        /// </summary>
        [TestMethod]
        public void ImageTagContextTest()
        {
            var context = new Mock<IBbParserContext>();
            context.Setup(c => c.GetAbsolutePathRelatedToConfig(It.IsAny<string>()))
                   .Returns((string s) => Path.Combine("C:\\", s));

            var target = new BbParser();
            string input = @"Text with [img=images\gorba.jpg] shown";

            var root = target.Parse(input, context.Object);
            Assert.AreEqual(@"Text with [img=C:\images\gorba.jpg] shown", target.Serialize(root));

            var children = root.Children.ToList();
            Assert.AreEqual(3, children.Count);
            Assert.AreEqual("Text with ", ((BbText)children[0]).Text);
            Assert.AreEqual(@"C:\images\gorba.jpg", ((Image)children[1]).FileName);
            Assert.AreEqual(" shown", ((BbText)children[2]).Text);
        }

        /// <summary>
        /// Tests the video tag.
        /// </summary>
        [TestMethod]
        public void VideoTagTest()
        {
            var target = new BbParser();
            string input = @"Text with [vid=videos\video.mpg] shown";

            var root = target.Parse(input);
            Assert.AreEqual(input, target.Serialize(root));

            var children = root.Children.ToList();
            Assert.AreEqual(3, children.Count);
            Assert.AreEqual("Text with ", ((BbText)children[0]).Text);
            Assert.AreEqual(@"videos\video.mpg", ((Video)children[1]).VideoUri);
            Assert.AreEqual(" shown", ((BbText)children[2]).Text);
        }

        /// <summary>
        /// Tests the video tag.
        /// </summary>
        [TestMethod]
        public void VideoTagContextTest()
        {
            var context = new Mock<IBbParserContext>();
            context.Setup(c => c.GetAbsolutePathRelatedToConfig(It.IsAny<string>()))
                   .Returns((string s) => Path.Combine("C:\\", s));

            var target = new BbParser();
            string input = @"Text with [vid=videos\video.mpg] shown";

            var root = target.Parse(input, context.Object);
            Assert.AreEqual(@"Text with [vid=C:\videos\video.mpg] shown", target.Serialize(root));

            var children = root.Children.ToList();
            Assert.AreEqual(3, children.Count);
            Assert.AreEqual("Text with ", ((BbText)children[0]).Text);
            Assert.AreEqual(@"C:\videos\video.mpg", ((Video)children[1]).VideoUri);
            Assert.AreEqual(" shown", ((BbText)children[2]).Text);
        }

        /// <summary>
        /// Tests the parsing of the [time=format] tag.
        /// </summary>
        [TestMethod]
        public void TimeTagTest()
        {
            var target = new BbParser();
            string input = @"Text with [time=HH:mm] (the current time)";

            var root = target.Parse(input);
            Assert.AreEqual(input, target.Serialize(root));

            var children = root.Children.ToList();
            Assert.AreEqual(3, children.Count);
            Assert.AreEqual("Text with ", ((BbText)children[0]).Text);
            Assert.AreEqual("HH:mm", ((Time)children[1]).TimeFormat);
            Assert.AreEqual(" (the current time)", ((BbText)children[2]).Text);
        }
    }
}
