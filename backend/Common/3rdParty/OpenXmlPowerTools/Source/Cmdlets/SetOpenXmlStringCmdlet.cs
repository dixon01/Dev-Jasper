/***************************************************************************

Copyright (c) Microsoft Corporation 2011.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System;
using System.Management.Automation;

namespace OpenXmlPowerTools.Commands
{
    [Cmdlet(VerbsCommon.Set, "OpenXmlString", SupportsShouldProcess = true)]
    [OutputType("OpenXmlPowerToolsDocument")]
    public class SetOpenXmlStringCmdlet : PowerToolsModifierCmdlet
    {
        #region Parameters

        [Parameter(Position = 2,
            Mandatory = true,
            HelpMessage = "Text to match in document")]
        [ValidateNotNullOrEmpty]
        public string Pattern;

        [Parameter(Position = 3,
            Mandatory = true,
            HelpMessage = "Text to replace in document")]
        [ValidateNotNullOrEmpty]
        public string Replace;

        [Parameter()]
        public SwitchParameter CaseSensitive;

        #endregion

        #region Cmdlet Overrides

        protected override void ProcessRecord()
        {
            foreach (var document in AllDocuments("Set-OpenXmlString"))
            {
                try
                {
                    if (!(document is WmlDocument) && !(document is PmlDocument))
                        throw new PowerToolsDocumentException("Not a supported document.");
                    if (document is WmlDocument)
                        OutputDocument(TextReplacer.SearchAndReplace((WmlDocument)document, Pattern, Replace, CaseSensitive));
                    if (document is PmlDocument)
                        OutputDocument(TextReplacer.SearchAndReplace((PmlDocument)document, Pattern, Replace, CaseSensitive));
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
