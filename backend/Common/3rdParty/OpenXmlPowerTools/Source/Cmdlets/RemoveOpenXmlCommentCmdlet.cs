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
    /// Remove-OpenXmlComment cmdlet	
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "OpenXmlComment", SupportsShouldProcess = true)]
    [OutputType("OpenXmlPowerToolsDocument")]
    public class RemoveOpenXmlCommentCmdlet : PowerToolsModifierCmdlet
    {
        #region Cmdlet Overrides

        protected override void ProcessRecord()
        {
            foreach (var document in AllDocuments("Remove-OpenXmlComment"))
            {
                try
                {
                    if (!(document is WmlDocument))
                        throw new PowerToolsDocumentException("Not a wordprocessing document.");
                    OutputDocument(CommentAccessor.RemoveAll((WmlDocument)document));
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