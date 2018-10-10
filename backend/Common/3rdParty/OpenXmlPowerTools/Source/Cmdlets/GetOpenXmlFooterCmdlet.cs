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
    /// Get the footer files of a word document
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "OpenXmlFooter")]
    [OutputType("XDocument")]
    public class GetOpenXmlFooter : PowerToolsReadOnlyCmdlet
    {
        #region Parameters

        private FooterType footerType;

        /// <summary>
        /// FooterType parameter
        /// </summary>
        [Parameter(Position = 1,
           Mandatory = false,
           HelpMessage = "Specifies the type of the footer to extract.")
        ]
        public FooterType FooterType
        {
            get
            {
                return footerType;
            }
            set
            {
                footerType = value;
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
        /// Entry point for PowerShell commandlets
        /// </summary>
        protected override void ProcessRecord()
        {
            foreach (var document in AllDocuments("Get-OpenXmlFooter"))
            {
                try
                {
                    if (!(document is WmlDocument))
                        throw new PowerToolsDocumentException("Not a wordprocessing document.");
                    WriteObject(FooterAccessor.GetFooter((WmlDocument)document, footerType, Section - 1), true);
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
