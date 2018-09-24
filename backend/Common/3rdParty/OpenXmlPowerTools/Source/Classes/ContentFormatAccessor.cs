/***************************************************************************

Copyright (c) Microsoft Corporation 2011.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System;
using System.Xml;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;

namespace OpenXmlPowerTools
{
    /// <summary>
    /// Provides access to content format operations
    /// </summary>
    public class ContentFormatAccessor
    {
        /// <summary>
        /// Inserts Xml markup representing format attributes inside a specific paragraph or paragraph run
        /// </summary>
        /// <param name="document">Document to insert formatting Xml tags</param>
        /// <param name="xpathInsertionPoint">Paragraph or paragraph run to set format</param>
        /// <param name="content">Formatting tags</param>
        public static OpenXmlPowerToolsDocument Insert(WmlDocument doc, string xpathInsertionPoint, string content)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(doc))
            {
                using (WordprocessingDocument document = streamDoc.GetWordprocessingDocument())
                {
                    XDocument xDocument = document.MainDocumentPart.GetXDocument();
                    XmlDocument xmlMainDocument = new XmlDocument();
                    xmlMainDocument.Load(xDocument.CreateReader());

                    XmlNamespaceManager namespaceManager = new XmlNamespaceManager(new NameTable());
                    namespaceManager.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");

                    XmlNodeList insertionPoints = xmlMainDocument.SelectNodes(xpathInsertionPoint, namespaceManager);

                    if (insertionPoints.Count == 0)
                        throw new ArgumentException("The xpath query did not return a valid location.");

                    foreach (XmlNode insertionPoint in insertionPoints)
                    {
                        XmlNode propertiesElement = insertionPoint.SelectSingleNode(@"w:pPr|w:rPr", namespaceManager);
                        if (insertionPoint.Name == "w:p")
                        {
                            // Checks if the rPr or pPr element exists
                            if (propertiesElement == null)
                            {
                                propertiesElement = xmlMainDocument.CreateElement("w", "pPr", namespaceManager.LookupNamespace("w"));
                                insertionPoint.PrependChild(propertiesElement);
                            }
                        }
                        else if (insertionPoint.Name == "w:r")
                        {
                            // Checks if the rPr or pPr element exists
                            if (propertiesElement == null)
                            {
                                propertiesElement = xmlMainDocument.CreateElement("w", "rPr", namespaceManager.LookupNamespace("w"));
                                insertionPoint.PrependChild(propertiesElement);
                            }
                        }

                        if (propertiesElement != null)
                        {
                            propertiesElement.InnerXml += content;
                        }
                        else
                        {
                            throw new ArgumentException("Specified xpath query result is not a valid location to place a formatting markup");
                        }

                    }
                    XDocument xNewDocument = new XDocument();
                    using (XmlWriter xWriter = xNewDocument.CreateWriter())
                        xmlMainDocument.WriteTo(xWriter);
                    document.MainDocumentPart.PutXDocument(xNewDocument);
                }
                return streamDoc.GetModifiedDocument();
            }
        }
    }
}
