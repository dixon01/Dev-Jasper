/***************************************************************************

Copyright (c) Microsoft Corporation 2011.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System;
using System.Linq;
using System.Management.Automation;

namespace OpenXmlPowerTools.Commands
{
    [Cmdlet(VerbsCommon.Add, "OpenXmlPicture", SupportsShouldProcess = true)]
    [OutputType("OpenXmlPowerToolsDocument")]
    public class AddOpenXmlPictureCmdlet : PowerToolsModifierCmdlet
    {
        #region Parameters

        private string xpathInsertionPoint;
        private string picturePath;
        private System.Drawing.Image image;

        [Parameter(Position = 2, Mandatory = true, HelpMessage = "Insertion point location")]
        [ValidateNotNullOrEmpty]
        public string InsertionPoint
        {
            get
            {
                return xpathInsertionPoint;
            }
            set
            {
                xpathInsertionPoint = value;
            }
        }

        [Parameter(Position = 3, Mandatory = false, HelpMessage = "Picture path")]
        [ValidateNotNullOrEmpty]
        public string PicturePath
        {
            get
            {
                return picturePath;
            }
            set
            {
                picturePath = SessionState.Path.GetResolvedPSPathFromPSPath(value).First().Path;
            }
        }

        /// <summary>
        /// ImageFile parameter
        /// </summary>
        [Parameter(
            Mandatory = false,
            ValueFromPipeline = true,
            HelpMessage = "Image object to be inserted")
        ]
        [ValidateNotNullOrEmpty]
        public System.IO.FileInfo ImageFile
        {
            get
            {
                return new System.IO.FileInfo(picturePath == null ? "." : picturePath);
            }
            set
            {
                picturePath = value.FullName;
            }
        }

        #endregion

        #region Cmdlet Overrides

        /// <summary>
        /// Entry point for PowerShell cmdlets
        /// </summary>
        protected override void ProcessRecord()
        {
            string picName = "PowerToolsPicture";
            if (image == null && picturePath != null)
            {
                image = System.Drawing.Image.FromFile(picturePath, true);
                picName = picturePath.Substring(picturePath.LastIndexOf('\\') + 1);
            }
            foreach (var document in AllDocuments("Add-OpenXmlPicture"))
            {
                try
                {
                    if (!(document is WmlDocument))
                        throw new PowerToolsDocumentException("Not a wordprocessing document.");
                    OutputDocument(PictureAccessor.Insert((WmlDocument)document, xpathInsertionPoint, image, picName));
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
