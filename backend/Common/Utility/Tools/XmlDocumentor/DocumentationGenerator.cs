// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentationGenerator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DocumentationGenerator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Tools.XmlDocumentor
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.Schema;

    using DocumentFormat.OpenXml;
    using DocumentFormat.OpenXml.Wordprocessing;

    using NLog;

    /// <summary>
    /// The class tasked to generate the documentation for one XSD file (plus a possible sample XML file).
    /// </summary>
    public class DocumentationGenerator
    {
        private const string CaptionStyle = "Caption";
        private const string TableStyle = "TableGrid";

        private static readonly Regex HeadingStyleRegex = new Regex(@"^(.+?)(\d+)$");
        private static readonly Regex CleanupDocRegex = new Regex(@"\s+");

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly int[] TableColumnWidth = { 2444, 641, 2835, 3857 };

        private static int nextHyperlinkIndex;

        private readonly string xsdFile;

        private readonly string xmlFile;

        private readonly Dictionary<XmlSchemaType, XmlSchemaElement> autoListOfElements =
            new Dictionary<XmlSchemaType, XmlSchemaElement>();

        private readonly Dictionary<XmlSchemaType, IEnumerable<XmlSchemaElement>> autoListOfChoiceElements =
            new Dictionary<XmlSchemaType, IEnumerable<XmlSchemaElement>>();

        private readonly Dictionary<string, string> hyperlinks = new Dictionary<string, string>();

        private XmlSchemaSet schemaSet;

        private string headingPrefix;
        private int topHeadingIndex;

        private OpenXmlCompositeElement currentElement;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentationGenerator"/> class.
        /// </summary>
        /// <param name="xsdFile">
        /// The path to an XSD file.
        /// </param>
        /// <param name="xmlFile">
        /// The path to an XML file.
        /// This XML file will first be validated against the XSD and then used as an example XML.
        /// This can be null.
        /// </param>
        public DocumentationGenerator(string xsdFile, string xmlFile)
        {
            this.xsdFile = xsdFile;
            this.xmlFile = xmlFile;
        }

        private enum CaptionType
        {
            Figure,
            Table
        }

        /// <summary>
        /// Generates the chapters of the documentation starting with the given paragraph.
        /// </summary>
        /// <param name="paragraph">
        /// The paragraph. This should always be a heading in the word document.
        /// </param>
        /// <returns>
        /// The last <see cref="OpenXmlCompositeElement"/> added to the document.
        /// </returns>
        public OpenXmlCompositeElement GenerateChapters(Paragraph paragraph)
        {
            this.currentElement = paragraph;
            this.schemaSet = new XmlSchemaSet();
            using (var reader = File.OpenText(this.xsdFile))
            {
                this.schemaSet.Add(XmlSchema.Read(reader, null));
            }

            this.schemaSet.Compile();

            var allTypes = this.schemaSet.GlobalTypes.Values.Cast<XmlSchemaType>().ToList();
            var displayTypes = new List<XmlSchemaType>();
            foreach (XmlSchemaElement element in this.schemaSet.GlobalElements.Values)
            {
                this.CollectChapters(element.ElementSchemaType, true, displayTypes, allTypes);
            }

            paragraph.GetOrAppendChild<ParagraphProperties>().KeepNext = new KeepNext();
            var paragraphStyle = paragraph.Descendants<ParagraphStyleId>().FirstOrDefault();
            if (paragraphStyle != null)
            {
                var match = HeadingStyleRegex.Match(paragraphStyle.Val);
                if (match.Success && int.TryParse(match.Groups[2].Value, out this.topHeadingIndex))
                {
                    this.headingPrefix = match.Groups[1].Value;
                }
            }

            if (!string.IsNullOrEmpty(this.xmlFile))
            {
                if (!File.Exists(this.xmlFile))
                {
                    Logger.Warn("xml parameter is not pointing to an existing file, ignoring it: {0}", this.xmlFile);
                }
                else
                {
                    if (this.GenerateExampleConfig())
                    {
                        this.currentElement =
                            this.currentElement.InsertAfterSelf(
                                new Paragraph(paragraph.ParagraphProperties.CloneNode(true)));
                    }
                }
            }

            this.GenerateXmlStructure(displayTypes);
            return this.currentElement;
        }

        private static Text CreateText(string text)
        {
            return new Text(text) { Space = SpaceProcessingModeValues.Preserve };
        }

        private static Paragraph CreateKeepNextParagraph()
        {
            return new Paragraph { ParagraphProperties = new ParagraphProperties { KeepNext = new KeepNext() } };
        }

        private static IEnumerable<string> SplitPascalCase(string text)
        {
            var parts = new List<string>();
            var currentText = new StringBuilder();
            var wasUpper = true;
            var wasNonLetterOrDigit = false;
            foreach (var c in text)
            {
                if (!char.IsLetterOrDigit(c))
                {
                    wasNonLetterOrDigit = true;
                }
                else if (wasNonLetterOrDigit)
                {
                    wasNonLetterOrDigit = false;
                    wasUpper = true;
                    parts.Add(currentText.ToString());
                    currentText.Clear();
                }
                else if (!char.IsUpper(c))
                {
                    wasUpper = false;
                }
                else if (!wasUpper)
                {
                    parts.Add(currentText.ToString());
                    currentText.Clear();
                }

                currentText.Append(c);
            }

            if (currentText.Length > 0)
            {
                parts.Add(currentText.ToString());
            }

            return parts;
        }

        private bool GenerateExampleConfig()
        {
            var readerSettings = new XmlReaderSettings { Schemas = this.schemaSet, IgnoreWhitespace = true };
            var valid = true;
            readerSettings.ValidationEventHandler += (sender, args) =>
                {
                    LogLevel logLevel;
                    if (args.Severity == XmlSeverityType.Error)
                    {
                        logLevel = LogLevel.Error;
                        valid = false;
                    }
                    else
                    {
                        logLevel = LogLevel.Warn;
                    }

                    Logger.LogException(logLevel, args.Message, args.Exception);
                };

            var doc = new XmlDocument();
            using (var reader = XmlReader.Create(this.xmlFile, readerSettings))
            {
                doc.Load(reader);
                if (!valid)
                {
                    Logger.Error("Not creating example config chapter because XML structure is not valid");
                    return false;
                }
            }

            this.currentElement.RemoveAllChildren<Run>();
            var heading = this.currentElement.AppendChild(new Run());
            heading.AppendChild(CreateText(StringResources.ExampleConfigHeading));

            this.AppendGeneratedNote(this.xmlFile);

            var table = this.AppendTable(1);
            var cells = this.AppendRowToTable(table);
            var paragraph = cells[0].AppendChild(new Paragraph());

            var settings = new XmlWriterSettings
                               {
                                   Indent = true,
                                   IndentChars = new string(' ', 2),
                                   NewLineHandling = NewLineHandling.Replace
                               };
            using (
                var writer = XmlWriter.Create(
                    new ParagraphXmlWriter(paragraph, this.schemaSet, this.AppendLinkToParagraph), settings))
            {
                doc.WriteTo(writer);
            }

            var caption = string.Format(StringResources.ExampleConfigCaption, Path.GetFileName(this.xmlFile));
            this.AppendCaption(CaptionType.Figure, caption);

            // add an empty paragraph for spacing
            this.currentElement = this.currentElement.InsertAfterSelf(new Paragraph());

            return true;
        }

        private void GenerateXmlStructure(IEnumerable<XmlSchemaType> displayTypes)
        {
            this.currentElement.RemoveAllChildren<Run>();
            var heading = this.currentElement.AppendChild(new Run());
            heading.AppendChild(CreateText(StringResources.XmlStructureHeading));

            this.AppendGeneratedNote(this.xsdFile);

            var rootRefPara = this.currentElement.InsertAfterSelf(CreateKeepNextParagraph());
            this.currentElement = rootRefPara;
            rootRefPara.AppendChild(new Run(CreateText(StringResources.RootElementRefPrefix)));

            var hadElement = false;
            foreach (XmlSchemaElement element in this.schemaSet.GlobalElements.Values)
            {
                if (hadElement)
                {
                    rootRefPara.AppendChild(new Run(CreateText(StringResources.RootElementRefSeparator)));
                }

                this.AppendLinkToParagraph(
                    rootRefPara,
                    element.ElementSchemaType.QualifiedName.Name,
                    element.ElementSchemaType,
                    false,
                    b => new Run());
                hadElement = true;
            }

            rootRefPara.AppendChild(new Run(CreateText(StringResources.RootElementRefPostfix)));

            // add an empty paragraph for spacing
            this.currentElement = this.currentElement.InsertAfterSelf(new Paragraph());

            foreach (var type in displayTypes)
            {
                var complex = type as XmlSchemaComplexType;
                if (complex != null)
                {
                    this.AddStructureChapter(complex);
                    continue;
                }

                // TODO: do we have any other simple types other than enums?
                this.AddEnumChapter((XmlSchemaSimpleType)type);
            }
        }

        private void CollectChapters(
            XmlSchemaType type,
            bool addChapter,
            ICollection<XmlSchemaType> displayTypes,
            ICollection<XmlSchemaType> allTypes)
        {
            if (!allTypes.Contains(type))
            {
                return;
            }

            if (addChapter)
            {
                allTypes.Remove(type);
            }

            var complex = type as XmlSchemaComplexType;
            if (complex == null)
            {
                if (addChapter)
                {
                    displayTypes.Add(type);
                }

                return;
            }

            var sequence = complex.Particle as XmlSchemaSequence;
            if (sequence != null && sequence.Items.Count == 1 && complex.Attributes.Count == 0)
            {
                var item = sequence.Items[0] as XmlSchemaElement;
                if (item != null && item.MaxOccurs > 1)
                {
                    // special case: this is simply a list of elements, we ignore it since it will be inlined
                    this.autoListOfElements[complex] = item;
                    addChapter = false;
                }
            }

            var choice = complex.Particle as XmlSchemaChoice;
            if (choice != null && choice.MaxOccurs > 1 && complex.Attributes.Count == 0)
            {
                this.autoListOfChoiceElements[complex] = choice.Items.Cast<XmlSchemaElement>().ToList();
                addChapter = false;
            }

            if (addChapter)
            {
                displayTypes.Add(complex);
            }

            foreach (var attribute in complex.GetAttributes())
            {
                this.CollectChapters(attribute.AttributeSchemaType, true, displayTypes, allTypes);
            }

            foreach (var element in complex.GetElements())
            {
                this.CollectChapters(element.ElementSchemaType, true, displayTypes, allTypes);
            }
        }

        private void AddEnumChapter(XmlSchemaSimpleType type)
        {
            var enums = type.GetEnumerations().ToArray();
            if (enums.Length == 0)
            {
                return;
            }

            this.AppendHeading(
                string.Format(StringResources.EnumHeading, type.Name),
                this.topHeadingIndex + 1,
                this.GetHyperlinkName(type));

            var table = this.AppendTable(2);
            foreach (var facet in enums)
            {
                var cells = this.AppendRowToTable(table);
                var para = cells[0].AppendChild(CreateKeepNextParagraph());
                para.AppendChild(new Run(new Text(facet.Value)));

                this.AppendAnnotations(cells[1], facet.Annotation);
            }

            var caption = string.Format(StringResources.EnumTableCaption, type.Name);
            this.AppendCaption(CaptionType.Table, caption);
            this.currentElement = this.currentElement.InsertAfterSelf(CreateKeepNextParagraph());
            this.currentElement = this.AppendAnnotations(
                (OpenXmlCompositeElement)this.currentElement.Parent, type.Annotation);

            // add an empty paragraph for spacing
            this.currentElement = this.currentElement.InsertAfterSelf(new Paragraph());
        }

        private void AddStructureChapter(XmlSchemaComplexType type)
        {
            this.AppendHeading(
                string.Format(StringResources.StructureHeading, type.Name),
                this.topHeadingIndex + 1,
                this.GetHyperlinkName(type));

            this.AddStructureAttributesTable(type);
            this.AddStructureElementsTable(type);
            this.currentElement = this.AppendAnnotations(
                (OpenXmlCompositeElement)this.currentElement.Parent, type.Annotation);

            // add an empty paragraph for spacing
            this.currentElement = this.currentElement.InsertAfterSelf(new Paragraph());
        }

        private void AddStructureAttributesTable(XmlSchemaComplexType type)
        {
            var attributes = type.GetAttributes().ToArray();
            if (attributes.Length == 0)
            {
                return;
            }

            var table = this.AppendTable(4);
            foreach (var attribute in attributes.Where(a => a.Use != XmlSchemaUse.Prohibited))
            {
                var cells = this.AppendRowToTable(table);

                var para = cells[0].AppendChild(CreateKeepNextParagraph());
                para.AppendChild(
                    new Run(CreateText(string.Format(StringResources.AttributeFormat, attribute.Name))));

                para = cells[1].AppendChild(CreateKeepNextParagraph());
                para.AppendChild(
                    new Run(
                        CreateText(
                            attribute.Use == XmlSchemaUse.Required
                                ? StringResources.AttributeRequired
                                : StringResources.AttributeOptional)));

                para = cells[2].AppendChild(CreateKeepNextParagraph());
                this.AppendLinkToParagraph(
                    para,
                    attribute.AttributeSchemaType.QualifiedName.Name,
                    attribute.AttributeSchemaType,
                    true,
                    b => new Run());

                if (attribute.DefaultValue != null)
                {
                    para.AppendChild(
                        new Run(
                            CreateText(
                                string.Format(StringResources.DefaultValuePostfix, attribute.DefaultValue))));
                }

                this.AppendAnnotations(cells[3], attribute.Annotation);
            }

            var caption = string.Format(StringResources.AttributesTableCaption, type.Name);
            this.AppendCaption(CaptionType.Table, caption);
            this.currentElement = this.currentElement.InsertAfterSelf(CreateKeepNextParagraph());
        }

        private void AddStructureElementsTable(XmlSchemaComplexType type)
        {
            var elements = type.GetElements().ToArray();
            if (elements.Length == 0)
            {
                return;
            }

            var table = this.AppendTable(4);
            foreach (var element in elements)
            {
                var cells = this.AppendRowToTable(table);

                var para = cells[0].AppendChild(CreateKeepNextParagraph());
                para.AppendChild(new Run(CreateText(string.Format(StringResources.ElementFormat, element.Name))));

                para = cells[1].AppendChild(CreateKeepNextParagraph());
                var isChoice = element.Parent is XmlSchemaChoice;
                var min = isChoice ? 0 : element.MinOccurs;
                var max = isChoice || element.MaxOccurs == decimal.MaxValue
                              ? "*"
                              : element.MaxOccurs.ToString(CultureInfo.InvariantCulture);
                para.AppendChild(new Run(CreateText(string.Format(StringResources.ElementOccurs, min, max))));

                para = cells[2].AppendChild(CreateKeepNextParagraph());
                var elementType = element.ElementSchemaType;

                XmlSchemaElement listElement;
                IEnumerable<XmlSchemaElement> choiceElements;
                if (this.autoListOfElements.TryGetValue(elementType, out listElement))
                {
                    // list types are simplified by inlining their element type
                    para.AppendChild(new Run(CreateText(StringResources.ListOfElementsPrefix)));
                    this.AppendLinkToParagraph(
                        para,
                        string.Format(StringResources.ElementFormat, listElement.Name),
                        listElement.ElementSchemaType,
                        false,
                        b => new Run());
                }
                else if (this.autoListOfChoiceElements.TryGetValue(elementType, out choiceElements))
                {
                    // special case: list of choices, we inline those
                    para.AppendChild(new Run(CreateText(StringResources.ListOfChoicePrefix)));
                    var choices = choiceElements.ToList();
                    for (int i = 0; i < choices.Count; i++)
                    {
                        if (i > 0)
                        {
                            para.AppendChild(new Run(CreateText(StringResources.ListOfChoiceSeparator)));
                        }

                        this.AppendLinkToParagraph(
                            para,
                            string.Format(StringResources.ElementFormat, choices[i].Name),
                            choices[i].ElementSchemaType,
                            false,
                            b => new Run());
                    }
                }
                else
                {
                    this.AppendLinkToParagraph(para, elementType.QualifiedName.Name, elementType, true, b => new Run());
                }

                if (element.DefaultValue != null)
                {
                    para.AppendChild(
                        new Run(
                            CreateText(string.Format(StringResources.DefaultValuePostfix, element.DefaultValue))));
                }

                this.AppendAnnotations(cells[3], element.Annotation);
            }

            var caption = string.Format(StringResources.ElementsTableCaption, type.Name);
            this.AppendCaption(CaptionType.Table, caption);
            this.currentElement = this.currentElement.InsertAfterSelf(CreateKeepNextParagraph());
        }

        private void AppendHeading(string heading, int headingIndex, string bookmark = null)
        {
            this.currentElement = this.currentElement.InsertAfterSelf(CreateKeepNextParagraph());

            if (this.headingPrefix != null)
            {
                var paraProps = this.currentElement.GetOrAppendChild<ParagraphProperties>();
                paraProps.ParagraphStyleId = new ParagraphStyleId { Val = this.headingPrefix + headingIndex };
            }

            if (bookmark != null)
            {
                this.currentElement.AppendChild(new BookmarkStart { Id = bookmark, Name = bookmark });
            }

            this.currentElement.AppendChild(new Run(CreateText(heading)));

            if (bookmark != null)
            {
                this.currentElement.AppendChild(new BookmarkEnd { Id = bookmark });
            }
        }

        private void AppendLinkToParagraph(
            Paragraph paragraph,
            string text,
            XmlSchemaType typeReference,
            bool insertSoftHyphens,
            Func<bool, Run> runConstructor)
        {
            Run run;
            if (!this.schemaSet.GlobalTypes.Contains(typeReference.QualifiedName)
                || this.autoListOfElements.ContainsKey(typeReference)
                || this.autoListOfChoiceElements.ContainsKey(typeReference))
            {
                run = runConstructor(false);
                run.AppendChild(CreateText(text));
                paragraph.AppendChild(run);
                return;
            }

            run = paragraph.AppendChild(runConstructor(true));
            run.AppendChild(new FieldChar { FieldCharType = FieldCharValues.Begin });

            var fieldCode = string.Format(@" HYPERLINK  \l ""{0}"" ", this.GetHyperlinkName(typeReference));
            run = paragraph.AppendChild(runConstructor(true));
            run.AppendChild(new FieldCode(fieldCode) { Space = SpaceProcessingModeValues.Preserve });

            run = paragraph.AppendChild(runConstructor(true));
            run.AppendChild(new FieldChar { FieldCharType = FieldCharValues.Separate });

            run = paragraph.AppendChild(runConstructor(true));
            var props = run.GetOrAppendChild<RunProperties>();
            props.Underline = new Underline { Val = UnderlineValues.Single };

            if (!insertSoftHyphens)
            {
                run.AppendChild(CreateText(text));
            }
            else
            {
                var first = true;
                foreach (var part in SplitPascalCase(text))
                {
                    if (!first)
                    {
                        run.AppendChild(new SoftHyphen());
                    }

                    first = false;
                    run.AppendChild(CreateText(part));
                }
            }

            run = paragraph.AppendChild(runConstructor(true));
            run.AppendChild(new FieldChar { FieldCharType = FieldCharValues.End });
        }

        private void AppendGeneratedNote(string filename)
        {
            this.currentElement = this.currentElement.InsertAfterSelf(CreateKeepNextParagraph());
            var size = StringResources.GeneratedChapterNoteSize;
            var runProps = new RunProperties(new FontSize { Val = size }, new FontSizeComplexScript { Val = size });
            var paraProps = this.currentElement.GetOrAppendChild<ParagraphProperties>();
            paraProps.AppendChild(runProps);

            var text = string.Format(
                StringResources.GeneratedChapterNote,
                Path.GetFileName(filename),
                DateTime.Now,
                Environment.UserName.ToUpper());
            this.currentElement.AppendChild(new Run(runProps.CloneNode(true), CreateText(text)));
        }

        private Table AppendTable(int colCount)
        {
            var table = this.currentElement.InsertAfterSelf(new Table());
            this.currentElement = table;
            table.AppendChild(
                new TableProperties(
                    new TableStyle { Val = TableStyle },
                    new TableWidth { Width = "0", Type = TableWidthUnitValues.Auto }));
            var grid = table.AppendChild(new TableGrid());
            colCount = Math.Min(colCount, TableColumnWidth.Length);
            for (int i = 0; i < colCount - 1; i++)
            {
                grid.AppendChild(new GridColumn { Width = TableColumnWidth[i].ToString(CultureInfo.InvariantCulture) });
            }

            var lastColumn = TableColumnWidth.Skip(colCount - 1).Sum();
            grid.AppendChild(new GridColumn { Width = lastColumn.ToString(CultureInfo.InvariantCulture) });

            return table;
        }

        private TableCell[] AppendRowToTable(Table table)
        {
            var columns = table.Descendants<GridColumn>();
            var row = table.AppendChild(new TableRow());
            var cells =
                columns.Select(
                    c =>
                    new TableCell(
                        new TableCellProperties(
                        new TableCellWidth
                            {
                                Type = TableWidthUnitValues.Dxa, Width = c.Width.Value
                            }))).ToArray();
            row.Append(cells.Cast<OpenXmlElement>());
            return cells;
        }

        private OpenXmlCompositeElement AppendAnnotations(
            OpenXmlCompositeElement parent, XmlSchemaAnnotation annotation)
        {
            if (annotation == null || annotation.Items == null)
            {
                return parent.AppendChild(CreateKeepNextParagraph());
            }

            var docs = annotation.Items.OfType<XmlSchemaDocumentation>().ToArray();
            if (docs.Length == 0)
            {
                return parent.AppendChild(CreateKeepNextParagraph());
            }

            Paragraph para = null;
            foreach (var doc in docs)
            {
                para = parent.AppendChild(CreateKeepNextParagraph());
                para.Append(
                        doc.Markup.Select(n => new Run(CreateText(CleanupDocRegex.Replace(n.InnerText.Trim(), " ")))));
            }

            return para;
        }

        private void AppendCaption(CaptionType type, string caption)
        {
            var paragraph = this.currentElement.AppendChild(CreateKeepNextParagraph());
            this.currentElement = paragraph;

            paragraph.GetOrAppendChild<ParagraphProperties>().ParagraphStyleId =
                new ParagraphStyleId { Val = CaptionStyle };

            var prefix = type == CaptionType.Figure ? StringResources.FigurePrefix : StringResources.TablePrefix;
            var fieldCode = string.Format(@" SEQ {0} \* ARABIC \s 1 ", type);
            paragraph.AppendChild(new Run(CreateText(prefix)));
            paragraph.AppendChild(new Run(new FieldChar { FieldCharType = FieldCharValues.Begin }));
            paragraph.AppendChild(new Run(new FieldCode(fieldCode) { Space = SpaceProcessingModeValues.Preserve }));
            paragraph.AppendChild(new Run(new FieldChar { FieldCharType = FieldCharValues.Separate }));
            paragraph.AppendChild(new Run(new Text("1")));
            paragraph.AppendChild(new Run(new FieldChar { FieldCharType = FieldCharValues.End }));
            paragraph.AppendChild(new Run(CreateText(" " + caption)));
        }

        private string GetHyperlinkName(XmlSchemaType typeReference)
        {
            string link;
            var name = typeReference.QualifiedName.Name;
            if (!this.hyperlinks.TryGetValue(name, out link))
            {
                link = string.Format("_Link_XD{0:X4}", nextHyperlinkIndex++);
                this.hyperlinks.Add(name, link);
            }

            return link;
        }
    }
}