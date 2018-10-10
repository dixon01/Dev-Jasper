// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParagraphXmlWriter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ParagraphXmlWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Tools.XmlDocumentor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using System.Xml.Schema;

    using DocumentFormat.OpenXml;
    using DocumentFormat.OpenXml.Wordprocessing;

    using NLog;

    /// <summary>
    /// <see cref="XmlWriter"/> implementation that writes to a <see cref="Paragraph"/>.
    /// </summary>
    internal class ParagraphXmlWriter : XmlWriter
    {
        private const string DefaultFontFace = "Consolas";
        private const string DefaultFontSize = "16";

        private const string BracketColor = "0000FF";
        private const string ElementNameColor = "A31515";
        private const string AttributeNameColor = "FF0000";
        private const string AttributeAsignColor = "0000FF";
        private const string AttributeQuoteColor = "000000";
        private const string AttributeValueColor = "0000FF";
        private const string TextColor = "000000";
        private const string CommentColor = "006600";

        private const int Indent = 2;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Paragraph paragraph;

        private readonly Action<Paragraph, string, XmlSchemaType, bool, Func<bool, Run>> linkWriter;

        private readonly XmlSchemaValidator validator;

        private readonly Stack<string> currentElement = new Stack<string>();

        private WriteState writeState;

        private bool validateAttributes = true;

        private bool writeBreak;

        private string currentAttributeName;
        private string currentAttributeNs;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParagraphXmlWriter"/> class.
        /// </summary>
        /// <param name="paragraph">
        /// The paragraph.
        /// </param>
        /// <param name="schemaSet">
        /// The schema set.
        /// </param>
        /// <param name="linkWriter">
        /// The method that is called to append a link for a given type to a paragraph.
        /// </param>
        public ParagraphXmlWriter(
            Paragraph paragraph,
            XmlSchemaSet schemaSet,
            Action<Paragraph, string, XmlSchemaType, bool, Func<bool, Run>> linkWriter)
        {
            this.paragraph = paragraph;
            this.linkWriter = linkWriter;

            this.paragraph.AppendChild(new ParagraphProperties(this.CreateRunProperties(TextColor, false)));

            var nameTable = new NameTable();
            var manager = new XmlNamespaceManager(nameTable);

            this.validator = new XmlSchemaValidator(nameTable, schemaSet, manager, XmlSchemaValidationFlags.None);
            this.validator.ValidationEventHandler += this.ValidatorOnValidationEventHandler;
            this.validator.Initialize();

            this.writeState = WriteState.Start;
        }

        /// <summary>
        /// When overridden in a derived class, gets the state of the writer.
        /// </summary>
        /// <returns>
        /// One of the <see cref="T:System.Xml.WriteState"/> values.
        /// </returns>
        public override WriteState WriteState
        {
            get
            {
                return this.writeState;
            }
        }

        /// <summary>
        /// When overridden in a derived class, writes the XML declaration with the version "1.0".
        /// </summary>
        public override void WriteStartDocument()
        {
        }

        /// <summary>
        /// When overridden in a derived class, writes the XML declaration with the
        /// version "1.0" and the standalone attribute.
        /// </summary>
        /// <param name="standalone">If true, it writes "standalone=yes"; if false, it writes "standalone=no". </param>
        public override void WriteStartDocument(bool standalone)
        {
        }

        /// <summary>
        /// When overridden in a derived class, closes any open elements or attributes and
        /// puts the writer back in the Start state.
        /// </summary>
        public override void WriteEndDocument()
        {
        }

        /// <summary>
        /// When overridden in a derived class, writes the DOCTYPE declaration with the specified name
        /// and optional attributes.
        /// </summary>
        /// <param name="name">The name of the DOCTYPE. This must be non-empty. </param>
        /// <param name="pubid">
        /// If non-null it also writes PUBLIC "<c>pubid</c>" "<c>sysid</c>" where <paramref name="pubid"/>
        /// and <paramref name="sysid"/> are replaced with the value of the given arguments.</param>
        /// <param name="sysid">
        /// If <paramref name="pubid"/> is null and <paramref name="sysid"/> is non-null
        /// it writes SYSTEM "<c>sysid</c>" where <paramref name="sysid"/> is replaced with the value of this argument.
        /// </param>
        /// <param name="subset">
        /// If non-null it writes [subset] where subset is replaced with the value of this argument.
        /// </param>
        public override void WriteDocType(string name, string pubid, string sysid, string subset)
        {
        }

        /// <summary>
        /// When overridden in a derived class, writes the specified start tag and associates it with
        /// the given namespace and prefix.
        /// </summary>
        /// <param name="prefix">The namespace prefix of the element. </param>
        /// <param name="localName">The local name of the element. </param>
        /// <param name="ns">The namespace URI to associate with the element. </param>
        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            this.HandlePossibleEndOfElement(true);

            var info = new XmlSchemaInfo();
            this.validator.ValidateElement(localName, ns, info);

            this.AppendRun("<", BracketColor);
            this.AppendLinkRun(localName, info.SchemaType, ElementNameColor);
            this.currentElement.Push(localName);
            this.writeState = WriteState.Element;
        }

        /// <summary>
        /// When overridden in a derived class, closes one element and pops the corresponding namespace scope.
        /// </summary>
        public override void WriteEndElement()
        {
            if (this.validateAttributes)
            {
                this.validator.ValidateEndOfAttributes(null);
            }

            this.validator.ValidateEndElement(null);

            this.AppendRun(" />", BracketColor, true);
            this.currentElement.Pop();
            this.writeState = WriteState.Content;
        }

        /// <summary>
        /// When overridden in a derived class, closes one element and pops the corresponding namespace scope.
        /// </summary>
        public override void WriteFullEndElement()
        {
            var info = new XmlSchemaInfo();
            this.validator.ValidateEndElement(info);

            var elementName = this.currentElement.Pop();
            this.AppendRun("</", BracketColor);
            this.AppendLinkRun(elementName, info.SchemaType, ElementNameColor);
            this.AppendRun(">", BracketColor, true);
            this.writeState = WriteState.Content;
        }

        /// <summary>
        /// When overridden in a derived class, writes the start of an attribute with the specified prefix,
        /// local name, and namespace URI.
        /// </summary>
        /// <param name="prefix">The namespace prefix of the attribute. </param>
        /// <param name="localName">The local name of the attribute. </param>
        /// <param name="ns">The namespace URI for the attribute. </param>
        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            this.AppendRun(" ", AttributeNameColor);
            if (this.validateAttributes)
            {
                this.currentAttributeName = localName;
                this.currentAttributeNs = ns;
                var attribute = this.validator.GetExpectedAttributes().FirstOrDefault(a => a.Name == localName);
                if (attribute == null)
                {
                    this.AppendRun(localName, AttributeNameColor);
                }
                else
                {
                    this.AppendLinkRun(localName, attribute.AttributeSchemaType, AttributeNameColor);
                }
            }
            else
            {
                this.AppendRun(localName, AttributeNameColor);
            }

            this.AppendRun("=", AttributeAsignColor);
            this.AppendRun("\"", AttributeQuoteColor);
            this.writeState = WriteState.Attribute;
        }

        /// <summary>
        /// When overridden in a derived class, closes the previous
        /// <see cref="M:System.Xml.XmlWriter.WriteStartAttribute(System.String,System.String)"/> call.
        /// </summary>
        public override void WriteEndAttribute()
        {
            this.AppendRun("\"", AttributeQuoteColor);
            this.writeState = WriteState.Element;
        }

        /// <summary>
        /// When overridden in a derived class, writes out a &lt;![CDATA[...]]&gt; block containing the specified text.
        /// </summary>
        /// <param name="text">The text to place inside the CDATA block. </param>
        public override void WriteCData(string text)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// When overridden in a derived class, writes out a comment &lt;!--...--&gt; containing the specified text.
        /// </summary>
        /// <param name="text">Text to place inside the comment. </param>
        public override void WriteComment(string text)
        {
            this.HandlePossibleEndOfElement(true);
            this.AppendRun("<!--" + text + "-->", CommentColor, true);
        }

        /// <summary>
        /// When overridden in a derived class, writes out a processing instruction with a space between
        /// the name and text as follows: &lt;?name text?&gt;.
        /// </summary>
        /// <param name="name">The name of the processing instruction. </param>
        /// <param name="text">The text to include in the processing instruction. </param>
        public override void WriteProcessingInstruction(string name, string text)
        {
            this.AppendRun("<?", BracketColor);
            this.AppendRun(name, ElementNameColor);

            var parts = text.Split(' ', '=', '"');
            for (int i = 0; i + 3 < parts.Length; i += 4)
            {
                this.validateAttributes = false;
                this.WriteStartAttribute(parts[i]);
                this.WriteString(parts[i + 2]);
                this.WriteEndAttribute();
                this.validateAttributes = true;
            }

            this.AppendRun("?>", BracketColor, true);
            this.writeState = WriteState.Prolog;
        }

        /// <summary>
        /// When overridden in a derived class, writes out an entity reference as &amp;name;.
        /// </summary>
        /// <param name="name">The name of the entity reference. </param>
        public override void WriteEntityRef(string name)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// When overridden in a derived class, forces the generation of a character entity
        /// for the specified Unicode character value.
        /// </summary>
        /// <param name="ch">The Unicode character for which to generate a character entity. </param>
        public override void WriteCharEntity(char ch)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// When overridden in a derived class, writes out the given white space.
        /// </summary>
        /// <param name="ws">The string of white space characters. </param>
        public override void WriteWhitespace(string ws)
        {
            this.HandlePossibleEndOfElement(false);
            this.AppendRun(ws, TextColor);
        }

        /// <summary>
        /// When overridden in a derived class, writes the given text content.
        /// </summary>
        /// <param name="text">The text to write. </param>
        public override void WriteString(string text)
        {
            if (this.writeState == WriteState.Element)
            {
                this.HandlePossibleEndOfElement(false);

                this.validator.ValidateText(text);

                this.AppendRun(text, TextColor);
                this.writeState = WriteState.Content;
            }
            else
            {
                if (this.validateAttributes)
                {
                    this.validator.ValidateAttribute(this.currentAttributeName, this.currentAttributeNs, text, null);
                }

                this.AppendRun(text, AttributeValueColor);
            }
        }

        /// <summary>
        /// When overridden in a derived class, generates and writes the surrogate character entity for the
        /// surrogate character pair.
        /// </summary>
        /// <param name="lowChar">The low surrogate. This must be a value between 0xDC00 and 0xDFFF. </param>
        /// <param name="highChar">The high surrogate. This must be a value between 0xD800 and 0xDBFF. </param>
        public override void WriteSurrogateCharEntity(char lowChar, char highChar)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// When overridden in a derived class, writes text one buffer at a time.
        /// </summary>
        /// <param name="buffer">Character array containing the text to write. </param>
        /// <param name="index">The position in the buffer indicating the start of the text to write. </param>
        /// <param name="count">The number of characters to write. </param>
        public override void WriteChars(char[] buffer, int index, int count)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// When overridden in a derived class, writes raw markup manually from a character buffer.
        /// </summary>
        /// <param name="buffer">Character array containing the text to write. </param>
        /// <param name="index">The position within the buffer indicating the start of the text to write. </param>
        /// <param name="count">The number of characters to write. </param>
        public override void WriteRaw(char[] buffer, int index, int count)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// When overridden in a derived class, writes raw markup manually from a string.
        /// </summary>
        /// <param name="data">String containing the text to write. </param>
        public override void WriteRaw(string data)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// When overridden in a derived class, encodes the specified binary bytes as Base64
        /// and writes out the resulting text.
        /// </summary>
        /// <param name="buffer">Byte array to encode. </param>
        /// <param name="index">The position in the buffer indicating the start of the bytes to write. </param>
        /// <param name="count">The number of bytes to write. </param>
        public override void WriteBase64(byte[] buffer, int index, int count)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// When overridden in a derived class, closes this stream and the underlying stream.
        /// </summary>
        public override void Close()
        {
            this.writeState = WriteState.Closed;
            try
            {
                this.validator.EndValidation();
            }
            catch (Exception ex)
            {
                Logger.WarnException("Couldn't end validation", ex);
            }
        }

        /// <summary>
        /// When overridden in a derived class, flushes whatever is in the buffer to the underlying streams
        /// and also flushes the underlying stream.
        /// </summary>
        public override void Flush()
        {
        }

        /// <summary>
        /// When overridden in a derived class, returns the closest prefix defined in the current
        /// namespace scope for the namespace URI.
        /// </summary>
        /// <returns>
        /// The matching prefix or null if no matching namespace URI is found in the current scope.
        /// </returns>
        /// <param name="ns">The namespace URI whose prefix you want to find. </param>
        public override string LookupPrefix(string ns)
        {
            throw new NotSupportedException();
        }

        private void HandlePossibleEndOfElement(bool addBreak)
        {
            if (this.writeState != WriteState.Element)
            {
                return;
            }

            if (this.validateAttributes)
            {
                this.validator.ValidateEndOfAttributes(null);
            }

            this.AppendRun(">", BracketColor, addBreak);
            this.writeState = WriteState.Content;
        }

        private void AppendRun(string text, string color, bool addBreak = false)
        {
            var run = this.paragraph.AppendChild(new Run());
            var props = this.CreateRunProperties(color, false);
            run.AppendChild(props);

            if (this.writeBreak)
            {
                run.AppendChild(new Break());
                text = new string(' ', this.currentElement.Count * Indent) + text;
            }

            run.AppendChild(new Text(text) { Space = SpaceProcessingModeValues.Preserve });

            this.writeBreak = addBreak;
        }

        private void AppendLinkRun(string text, XmlSchemaType type, string color)
        {
            this.linkWriter(this.paragraph, text, type, false, link => new Run(this.CreateRunProperties(color, link)));
        }

        private RunProperties CreateRunProperties(string color, bool underline)
        {
            var props = new RunProperties();
            props.Color = new Color { Val = color };
            props.FontSize = new FontSize { Val = DefaultFontSize };
            props.FontSizeComplexScript = new FontSizeComplexScript { Val = DefaultFontSize };
            props.RunFonts = new RunFonts
                                 {
                                     Ascii = DefaultFontFace,
                                     HighAnsi = DefaultFontFace,
                                     EastAsia = DefaultFontFace,
                                     ComplexScript = DefaultFontFace
                                 };
            if (underline)
            {
                props.Underline = new Underline { Val = UnderlineValues.Single };
            }

            return props;
        }

        private void ValidatorOnValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Exception != null)
            {
                throw e.Exception;
            }
        }
    }
}