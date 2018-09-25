/***************************************************************************

Copyright (c) Microsoft Corporation 2011.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;

namespace OpenXmlPowerTools
{
    /// <summary>
    /// process available index references
    /// </summary>
    public class IndexAccessor
    {
        private static XNamespace ns;

        static IndexAccessor()
        {
            ns = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
        }

        public static void Generate(WordprocessingDocument document)
        {
            XElement Index = new XElement("Index");

            XElement IndexFirstPart =
                        new XElement(ns + "p",
                            new XElement(ns + "r",
                                new XElement(ns + "fldChar",
                                    new XAttribute(ns + "fldCharType", "begin")),
                                new XElement(ns + "instrText",
                                    new XAttribute(XNamespace.Xml + "space", "preserve"),
                                    @" INDEX \h ""A"" \c ""2"" \z ""1033"" "),
                                new XElement(ns + "fldChar",
                                    new XAttribute(ns + "fldCharType", "separate"))));

            Index.Add(IndexFirstPart);

            //  Build the index with the IndexReferences
            foreach (XElement reference in IndexReferences(document))
            {
                //string fieldCode = GetFieldCode(reference);
                string mainEntry = reference.Value;//GetIndexMainEntry(reference.Value);

                //  Build the XElement containing the index reference
                XElement IndexElement =
                    new XElement(ns + "p",
                        new XElement(ns + "pPr",
                            new XElement(ns + "pStyle",
                                new XAttribute(ns + "val", "Index1")),
                                new XElement(ns + "tabs",
                                    new XElement(ns + "tab",
                                        new XAttribute(ns + "val", "right"),
                                        new XAttribute(ns + "leader", "dot"),
                                        new XAttribute(ns + "pos", "9350")))),
                        new XElement(ns + "r",
                            new XElement(ns + "t", mainEntry)));

                Index.Add(IndexElement);
            }

            //  Close the open character field
            Index.Add(
                new XElement(ns + "p",
                    new XElement(ns + "r",
                        new XElement(ns + "fldChar",
                            new XAttribute(ns + "fldCharType", "end")))));

            XDocument mainDocumentPart = document.MainDocumentPart.GetXDocument();
            foreach (XElement IndexElement in Index.Elements())
            {
                mainDocumentPart.Descendants(ns + "body").First().Add(IndexElement);
            }
            document.MainDocumentPart.PutXDocument();
        }

        private static IEnumerable<XElement> IndexReferences(WordprocessingDocument document)
        {
            XDocument mainDocument = document.MainDocumentPart.GetXDocument();
            IEnumerable<XElement> results =
                mainDocument
                .Descendants(ns + "p")
                .Elements(ns + "r")
                .Where(
                    r =>
                        r.Elements(ns + "instrText").Count() > 0 && 
                        r.ElementsBeforeSelf().Last().Element(ns + "instrText")!= null &&
                        r.ElementsBeforeSelf().Last().Element(ns + "instrText").Value.EndsWith("\"") && 
                        r.ElementsAfterSelf().First().Element(ns + "instrText") != null &&
                        r.ElementsAfterSelf().First().Element(ns + "instrText").Value.StartsWith("\"")
                );

            return results;
        }
    }
}
