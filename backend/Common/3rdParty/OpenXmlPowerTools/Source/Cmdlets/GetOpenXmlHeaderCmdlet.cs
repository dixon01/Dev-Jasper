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
    /// Get the header files of a word document
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "OpenXmlHeader")]
    [OutputType("XDocument")]
    public class GetOpenXmlHeader : PowerToolsReadOnlyCmdlet
    {
        #region Parameters

        private HeaderType headerType;

        [Parameter(Position = 1,
           Mandatory = false,
           HelpMessage = "Specifies the type of the header to extract.")
        ]
        public HeaderType HeaderType
        {
            get
            {
                return headerType;
            }
            set
            {
                headerType = value;
            }
        }

        [Parameter(Position = 2,
           Mandatory = false,
           HelpMessage = "Number of section")
        ]
        public int Section = 1;

        # endregion

        #region Cmdlet Overrides

        /// <summary>
        /// Entry point for PowerShell cmdlets
        /// </summary>
        protected override void ProcessRecord()
        {
            foreach (var document in AllDocuments("Get-OpenXmlHeader"))
            {
                try
                {
                    if (!(document is WmlDocument))
                        throw new PowerToolsDocumentException("Not a wordprocessing document.");
                    WriteObject(HeaderAccessor.GetHeader((WmlDocument)document, headerType, Section - 1), true);
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
