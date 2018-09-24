/***************************************************************************

Copyright (c) Microsoft Corporation 2011.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Packaging;

namespace OpenXmlPowerTools
{
    public enum HeaderType
    {
        Default,
        First,
        Even
    }

    /// <summary>
    /// Provides access to header operations
    /// </summary>
    public class HeaderAccessor
    {
        private const string defaultHeaderType = "default";
        private static XNamespace ns;
        private static XNamespace relationshipns;
        private static XNamespace officens;
        private static XNamespace vmlns;
        private static XNamespace wordns;

        static HeaderAccessor() {
            ns = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";        
            relationshipns =  "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
            officens = "urn:schemas-microsoft-com:office:office";
            vmlns = "urn:schemas-microsoft-com:vml";
            wordns = "urn:schemas-microsoft-com:office:word";
        }

        /// <summary>
        /// Elements tagged as section properties
        /// </summary>
        /// <returns>IEnumerable&lt;XElement&gt; containing all the section properties elements found it in the document</returns>
        private static IEnumerable<XElement> SectionPropertiesElements(WordprocessingDocument document)
        {
            XDocument mainDocument = document.MainDocumentPart.GetXDocument();
            IEnumerable<XElement> results =
                mainDocument
                .Descendants(ns + "p")
                .Elements(ns + "pPr")
                .Elements(ns + "sectPr");
            if (results.Count() == 0)
                results = mainDocument.Root.Elements(ns + "body").Elements(ns + "sectPr");
            return results;
        }

        /// <summary>
        /// Adds a new header reference in the section properties
        /// </summary>
        /// <param name="type">The header part type</param>
        /// <param name="headerPartId">the header part id</param>
        public static void AddHeaderReference(WordprocessingDocument document, HeaderType type, string headerPartId, int sectionIndex)
        {
            //  If the document does not have a property section a new one must be created
            if (SectionPropertiesElements(document).Count() == 0)
            {
                AddDefaultSectionProperties(document);
            }

            string typeName = "";
            switch ((HeaderType)type)
            {
                case HeaderType.First:
                    typeName = "first";
                    break;
                case HeaderType.Even:
                    typeName = "even";
                    break;
                case HeaderType.Default:
                    typeName = "default";
                    break;
            }

            XElement sectionPropertyElement = SectionPropertiesElements(document).Skip(sectionIndex).FirstOrDefault();
            if (sectionPropertyElement != null)
            {
                sectionPropertyElement.Add(
                    new XElement(ns + "headerReference",
                        new XAttribute(ns + "type", typeName),
                        new XAttribute(relationshipns + "id", headerPartId)));

                if (sectionPropertyElement.Element(ns + "titlePg") == null)
                    sectionPropertyElement.Add(
                        new XElement(ns + "titlePg")
                        );
            }
            document.MainDocumentPart.PutXDocument();
        }

        /// <summary>
        /// Adds a new header part in the document
        /// </summary>
        /// <param name="type">The footer part type</param>
        /// <returns>A XDocument contaning the added header</returns>
        public static XDocument AddNewHeader(WordprocessingDocument document, HeaderType type)
        {
            // Creates the new header part
            HeaderPart newHeaderPart = document.MainDocumentPart.AddNewPart<HeaderPart>();

            XDocument emptyHeader = CreateEmptyHeaderDocument();
            newHeaderPart.PutXDocument(emptyHeader);

            string newHeaderPartId = document.MainDocumentPart.GetIdOfPart(newHeaderPart);
            AddHeaderReference(document, type, newHeaderPartId, 0);

            return emptyHeader;
        }

        /// <summary>
        /// Creates an empty header document
        /// </summary>
        /// <returns>A XDocument containing the xml of an empty header part </returns>
        private static XDocument CreateEmptyHeaderDocument()
        {
            return new XDocument(
                new XElement(ns + "hdr",
                    new XAttribute(XNamespace.Xmlns + "w", ns),
                    new XAttribute(XNamespace.Xmlns + "r", relationshipns),
                    new XAttribute(XNamespace.Xmlns + "o", officens),
                    new XAttribute(XNamespace.Xmlns + "v", vmlns),
                    new XAttribute(XNamespace.Xmlns + "w10", wordns)
                )
            );
        }

        /// <summary>
        /// Header reference nodes inside the document
        /// </summary>
        /// <param name="type">The header part type</param>
        /// <returns>XElement containing the part reference in the document</returns>
        public static XElement GetHeaderReference(WordprocessingDocument document, HeaderType type, int sectionIndex)
        {
            XDocument mainDocument = document.MainDocumentPart.GetXDocument();
            XName headerReferenceTag = ns + "headerReference";
            XName typeTag = ns + "type";
            string typeName = "";

            switch (type)
            {
                case HeaderType.First: typeName = "first";
                    break;
                case HeaderType.Even: typeName = "even";
                    break;
                case HeaderType.Default: typeName = "default";
                    break;
            }

            XElement sectionPropertyElement = SectionPropertiesElements(document).Skip(sectionIndex).FirstOrDefault();
            if (sectionPropertyElement != null)
            {
                return sectionPropertyElement.Descendants().Where(tag => (tag.Name == headerReferenceTag) && (tag.Attribute(typeTag).Value == typeName)).FirstOrDefault();
            }
            return null;
        }

        /// <summary>
        /// Get the specified header from the document
        /// </summary>
        /// <param name="type">The header part type</param>
        /// <returns>A XDocument containing the header</returns>
        public static XDocument GetHeader(WmlDocument doc, HeaderType type, int sectionIndex)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(doc))
            using (WordprocessingDocument document = streamDoc.GetWordprocessingDocument())
            {
                OpenXmlPart header = GetHeaderPart(document, type, sectionIndex);
                if (header != null)
                    return header.GetXDocument();
                return null;
            }
        }

        public static XDocument GetHeader(WordprocessingDocument document, HeaderType type, int sectionIndex)
        {
            OpenXmlPart header = GetHeaderPart(document, type, sectionIndex);
            if (header != null)
                return header.GetXDocument();
            return null;
        }


        /// <summary>
        /// The specified header part from the document
        /// </summary>
        /// <param name="type">The header part type</param>
        /// <returns>A OpenXmlPart containing the header part</returns>
        public static OpenXmlPart GetHeaderPart(WordprocessingDocument document, HeaderType type, int sectionIndex)
        {
            // look in the section properties of the main document part, the respective Header
            // needed to extract
            XElement headerReferenceElement = GetHeaderReference(document, type, sectionIndex);
            if (headerReferenceElement != null)
            {
                //  get the relation id of the Header part to extract from the document
                string relationId = headerReferenceElement.Attribute(relationshipns + "id").Value;
                return document.MainDocumentPart.GetPartById(relationId);
            }
            else
                return null;
        }

        /// <summary>
        /// Removes the specified header in the document
        /// </summary>
        /// <param name="type">The header part type</param>
        public void RemoveHeader(WmlDocument doc, HeaderType type, int sectionIndex)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(doc))
            using (WordprocessingDocument document = streamDoc.GetWordprocessingDocument())
            {
                OpenXmlPart headerPart = GetHeaderPart(document, type, sectionIndex);
                headerPart.RemovePart();
            }
        }

        /// <summary>
        /// Set a new header in a document
        /// </summary>
        /// <param name="header">XDocument containing the header to add in the document</param>
        /// <param name="type">The header part type</param>
        public static OpenXmlPowerToolsDocument SetHeader(WmlDocument doc, XDocument header, HeaderType type, int sectionIndex)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(doc))
            {
                using (WordprocessingDocument document = streamDoc.GetWordprocessingDocument())
                {
                    //  Removes the reference in the document.xml and the header part if those already
                    //  exist
                    XElement headerReferenceElement = GetHeaderReference(document, type, sectionIndex);
                    if (headerReferenceElement != null)
                    {
                        GetHeaderPart(document, type, sectionIndex).RemovePart();
                        headerReferenceElement.Remove();
                    }

                    //  Add the new header
                    HeaderPart headerPart = document.MainDocumentPart.AddNewPart<HeaderPart>();
                    headerPart.PutXDocument(header);

                    //  Creates the relationship of the header inside the section properties in the document
                    string relID = document.MainDocumentPart.GetIdOfPart(headerPart);
                    AddHeaderReference(document, type, relID, sectionIndex);

                    // add in the settings part the EvendAndOddHeaders. this element
                    // allow to see the odd and even headers and headers in the document.
                    SettingAccessor.AddEvenAndOddHeadersElement(document);
                }
                return streamDoc.GetModifiedDocument();
            }
        }

        /// <summary>
        /// Adds a default sectPr element into the main document
        /// </summary>
        private static void AddDefaultSectionProperties(WordprocessingDocument document)
        {
            XDocument mainDocument = document.MainDocumentPart.GetXDocument();
            mainDocument.Element(ns + "body").Add(
                new XElement(ns + "sectPr")
            );
        }
    }
}
