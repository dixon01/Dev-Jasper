/***************************************************************************

Copyright (c) Microsoft Corporation 2011.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System.Linq;
using System.Xml.Linq;
using System.Xml;
using DocumentFormat.OpenXml.Packaging;

namespace OpenXmlPowerTools
{
    /// <summary>
    /// Class to manage the Style Part for an SpreadSheetML document
    /// </summary>
    public class SpreadSheetStyleAccessor
    {
        private static XNamespace ns;

        /// <summary>
        /// Static constructor
        /// </summary>
        static SpreadSheetStyleAccessor()
        {
            ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        }

        /// <summary>
        /// XDocument containing Xml content of the styles part
        /// </summary>
        public static XDocument GetStylesDocument(SpreadsheetDocument document)
        { 
            if (document.WorkbookPart.WorkbookStylesPart != null)
                return document.WorkbookPart.WorkbookStylesPart.GetXDocument();
            else
                return null;
        }

        /// <summary>
        /// Sets a new styles part inside the document
        /// </summary>
        /// <param name="newStylesDocument">Path of styles definition file</param>
        public static void SetStylePart(SpreadsheetDocument document, XDocument newStylesDocument)
        {
            try
            {
                // Replaces XDocument with the style file to transfer
                XDocument stylesDocument = GetStylesDocument(document);
                if (stylesDocument == null)
                {
                    WorkbookStylesPart stylesPart =
                        document.WorkbookPart.AddNewPart<WorkbookStylesPart>();
                    stylesDocument = stylesPart.GetXDocument();
                }
                if (stylesDocument.Root == null)
                    stylesDocument.Add(newStylesDocument.Root);
                else
                    stylesDocument.Root.ReplaceWith(newStylesDocument.Root);
                document.WorkbookPart.WorkbookStylesPart.PutXDocument();
            }
            catch (XmlException ex)
            {
                throw new XmlException("File specified is not a valid XML file", ex);
            }
        }

        /// <summary>
        /// Returns the index inside the style part for a specific cell style
        /// </summary>
        /// <param name="cellStyle">Name for cell style to return the index of</param>
        /// <returns></returns>
        public static int GetCellStyleIndex(SpreadsheetDocument document, string cellStyle)
        {
            XDocument stylesXDocument = GetStylesDocument(document);
            if (stylesXDocument == null)
                return -1;
            var cellStyleXElement =
                stylesXDocument.Root
                .Element(ns + "cellStyles")
                .Elements(ns + "cellStyle")
                .Where(c=> c.Attribute("name").Value.ToLower().Equals(cellStyle.ToLower())).FirstOrDefault<XElement>();

            if (cellStyleXElement != null)
            {
                return System.Convert.ToInt32(cellStyleXElement.Attribute("xfId").Value);
            }
            return -1;
        }

    }
}
