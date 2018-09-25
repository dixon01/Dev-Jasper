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
    /// Get-OpenXmlDocument Cmdlet
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "OpenXmlDocument")]
    [OutputType("OpenXmlPowerToolsDocument")]
    public class GetOpenXmlDocumentCmdlet : PowerToolsReadOnlyCmdlet
    {
        #region Cmdlet Overrides

        protected override void ProcessRecord()
        {
            foreach (var document in AllDocuments("Get-OpenXmlCustomXmlData"))
            {
                try
                {
                    WriteObject(new OpenXmlPowerToolsDocument(document), true);
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