/***************************************************************************

Copyright (c) Microsoft Corporation 2011.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System.Linq;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;

namespace OpenXmlPowerTools
{
    /// <summary>
    /// Provides access to setting operations
    /// </summary>
    public class SettingAccessor
    {
        private static XNamespace ns;
        private static XNamespace settingsns;
        private static XNamespace relationshipns;

        static SettingAccessor()
        {
            ns = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            settingsns = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/settings";
            relationshipns = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
        }

        /// <summary>
        /// Nodes list with displayBackgroundShape elements
        /// </summary>
        public static XElement DisplayBackgroundShapeElements(WordprocessingDocument document)
        {
            XDocument settingsDocument = document.MainDocumentPart.DocumentSettingsPart.GetXDocument();
            return settingsDocument.Descendants(settingsns + "displayBackgroundShape").FirstOrDefault();
        }

        /// <summary>
        /// Adds a displayBackgroundShape element to the settings file
        /// </summary>
        public static void AddBackgroundShapeElement(WordprocessingDocument document)
        {
            XDocument settingsDocument = document.MainDocumentPart.DocumentSettingsPart.GetXDocument();
            settingsDocument.Root.Add(
                new XElement(ns + "displayBackgroundShape")
            );
            document.MainDocumentPart.DocumentSettingsPart.PutXDocument();
        }

        /// <summary>
        /// Adds a the evenAndOddHeaders element, which allows to define distinct headers and footers for odd and even pages
        /// </summary>
        public static void AddEvenAndOddHeadersElement(WordprocessingDocument document)
        {
            XDocument settingsDocument = document.MainDocumentPart.DocumentSettingsPart.GetXDocument();
            if (settingsDocument.Descendants(ns + "evenAndOddHeaders").FirstOrDefault() == null)
            {
                settingsDocument.Root.Add(
                    new XElement(ns + "evenAndOddHeaders"));
                document.MainDocumentPart.DocumentSettingsPart.PutXDocument();
            }
        }

        /// <summary>
        /// Creates an empty base structure for a settings part
        /// </summary>
        /// <returns></returns>
        private static XDocument CreateEmptySettings()
        {
            XDocument document =
                new XDocument(
                    new XElement(ns + "settings",
                        new XAttribute(XNamespace.Xmlns + "w", ns),
                        new XAttribute(XNamespace.Xmlns + "r", relationshipns)
                    )
                );
            return document;
        }
    }
}
