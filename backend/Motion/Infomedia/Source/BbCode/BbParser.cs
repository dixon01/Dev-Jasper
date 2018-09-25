// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BbParser.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Motion.Infomedia.BbCode
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Parser that understands BBCode strings and parses them
    /// into a tree of <see cref="BbNode"/>s.
    /// </summary>
    public class BbParser
    {
        private bool insideBrackets;

        private bool insideEndTag;

        private StringBuilder currentText;

        /// <summary>
        /// Escapes brackets that would cause <see cref="BbParseException"/>s.
        /// </summary>
        /// <param name="input">
        /// The input string.
        /// </param>
        /// <returns>
        /// The output string with escaped brackets.
        /// </returns>
        /// <exception cref="ArgumentNullException">The input string is null.</exception>
        public static string EscapeBbCode(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            return input.Replace("[", "[[").Replace("]", "]]");
        }

        /// <summary>
        /// Parses an input BBCode text into a tree of <see cref="BbNode"/>s.
        /// </summary>
        /// <param name="input">
        /// The input BBCode text.
        /// </param>
        /// <returns>
        /// the root node of the created tree.
        /// </returns>
        /// <exception cref="BbParseException">
        /// If anything went wrong during parsing of the <see cref="input"/>.
        /// </exception>
        public BbRoot Parse(string input)
        {
            return this.Parse(input, new NullContext());
        }

        /// <summary>
        /// Parses an input BBCode text into a tree of <see cref="BbNode"/>s.
        /// </summary>
        /// <param name="input">
        /// The input BBCode text.
        /// </param>
        /// <param name="context">
        /// The context of this parser, can't be null!
        /// </param>
        /// <returns>
        /// the root node of the created tree.
        /// </returns>
        /// <exception cref="BbParseException">
        /// If anything went wrong during parsing of the <see cref="input"/>.
        /// </exception>
        public BbRoot Parse(string input, IBbParserContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var root = new BbRoot();
            var offset = 0;
            this.Parse(root, input, context, ref offset);
            if (offset != input.Length)
            {
                throw new BbParseException("Parser ended before EOS: " + offset);
            }

            return root;
        }

        /// <summary>
        /// Serializes the given root to a string.
        /// </summary>
        /// <param name="root">
        /// The root.
        /// </param>
        /// <returns>
        /// The serialized <see cref="string"/>.
        /// </returns>
        public string Serialize(BbRoot root)
        {
            using (var writer = new StringWriter())
            {
                this.SerializeChildren(root, writer);
                return writer.ToString();
            }
        }

        private void Parse(BbBranch parent, string input, IBbParserContext context, ref int offset)
        {
            this.insideBrackets = false;
            this.insideEndTag = false;
            this.currentText = new StringBuilder(input.Length);
            offset--; // makes the loop a bit easier
            while (++offset < input.Length)
            {
                char c = input[offset];
                if (c == '[')
                {
                    this.HandleOpenTag(parent, input, ref offset);
                }
                else if (c == ']')
                {
                    if (this.HandleCloseTag(parent, input, ref offset, context, c))
                    {
                        return;
                    }
                }
                else
                {
                    this.currentText.Append(c);
                }
            }

            if (this.insideBrackets)
            {
                throw new BbParseException("Missing tag closing ']' at EOS");
            }

            if (parent.Parent != null)
            {
                // if we get here and the parent is not a root, we did not get our parent's end tag
                throw new BbParseException("Missing closing tag at EOS for " + parent.GetType().Name);
            }

            if (this.currentText.Length > 0)
            {
                parent.Add(new BbText(parent, this.currentText.ToString()));
            }
        }

        private bool HandleCloseTag(BbBranch parent, string input, ref int offset, IBbParserContext context, char c)
        {
            if (!this.insideBrackets)
            {
                if (offset + 1 == input.Length)
                {
                    throw new BbParseException("Invalid tag closing ']' at EOS");
                }

                if (input[offset + 1] == ']')
                {
                    // double ]] --> means escaped ]
                    offset++;
                    this.currentText.Append(c);
                    return false;
                }

                throw new BbParseException("Invalid tag closing ']' without matching opening.");
            }

            // if (insideBrackets)
            if (offset + 1 < input.Length && input[offset + 1] == ']')
            {
                // double ]] --> means escaped ]
                offset++;
                this.currentText.Append(c);
                return false;
            }

            if (this.insideEndTag)
            {
                var parentTag = parent as BbTag;
                if (parentTag == null || parentTag.TagName != this.currentText.ToString())
                {
                    throw new BbParseException(
                        string.Format(
                            "End tag doesn't match start tag: got {0} instead of ending value for {1}",
                            this.currentText,
                            parent.GetType().Name));
                }

                // we reached our own end tag, let's return
                this.currentText.Length = 0;
                return true;
            }

            // we have a start tag
            var tag = this.CreateBranch(parent, this.currentText.ToString());
            this.currentText.Length = 0;
            if (!(tag is BbLeafTag))
            {
                // the tag is not a leaf, so let's parse its contents
                // when this method returns, the end tag has already
                // been read and verified
                offset++;
                this.Parse(tag, input, context, ref offset);
            }

            // call Cleanup() to give tags the chance to modify their
            // contents before adding it to the parent
            parent.Add(tag.Cleanup(context));
            this.insideBrackets = false;
            return false;
        }

        private void HandleOpenTag(BbBranch parent, string input, ref int offset)
        {
            // possible start of a tag
            if (offset + 1 == input.Length)
            {
                throw new BbParseException("Invalid tag opening '[' at EOS");
            }

            if (input[offset + 1] == '[')
            {
                // double [[ --> means escaped [
                this.currentText.Append('[');
                offset++;
                return;
            }

            if (this.insideBrackets)
            {
                throw new BbParseException("Invalid tag opening inside tag");
            }

            this.insideBrackets = true;
            if (this.currentText.Length > 0)
            {
                parent.Add(new BbText(parent, this.currentText.ToString()));
                this.currentText.Length = 0;
            }

            // [/ --> means this is an end tag
            this.insideEndTag = input[offset + 1] == '/';
            if (this.insideEndTag)
            {
                // skip over the '/'
                offset++;
            }
        }

        private BbBranch CreateBranch(BbBranch parent, string tagContents)
        {
            if (string.IsNullOrEmpty(tagContents))
            {
                throw new BbParseException("Empty tags are not supported");
            }

            // some tags have [tag=value], others only [tag]
            var index = tagContents.IndexOf('=');

            var tagName = index < 0 ? tagContents : tagContents.Substring(0, index);
            var value = index < 0 ? null : tagContents.Substring(index + 1);

            switch (tagName.ToLower())
            {
                case "|":
                    return new Alternating.Delimiter(parent, tagName);
                case "a":
                    return new Alternating(parent, tagName, value);
                case "align":
                    return new HorizontalAlign(parent, tagName, value);
                case "b":
                    return new Bold(parent, tagName);
                case "bl":
                    return new Blink(parent, tagName);
                case "br":
                    return new NewLine(parent, tagName);
                case "color":
                    return new Color(parent, tagName, value);
                case "face":
                    return new Face(parent, tagName, value);
                case "i":
                    return new Italic(parent, tagName);
                case "inv":
                    return new Invert(parent, tagName);
                case "img":
                    return new Image(parent, tagName, value);
                case "s":
                    throw new NotImplementedException("Strike-through is not yet implemented.");
                case "size":
                    return new Size(parent, tagName, value);
                case "time":
                    return new Time(parent, tagName, value);
                case "u":
                    throw new NotImplementedException("Underline is not yet implemented.");
                case "valign":
                    return new VerticalAlign(parent, tagName, value);
                case "vid":
                    return new Video(parent, tagName, value);
                default:
                    throw new BbParseException("Unsupported tag " + tagName);
            }
        }

        private void Serialize(BbNode node, TextWriter writer)
        {
            var text = node as BbText;
            if (text != null)
            {
                writer.Write(text.Text.Replace("[", "[[").Replace("]", "]]"));
                return;
            }

            var alt = node as Alternating;
            if (alt != null)
            {
                if (alt.IntervalSeconds.HasValue)
                {
                    this.WriteStartTag(writer, alt.TagName, alt.Value);
                }
                else
                {
                    this.WriteStartTag(writer, alt.TagName);
                }

                var first = true;
                foreach (Alternation child in alt.Children)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        this.WriteStartTag(writer, "|");
                    }

                    this.SerializeChildren(child, writer);
                }

                this.WriteEndTag(writer, alt.TagName);
                return;
            }

            var valueLeaf = node as BbLeafValueTag;
            if (valueLeaf != null)
            {
                this.WriteStartTag(writer, valueLeaf.TagName, valueLeaf.Value);
                return;
            }

            var leaf = node as BbLeafTag;
            if (leaf != null)
            {
                this.WriteStartTag(writer, leaf.TagName);
                return;
            }

            var valueTag = node as BbValueTag;
            if (valueTag != null)
            {
                this.WriteStartTag(writer, valueTag.TagName, valueTag.Value);
                this.SerializeChildren(valueTag, writer);
                this.WriteEndTag(writer, valueTag.TagName);
                return;
            }

            var tag = node as BbTag;
            if (tag != null)
            {
                this.WriteStartTag(writer, tag.TagName);
                this.SerializeChildren(tag, writer);
                this.WriteEndTag(writer, tag.TagName);
                return;
            }

            throw new BbParseException("Can't serialize unknown node " + node.GetType().Name);
        }

        private void SerializeChildren(BbBranch parent, TextWriter writer)
        {
            foreach (var child in parent.Children)
            {
                this.Serialize(child, writer);
            }
        }

        private void WriteStartTag(TextWriter writer, string tagName)
        {
            writer.Write('[');
            writer.Write(tagName);
            writer.Write(']');
        }

        private void WriteStartTag(TextWriter writer, string tagName, string value)
        {
            writer.Write('[');
            writer.Write(tagName);
            writer.Write('=');
            writer.Write(value);
            writer.Write(']');
        }

        private void WriteEndTag(TextWriter writer, string tagName)
        {
            writer.Write('[');
            writer.Write('/');
            writer.Write(tagName);
            writer.Write(']');
        }

        private class NullContext : IBbParserContext
        {
            /// <summary>
            /// The get absolute path related to config.
            /// </summary>
            /// <param name="filename">
            /// The filename.
            /// </param>
            /// <returns>
            /// The <see cref="string"/>.
            /// </returns>
            public string GetAbsolutePathRelatedToConfig(string filename)
            {
                return filename;
            }
        }
    }
}
