/***************************************************************************

Copyright (c) Microsoft Corporation 2011.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System.IO;
using System.Linq;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;

namespace OpenXmlPowerTools
{
    public partial class WmlDocument : OpenXmlPowerToolsDocument
    {
        public string GetBackgroundColor()
        {
            return BackgroundAccessor.GetBackgroundColor(this);
        }
        public string GetBackgroundImageFileName()
        {
            return BackgroundAccessor.GetImageFileName(this);
        }
        public void SaveBackgroundImageToFile(string fileName)
        {
            BackgroundAccessor.SaveImageToFile(this, fileName);
        }
        public WmlDocument SetBackgroundColor(string colorValue)
        {
            return (WmlDocument)BackgroundAccessor.SetColor(this, colorValue);
        }
        public WmlDocument SetBackgroundImage(string imagePath)
        {
            return (WmlDocument)BackgroundAccessor.SetImage(this, imagePath);
        }
    }

    /// <summary>
    /// Provides access to background operations
    /// </summary>
    public class BackgroundAccessor
    {
        private const string defaultBackgroundColor = "FFFFFF";
        private const string defaultBWMode = "white";
        private const string defaultTargetScreenSize = "800,600";
        private const string defaultVmlBackgroundImageId = "_x0000_s1025";
        private const string defaultImageRecolor = "t";
        private const string defaultImageType = "frame";

        private static XNamespace ns;
        private static XNamespace vmlns;
        private static XNamespace officens;
        private static XNamespace relationshipsns;


        public static string GetBackgroundColor(WmlDocument doc)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(doc))
            using (WordprocessingDocument document = streamDoc.GetWordprocessingDocument())
            {
                XDocument mainDocument = document.MainDocumentPart.GetXDocument();
                XElement backgroundElement = mainDocument.Descendants(W.background).FirstOrDefault();
                return (backgroundElement == null) ? string.Empty : backgroundElement.Attribute(W.color).Value;
            }
        }
        public static string GetImageFileName(WmlDocument doc)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(doc))
            using (WordprocessingDocument document = streamDoc.GetWordprocessingDocument())
            {
                XDocument mainDocument = document.MainDocumentPart.GetXDocument();
                XElement fillElement = mainDocument.Descendants(W.background).Descendants(VML.fill).FirstOrDefault();
                if (fillElement != null)
                {
                    string imageRelationshipId = fillElement.Attribute(R.id).Value;
                    OpenXmlPart imagePart = document.MainDocumentPart.GetPartById(imageRelationshipId);

                    // Gets the image name (path stripped)
                    string imagePath = imagePart.Uri.OriginalString;
                    return imagePath.Substring(imagePath.LastIndexOf('/') + 1);
                }
                else
                    return string.Empty;
            }
        }
        public static void SaveImageToFile(WmlDocument doc, string fileName)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(doc))
            using (WordprocessingDocument document = streamDoc.GetWordprocessingDocument())
            {
                XDocument mainDocument = document.MainDocumentPart.GetXDocument();
                XElement fillElement = mainDocument.Descendants(W.background).Descendants(VML.fill).FirstOrDefault();
                if (fillElement != null)
                {
                    string imageRelationshipId = fillElement.Attribute(R.id).Value;
                    OpenXmlPart imagePart = document.MainDocumentPart.GetPartById(imageRelationshipId);

                    // Gets the image name (path stripped)
                    string imagePath = imagePart.Uri.OriginalString;

                    // Writes the image outside the package
                    OpenXmlPowerToolsDocument.SavePartAs(imagePart, fileName);
                }
            }
        }

        static BackgroundAccessor()
        {
            ns = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            vmlns = "urn:schemas-microsoft-com:vml";
            officens = "urn:schemas-microsoft-com:office:office";
            relationshipsns = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
        }

        /// <summary>
        /// Nodes list with background elements
        /// </summary>
        private static XElement BackgroundElement(WordprocessingDocument document)
        {
            XDocument mainDocument = document.MainDocumentPart.GetXDocument();
            XElement result = mainDocument.Descendants(ns + "background").FirstOrDefault();
            return result;
        }

        /// <summary>
        /// Nodes list with background elements
        /// </summary>
        private static XElement BackgroundFillElement(WordprocessingDocument document)
        {
            XDocument mainDocument = document.MainDocumentPart.GetXDocument();
            XElement result = mainDocument.Descendants(ns + "background").Descendants(vmlns + "fill").FirstOrDefault();
            return result;
        }

        /// <summary>
        /// Sets the document background color
        /// </summary>
        /// <param name="colorValue">String representation of the hexadecimal RGB color</param>
        public static OpenXmlPowerToolsDocument SetColor(WmlDocument doc, string colorValue)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(doc))
            {
                using (WordprocessingDocument document = streamDoc.GetWordprocessingDocument())
                {
                    XDocument mainDocument = document.MainDocumentPart.GetXDocument();

                    // If the background element already exists, deletes it
                    XElement backgroundElement = BackgroundElement(document);
                    if (backgroundElement != null)
                        backgroundElement.Remove();

                    mainDocument.Root.AddFirst(
                        new XElement(ns + "background",
                            new XAttribute(ns + "color", colorValue)
                        )
                    );

                    // Enables background displaying by adding "displayBackgroundShape" tag
                    if (SettingAccessor.DisplayBackgroundShapeElements(document) == null)
                        SettingAccessor.AddBackgroundShapeElement(document);

                    document.MainDocumentPart.PutXDocument();
                }
                return streamDoc.GetModifiedDocument();
            }
        }

        /// <summary>
        /// Sets the document background image
        /// </summary>
        /// <param name="imagePath">Path of the background image</param>
        public static OpenXmlPowerToolsDocument SetImage(WmlDocument doc, string imagePath)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(doc))
            {
                using (WordprocessingDocument document = streamDoc.GetWordprocessingDocument())
                {
                    XDocument mainDocument = document.MainDocumentPart.GetXDocument();

                    // Adds the image to the package
                    ImagePart imagePart = document.MainDocumentPart.AddImagePart(ImagePartType.Bmp);
                    Stream imageStream = new StreamReader(imagePath).BaseStream;
                    byte[] imageBytes = new byte[imageStream.Length];
                    imageStream.Read(imageBytes, 0, imageBytes.Length);
                    imagePart.GetStream().Write(imageBytes, 0, imageBytes.Length);

                    // Creates a "background" element relating the image and the document

                    // If the background element already exists, deletes it
                    XElement backgroundElement = BackgroundElement(document);
                    if (backgroundElement != null)
                        backgroundElement.Remove();

                    // Background element construction
                    mainDocument.Root.Add(
                        new XElement(ns + "background",
                            new XAttribute(ns + "color", defaultBackgroundColor),
                            new XElement(vmlns + "background",
                                new XAttribute(vmlns + "id", defaultVmlBackgroundImageId),
                                new XAttribute(officens + "bwmode", defaultBWMode),
                                new XAttribute(officens + "targetscreensize", defaultTargetScreenSize),
                                new XElement(vmlns + "fill",
                                    new XAttribute(relationshipsns + "id", document.MainDocumentPart.GetIdOfPart(imagePart)),
                                    new XAttribute("recolor", defaultImageRecolor),
                                    new XAttribute("type", defaultImageType)
                                )
                            )
                        )
                    );


                    // Enables background displaying by adding "displayBackgroundShape" tag
                    if (SettingAccessor.DisplayBackgroundShapeElements(document) == null)
                        SettingAccessor.AddBackgroundShapeElement(document);

                    document.MainDocumentPart.PutXDocument();
                }
                return streamDoc.GetModifiedDocument();
            }
        }
    }
}
