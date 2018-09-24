/***************************************************************************

Copyright (c) Microsoft Corporation 2011.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System;
using System.Management.Automation;

namespace OpenXmlPowerTools.Commands
{
    /// <summary>
    /// Get-OpenXmlTheme cmdlet
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "OpenXmlTheme")]
    [OutputType("OpenXmlPowerToolsDocument")]
    public class GetOpenXmlThemeCmdlet : PowerToolsReadOnlyCmdlet
    {
        #region Parameters

        [Parameter(
            Mandatory = false,
            HelpMessage = "Use this switch to pipe out the extracted theme.")
        ]
        public SwitchParameter PassThru;

        [Parameter(Position = 1,
            Mandatory = false,
            ValueFromPipeline = false,
            HelpMessage = "Path of folder to store result documents")
        ]
        [ValidateNotNullOrEmpty]
        public string OutputPath;

        #endregion

        #region Cmdlet Overrides

        protected override void ProcessRecord()
        {
            foreach (var document in AllDocuments("Get-OpenXmlTheme"))
            {
                try
                {
                    if (!(document is WmlDocument))
                        throw new PowerToolsDocumentException("Not a wordprocessing document.");
                    OpenXmlPowerToolsDocument theme = ThemeAccessor.GetTheme((WmlDocument)document);
                    if (OutputPath != null)
                        theme.SaveAs(System.IO.Path.Combine(SessionState.Path.CurrentLocation.Path, OutputPath));
                    if (PassThru || OutputPath == null)
                        WriteObject(theme, true);
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