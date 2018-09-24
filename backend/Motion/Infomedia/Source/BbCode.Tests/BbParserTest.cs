// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BbParserTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BbParserTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.BbCode.Tests
{
    using System;

    using Gorba.Motion.Infomedia.BbCode;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for <see cref="BbParser"/>.
    /// </summary>
    [TestClass]
    public class BbParserTest
    {
        /// <summary>
        /// Gets or sets TestContext.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Parses a simple string.
        /// </summary>
        [TestMethod]
        public void SimpleParseTest()
        {
            var target = new BbParser();
            string input = "Hello World";

            var actual = target.Parse(input);
            Assert.AreEqual(input, target.Serialize(actual));

            Assert.IsNotNull(actual);
            var children = actual.Children.GetEnumerator();
            Assert.IsTrue(children.MoveNext());
            Assert.IsInstanceOfType(children.Current, typeof(BbText));
            var text = (BbText)children.Current;
            Assert.AreEqual(input, text.Text);
            Assert.IsFalse(children.MoveNext());
        }

        /// <summary>
        /// Parses a string with bold formatting.
        /// </summary>
        [TestMethod]
        public void BoldParseTest()
        {
            var target = new BbParser();
            string input = "[b]Hello World[/b]";

            var actual = target.Parse(input);
            Assert.AreEqual(input, target.Serialize(actual));

            Assert.IsNotNull(actual);
            var children = actual.Children.GetEnumerator();
            Assert.IsTrue(children.MoveNext());
            Assert.IsInstanceOfType(children.Current, typeof(Bold));
            var bold = (Bold)children.Current;
            Assert.IsFalse(children.MoveNext());

            children = bold.Children.GetEnumerator();
            Assert.IsTrue(children.MoveNext());
            Assert.IsInstanceOfType(children.Current, typeof(BbText));
            var text = (BbText)children.Current;
            Assert.AreEqual("Hello World", text.Text);
            Assert.IsFalse(children.MoveNext());
        }

        /// <summary>
        /// Parses a string with font face formatting.
        /// </summary>
        [TestMethod]
        public void FaceParseTest()
        {
            var target = new BbParser();
            string input = "[face=Times New Roman]Hello World[/face]";

            var actual = target.Parse(input);
            Assert.AreEqual(input, target.Serialize(actual));

            Assert.IsNotNull(actual);
            var children = actual.Children.GetEnumerator();
            Assert.IsTrue(children.MoveNext());
            Assert.IsInstanceOfType(children.Current, typeof(Face));
            var face = (Face)children.Current;
            Assert.AreEqual("Times New Roman", face.FaceName);
            Assert.IsFalse(children.MoveNext());

            children = face.Children.GetEnumerator();
            Assert.IsTrue(children.MoveNext());
            Assert.IsInstanceOfType(children.Current, typeof(BbText));
            var text = (BbText)children.Current;
            Assert.AreEqual("Hello World", text.Text);
            Assert.IsFalse(children.MoveNext());
        }

        /// <summary>
        /// Parses a string with alternating text.
        /// </summary>
        [TestMethod]
        public void AlternatingParseTest()
        {
            var target = new BbParser();
            string input = "[a]Hello[|]World[/a]";

            var actual = target.Parse(input);
            Assert.AreEqual(input, target.Serialize(actual));

            Assert.IsNotNull(actual);
            var children = actual.Children.GetEnumerator();
            Assert.IsTrue(children.MoveNext());
            Assert.IsInstanceOfType(children.Current, typeof(Alternating));
            var alt = (Alternating)children.Current;
            Assert.IsFalse(children.MoveNext());

            Assert.IsFalse(alt.IntervalSeconds.HasValue);

            var alts = alt.Children.GetEnumerator();
            Assert.IsTrue(alts.MoveNext());
            Assert.IsInstanceOfType(alts.Current, typeof(Alternation));
            var child = (Alternation)alts.Current;
            children = child.Children.GetEnumerator();
            Assert.IsTrue(children.MoveNext());
            Assert.IsInstanceOfType(children.Current, typeof(BbText));
            var text = (BbText)children.Current;
            Assert.AreEqual("Hello", text.Text);
            Assert.IsFalse(children.MoveNext());

            Assert.IsTrue(alts.MoveNext());
            Assert.IsInstanceOfType(alts.Current, typeof(Alternation));
            child = (Alternation)alts.Current;
            children = child.Children.GetEnumerator();
            Assert.IsTrue(children.MoveNext());
            Assert.IsInstanceOfType(children.Current, typeof(BbText));
            text = (BbText)children.Current;
            Assert.AreEqual("World", text.Text);
            Assert.IsFalse(children.MoveNext());
            Assert.IsFalse(alts.MoveNext());
        }

        /// <summary>
        /// Parses a string with alternating text that has an interval set.
        /// </summary>
        [TestMethod]
        public void AlternatingWithIntervalParseTest()
        {
            var target = new BbParser();
            string input = "[a=10]Hello[|]World[/a]";

            var actual = target.Parse(input);
            Assert.AreEqual(input, target.Serialize(actual));

            Assert.IsNotNull(actual);
            var children = actual.Children.GetEnumerator();
            Assert.IsTrue(children.MoveNext());
            Assert.IsInstanceOfType(children.Current, typeof(Alternating));
            var alt = (Alternating)children.Current;
            Assert.IsFalse(children.MoveNext());

            Assert.AreEqual(10, alt.IntervalSeconds);

            var alts = alt.Children.GetEnumerator();
            Assert.IsTrue(alts.MoveNext());
            Assert.IsInstanceOfType(alts.Current, typeof(Alternation));
            var child = (Alternation)alts.Current;
            children = child.Children.GetEnumerator();
            Assert.IsTrue(children.MoveNext());
            Assert.IsInstanceOfType(children.Current, typeof(BbText));
            var text = (BbText)children.Current;
            Assert.AreEqual("Hello", text.Text);
            Assert.IsFalse(children.MoveNext());

            Assert.IsTrue(alts.MoveNext());
            Assert.IsInstanceOfType(alts.Current, typeof(Alternation));
            child = (Alternation)alts.Current;
            children = child.Children.GetEnumerator();
            Assert.IsTrue(children.MoveNext());
            Assert.IsInstanceOfType(children.Current, typeof(BbText));
            text = (BbText)children.Current;
            Assert.AreEqual("World", text.Text);
            Assert.IsFalse(children.MoveNext());
            Assert.IsFalse(alts.MoveNext());
        }

        /// <summary>
        /// Parses a string with escaped tag starts and ends.
        /// </summary>
        [TestMethod]
        public void EscapesParseTest()
        {
            var target = new BbParser();
            string input = "Hello [[modern]] World";

            var actual = target.Parse(input);
            Assert.AreEqual(input, target.Serialize(actual));

            Assert.IsNotNull(actual);
            var children = actual.Children.GetEnumerator();
            Assert.IsTrue(children.MoveNext());
            Assert.IsInstanceOfType(children.Current, typeof(BbText));
            var text = (BbText)children.Current;
            Assert.AreEqual("Hello [modern] World", text.Text);
            Assert.IsFalse(children.MoveNext());
        }

        /// <summary>
        /// Parses a string with missing end tag.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(BbParseException))]
        public void NoClosingTagTest()
        {
            var target = new BbParser();
            string input = "[b]Hello World";

            target.Parse(input);
        }

        /// <summary>
        /// Parses a string with an empty tag name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(BbParseException))]
        public void EmptyTagTest()
        {
            var target = new BbParser();
            string input = "[]Hello World[/]";

            target.Parse(input);
        }

        /// <summary>
        /// Parses a string with an unknown tag name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(BbParseException))]
        public void UnknownTagTest()
        {
            var target = new BbParser();
            string input = "[hello]Hello World[/hello]";

            target.Parse(input);
        }

        /// <summary>
        /// Parses a string with the wrong end tag.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(BbParseException))]
        public void WrongClosingTagTest()
        {
            var target = new BbParser();
            string input = "[b]Hello World[/i]";

            target.Parse(input);
        }

        /// <summary>
        /// Parses a string with a tag within a tag.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(BbParseException))]
        public void TagInTagTest()
        {
            var target = new BbParser();
            string input = "[face=[test]Hello World[/face]";

            target.Parse(input);
        }

        /// <summary>
        /// Parses a string with closing tags in the wrong order.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(BbParseException))]
        public void WrongNestingTagTest()
        {
            var target = new BbParser();
            string input = "[b][i]Hello World[/b][/i]";

            target.Parse(input);
        }

        /// <summary>
        /// Parses a string with an incomplete closing tag.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(BbParseException))]
        public void BadClosingTagTest()
        {
            var target = new BbParser();
            string input = "[b]Hello World[/b";

            target.Parse(input);
        }

        /// <summary>
        /// Parses a string with an incomplete closing tag.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(BbParseException))]
        public void BadClosingTagTest2()
        {
            var target = new BbParser();
            string input = "[b]Hello World[/";

            target.Parse(input);
        }

        /// <summary>
        /// Parses a string with an incomplete closing tag.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(BbParseException))]
        public void BadClosingTagTest3()
        {
            var target = new BbParser();
            string input = "[b]Hello World[";

            target.Parse(input);
        }

        /// <summary>
        /// Parses a string with a missing opening tag.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(BbParseException))]
        public void NoOpeningTagTest()
        {
            var target = new BbParser();
            string input = "Hello World[/b]";

            target.Parse(input);
        }

        /// <summary>
        /// Parses a string with an non-escaped opening tag.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(BbParseException))]
        public void NotEscapedOpeningTagTest()
        {
            var target = new BbParser();
            string input = "[Hello World";

            target.Parse(input);
        }

        /// <summary>
        /// Parses a string with an non-escaped closing tag.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(BbParseException))]
        public void NotEscapedClosingTagTest()
        {
            var target = new BbParser();
            string input = "Hello ]World";

            target.Parse(input);
        }

        /// <summary>
        /// Parses a string with an non-escaped closing tag at the end of a string.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(BbParseException))]
        public void NotEscapedClosingTagAtEosTest()
        {
            var target = new BbParser();
            string input = "Hello World]";

            target.Parse(input);
        }

        /// <summary>
        /// A string without brackets is returned as it is.
        /// </summary>
        [TestMethod]
        public void EscapeStringWithoutTags()
        {
            var input = "Hello World";
            var output = BbParser.EscapeBbCode(input);
            Assert.AreEqual(input, output);
        }

        /// <summary>
        /// An empty string returns again an empty string.
        /// </summary>
        [TestMethod]
        public void EscapeEmptyString()
        {
            var output = BbParser.EscapeBbCode(string.Empty);
            Assert.AreEqual(string.Empty, output);
        }

        /// <summary>
        /// The escape null string.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EscapeNullString()
        {
            BbParser.EscapeBbCode(null);
        }

        /// <summary>
        /// Verifies that a valid tag is escaped.
        /// </summary>
        [TestMethod]
        public void EscapeValidTag()
        {
            const string Input = @"Hello World with a [b]bold[/b] text to show";
            var output = BbParser.EscapeBbCode(Input);
            const string ExpectedOutput = @"Hello World with a [[b]]bold[[/b]] text to show";
            Assert.AreEqual(ExpectedOutput, output);
        }

        /// <summary>
        /// Verifies that a simple tag is escaped.
        /// </summary>
        [TestMethod]
        public void EscapeValidSimpleTag()
        {
            const string Input = @"Hello World with an [img=images\testpath.jpg] to show";
            var output = BbParser.EscapeBbCode(Input);
            const string ExpectedOutput = @"Hello World with an [[img=images\testpath.jpg]] to show";
            Assert.AreEqual(ExpectedOutput, output);
        }

        /// <summary>
        /// Verifies that unpaired brackets are also escaped.
        /// </summary>
        [TestMethod]
        public void EscapePartialTag()
        {
            const string Input = @"Hello partial tag ]";
            var output = BbParser.EscapeBbCode(Input);
            const string ExpectedOutput = @"Hello partial tag ]]";
            Assert.AreEqual(ExpectedOutput, output);
        }
    }
}