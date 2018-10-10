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
    public enum FooterType
    {
        Default,
        First,
        Even
    }

    /// <summary>
    /// Provides access to footer operations
    /// </summary>
    public class FooterAccessor
    {
        private static XNamespace ns;
        private static XNamespace relationshipns;

        static FooterAccessor()
        {
            ns = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            relationshipns = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
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
        /// Footer reference nodes inside the document
        /// </summary>
        /// <param name="type">The footer part type</param>
        /// <returns>XElement containing the part reference in the document</returns>
        private static XElement GetFooterReference(WordprocessingDocument document, FooterType type, int sectionIndex)
        {
            XDocument mainDocument = document.MainDocumentPart.GetXDocument();
            XName footerReferenceTag = ns + "footerReference";
            XName typeTag = ns + "type";
            string typeName = "";

            switch (type)
            {
                case FooterType.First: typeName = "first";
                    break;
                case FooterType.Even: typeName = "even";
                    break;
                case FooterType.Default: typeName = "default";
                    break;
            }

            XElement sectionPropertyElement = SectionPropertiesElements(document).Skip(sectionIndex).FirstOrDefault();
            if (sectionPropertyElement != null)
            {
                return sectionPropertyElement.Descendants().Where(tag => (tag.Name == footerReferenceTag) && (tag.Attribute(typeTag).Value == typeName)).FirstOrDefault();
            }
            return null;                    
        }

        /// <summary>
        /// Get the specified footer from the document
        /// </summary>
        /// <param name="type">The footer part type</param>
        /// <returns>the XDocument containing the footer</returns>
        public static XDocument GetFooter(WmlDocument doc, FooterType type, int sectionIndex)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(doc))
            using (WordprocessingDocument document = streamDoc.GetWordprocessingDocument())
            {
                OpenXmlPart footer = GetFooterPart(document, type, sectionIndex);
                if (footer != null)
                    return footer.GetXDocument();
                return null;
            }
        }


        /// <summary>
        /// The specified footer part from the document
        /// </summary>
        /// <param name="type">The footer part type</param>
        /// <returns>A OpenXmlPart containing the footer part</returns>
        public static OpenXmlPart GetFooterPart(WordprocessingDocument document, FooterType type, int sectionIndex)
        {
            // look in the section properties of the main document part, the respective footer
            // needed to extract
            XElement footerReferenceElement = GetFooterReference(document, type, sectionIndex);
            if (footerReferenceElement != null)
            {
                //  get the relation id of the footer part to extract from the document
                string relationshipId = footerReferenceElement.Attribute(relationshipns + "id").Value;
                return document.MainDocumentPart.GetPartById(relationshipId);
            }
            return null;
        }

        /// <summary>
        /// Removes the specified footer in the document
        /// </summary>
        /// <param name="type">The footer part type</param>
        public static void RemoveFooter(WmlDocument doc, FooterType type, int sectionIndex)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(doc))
            using (WordprocessingDocument document = streamDoc.GetWordprocessingDocument())
            {
                OpenXmlPart footerPart = GetFooterPart(document, type, sectionIndex);
                footerPart.RemovePart();
            }
        }

        /// <summary>
        /// Set a new footer in a document
        /// </summary>
        /// <param name="footer">XDocument containing the footer to add in the document</param>
        /// <param name="type">The footer part type</param>
        public static OpenXmlPowerToolsDocument SetFooter(WmlDocument doc, XDocument footer, FooterType type, int sectionIndex)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(doc))
            {
                using (WordprocessingDocument document = streamDoc.GetWordprocessingDocument())
                {
                    //  Removes the reference in the document.xml and the footer part if those already
                    //  exist
                    XElement footerReferenceElement = GetFooterReference(document, type, sectionIndex);
                    if (footerReferenceElement != null)
                    {
                        GetFooterPart(document, type, sectionIndex).RemovePart();
                        footerReferenceElement.Remove();
                    }

                    //  Add the new footer
                    FooterPart footerPart = document.MainDocumentPart.AddNewPart<FooterPart>();
                    footerPart.PutXDocument(footer);

                    //  If the document does not have a property section a new one must be created
                    if (SectionPropertiesElements(document).Count() == 0)
                    {
                        AddDefaultSectionProperties(document);
                    }

                    //  Creates the relationship of the footer inside the section properties in the document
                    string relID = document.MainDocumentPart.GetIdOfPart(footerPart);
                    string kindName = "";
                    switch ((FooterType)type)
                    {
                        case FooterType.First:
                            kindName = "first";
                            break;
                        case FooterType.Even:
                            kindName = "even";
                            break;
                        case FooterType.Default:
                            kindName = "default";
                            break;
                    }

                    XElement sectionPropertyElement = SectionPropertiesElements(document).Skip(sectionIndex).FirstOrDefault();
                    if (sectionPropertyElement != null)
                    {
                        sectionPropertyElement.Add(
                            new XElement(ns + "footerReference",
                                new XAttribute(ns + "type", kindName),
                                new XAttribute(relationshipns + "id", relID)));

                        if (sectionPropertyElement.Element(ns + "titlePg") == null)
                            sectionPropertyElement.Add(
                                new XElement(ns + "titlePg")
                                );
                    }
                    document.MainDocumentPart.PutXDocument();

                    // add in the settings part the EvendAndOddHeaders. this element
                    // allow to see the odd and even footers and headers in the document.
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
            mainDocument.Descendants(ns + "body").First().Add(
                new XElement(ns + "sectPr")
            );

        }
    }
}
