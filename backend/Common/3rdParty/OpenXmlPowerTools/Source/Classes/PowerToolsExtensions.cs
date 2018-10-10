/***************************************************************************

Copyright (c) Microsoft Corporation 2011.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.IO.Packaging;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Validation;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenXmlPowerTools
{
    /// <summary>
    /// Contains extension methods to modify Open XML Documents
    /// </summary>
    public static class PowerToolsExtensions
    {
        public static void RemovePart(this OpenXmlPart part)
        {
            var parentParts = part.GetParentParts().ToList();
            foreach (var parentPart in parentParts)
                parentPart.DeletePart(part);
        }

        /// <summary>
        /// Removes personal information from the document.
        /// </summary>
        /// <param name="document"></param>
        public static OpenXmlPowerToolsDocument RemovePersonalInformation(WmlDocument doc)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(doc))
            {
                using (WordprocessingDocument document = streamDoc.GetWordprocessingDocument())
                {
                    XNamespace x = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties";
                    XDocument extendedFileProperties = document.ExtendedFilePropertiesPart.GetXDocument();
                    extendedFileProperties.Elements(x + "Properties").Elements(x + "Company").Remove();
                    XElement totalTime = extendedFileProperties.Elements(x + "Properties").Elements(x + "TotalTime").FirstOrDefault();
                    if (totalTime != null)
                        totalTime.Value = "0";
                    document.ExtendedFilePropertiesPart.PutXDocument();

                    XNamespace dc = "http://purl.org/dc/elements/1.1/";
                    XNamespace cp = "http://schemas.openxmlformats.org/package/2006/metadata/core-properties";
                    XDocument coreFileProperties = document.CoreFilePropertiesPart.GetXDocument();
                    foreach (var textNode in coreFileProperties.Elements(cp + "coreProperties")
                                                               .Elements(dc + "creator")
                                                               .Nodes()
                                                               .OfType<XText>())
                        textNode.Value = "";
                    foreach (var textNode in coreFileProperties.Elements(cp + "coreProperties")
                                                               .Elements(cp + "lastModifiedBy")
                                                               .Nodes()
                                                               .OfType<XText>())
                        textNode.Value = "";
                    foreach (var textNode in coreFileProperties.Elements(cp + "coreProperties")
                                                               .Elements(dc + "title")
                                                               .Nodes()
                                                               .OfType<XText>())
                        textNode.Value = "";
                    XElement revision = coreFileProperties.Elements(cp + "coreProperties").Elements(cp + "revision").FirstOrDefault();
                    if (revision != null)
                        revision.Value = "1";
                    document.CoreFilePropertiesPart.PutXDocument();

                    // add w:removePersonalInformation, w:removeDateAndTime to DocumentSettingsPart
                    XNamespace w = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
                    XDocument documentSettings = document.MainDocumentPart.DocumentSettingsPart.GetXDocument();
                    // add the new elements in the right position.  Add them after the following three elements
                    // (which may or may not exist in the xml document).
                    XElement settings = documentSettings.Root;
                    XElement lastOfTop3 = settings.Elements()
                        .Where(e => e.Name == w + "writeProtection" ||
                            e.Name == w + "view" ||
                            e.Name == w + "zoom")
                        .InDocumentOrder()
                        .LastOrDefault();
                    if (lastOfTop3 == null)
                    {
                        // none of those three exist, so add as first children of the root element
                        settings.AddFirst(
                            settings.Elements(w + "removePersonalInformation").Any() ?
                                null :
                                new XElement(w + "removePersonalInformation"),
                            settings.Elements(w + "removeDateAndTime").Any() ?
                                null :
                                new XElement(w + "removeDateAndTime")
                        );
                    }
                    else
                    {
                        // one of those three exist, so add after the last one
                        lastOfTop3.AddAfterSelf(
                            settings.Elements(w + "removePersonalInformation").Any() ?
                                null :
                                new XElement(w + "removePersonalInformation"),
                            settings.Elements(w + "removeDateAndTime").Any() ?
                                null :
                                new XElement(w + "removeDateAndTime")
                        );
                    }
                    document.MainDocumentPart.DocumentSettingsPart.PutXDocument();
                }
                return streamDoc.GetModifiedDocument();
            }
        }

        private static string StringConcatenate<T>(this IEnumerable<T> source, Func<T, string> func, string separator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (T item in source)
                sb.Append(func(item)).Append(separator);
            if (sb.Length > separator.Length)
                sb.Length -= separator.Length;
            return sb.ToString();
        }

        static string ContainsAnyStyles(IEnumerable<string> stylesToSearch, IEnumerable<string> searchStrings)
        {
            if (searchStrings == null)
                return null;
            foreach (var style in stylesToSearch)
                foreach (var s in searchStrings)
                    if (String.Compare(style, s, true) == 0)
                        return s;
            return null;
        }

        static string ContainsAnyContent(string stringToSearch, IEnumerable<string> searchStrings,
            IEnumerable<Regex> regularExpressions, bool isRegularExpression, bool caseInsensitive)
        {
            if (searchStrings == null)
                return null;
            if (isRegularExpression)
            {
                foreach (var r in regularExpressions)
                    if (r.IsMatch(stringToSearch))
                        return r.ToString();
            }
            else
                if (caseInsensitive)
                {
                    foreach (var s in searchStrings)
                        if (stringToSearch.ToLower().Contains(s.ToLower()))
                            return s;
                }
                else
                {
                    foreach (var s in searchStrings)
                        if (stringToSearch.Contains(s))
                            return s;
                }

            return null;
        }

        static IEnumerable<string> GetAllStyleIdsAndNames(WordprocessingDocument doc, string styleId)
        {
            string localStyleId = styleId;
            XNamespace w =
              "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

            yield return styleId;

            string styleNameForFirstStyle = (string)doc
                .MainDocumentPart
                .StyleDefinitionsPart
                .GetXDocument()
                .Root
                .Elements(w + "style")
                .Where(e => (string)e.Attribute(w + "type") == "paragraph" &&
                    (string)e.Attribute(w + "styleId") == styleId)
                .Elements(w + "name")
                .Attributes(w + "val")
                .FirstOrDefault();

            if (styleNameForFirstStyle != null)
                yield return styleNameForFirstStyle;

            while (true)
            {
                XElement style = doc
                    .MainDocumentPart
                    .StyleDefinitionsPart
                    .GetXDocument()
                    .Root
                    .Elements(w + "style")
                    .Where(e => (string)e.Attribute(w + "type") == "paragraph" &&
                        (string)e.Attribute(w + "styleId") == localStyleId)
                    .FirstOrDefault();

                if (style == null)
                    yield break;

                var basedOn = (string)style
                    .Elements(w + "basedOn")
                    .Attributes(w + "val")
                    .FirstOrDefault();

                if (basedOn == null)
                    yield break;

                yield return basedOn;

                XElement basedOnStyle = doc
                    .MainDocumentPart
                    .StyleDefinitionsPart
                    .GetXDocument()
                    .Root
                    .Elements(w + "style")
                    .Where(e => (string)e.Attribute(w + "type") == "paragraph" &&
                        (string)e.Attribute(w + "styleId") == basedOn)
                    .FirstOrDefault();

                string basedOnStyleName = (string)basedOnStyle
                    .Elements(w + "name")
                    .Attributes(w + "val")
                    .FirstOrDefault();


                if (basedOnStyleName != null)
                    yield return basedOnStyleName;

                localStyleId = basedOn;
            }
        }
        private static IEnumerable<string> GetInheritedStyles(WordprocessingDocument doc, string styleName)
        {
            string localStyleName = styleName;
            XNamespace w =
              "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

            yield return styleName;
            while (true)
            {
                XElement style = doc
                    .MainDocumentPart
                    .StyleDefinitionsPart
                    .GetXDocument()
                    .Root
                    .Elements(w + "style")
                    .Where(e => (string)e.Attribute(w + "type") == "paragraph" &&
                        (string)e.Element(w + "name").Attribute(w + "val") == localStyleName)
                    .FirstOrDefault();

                if (style == null)
                    yield break;

                var basedOn = (string)style
                    .Elements(w + "basedOn")
                    .Attributes(w + "val")
                    .FirstOrDefault();

                if (basedOn == null)
                    yield break;

                yield return basedOn;
                localStyleName = basedOn;
            }
        }

        static public Commands.MatchInfo[] SearchInDocument(WmlDocument doc,
            IEnumerable<string> styleSearchString, IEnumerable<string> contentSearchString,
            bool isRegularExpression, bool caseInsensitive)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(doc))
            using (WordprocessingDocument document = streamDoc.GetWordprocessingDocument())
            {
                XNamespace w =
                  "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
                XName r = w + "r";
                XName ins = w + "ins";

                RegexOptions options;
                Regex[] regularExpressions = null;
                if (isRegularExpression && contentSearchString != null)
                {
                    if (caseInsensitive)
                        options = RegexOptions.IgnoreCase | RegexOptions.Compiled;
                    else
                        options = RegexOptions.Compiled;
                    regularExpressions = contentSearchString
                        .Select(s => new Regex(s, options)).ToArray();
                }

                var defaultStyleName = (string)document
                    .MainDocumentPart
                    .StyleDefinitionsPart
                    .GetXDocument()
                    .Root
                    .Elements(w + "style")
                    .Where(style =>
                        (string)style.Attribute(w + "type") == "paragraph" &&
                        (string)style.Attribute(w + "default") == "1")
                    .First()
                    .Attribute(w + "styleId");

                var q1 = document
                    .MainDocumentPart
                    .GetXDocument()
                    .Root
                    .Element(w + "body")
                    .Elements()
                    .Select((p, i) =>
                    {
                        var styleNode = p
                            .Elements(w + "pPr")
                            .Elements(w + "pStyle")
                            .FirstOrDefault();
                        var styleName = styleNode != null ?
                            (string)styleNode.Attribute(w + "val") :
                            defaultStyleName;
                        return new
                        {
                            Element = p,
                            Index = i,
                            StyleName = styleName
                        };
                    }
                    );

                var q2 = q1
                    .Select(i =>
                    {
                        string text = null;
                        if (i.Element.Name == w + "p")
                            text = i.Element.Elements()
                                .Where(z => z.Name == r || z.Name == ins)
                                .Descendants(w + "t")
                                .StringConcatenate(element => (string)element);
                        else
                            text = i.Element
                                .Descendants(w + "p")
                                .StringConcatenate(p => p
                                    .Elements()
                                    .Where(z => z.Name == r || z.Name == ins)
                                    .Descendants(w + "t")
                                    .StringConcatenate(element => (string)element),
                                    Environment.NewLine
                                );

                        return new
                        {
                            Element = i.Element,
                            StyleName = i.StyleName,
                            Index = i.Index,
                            Text = text
                        };
                    }
                    );

                var q3 = q2
                    .Select(i =>
                        new Commands.MatchInfo
                        {
                            ElementNumber = i.Index + 1,
                            Content = i.Text,
                            Style = ContainsAnyStyles(GetAllStyleIdsAndNames(document, i.StyleName).Distinct(), styleSearchString),
                            Pattern = ContainsAnyContent(i.Text, contentSearchString, regularExpressions, isRegularExpression, caseInsensitive),
                            IgnoreCase = caseInsensitive
                        }
                    )
                    .Where(i => (styleSearchString == null || i.Style != null) && (contentSearchString == null || i.Pattern != null));
                return q3.ToArray();
            }
        }

        public static OpenXmlPowerToolsDocument InsertXml(OpenXmlPowerToolsDocument doc, string[] partPaths, string insertionXpath, string content)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(doc))
            {
                using (Package package = streamDoc.GetPackage())
                {
                    foreach (string partPath in partPaths)
                    {
                        Uri xmlPartUri;
                        XmlDocument xmlDocument;
                        PackagePart xmlPart = null;

                        // Searches for the given part
                        xmlPartUri = new Uri(partPath, UriKind.Relative);
                        //if (!document.Package.PartExists(xmlPartUri))
                        //throw new ArgumentException("Specified part does not exist.");

                        xmlPart = package.GetPart(xmlPartUri);
                        using (XmlReader xmlReader = XmlReader.Create(xmlPart.GetStream(FileMode.Open, FileAccess.Read)))
                        {
                            try
                            {
                                xmlDocument = new XmlDocument();
                                xmlDocument.Load(xmlReader);
                            }
                            catch (XmlException)
                            {
                                xmlDocument = new XmlDocument();
                            }
                        }

                        // Looks into the XmlDocument for nodes at the specified path
                        XmlNamespaceManager namespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
                        namespaceManager.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
                        XmlNode insertionPoint =
                            xmlDocument.SelectSingleNode(insertionXpath, namespaceManager);
                        if (insertionPoint == null)
                            throw new ArgumentException("Insertion point does not exist.");

                        StringReader r = new StringReader("<w:node xmlns:w='http://schemas.openxmlformats.org/wordprocessingml/2006/main'>" + content + "</w:node>");

                        XmlNode nodoid = xmlDocument.ReadNode(XmlReader.Create(r));
                        //doc.LoadXml("<w:node xmlns:w='http://schemas.openxmlformats.org/wordprocessingml/2006/main'>" + xmlContent + "</w:node>");


                        //// Inserts new contents into the given part
                        XmlNode xmlNodeToInsert =
                            nodoid;// doc.FirstChild;
                        //    xmlDocument.CreateElement("w","node", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");


                        //xmlNodeToInsert. .InnerXml = xmlContent;
                        XmlNodeList nodes = xmlNodeToInsert.ChildNodes;
                        if (nodes.Count > 0)
                            for (int i = nodes.Count - 1; i >= 0; i--)
                            {
                                XmlNode node = nodes[i];
                                insertionPoint.ParentNode.InsertAfter(node, insertionPoint);
                            }

                        // Writes the XmlDocument back to the part
                        using (XmlWriter writer = XmlWriter.Create(xmlPart.GetStream(FileMode.Create, FileAccess.Write)))
                        {
                            xmlDocument.WriteTo(writer);
                        }
                    }
                }
                return streamDoc.GetModifiedDocument();
            }
        }

        public static OpenXmlPowerToolsDocument Lock(WmlDocument doc)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(doc))
            {
                using (WordprocessingDocument document = streamDoc.GetWordprocessingDocument())
                {
                    //Finds the settings part
                    XDocument settingsDocument;
                    XElement documentProtectionElement = null;
                    if (document.MainDocumentPart.DocumentSettingsPart == null)
                    {
                        //If settings part does not exist creates a new one
                        DocumentSettingsPart settingsPart = document.MainDocumentPart.AddNewPart<DocumentSettingsPart>();
                        settingsDocument = settingsPart.GetXDocument();
                        settingsDocument.Add(new XElement(W.settings,
                                new XAttribute(XNamespace.Xmlns + "w", W.w),
                                new XAttribute(XNamespace.Xmlns + "r", R.r)));
                    }
                    else
                    {
                        //If the settings part does exist looks if documentProtection has been included
                        settingsDocument = document.MainDocumentPart.DocumentSettingsPart.GetXDocument();
                        documentProtectionElement = settingsDocument.Element(W.settings).Element(W.documentProtection);
                    }

                    //Creates the documentProtection element, or edits it if it exists
                    if (documentProtectionElement == null)
                    {
                        settingsDocument
                            .Element(W.settings)
                            .Add(
                                new XElement(W.documentProtection,
                                    new XAttribute(W.edit, "readOnly")
                                )
                            );
                    }
                    else
                        documentProtectionElement.SetAttributeValue(W.edit, "readOnly");
                    document.MainDocumentPart.DocumentSettingsPart.PutXDocument();
                }
                return streamDoc.GetModifiedDocument();
            }
        }
        public static void SetContent(WordprocessingDocument document, IEnumerable<XElement> contents)
        {
            MainDocumentPart mainDocumentPart = document.MainDocumentPart;
            if (mainDocumentPart == null)
                mainDocumentPart = document.AddMainDocumentPart();
            XDocument mainDocumentPartContentToInsert = new XDocument(
                new XElement(W.document,
                    new XAttribute(XNamespace.Xmlns + "w", W.w),
                    new XAttribute(XNamespace.Xmlns + "r", R.r),
                    new XElement(W.body, contents)));
            XDocument mainDocumentPartContent = mainDocumentPart.GetXDocument();
            if (mainDocumentPartContent.Root == null)
                mainDocumentPartContent.Add(mainDocumentPartContentToInsert.Root);
            else
                mainDocumentPartContent.Root.ReplaceWith(mainDocumentPartContentToInsert.Root);
            mainDocumentPart.PutXDocument();
        }

        public static IEnumerable<ValidationErrorInfo> ValidateXml(OpenXmlPowerToolsDocument document)
        {
            if (document is WmlDocument)
            {
                using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(document))
                using (WordprocessingDocument doc = streamDoc.GetWordprocessingDocument())
                {
                    OpenXmlValidator validator = new OpenXmlValidator();
                    return validator.Validate(doc);
                }
            }
            else if (document is SmlDocument)
            {
                using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(document))
                using (SpreadsheetDocument doc = streamDoc.GetSpreadsheetDocument())
                {
                    OpenXmlValidator validator = new OpenXmlValidator();
                    return validator.Validate(doc);
                }
            }
            else if (document is PmlDocument)
            {
                using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(document))
                using (PresentationDocument doc = streamDoc.GetPresentationDocument())
                {
                    OpenXmlValidator validator = new OpenXmlValidator();
                    return validator.Validate(doc);
                }
            }
            throw new PowerToolsDocumentException("Not an Open XML document.");
        }
    }
}