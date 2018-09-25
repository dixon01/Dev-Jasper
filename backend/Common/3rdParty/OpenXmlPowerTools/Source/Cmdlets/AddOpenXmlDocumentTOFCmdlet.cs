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
    /// Add-OpenXmlDocumentTOF cmdlet
    /// </summary>
    [Cmdlet(VerbsCommon.Add, "OpenXmlDocumentTOF", SupportsShouldProcess = true)]
    [OutputType("OpenXmlPowerToolsDocument")]
    public class AddOpenXmlDocumentTOFCmdlet : PowerToolsModifierCmdlet
    {
        #region Parameters

        [Parameter(
            Position = 2,
            Mandatory = true,
            HelpMessage = "XPath where the table of figures will be created.")
        ]
        [ValidateNotNullOrEmpty]
        public string InsertionPoint;

        [Parameter(
            Position = 3,
            Mandatory = true,
            HelpMessage = "Choices of how the table of contents will be created.")
        ]
        [ValidateNotNullOrEmpty]
        public string Switches;

        #endregion

        #region Cmdlet Overrides

        protected override void ProcessRecord()
        {
            foreach (var document in AllDocuments("Add-OpenXmlDocumentTOF"))
            {
                try
                {
                    if (!(document is WmlDocument))
                        throw new PowerToolsDocumentException("Not a wordprocessing document.");
                    OutputDocument(ReferenceAdder.AddTof((WmlDocument)document, InsertionPoint, Switches, null));
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
