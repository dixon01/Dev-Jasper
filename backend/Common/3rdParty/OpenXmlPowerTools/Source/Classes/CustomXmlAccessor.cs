/***************************************************************************

Copyright (c) Microsoft Corporation 2011.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System;
using System.Linq;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;

namespace OpenXmlPowerTools
{
    /// <summary>
    /// Performs operations on document custom xml
    /// </summary>
    public class CustomXmlAccessor{
        /// <summary>
        /// Searches for a custom Xml part with a given name
        /// </summary>
        /// <param name="xmlPartName">Name of custom Xml part</param>
        /// <returns>XDocument with customXml part loaded</returns>
        public static XDocument Find(WmlDocument doc, string xmlPartName)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(doc))
            {
                WordprocessingDocument document = streamDoc.GetWordprocessingDocument();
                string partName = "/" + xmlPartName;
                var customXmlPart =
                    document.MainDocumentPart.CustomXmlParts.Where(
                        t => t.Uri.OriginalString.EndsWith(partName, System.StringComparison.OrdinalIgnoreCase)
                    ).FirstOrDefault();
                if (customXmlPart == null)
                    throw new ArgumentException("Part name '" + xmlPartName + "' not found.");
                return customXmlPart.GetXDocument();
            }
        }

        /// <summary>
        /// Replaces a previously existing customXml part by another one
        /// </summary>
        /// <param name="customXmlDocument">XDocument of part to replace inside the document package</param>
        /// <param name="partNameOnly">Name of the part.</param>
        public static OpenXmlPowerToolsDocument SetDocument(WmlDocument doc, XDocument customXmlDocument, string partNameOnly)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(doc))
            {
                using (WordprocessingDocument document = streamDoc.GetWordprocessingDocument())
                {
                    string partName = "/" + partNameOnly;
                    var customXmlPart = document.MainDocumentPart.CustomXmlParts.Where(
                            t => t.Uri.OriginalString.EndsWith(partName, System.StringComparison.OrdinalIgnoreCase)
                        ).FirstOrDefault();

                    if (customXmlPart == null)
                        customXmlPart = document.MainDocumentPart.AddCustomXmlPart(CustomXmlPartType.CustomXml);
                    customXmlPart.PutXDocument(customXmlDocument);
                }
                return streamDoc.GetModifiedDocument();
            }
        }
    }
}
