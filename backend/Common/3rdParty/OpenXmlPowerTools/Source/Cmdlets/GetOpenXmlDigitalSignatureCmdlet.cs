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
    /// Gets a summary of digital signatures present inside a document
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "OpenXmlDigitalSignature")]
    [OutputType("string")]
    public class GetOpenXmlDigitalSignatureCmdlet : PowerToolsReadOnlyCmdlet
    {
        #region Cmdlet Overrides
        /// <summary>
        /// Entry point for Power Shell Cmdlets
        /// </summary>
        protected override void ProcessRecord()
        {
            foreach (var document in AllDocuments("Get-OpenXmlDigitalSignature"))
            {
                try
                {
                    WriteObject(DigitalSignatureAccessor.GetList(document), true);
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