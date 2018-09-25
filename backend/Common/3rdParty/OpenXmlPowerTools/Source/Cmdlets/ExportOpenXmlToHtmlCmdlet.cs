/***************************************************************************

Copyright (c) Microsoft Corporation 2011.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Xml.Linq;

namespace OpenXmlPowerTools.Commands
{
    /// <summary>
    /// Transform-OpenXmlToHtml cmdlet	
    /// </summary>
    [Cmdlet(VerbsData.Export, "OpenXmlToHtml", SupportsShouldProcess = true)]
    public class ExportOpenXmlToHtmlCmdlet : PowerToolsReadOnlyCmdlet
    {
        #region Parameters

        [Parameter(
            Position = 1,
            Mandatory = true,
            HelpMessage = "Title for the HTML page")
        ]
        public string PageTitle;

        [Parameter(
            Position = 2,
            Mandatory = false,
            HelpMessage = "Name of the html output file")
        ]
        public string OutputPath;

        [Parameter(
            Position = 3,
            Mandatory = false,
            HelpMessage = "Folder where image files are going to be placed")
        ]
        public string ImageFolder;

        [Parameter(
            Position = 4,
            Mandatory = false,
            HelpMessage = "Prefix for image file names")
        ]
        public string ImagePrefix;

        [Parameter(
            Position = 5,
            Mandatory = false,
            HelpMessage = "CSS file name")
        ]
        public string CssPath;

        [Parameter(
            Position = 6,
            Mandatory = false,
            HelpMessage = "CSS class name prefix")
        ]
        public string CssClassPrefix;

        #endregion

        #region Cmdlet Overrides

        protected override void ProcessRecord()
        {
            foreach (var document in AllDocuments("Export-OpenXmlToHtml"))
            {
                try
                {
                    if (!(document is WmlDocument))
                        throw new PowerToolsDocumentException("Not a wordprocessing document.");
                    FileInfo info = null;
                    if (document.FileName != null)
                        info = new FileInfo(document.FileName);
                    string htmlFileName;
                    if (OutputPath != null)
                        htmlFileName = System.IO.Path.Combine(SessionState.Path.CurrentLocation.Path, OutputPath);
                    else
                    {
                        if (info == null)
                            throw new ArgumentException("No output file name available.");
                        htmlFileName = System.IO.Path.Combine(info.DirectoryName, info.Name.Substring(0, info.Name.Length - info.Extension.Length) + ".html");
                    }
                    string imageFolder;
                    FileInfo outputInfo = new FileInfo(htmlFileName);
                    if (ImageFolder != null)
                        imageFolder = SessionState.Path.GetResolvedPSPathFromPSPath(ImageFolder).First().Path;
                    else
                        imageFolder = System.IO.Path.Combine(outputInfo.DirectoryName, "images");
                    DirectoryInfo imageFolderInfo = new DirectoryInfo(imageFolder);
                    if (!imageFolderInfo.Exists)
                        imageFolderInfo.Create();
                    HtmlConverterSettings settings = new HtmlConverterSettings();
                    settings.PageTitle = PageTitle;
                    if (CssPath != null)
                    {
                        settings.CssClassPrefix = CssClassPrefix;
                        // Read CSS file into a string
                        string cssFileName = SessionState.Path.GetResolvedPSPathFromPSPath(CssPath).First().Path;
                        settings.Css = File.ReadAllText(cssFileName);
                    }
                    int imageCounter = 0;
                    XElement html = HtmlConverter.ConvertToHtml((WmlDocument)document, settings,
                    imageInfo =>
                    {
                        ++imageCounter;
                        string extension = imageInfo.ContentType.Split('/')[1].ToLower();
                        ImageFormat imageFormat = null;
                        if (extension == "png")
                        {
                            // Convert png to jpeg.
                            extension = "jpeg";
                            imageFormat = ImageFormat.Jpeg;
                        }
                        else if (extension == "bmp")
                            imageFormat = ImageFormat.Bmp;
                        else if (extension == "jpeg")
                            imageFormat = ImageFormat.Jpeg;
                        else if (extension == "tiff")
                            imageFormat = ImageFormat.Tiff;

                        // If the image format isn't one that we expect, ignore it,
                        // and don't return markup for the link.
                        if (imageFormat == null)
                            return null;

                        string imageFileName = System.IO.Path.Combine(imageFolder, "image" + imageCounter.ToString() + "." + extension);
                        try
                        {
                            imageInfo.Bitmap.Save(imageFileName, imageFormat);
                        }
                        catch (System.Runtime.InteropServices.ExternalException)
                        {
                            return null;
                        }
                        XElement img = new XElement(Xhtml.img,
                            new XAttribute(NoNamespace.src, imageFileName),
                            imageInfo.ImgStyleAttribute,
                            imageInfo.AltText != null ?
                                new XAttribute(NoNamespace.alt, imageInfo.AltText) : null);
                        return img;
                    });
                    File.WriteAllText(htmlFileName, html.ToStringNewLineOnAttributes());
                }
                catch (Exception e)
                {
                    WriteError(PowerToolsExceptionHandling.GetExceptionErrorRecord(e, document));
                }
            }
        }
        #endregion
    }
}
