// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextFactoryTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextFactoryTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Tests.Text
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Motion.Infomedia.RendererBase.Text;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit test class for <see cref="SimpleTextFactory"/>.
    /// </summary>
    [TestClass]
    public class TextFactoryTest
    {
        /// <summary>
        /// Parses lines and verifies their contents.
        /// </summary>
        [TestMethod]
        public void ParseLinesTest()
        {
            const string Input = "Some [i]texts[/i] with[br]multiple[br][b]new[/b] lines.";
            var font = new Font { Face = "Arial", Color = "black", Height = 10, Italic = false, Weight = 100 };

            var target = new SimpleTextFactory();
            target.BoldWeight = 900;

            var alternatives = target.ParseAlternatives(Input, font);
            Assert.IsNull(alternatives.Interval);

            Assert.AreEqual(1, alternatives.Count);
            var alt = alternatives[0];
            Assert.IsFalse(alt.IsInverted);
            Assert.AreEqual(null, alt.VerticalAlignment);
            Assert.AreEqual(3, alt.Lines.Count);
            var line = alt.Lines[0];
            Assert.AreEqual(null, line.HorizontalAlignment);
            Assert.AreEqual(3, line.Parts.Count);
            CollectionAssert.AllItemsAreInstancesOfType(line.Parts, typeof(SimpleTextPart));
            Assert.AreEqual("Some ", ((SimpleTextPart)line.Parts[0]).Text);
            Assert.AreEqual("texts", ((SimpleTextPart)line.Parts[1]).Text);
            Assert.AreEqual(" with", ((SimpleTextPart)line.Parts[2]).Text);

            line = alt.Lines[1];
            Assert.AreEqual(null, line.HorizontalAlignment);
            Assert.AreEqual(1, line.Parts.Count);
            CollectionAssert.AllItemsAreInstancesOfType(line.Parts, typeof(SimpleTextPart));
            Assert.AreEqual("multiple", ((SimpleTextPart)line.Parts[0]).Text);

            line = alt.Lines[2];
            Assert.AreEqual(null, line.HorizontalAlignment);
            Assert.AreEqual(2, line.Parts.Count);
            CollectionAssert.AllItemsAreInstancesOfType(line.Parts, typeof(SimpleTextPart));
            Assert.AreEqual("new", ((SimpleTextPart)line.Parts[0]).Text);
            Assert.AreEqual(" lines.", ((SimpleTextPart)line.Parts[1]).Text);
        }

        /// <summary>
        /// Parses lines and verifies their alignment.
        /// </summary>
        [TestMethod]
        public void ParseAlignedLinesTest()
        {
            const string Input =
                "[align=Center]Some [i]texts[/i] with[/align][br][align=Right]multiple[/align][br][b]new[/b] lines.";
            var font = new Font { Face = "Arial", Color = "black", Height = 10, Italic = false, Weight = 100 };

            var target = new SimpleTextFactory();
            target.BoldWeight = 900;

            var alternatives = target.ParseAlternatives(Input, font);
            Assert.IsNull(alternatives.Interval);

            Assert.AreEqual(1, alternatives.Count);
            var alt = alternatives[0];
            Assert.IsFalse(alt.IsInverted);
            Assert.AreEqual(null, alt.VerticalAlignment);
            Assert.AreEqual(3, alt.Lines.Count);
            var line = alt.Lines[0];
            Assert.AreEqual(HorizontalAlignment.Center, line.HorizontalAlignment);
            Assert.AreEqual(3, line.Parts.Count);
            CollectionAssert.AllItemsAreInstancesOfType(line.Parts, typeof(SimpleTextPart));
            Assert.AreEqual("Some ", ((SimpleTextPart)line.Parts[0]).Text);
            Assert.AreEqual("texts", ((SimpleTextPart)line.Parts[1]).Text);
            Assert.AreEqual(" with", ((SimpleTextPart)line.Parts[2]).Text);

            line = alt.Lines[1];
            Assert.AreEqual(HorizontalAlignment.Right, line.HorizontalAlignment);
            Assert.AreEqual(1, line.Parts.Count);
            CollectionAssert.AllItemsAreInstancesOfType(line.Parts, typeof(SimpleTextPart));
            Assert.AreEqual("multiple", ((SimpleTextPart)line.Parts[0]).Text);

            line = alt.Lines[2];
            Assert.AreEqual(null, line.HorizontalAlignment);
            Assert.AreEqual(2, line.Parts.Count);
            CollectionAssert.AllItemsAreInstancesOfType(line.Parts, typeof(SimpleTextPart));
            Assert.AreEqual("new", ((SimpleTextPart)line.Parts[0]).Text);
            Assert.AreEqual(" lines.", ((SimpleTextPart)line.Parts[1]).Text);
        }

        /// <summary>
        /// Parses nested alternatives and and tests that the total number
        /// of alternatives is correct.
        /// </summary>
        [TestMethod]
        public void ParseAlternativesTest()
        {
            const string Input = "Some [a] alternating [|] texts [a] with [|] nested[/a] alts[/a].";
            var font = new Font { Face = "Arial", Color = "black", Height = 10, Italic = false, Weight = 100 };

            var target = new SimpleTextFactory();
            target.BoldWeight = 900;

            var alternatives = target.ParseAlternatives(Input, font);
            Assert.IsNull(alternatives.Interval);

            Assert.AreEqual(3, alternatives.Count);
            var alt = alternatives[0];
            Assert.IsFalse(alt.IsInverted);
            Assert.AreEqual(null, alt.VerticalAlignment);
            Assert.AreEqual(1, alt.Lines.Count);
            var line = alt.Lines[0];
            Assert.AreEqual(3, line.Parts.Count);
            CollectionAssert.AllItemsAreInstancesOfType(line.Parts, typeof(SimpleTextPart));
            Assert.AreEqual("Some ", ((SimpleTextPart)line.Parts[0]).Text);
            Assert.AreEqual(" alternating ", ((SimpleTextPart)line.Parts[1]).Text);
            Assert.AreEqual(".", ((SimpleTextPart)line.Parts[2]).Text);

            alt = alternatives[1];
            Assert.IsFalse(alt.IsInverted);
            Assert.AreEqual(null, alt.VerticalAlignment);
            Assert.AreEqual(1, alt.Lines.Count);
            line = alt.Lines[0];
            Assert.AreEqual(5, line.Parts.Count);
            CollectionAssert.AllItemsAreInstancesOfType(line.Parts, typeof(SimpleTextPart));
            Assert.AreEqual("Some ", ((SimpleTextPart)line.Parts[0]).Text);
            Assert.AreEqual(" texts ", ((SimpleTextPart)line.Parts[1]).Text);
            Assert.AreEqual(" with ", ((SimpleTextPart)line.Parts[2]).Text);
            Assert.AreEqual(" alts", ((SimpleTextPart)line.Parts[3]).Text);
            Assert.AreEqual(".", ((SimpleTextPart)line.Parts[4]).Text);

            alt = alternatives[2];
            Assert.IsFalse(alt.IsInverted);
            Assert.AreEqual(null, alt.VerticalAlignment);
            Assert.AreEqual(1, alt.Lines.Count);
            line = alt.Lines[0];
            Assert.AreEqual(5, line.Parts.Count);
            CollectionAssert.AllItemsAreInstancesOfType(line.Parts, typeof(SimpleTextPart));
            Assert.AreEqual("Some ", ((SimpleTextPart)line.Parts[0]).Text);
            Assert.AreEqual(" texts ", ((SimpleTextPart)line.Parts[1]).Text);
            Assert.AreEqual(" nested", ((SimpleTextPart)line.Parts[2]).Text);
            Assert.AreEqual(" alts", ((SimpleTextPart)line.Parts[3]).Text);
            Assert.AreEqual(".", ((SimpleTextPart)line.Parts[4]).Text);
        }

        /// <summary>
        /// Parses nested alternatives and and tests that the total number
        /// of alternatives is correct.
        /// </summary>
        [TestMethod]
        public void ParseAlternativesWithNewLinesTest()
        {
            const string Input =
                "Some [a] other[br]alternating [|] texts [a] with[br]multiple [|] nested[/a] alts[/a].";
            var font = new Font { Face = "Arial", Color = "black", Height = 10, Italic = false, Weight = 100 };

            var target = new SimpleTextFactory();
            target.BoldWeight = 900;

            var alternatives = target.ParseAlternatives(Input, font);
            Assert.IsNull(alternatives.Interval);

            Assert.AreEqual(3, alternatives.Count);
            var alt = alternatives[0];
            Assert.IsFalse(alt.IsInverted);
            Assert.AreEqual(null, alt.VerticalAlignment);
            Assert.AreEqual(2, alt.Lines.Count);
            var line = alt.Lines[0];
            Assert.AreEqual(2, line.Parts.Count);
            CollectionAssert.AllItemsAreInstancesOfType(line.Parts, typeof(SimpleTextPart));
            Assert.AreEqual("Some ", ((SimpleTextPart)line.Parts[0]).Text);
            Assert.AreEqual(" other", ((SimpleTextPart)line.Parts[1]).Text);

            line = alt.Lines[1];
            Assert.AreEqual(2, line.Parts.Count);
            CollectionAssert.AllItemsAreInstancesOfType(line.Parts, typeof(SimpleTextPart));
            Assert.AreEqual("alternating ", ((SimpleTextPart)line.Parts[0]).Text);
            Assert.AreEqual(".", ((SimpleTextPart)line.Parts[1]).Text);

            alt = alternatives[1];
            Assert.IsFalse(alt.IsInverted);
            Assert.AreEqual(null, alt.VerticalAlignment);
            Assert.AreEqual(2, alt.Lines.Count);
            line = alt.Lines[0];
            Assert.AreEqual(3, line.Parts.Count);
            CollectionAssert.AllItemsAreInstancesOfType(line.Parts, typeof(SimpleTextPart));
            Assert.AreEqual("Some ", ((SimpleTextPart)line.Parts[0]).Text);
            Assert.AreEqual(" texts ", ((SimpleTextPart)line.Parts[1]).Text);
            Assert.AreEqual(" with", ((SimpleTextPart)line.Parts[2]).Text);

            line = alt.Lines[1];
            Assert.AreEqual(3, line.Parts.Count);
            CollectionAssert.AllItemsAreInstancesOfType(line.Parts, typeof(SimpleTextPart));
            Assert.AreEqual("multiple ", ((SimpleTextPart)line.Parts[0]).Text);
            Assert.AreEqual(" alts", ((SimpleTextPart)line.Parts[1]).Text);
            Assert.AreEqual(".", ((SimpleTextPart)line.Parts[2]).Text);

            alt = alternatives[2];
            Assert.IsFalse(alt.IsInverted);
            Assert.AreEqual(null, alt.VerticalAlignment);
            Assert.AreEqual(1, alt.Lines.Count);
            line = alt.Lines[0];
            Assert.AreEqual(5, line.Parts.Count);
            CollectionAssert.AllItemsAreInstancesOfType(line.Parts, typeof(SimpleTextPart));
            Assert.AreEqual("Some ", ((SimpleTextPart)line.Parts[0]).Text);
            Assert.AreEqual(" texts ", ((SimpleTextPart)line.Parts[1]).Text);
            Assert.AreEqual(" nested", ((SimpleTextPart)line.Parts[2]).Text);
            Assert.AreEqual(" alts", ((SimpleTextPart)line.Parts[3]).Text);
            Assert.AreEqual(".", ((SimpleTextPart)line.Parts[4]).Text);
        }

        /// <summary>
        /// Parses nested alternatives with intervals and and tests that the total number
        /// of alternatives is correct.
        /// </summary>
        [TestMethod]
        public void ParseAlternativesWithIntervalTest()
        {
            const string Input = "Some [a=10] alternating [|] texts [a=7] with [|] nested[/a] alts[/a].";
            var font = new Font { Face = "Arial", Color = "black", Height = 10, Italic = false, Weight = 100 };

            var target = new SimpleTextFactory();
            target.BoldWeight = 900;

            var alternatives = target.ParseAlternatives(Input, font);
            Assert.AreEqual(TimeSpan.FromSeconds(10), alternatives.Interval);

            Assert.AreEqual(3, alternatives.Count);
            var alt = alternatives[0];
            Assert.IsFalse(alt.IsInverted);
            Assert.AreEqual(null, alt.VerticalAlignment);
            Assert.AreEqual(1, alt.Lines.Count);
            var line = alt.Lines[0];
            Assert.AreEqual(3, line.Parts.Count);
            CollectionAssert.AllItemsAreInstancesOfType(line.Parts, typeof(SimpleTextPart));
            Assert.AreEqual("Some ", ((SimpleTextPart)line.Parts[0]).Text);
            Assert.AreEqual(" alternating ", ((SimpleTextPart)line.Parts[1]).Text);
            Assert.AreEqual(".", ((SimpleTextPart)line.Parts[2]).Text);

            alt = alternatives[1];
            Assert.IsFalse(alt.IsInverted);
            Assert.AreEqual(null, alt.VerticalAlignment);
            Assert.AreEqual(1, alt.Lines.Count);
            line = alt.Lines[0];
            Assert.AreEqual(5, line.Parts.Count);
            CollectionAssert.AllItemsAreInstancesOfType(line.Parts, typeof(SimpleTextPart));
            Assert.AreEqual("Some ", ((SimpleTextPart)line.Parts[0]).Text);
            Assert.AreEqual(" texts ", ((SimpleTextPart)line.Parts[1]).Text);
            Assert.AreEqual(" with ", ((SimpleTextPart)line.Parts[2]).Text);
            Assert.AreEqual(" alts", ((SimpleTextPart)line.Parts[3]).Text);
            Assert.AreEqual(".", ((SimpleTextPart)line.Parts[4]).Text);

            alt = alternatives[2];
            Assert.IsFalse(alt.IsInverted);
            Assert.AreEqual(null, alt.VerticalAlignment);
            Assert.AreEqual(1, alt.Lines.Count);
            line = alt.Lines[0];
            Assert.AreEqual(5, line.Parts.Count);
            CollectionAssert.AllItemsAreInstancesOfType(line.Parts, typeof(SimpleTextPart));
            Assert.AreEqual("Some ", ((SimpleTextPart)line.Parts[0]).Text);
            Assert.AreEqual(" texts ", ((SimpleTextPart)line.Parts[1]).Text);
            Assert.AreEqual(" nested", ((SimpleTextPart)line.Parts[2]).Text);
            Assert.AreEqual(" alts", ((SimpleTextPart)line.Parts[3]).Text);
            Assert.AreEqual(".", ((SimpleTextPart)line.Parts[4]).Text);
        }

        /// <summary>
        /// Parses nested alternatives with some special formatting.
        /// </summary>
        [TestMethod]
        public void ParseInvertAlignAlternativesTest()
        {
            const string Input =
                "[inv]Some [a] alternating [|][valign=top] texts [/valign][a] with [|] nested[/a] alts[/a].[/inv]";
            var font = new Font { Face = "Arial", Color = "black", Height = 10, Italic = false, Weight = 100 };

            var target = new SimpleTextFactory();
            target.BoldWeight = 900;

            var alternatives = target.ParseAlternatives(Input, font);
            Assert.IsNull(alternatives.Interval);

            Assert.AreEqual(3, alternatives.Count);
            var alt = alternatives[0];
            Assert.IsTrue(alt.IsInverted);
            Assert.AreEqual(null, alt.VerticalAlignment);
            Assert.AreEqual(1, alt.Lines.Count);
            var line = alt.Lines[0];
            Assert.AreEqual(3, line.Parts.Count);
            CollectionAssert.AllItemsAreInstancesOfType(line.Parts, typeof(SimpleTextPart));
            Assert.AreEqual("Some ", ((SimpleTextPart)line.Parts[0]).Text);
            Assert.AreEqual(" alternating ", ((SimpleTextPart)line.Parts[1]).Text);
            Assert.AreEqual(".", ((SimpleTextPart)line.Parts[2]).Text);

            alt = alternatives[1];
            Assert.IsTrue(alt.IsInverted);
            Assert.AreEqual(VerticalAlignment.Top, alt.VerticalAlignment);
            Assert.AreEqual(1, alt.Lines.Count);
            line = alt.Lines[0];
            Assert.AreEqual(5, line.Parts.Count);
            CollectionAssert.AllItemsAreInstancesOfType(line.Parts, typeof(SimpleTextPart));
            Assert.AreEqual("Some ", ((SimpleTextPart)line.Parts[0]).Text);
            Assert.AreEqual(" texts ", ((SimpleTextPart)line.Parts[1]).Text);
            Assert.AreEqual(" with ", ((SimpleTextPart)line.Parts[2]).Text);
            Assert.AreEqual(" alts", ((SimpleTextPart)line.Parts[3]).Text);
            Assert.AreEqual(".", ((SimpleTextPart)line.Parts[4]).Text);

            alt = alternatives[2];
            Assert.IsTrue(alt.IsInverted);
            Assert.AreEqual(VerticalAlignment.Top, alt.VerticalAlignment);
            Assert.AreEqual(1, alt.Lines.Count);
            line = alt.Lines[0];
            Assert.AreEqual(5, line.Parts.Count);
            CollectionAssert.AllItemsAreInstancesOfType(line.Parts, typeof(SimpleTextPart));
            Assert.AreEqual("Some ", ((SimpleTextPart)line.Parts[0]).Text);
            Assert.AreEqual(" texts ", ((SimpleTextPart)line.Parts[1]).Text);
            Assert.AreEqual(" nested", ((SimpleTextPart)line.Parts[2]).Text);
            Assert.AreEqual(" alts", ((SimpleTextPart)line.Parts[3]).Text);
            Assert.AreEqual(".", ((SimpleTextPart)line.Parts[4]).Text);
        }

        /// <summary>
        /// Parses some formatting.
        /// </summary>
        [TestMethod]
        public void SimpleParseAlternativesTest()
        {
            const string Input = "Some [b]text[/b] with [i]tags[/i].";
            var font = new Font { Face = "Arial", Color = "black", Height = 10, Italic = false, Weight = 100 };

            var target = new SimpleTextFactory();
            target.BoldWeight = 900;

            var alternatives = target.ParseAlternatives(Input, font);

            Assert.AreEqual(1, alternatives.Count);

            // check texts
            var alt = alternatives[0];
            Assert.IsFalse(alt.IsInverted);
            Assert.AreEqual(null, alt.VerticalAlignment);
            Assert.AreEqual(1, alt.Lines.Count);
            var line = alt.Lines[0];
            Assert.AreEqual(5, line.Parts.Count);
            CollectionAssert.AllItemsAreInstancesOfType(line.Parts, typeof(SimpleTextPart));
            Assert.AreEqual("Some ", ((SimpleTextPart)line.Parts[0]).Text);
            Assert.AreEqual("text", ((SimpleTextPart)line.Parts[1]).Text);
            Assert.AreEqual(" with ", ((SimpleTextPart)line.Parts[2]).Text);
            Assert.AreEqual("tags", ((SimpleTextPart)line.Parts[3]).Text);
            Assert.AreEqual(".", ((SimpleTextPart)line.Parts[4]).Text);

            // check fonts
            var part = (SimpleTextPart)line.Parts[0];
            Assert.AreEqual("Arial", part.Font.Face);
            Assert.AreEqual("black", part.Font.Color);
            Assert.AreEqual(10, part.Font.Height);
            Assert.AreEqual(false, part.Font.Italic);
            Assert.AreEqual(100, part.Font.Weight);
            Assert.AreEqual(false, part.Blink);

            part = (SimpleTextPart)line.Parts[1];
            Assert.AreEqual("Arial", part.Font.Face);
            Assert.AreEqual("black", part.Font.Color);
            Assert.AreEqual(10, part.Font.Height);
            Assert.AreEqual(false, part.Font.Italic);
            Assert.AreEqual(900, part.Font.Weight);
            Assert.AreEqual(false, part.Blink);

            part = (SimpleTextPart)line.Parts[2];
            Assert.AreEqual("Arial", part.Font.Face);
            Assert.AreEqual("black", part.Font.Color);
            Assert.AreEqual(10, part.Font.Height);
            Assert.AreEqual(false, part.Font.Italic);
            Assert.AreEqual(100, part.Font.Weight);
            Assert.AreEqual(false, part.Blink);

            part = (SimpleTextPart)line.Parts[3];
            Assert.AreEqual("Arial", part.Font.Face);
            Assert.AreEqual("black", part.Font.Color);
            Assert.AreEqual(10, part.Font.Height);
            Assert.AreEqual(true, part.Font.Italic);
            Assert.AreEqual(100, part.Font.Weight);
            Assert.AreEqual(false, part.Blink);

            part = (SimpleTextPart)line.Parts[4];
            Assert.AreEqual("Arial", part.Font.Face);
            Assert.AreEqual("black", part.Font.Color);
            Assert.AreEqual(10, part.Font.Height);
            Assert.AreEqual(false, part.Font.Italic);
            Assert.AreEqual(100, part.Font.Weight);
            Assert.AreEqual(false, part.Blink);
        }

        /// <summary>
        /// Parses text with a lot of nested formatting but no alternatives.
        /// </summary>
        [TestMethod]
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Unit test method")]
        public void NestedTagsParseAlternativesTest()
        {
            const string Input = "This [face=Verdana]is [b]some [size=50%]random[/size][/b] text[/face] "
                + "[size=14]with[/size] [i]different[/i] [bl][color=red]sty[/color]les[/bl].";
            var font = new Font { Face = "Arial", Color = "black", Height = 10, Italic = false, Weight = 100 };

            var target = new SimpleTextFactory();
            target.BoldWeight = 900;

            var alternatives = target.ParseAlternatives(Input, font);

            Assert.AreEqual(1, alternatives.Count);

            // check texts
            var alt = alternatives[0];
            Assert.IsFalse(alt.IsInverted);
            Assert.AreEqual(null, alt.VerticalAlignment);
            Assert.AreEqual(1, alt.Lines.Count);
            var line = alt.Lines[0];
            Assert.AreEqual(13, line.Parts.Count);
            CollectionAssert.AllItemsAreInstancesOfType(line.Parts, typeof(SimpleTextPart));
            Assert.AreEqual("This ", ((SimpleTextPart)line.Parts[0]).Text);
            Assert.AreEqual("is ", ((SimpleTextPart)line.Parts[1]).Text);
            Assert.AreEqual("some ", ((SimpleTextPart)line.Parts[2]).Text);
            Assert.AreEqual("random", ((SimpleTextPart)line.Parts[3]).Text);
            Assert.AreEqual(" text", ((SimpleTextPart)line.Parts[4]).Text);
            Assert.AreEqual(" ", ((SimpleTextPart)line.Parts[5]).Text);
            Assert.AreEqual("with", ((SimpleTextPart)line.Parts[6]).Text);
            Assert.AreEqual(" ", ((SimpleTextPart)line.Parts[7]).Text);
            Assert.AreEqual("different", ((SimpleTextPart)line.Parts[8]).Text);
            Assert.AreEqual(" ", ((SimpleTextPart)line.Parts[9]).Text);
            Assert.AreEqual("sty", ((SimpleTextPart)line.Parts[10]).Text);
            Assert.AreEqual("les", ((SimpleTextPart)line.Parts[11]).Text);
            Assert.AreEqual(".", ((SimpleTextPart)line.Parts[12]).Text);

            // check fonts
            var part = (SimpleTextPart)line.Parts[0];
            Assert.AreEqual("Arial", part.Font.Face);
            Assert.AreEqual("black", part.Font.Color);
            Assert.AreEqual(10, part.Font.Height);
            Assert.AreEqual(false, part.Font.Italic);
            Assert.AreEqual(100, part.Font.Weight);
            Assert.AreEqual(false, part.Blink);

            part = (SimpleTextPart)line.Parts[1];
            Assert.AreEqual("Verdana", part.Font.Face);
            Assert.AreEqual("black", part.Font.Color);
            Assert.AreEqual(10, part.Font.Height);
            Assert.AreEqual(false, part.Font.Italic);
            Assert.AreEqual(100, part.Font.Weight);
            Assert.AreEqual(false, part.Blink);

            part = (SimpleTextPart)line.Parts[2];
            Assert.AreEqual("Verdana", part.Font.Face);
            Assert.AreEqual("black", part.Font.Color);
            Assert.AreEqual(10, part.Font.Height);
            Assert.AreEqual(false, part.Font.Italic);
            Assert.AreEqual(900, part.Font.Weight);
            Assert.AreEqual(false, part.Blink);

            part = (SimpleTextPart)line.Parts[3];
            Assert.AreEqual("Verdana", part.Font.Face);
            Assert.AreEqual("black", part.Font.Color);
            Assert.AreEqual(5, part.Font.Height);
            Assert.AreEqual(false, part.Font.Italic);
            Assert.AreEqual(900, part.Font.Weight);
            Assert.AreEqual(false, part.Blink);

            part = (SimpleTextPart)line.Parts[4];
            Assert.AreEqual("Verdana", part.Font.Face);
            Assert.AreEqual("black", part.Font.Color);
            Assert.AreEqual(10, part.Font.Height);
            Assert.AreEqual(false, part.Font.Italic);
            Assert.AreEqual(100, part.Font.Weight);
            Assert.AreEqual(false, part.Blink);

            part = (SimpleTextPart)line.Parts[5];
            Assert.AreEqual("Arial", part.Font.Face);
            Assert.AreEqual("black", part.Font.Color);
            Assert.AreEqual(10, part.Font.Height);
            Assert.AreEqual(false, part.Font.Italic);
            Assert.AreEqual(100, part.Font.Weight);
            Assert.AreEqual(false, part.Blink);

            part = (SimpleTextPart)line.Parts[6];
            Assert.AreEqual("Arial", part.Font.Face);
            Assert.AreEqual("black", part.Font.Color);
            Assert.AreEqual(14, part.Font.Height);
            Assert.AreEqual(false, part.Font.Italic);
            Assert.AreEqual(100, part.Font.Weight);
            Assert.AreEqual(false, part.Blink);

            part = (SimpleTextPart)line.Parts[7];
            Assert.AreEqual("Arial", part.Font.Face);
            Assert.AreEqual("black", part.Font.Color);
            Assert.AreEqual(10, part.Font.Height);
            Assert.AreEqual(false, part.Font.Italic);
            Assert.AreEqual(100, part.Font.Weight);
            Assert.AreEqual(false, part.Blink);

            part = (SimpleTextPart)line.Parts[8];
            Assert.AreEqual("Arial", part.Font.Face);
            Assert.AreEqual("black", part.Font.Color);
            Assert.AreEqual(10, part.Font.Height);
            Assert.AreEqual(true, part.Font.Italic);
            Assert.AreEqual(100, part.Font.Weight);
            Assert.AreEqual(false, part.Blink);

            part = (SimpleTextPart)line.Parts[9];
            Assert.AreEqual("Arial", part.Font.Face);
            Assert.AreEqual("black", part.Font.Color);
            Assert.AreEqual(10, part.Font.Height);
            Assert.AreEqual(false, part.Font.Italic);
            Assert.AreEqual(100, part.Font.Weight);
            Assert.AreEqual(false, part.Blink);

            part = (SimpleTextPart)line.Parts[10];
            Assert.AreEqual("Arial", part.Font.Face);
            Assert.AreEqual("red", part.Font.Color);
            Assert.AreEqual(10, part.Font.Height);
            Assert.AreEqual(false, part.Font.Italic);
            Assert.AreEqual(100, part.Font.Weight);
            Assert.AreEqual(true, part.Blink);

            part = (SimpleTextPart)line.Parts[11];
            Assert.AreEqual("Arial", part.Font.Face);
            Assert.AreEqual("black", part.Font.Color);
            Assert.AreEqual(10, part.Font.Height);
            Assert.AreEqual(false, part.Font.Italic);
            Assert.AreEqual(100, part.Font.Weight);
            Assert.AreEqual(true, part.Blink);

            part = (SimpleTextPart)line.Parts[12];
            Assert.AreEqual("Arial", part.Font.Face);
            Assert.AreEqual("black", part.Font.Color);
            Assert.AreEqual(10, part.Font.Height);
            Assert.AreEqual(false, part.Font.Italic);
            Assert.AreEqual(100, part.Font.Weight);
            Assert.AreEqual(false, part.Blink);
        }
    }
}
