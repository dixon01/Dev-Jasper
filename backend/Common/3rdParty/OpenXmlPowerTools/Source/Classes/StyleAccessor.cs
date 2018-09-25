/***************************************************************************

Copyright (c) Microsoft Corporation 2011.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using DocumentFormat.OpenXml.Packaging;
using System.Collections.ObjectModel;
using System;

namespace OpenXmlPowerTools
{
    /// <summary>
    /// Provides access to style operations
    /// </summary>
    public class StyleAccessor
    {
        //private XDocument xmlStylesDefinitionDocument;
        private static XNamespace ns;
        /// <summary>
        /// newStyleNameSuffic variable
        /// </summary>
        public static readonly string newStyleNameSuffix = "_1";

        static StyleAccessor()
        {
            ns = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
        }

        public static void CreateIndexStyles(WordprocessingDocument document, string stylesSourceFile, bool addDefaultStyles)
        {
            if (stylesSourceFile == "")
            {
                if (addDefaultStyles)
                {
                    XElement Index1 = new XElement(ns + "style",
                        new XAttribute(ns + "type", "paragraph"),
                        new XAttribute(ns + "styleId", "Index1"),
                        new XElement(ns + "name",
                           new XAttribute(ns + "val", "index 1")),
                        new XElement(ns + "basedOn",
                           new XAttribute(ns + "val", "Normal")),
                        new XElement(ns + "next",
                            new XAttribute(ns + "val", "Normal")),
                        new XElement(ns + "autoRedefine"),
                        new XElement(ns + "semiHidden"),
                        new XElement(ns + "unhideWhenUsed"),
                        new XElement(ns + "pPr",
                            new XElement(ns + "spacing",
                                new XAttribute(ns + "after", "0"),
                                new XAttribute(ns + "line", "240"),
                                new XAttribute(ns + "lineRule", "auto")),
                            new XElement(ns + "ind",
                                new XAttribute(ns + "left", "220"),
                                new XAttribute(ns + "hanging", "220"))));

                    AddStyleDefinition(document, Index1);
                }
            }
            else
            {
                //  add the styles from the styles source file
                XDocument StyleXmlPart = GetStylesDocument(document);
                //  the preffix must be empty, because the styles need to be recognized by the TOC
                XDocument stylesSource = XDocument.Load(stylesSourceFile);
                IEnumerable<XElement> IndexStyles = GetStyleHierarchy("Index1", stylesSource, string.Empty);
                AddStyleDefinition(document, IndexStyles);
            }
            document.MainDocumentPart.StyleDefinitionsPart.PutXDocument();
        }
        public static void CreateTOAStyles(WordprocessingDocument document, string stylesSourceFile, bool addDefaultStyles)
        {
            if (stylesSourceFile == "")
            {
                if (addDefaultStyles)
                {
                    XElement TOAHeading = new XElement(ns + "style",
                        new XAttribute(ns + "type", "paragraph"),
                        new XAttribute(ns + "styleId", "TOAHeading"),
                        new XElement(ns + "name",
                           new XAttribute(ns + "val", "toa heading")),
                        new XElement(ns + "basedOn",
                           new XAttribute(ns + "val", "Normal")),
                        new XElement(ns + "next",
                            new XAttribute(ns + "val", "Normal")),
                        new XElement(ns + "semiHidden"),
                        new XElement(ns + "unhideWhenUsed"),
                        new XElement(ns + "pPr",
                            new XElement(ns + "spacing",
                                new XAttribute(ns + "before", "120"))),
                        new XElement(ns + "rPr",
                            new XElement(ns + "b"),
                            new XElement(ns + "bCs"),
                            new XElement(ns + "sz",
                                new XAttribute(ns + "val", 24)),
                            new XElement(ns + "szCs",
                                new XAttribute(ns + "val", 24))));

                    AddStyleDefinition(document, TOAHeading);

                    XElement tableOfAuthorities = new XElement(ns + "style",
                        new XAttribute(ns + "type", "paragraph"),
                        new XAttribute(ns + "styleId", "TableofAuthorities"),
                        new XElement(ns + "name",
                            new XAttribute(ns + "val", "table of authorities")),
                        new XElement(ns + "basedOn",
                            new XAttribute(ns + "val", "Normal")),
                        new XElement(ns + "next",
                            new XAttribute(ns + "val", "Normal")),
                        new XElement(ns + "semiHidden"),
                        new XElement(ns + "unhideWhenUsed"),
                        new XElement(ns + "pPr",
                            new XElement(ns + "spacing",
                                new XAttribute(ns + "after", "0")),
                            new XElement(ns + "ind",
                                new XAttribute(ns + "left", "220"),
                                new XAttribute(ns + "hanging", "220"))));

                    AddStyleDefinition(document, tableOfAuthorities);
                }
            }
            else
            {
                //  add the styles from the styles source file
                XDocument StyleXmlPart = GetStylesDocument(document);
                //  the prefix must be empty, because the styles need to be recognized by the TOC
                XDocument stylesSource = XDocument.Load(stylesSourceFile);
                IEnumerable<XElement> TOAStyles = GetStyleHierarchy("TOAHeading", stylesSource, string.Empty);
                TOAStyles = TOAStyles.Concat(GetStyleHierarchy("TableofAuthorities", stylesSource, string.Empty));
                AddStyleDefinition(document, TOAStyles);
            }
            document.MainDocumentPart.StyleDefinitionsPart.PutXDocument();
        }
        private static XDocument GetStylesDocument(WordprocessingDocument document)
        {
            if (document.MainDocumentPart.StyleDefinitionsPart != null)
                return document.MainDocumentPart.StyleDefinitionsPart.GetXDocument();
            return null;
        }
        public static XDocument GetStylesDocument(WmlDocument doc)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(doc))
            using (WordprocessingDocument document = streamDoc.GetWordprocessingDocument())
            {
                return GetStylesDocument(document);
            }
        }

        /// <summary>
        /// Gets the style hierarchy (link styles, next styles and basedOn styles) associated
        /// to the specified style, in a XElement collection
        /// </summary>
        /// <param name="styleName">Name of the style from where to get the hierarchy</param>
        /// <param name="stylesFile">File from where styles are taken </param>
        /// <param name="styleNameSuffix">Suffix of style name.</param>
        /// <returns>a collection containing the specified style and all the styles associated with it</returns>
        private static Collection<XElement> GetStyleHierarchy(string styleName, XDocument stylesFile, string styleNameSuffix)
        {
            try
            {
                Collection<XElement> stylesCollection = new Collection<XElement>();
                GetStyleHierarchyProcess(styleName, stylesFile, stylesCollection);

                return stylesCollection;
            }
            catch (XmlException ex)
            {
                throw new ArgumentException("File specified is not a valid XML file", ex);
            }

        }

        /// <summary>
        /// Gets the xml of a specific style definition
        /// </summary>
        /// <param name="styleName">Name of the style to get from the styles file</param>
        /// <param name="xmlStyleDefinitions">Style definitions</param>
        /// <param name="stylesCollection">Collection of styles</param>
        private static void GetStyleHierarchyProcess(string styleName, XDocument xmlStyleDefinitions, Collection<XElement> stylesCollection)
        {
            XName style = ns + "style";
            XName styleId = ns + "styleId";

            //  the style name can come empty, because the given style could not have a link, basedOn or next style.
            //  In those cases the stylename will come empty
            if (styleName != "")
            {
                // Creates a copy of the xmlStyleDefinition variable so the original xml will not be altered
                XElement actualStyle = new XElement(xmlStyleDefinitions.Root);
                actualStyle = actualStyle.Descendants().Where
                    (
                        tag =>
                        (tag.Name == style) && (tag.Attribute(styleId).Value.ToUpper() == styleName.ToUpper())
                     ).ToList().FirstOrDefault();

                if (actualStyle != null)
                {
                    // Looks in the stylesCollection if the style has already been added
                    IEnumerable<XElement> insertedStyles =
                        stylesCollection.Where
                        (
                            tag =>
                                (tag.Name == style) && (tag.Attribute(styleId).Value.ToUpper() == styleName.ToUpper())
                        );

                    // If the style has not been inserted
                    if (insertedStyles.Count() == 0)
                    {
                        stylesCollection.Add(actualStyle);
                        GetStyleHierarchyProcess(GetLinkStyleId(actualStyle), xmlStyleDefinitions, stylesCollection);
                        GetStyleHierarchyProcess(GetNextStyleId(actualStyle), xmlStyleDefinitions, stylesCollection);
                        GetStyleHierarchyProcess(GetBasedOnStyleId(actualStyle), xmlStyleDefinitions, stylesCollection);

                    }
                    // Changes the name of the style, so there would be no conflict with the original styles definition
                    actualStyle.Attribute(styleId).Value = actualStyle.Attribute(styleId).Value + newStyleNameSuffix;
                }
                else
                    throw new InvalidOperationException("Style or dependencies not found in the given style library.");
            }
        }

        /// <summary>
        /// Gets the name of the 'link' style associated to the given style
        /// </summary>
        /// <param name="xmlStyle">Xml to search for linked style</param>
        /// <returns>Name of the style</returns>
        private static string GetLinkStyleId(XElement xmlStyle)
        {
            XName val = ns + "val";
            string linkStyleId = "";
            XElement linkStyle = xmlStyle.Descendants(ns + "link").FirstOrDefault();
            if (linkStyle != null)
            {
                linkStyleId = linkStyle.Attribute(val).Value;
                //  Changes the name of the attribute, because the new added style is being renamed
                linkStyle.Attribute(val).Value = linkStyle.Attribute(val).Value + newStyleNameSuffix;
            }
            return linkStyleId;
        }

        /// <summary>
        /// Gets the name of the style tagged as 'next' associated to a given style
        /// </summary>
        /// <param name="xmlStyle">Xml to search for next style</param>
        /// <returns>Name of the style</returns>
        private static string GetNextStyleId(XElement xmlStyle)
        {
            XName val = ns + "val";
            string nextStyleId = "";
            XElement nextStyle = xmlStyle.Descendants(ns + "next").FirstOrDefault();
            if (nextStyle != null)
            {
                nextStyleId = nextStyle.Attribute(val).Value;
                // Changes the name of the attribute, because the new added style is being renamed
                nextStyle.Attribute(val).Value = nextStyle.Attribute(val).Value + newStyleNameSuffix;
            }
            return nextStyleId;
        }

        /// <summary>
        /// Gets the name of the style tagged as 'basedOn' associated to a given style
        /// </summary>
        /// <param name="xmlStyle">Xml to search for basedOn style</param>
        /// <returns>Name of the style</returns>
        private static string GetBasedOnStyleId(XElement xmlStyle)
        {
            XName val = ns + "val";
            string basedOnStyleId = "";
            XElement basedOnStyle = xmlStyle.Descendants(ns + "basedOn").FirstOrDefault();
            if (basedOnStyle != null)
            {
                basedOnStyleId = basedOnStyle.Attribute(val).Value;
                // Change the name of the attribute, because the new added style is being renamed
                basedOnStyle.Attribute(val).Value = basedOnStyle.Attribute(val).Value + newStyleNameSuffix;
            }
            return basedOnStyleId;
        }

        /// <summary>
        /// Sets a new styles part inside the document
        /// </summary>
        /// <param name="newStylesDocument">Path of styles definition file</param>
        public static OpenXmlPowerToolsDocument SetStylePart(WmlDocument doc, XDocument newStylesDocument)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(doc))
            {
                using (WordprocessingDocument document = streamDoc.GetWordprocessingDocument())
                {
                    // Replaces XDocument with the style file to transfer
                    if (document.MainDocumentPart.StyleDefinitionsPart == null)
                        document.MainDocumentPart.AddNewPart<StyleDefinitionsPart>();
                    document.MainDocumentPart.StyleDefinitionsPart.PutXDocument(newStylesDocument);
                }
                return streamDoc.GetModifiedDocument();
            }
        }

        /// <summary>
        /// Adds a new style definition
        /// </summary>
        /// <param name="xmlStyleDefinition">Style definition</param>
        public static void AddStyleDefinition(WordprocessingDocument document, XElement xmlStyleDefinition)
        {
            // Inserts the new style
            XDocument stylesPart = GetStylesDocument(document);
            stylesPart.Root.Add(xmlStyleDefinition);
        }

        /// <summary>
        /// Adds a set of styles in the styles.xml file
        /// </summary>
        /// <param name="xmlStyleDefinitions">Collection of style definitions</param>
        public static void AddStyleDefinition(WordprocessingDocument document, IEnumerable<XElement> xmlStyleDefinitions)
        {
            XDocument stylesPart = GetStylesDocument(document);
            foreach (XElement xmlStyleDefinition in xmlStyleDefinitions)
            {
                stylesPart.Root.Add(xmlStyleDefinition);
            }
        }

        /// <summary>
        /// Insert a style into a given xmlpath inside the document part
        /// </summary>
        /// <param name="xpathInsertionPoint">place where we are going to put the style</param>
        /// <param name="styleValue">name of the style</param>
        /// <param name="stylesSource">XDocument containing styles</param>
        public static OpenXmlPowerToolsDocument Insert(WmlDocument doc, string xpathInsertionPoint, string styleValue, XDocument stylesSource)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(doc))
            {
                using (WordprocessingDocument document = streamDoc.GetWordprocessingDocument())
                {
                    XDocument xDocument = document.MainDocumentPart.GetXDocument();
                    XmlDocument xmlMainDocument = new XmlDocument();
                    xmlMainDocument.Load(xDocument.CreateReader());

                    //  create the style element to add in the document, based upon the style name.
                    //  this is an example of an style element

                    //  <w:pStyle w:val="Heading1" /> 

                    //  so, in order to construct this, we have to know already if the style will be placed inside
                    //  a run or inside a paragraph. to know this we have to verify against the xpath, and know if
                    //  the query want to access a 'run' or a paragraph

                    XmlNamespaceManager namespaceManager = new XmlNamespaceManager(xmlMainDocument.NameTable);
                    namespaceManager.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");

                    XmlNodeList insertionPoints = xmlMainDocument.SelectNodes(xpathInsertionPoint, namespaceManager);

                    if (insertionPoints.Count == 0)
                        throw new ArgumentException("The xpath query did not return a valid location.");

                    foreach (XmlNode insertionPoint in insertionPoints)
                    {
                        XmlElement xmlStyle = null;

                        if (insertionPoint.LocalName == "r" || insertionPoint.LocalName == "p")
                        {
                            XmlNode propertiesElement = insertionPoint.SelectSingleNode(@"w:pPr|w:rPr", namespaceManager);

                            //if (propertiesElement != null)
                            //{
                            if (insertionPoint.Name == "w:p")
                            {
                                xmlStyle = xmlMainDocument.CreateElement("w", "pStyle", namespaceManager.LookupNamespace("w"));

                                //  retrieve the suffix from the styleAccesor class
                                xmlStyle.SetAttribute("val", namespaceManager.LookupNamespace("w"), styleValue + newStyleNameSuffix);

                                //  check if the rPr or pPr element exist, if so, then add the style xml element
                                //  inside, if not, then add a new rPr or pPr element
                                if (propertiesElement != null)
                                {
                                    //  check if there is already a style node and remove it
                                    XmlNodeList xmlStyleList = propertiesElement.SelectNodes("w:pStyle", namespaceManager);
                                    for (int i = 0; i < xmlStyleList.Count; i++)
                                    {
                                        propertiesElement.RemoveChild(xmlStyleList[i]);
                                    }
                                    propertiesElement.PrependChild(xmlStyle);
                                }
                                else
                                {
                                    propertiesElement = xmlMainDocument.CreateElement("w", "pPr", namespaceManager.LookupNamespace("w"));
                                    propertiesElement.PrependChild(xmlStyle);
                                    insertionPoint.PrependChild(propertiesElement);
                                }
                            }

                            if (insertionPoint.Name == "w:r")
                            {
                                xmlStyle = xmlMainDocument.CreateElement("w", "rStyle", namespaceManager.LookupNamespace("w"));
                                xmlStyle.SetAttribute("val", namespaceManager.LookupNamespace("w"), styleValue + newStyleNameSuffix);
                                if (propertiesElement != null)
                                {
                                    // check if there is already a style node and remove it
                                    XmlNodeList xmlStyleList = propertiesElement.SelectNodes("w:rStyle", namespaceManager);
                                    for (int i = 0; i < xmlStyleList.Count; i++)
                                    {
                                        propertiesElement.RemoveChild(xmlStyleList[i]);
                                    }
                                    propertiesElement.PrependChild(xmlStyle);
                                }
                                else
                                {
                                    propertiesElement = xmlMainDocument.CreateElement("w", "rPr", namespaceManager.LookupNamespace("w"));
                                    propertiesElement.PrependChild(xmlStyle);
                                    insertionPoint.PrependChild(propertiesElement);
                                }
                            }
                            //}
                        }
                        else
                        {
                            throw new ArgumentException("The xpath query did not return a valid location.");
                        }
                        XDocument xNewDocument = new XDocument();
                        using (XmlWriter xWriter = xNewDocument.CreateWriter())
                            xmlMainDocument.WriteTo(xWriter);
                        document.MainDocumentPart.PutXDocument(xNewDocument);

                        //  the style has been added in the main document part, but now we have to add the
                        //  style definition in the styles definitions part. the style definition need to be
                        //  extracted from the given inputStyle file.

                        //  Because a style can be linked with other styles and
                        //  can also be based on other styles,  all the complete hierarchy of styles has
                        //  to be added
                        Collection<XElement> styleHierarchy = StyleAccessor.GetStyleHierarchy(styleValue, stylesSource, newStyleNameSuffix);

                        //  open the styles file in the document
                        XDocument xmlStylesDefinitionDocument = StyleAccessor.GetStylesDocument(document);

                        XDocument xElem = new XDocument();
                        xElem.Add(xmlStylesDefinitionDocument.Root);
                        //insert the new style
                        foreach (XElement xmlStyleDefinition in styleHierarchy)
                        {
                            xElem.Root.Add(xmlStyleDefinition);
                        }
                        document.MainDocumentPart.StyleDefinitionsPart.PutXDocument(xElem);
                    }
                }
                return streamDoc.GetModifiedDocument();
            }
        }
    }
}
