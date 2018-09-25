// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BbTextTest.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BbTextTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.BbCode.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for <see cref="BbText"/>.
    /// </summary>
    [TestClass]
    public class BbTextTest
    {
        /// <summary>
        /// Gets or sets TestContext.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Tests the <see cref="BbText.GetClosestParent{T}"/> method in a complex string.
        /// </summary>
        [TestMethod]
        public void GetClosestParentTest()
        {
            var parser = new BbParser();
            var root = parser.Parse("This [face=Arial]is [b]some [face=Courier]random[/face][/b] text[/face] with [i]different[/i] styles.");
            var texts = root.FindNodesOfType<BbText>().GetEnumerator();

            // "This "
            Assert.IsTrue(texts.MoveNext());
            var text = texts.Current;
            Assert.IsNull(text.GetClosestParent<Bold>());
            Assert.IsNull(text.GetClosestParent<Italic>());
            Assert.IsNull(text.GetClosestParent<Face>());

            // "is "
            Assert.IsTrue(texts.MoveNext());
            text = texts.Current;
            Assert.IsNull(text.GetClosestParent<Bold>());
            Assert.IsNull(text.GetClosestParent<Italic>());
            Assert.IsNotNull(text.GetClosestParent<Face>());
            Assert.AreEqual("Arial", text.GetClosestParent<Face>().FaceName);

            // "some "
            Assert.IsTrue(texts.MoveNext());
            text = texts.Current;
            Assert.IsNotNull(text.GetClosestParent<Bold>());
            Assert.IsNull(text.GetClosestParent<Italic>());
            Assert.IsNotNull(text.GetClosestParent<Face>());
            Assert.AreEqual("Arial", text.GetClosestParent<Face>().FaceName);

            // "random"
            Assert.IsTrue(texts.MoveNext());
            text = texts.Current;
            Assert.IsNotNull(text.GetClosestParent<Bold>());
            Assert.IsNull(text.GetClosestParent<Italic>());
            Assert.IsNotNull(text.GetClosestParent<Face>());
            Assert.AreEqual("Courier", text.GetClosestParent<Face>().FaceName);

            // " text"
            Assert.IsTrue(texts.MoveNext());
            text = texts.Current;
            Assert.IsNull(text.GetClosestParent<Bold>());
            Assert.IsNull(text.GetClosestParent<Italic>());
            Assert.IsNotNull(text.GetClosestParent<Face>());
            Assert.AreEqual("Arial", text.GetClosestParent<Face>().FaceName);

            // " with "
            Assert.IsTrue(texts.MoveNext());
            text = texts.Current;
            Assert.IsNull(text.GetClosestParent<Bold>());
            Assert.IsNull(text.GetClosestParent<Italic>());
            Assert.IsNull(text.GetClosestParent<Face>());

            // "different"
            Assert.IsTrue(texts.MoveNext());
            text = texts.Current;
            Assert.IsNull(text.GetClosestParent<Bold>());
            Assert.IsNotNull(text.GetClosestParent<Italic>());
            Assert.IsNull(text.GetClosestParent<Face>());

            // "styles."
            Assert.IsTrue(texts.MoveNext());
            text = texts.Current;
            Assert.IsNull(text.GetClosestParent<Bold>());
            Assert.IsNull(text.GetClosestParent<Italic>());
            Assert.IsNull(text.GetClosestParent<Face>());
        }
    }
}
