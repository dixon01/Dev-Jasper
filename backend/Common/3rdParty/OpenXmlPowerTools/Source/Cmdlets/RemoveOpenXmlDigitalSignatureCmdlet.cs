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
    /// Remove-OpenXmlDigitalSignature cmdlet	
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "OpenXmlDigitalSignature", SupportsShouldProcess = true)]
    [OutputType("OpenXmlPowerToolsDocument")]
    public class RemoveOpenXmlDigitalSignaturesCmdlet : PowerToolsModifierCmdlet
    {
        #region Cmdlet Overrides

        /// <summary>
        /// Entry point for PowerShell cmdlets
        /// </summary>
        protected override void ProcessRecord()
        {
            foreach (var document in AllDocuments("Remove-OpenXmlDigitalSignature"))
            {
                try
                {
                    OutputDocument(DigitalSignatureAccessor.RemoveAll(document));
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