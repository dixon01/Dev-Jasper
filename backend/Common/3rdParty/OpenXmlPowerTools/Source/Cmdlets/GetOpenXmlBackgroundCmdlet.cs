/***************************************************************************

Copyright (c) Microsoft Corporation 2011.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System;
using System.IO;
using System.Management.Automation;

namespace OpenXmlPowerTools.Commands
{
    /// <summary>
    /// Get-OpenXmlBackground cmdlet
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "OpenXmlBackground", SupportsShouldProcess = true)]
    public class GetOpenXmlBackgroundCmdlet : PowerToolsReadOnlyCmdlet
    {
        private bool backgroundColor;
        private bool backgroundImage;
        private string outputFolder;

        #region Parameters

        [Parameter(Position = 1,
            Mandatory = false,
            ValueFromPipeline = false,
            HelpMessage = "Path of folder to store result documents")
        ]
        public string OutputFolder
        {
            get
            {
                return outputFolder;
            }
            set
            {
                outputFolder = SessionState.Path.Combine(SessionState.Path.CurrentLocation.Path, value);
            }
        }

        [Parameter(
           Mandatory = false,
           HelpMessage = "Gets the image from the background.")
        ]
        [ValidateNotNullOrEmpty]
        public SwitchParameter Image
        {
            get
            {
                return backgroundImage;
            }
            set
            {
                backgroundImage = value;
            }
        }

        [Parameter(
            Mandatory = false,
            HelpMessage = "Gets the color of the background (hexadecimal format)")
        ]
        [ValidateNotNullOrEmpty]
        public SwitchParameter Color
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

        #endregion

        #region Cmdlet Overrides

        protected override void ProcessRecord()
        {
            if (backgroundColor || backgroundImage)
            {
                foreach (var document in AllDocuments("Get-OpenXmlBackground"))
                {
                    try
                    {
                        if (!(document is WmlDocument))
                            throw new PowerToolsDocumentException("Not a wordprocessing document.");
                        string bgColor = string.Empty;
                        string bgImagePath = string.Empty;
                        if (backgroundColor)
                        {
                            bgColor = BackgroundAccessor.GetBackgroundColor((WmlDocument)document);
                            if (bgColor != "")
                                WriteObject(System.Drawing.Color.FromArgb(Convert.ToInt32(bgColor, 16)), true);
                        }
                        else if (backgroundImage)
                        {
                            string filename = BackgroundAccessor.GetImageFileName((WmlDocument)document);
                            if (filename != "")
                            {
                                string target = filename;
                                if (OutputFolder != null)
                                {
                                    FileInfo temp = new FileInfo(filename);
                                    target = OutputFolder + "\\" + temp.Name;
                                }
                                if (!File.Exists(target) || ShouldProcess(target, "Get-OpenXmlBackground"))
                                {
                                    BackgroundAccessor.SaveImageToFile((WmlDocument)document, target);
                                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(filename);
                                    WriteObject(fileInfo, true);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        WriteError(PowerToolsExceptionHandling.GetExceptionErrorRecord(e, document));
                    }
                }
            }
            else
                WriteError(new ErrorRecord(new ArgumentException("Requires one of the two switches: Color or Image."), "OpenXmlPowerToolsError", ErrorCategory.InvalidArgument, null));
        }

        #endregion
    }
}
