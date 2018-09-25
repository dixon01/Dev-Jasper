/***************************************************************************

Copyright (c) Microsoft Corporation 2011.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System;
using System.Linq;
using System.IO.Packaging;
using System.Management.Automation;

namespace OpenXmlPowerTools.Commands
{
    /// <summary>
    /// Set-OpenXmlTheme cmdlet
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "OpenXmlTheme", SupportsShouldProcess = true)]
    [OutputType("OpenXmlPowerToolsDocument")]
    public class SetOpenXmlThemeCmdlet : PowerToolsModifierCmdlet
    {
        #region Parameters

        private string themePath;
        private OpenXmlPowerToolsDocument themePackage;

        /// <summary>
        /// ThemePath parameter
        /// </summary>
        [Parameter(Position = 2,
            Mandatory = false,
            HelpMessage = "Theme path")]
        [ValidateNotNullOrEmpty]
        public string ThemePath
        {
            set
            {
                themePath = SessionState.Path.GetResolvedPSPathFromPSPath(value).First().Path;
            }
        }

        /// <summary>
        /// ThemePackage parameter
        /// </summary>
        [Parameter(
           Mandatory = false,
           ValueFromPipeline = true,
           HelpMessage = "Theme path")]
        [ValidateNotNullOrEmpty]
        public OpenXmlPowerToolsDocument ThemePackage
        {
            get
            {
                return themePackage;
            }
            set
            {
                themePackage = value;
            }
        }

        #endregion

        #region Cmdlet Overrides

        /// <summary>
        /// Entry point for PowerShell cmdlets
        /// </summary>
        protected override void ProcessRecord()
        {
            if (themePath != null)
                themePackage = OpenXmlPowerToolsDocument.FromFileName(themePath);
            if (themePackage == null)
            {
                WriteError(new ErrorRecord(new ArgumentException("No theme was specified."), "OpenXmlPowerTools", ErrorCategory.InvalidArgument, null));
            }
            else
            {
                foreach (var document in AllDocuments("Set-OpenXmlTheme"))
                {
                    try
                    {
                        if (!(document is WmlDocument))
                            throw new PowerToolsDocumentException("Not a wordprocessing document.");
                        OutputDocument(ThemeAccessor.SetTheme((WmlDocument)document, themePackage));
                    }
                    catch (Exception e)
                    {
                        WriteError(PowerToolsExceptionHandling.GetExceptionErrorRecord(e, document));
                    }
                }
            }
        }
        #endregion
    }
}