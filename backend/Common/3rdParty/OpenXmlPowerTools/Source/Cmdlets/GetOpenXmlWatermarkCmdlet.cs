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
    /// Gets the text of a watermark in a document.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "OpenXmlWatermark")]
    [OutputType("string")]
    public class GetOpenXmlWatermarkCmdlet : PowerToolsReadOnlyCmdlet
    {
        #region Cmdlet Overrides

        /// <summary>
        /// Entry point for PowerShell cmdlets
        /// </summary>
        protected override void ProcessRecord()
        {
            foreach (var document in AllDocuments("Get-OpenXmlWatermark"))
            {
                try
                {
                    if (!(document is WmlDocument))
                        throw new PowerToolsDocumentException("Not a wordprocessing document.");
                    WriteObject(WatermarkAccessor.GetWatermarkText((WmlDocument)document), true);
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
