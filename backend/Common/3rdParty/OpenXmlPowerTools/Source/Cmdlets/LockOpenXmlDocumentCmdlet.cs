/***************************************************************************

Copyright (c) Microsoft Corporation 2011.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System;
using System.Management.Automation;
using DocumentFormat.OpenXml.Packaging;

namespace OpenXmlPowerTools.Commands
{
    [Cmdlet(VerbsCommon.Lock, "OpenXmlDocument", SupportsShouldProcess = true)]
    [OutputType("OpenXmlPowerToolsDocument")]
    public class LockOpenXmlDocumentCmdlet : PowerToolsModifierCmdlet
    {
        #region Cmdlet Overrides

        protected override void ProcessRecord()
        {
            foreach (var document in AllDocuments("Lock-OpenXmlDocument"))
            {
                try
                {
                    if (!(document is WmlDocument))
                        throw new PowerToolsDocumentException("Not a wordprocessing document.");
                    OutputDocument(PowerToolsExtensions.Lock((WmlDocument)document));
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
