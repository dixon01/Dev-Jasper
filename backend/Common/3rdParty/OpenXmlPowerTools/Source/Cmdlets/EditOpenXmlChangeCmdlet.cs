/***************************************************************************

Copyright (c) Microsoft Corporation 2011.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System;
using System.Management.Automation;

namespace OpenXmlPowerTools.Commands
{
    [Cmdlet(VerbsData.Edit, "OpenXmlChange", SupportsShouldProcess = true)]
    [OutputType("OpenXmlPowerToolsDocument")]
    public class EditOpenXmlChangeCmdlet : PowerToolsModifierCmdlet
    {
        [Parameter(Mandatory = false)]
        public SwitchParameter Accept;

        // Future option
        //[Parameter(Mandatory = false)]
        //public SwitchParameter Reject;

        #region Cmdlet Overrides

        protected override void ProcessRecord()
        {
            foreach (var document in AllDocuments("Edit-OpenXmlChange"))
            {
                try
                {
                    if (!(document is WmlDocument))
                        throw new PowerToolsDocumentException("Not a wordprocessing document.");
                    if (Accept)
                        OutputDocument(RevisionAccepter.AcceptRevisions((WmlDocument)document));
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