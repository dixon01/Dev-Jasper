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
    /// <summary>
    /// Set-OpenXmlBackground cmdlet
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "OpenXmlBackground", SupportsShouldProcess = true)]
    [OutputType("OpenXmlPowerToolsDocument")]
    public class SetOpenXmlBackgroundCmdlet : PowerToolsModifierCmdlet
    {
        #region Parameters

        private string backgroundColor;
        private string backgroundImagePath;

        /// <summary>
        /// ImagePath parameter
        /// </summary>
        [Parameter(
            Mandatory = false,
            HelpMessage = "Path of image to set as document background")
        ]
        [ValidateNotNullOrEmpty]
        public string ImagePath
        {
            get
            {
                return backgroundImagePath;
            }
            set
            {
                backgroundImagePath = SessionState.Path.GetResolvedPSPathFromPSPath(value).First().Path;
            }
        }

        /// <summary>
        /// ColorName parameter
        /// </summary>
        [Parameter(
            Mandatory = false,
            HelpMessage = "Color code (hexadecimal) to set as the document background")
        ]
        [ValidateNotNullOrEmpty]
        public string ColorName
        {
            get
            {
                return backgroundColor;
            }
            set
            {
                backgroundColor = value;
            }
        }

        /// <summary>
        /// Color parameter
        /// </summary>
        [Parameter(
            Mandatory = false,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Color object to set as the document background")
        ]
        [ValidateNotNullOrEmpty]
        public System.Drawing.Color Color
        {
            get
            {
                return System.Drawing.Color.FromArgb(backgroundColor == null ? 0 : Convert.ToInt32(backgroundColor,16));
            }
            set
            {
                backgroundColor = String.Format("{0:X2}{1:X2}{2:X2}", value.R, value.G, value.B);
            }
        }

        /// <summary>
        /// ImageFile parameter
        /// </summary>
        [Parameter(
            Mandatory = false,
            ValueFromPipeline = true,
            HelpMessage = "Image object to use as document background")
        ]
        [ValidateNotNullOrEmpty]
        public System.IO.FileInfo ImageFile
        {
            get
            {
                return new System.IO.FileInfo(backgroundImagePath == null ? "." : backgroundImagePath);
            }
            set
            {
                backgroundImagePath = value.FullName;
            }
        }

        #endregion

        #region Cmdlet Overrides

        /// <summary>
        /// Entry point for PowerShell cmdlets
        /// </summary>
        protected override void ProcessRecord()
        {
            // If an image was piped in, the background color string will not be valid.
            if (backgroundImagePath != null)
                backgroundColor = null;
            // At least one of the backgroundColor or backgroundImage parameters must be set it
            if (backgroundColor == null && backgroundImagePath == null)
            {
                WriteError(new ErrorRecord(new ArgumentException("Requires at least one of the three parameters: Color, Image or ImagePath."), "OpenXmlPowerTools", ErrorCategory.InvalidArgument, null));
                return;
            }

            foreach (var document in AllDocuments("Set-OpenXmlBackground"))
            {
                try
                {
                    if (!(document is WmlDocument))
                        throw new PowerToolsDocumentException("Not a wordprocessing document.");
                    if (backgroundImagePath != null)
                    {
                        // Open as image to verify that it is valid
                        System.Drawing.Image.FromFile(backgroundImagePath);
                        OutputDocument(BackgroundAccessor.SetImage((WmlDocument)document, backgroundImagePath));
                    }
                    else
                    {
                        // Validate color value
                        System.Drawing.Color.FromArgb(Convert.ToInt32(backgroundColor, 16));
                        OutputDocument(BackgroundAccessor.SetColor((WmlDocument)document, backgroundColor));
                    }
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
