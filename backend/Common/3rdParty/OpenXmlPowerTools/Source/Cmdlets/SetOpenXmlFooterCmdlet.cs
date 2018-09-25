/***************************************************************************

Copyright (c) Microsoft Corporation 2011.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System;
using System.Linq;
using System.Xml.Linq;
using System.Management.Automation;

namespace OpenXmlPowerTools.Commands
{

    /// <summary>
    /// Set the footer files of a word document
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "OpenXmlFooter", SupportsShouldProcess = true)]
    [OutputType("OpenXmlPowerToolsDocument")]
    public class SetOpenXmlFooterCmdlet : PowerToolsModifierCmdlet
    {
        #region Parameters

        private string footerPath;
        private XDocument footer;
        private FooterType kind;

        [Parameter(Position = 2,
           Mandatory = false,
           HelpMessage = "The path of the footer to add in the document")
        ]
        [ValidateNotNullOrEmpty]
        public string FooterPath
        {
            get
            {

                return footerPath;
            }
            set
            {
                footerPath = SessionState.Path.GetResolvedPSPathFromPSPath(value).First().Path;
            }
        }

        [Parameter(Position = 3,
           Mandatory = false,
           HelpMessage = "Specify the kind of the footer to extract")
        ]
        public FooterType FooterType
        {
            get
            {
                return kind;
            }
            set
            {
                kind = value;
            }
        }

        [Parameter(Position = 4,
           Mandatory = false,
           HelpMessage = "Number of section to modify")
        ]
        public int Section = 1;

        [Parameter(
           Mandatory = false,
           ValueFromPipeline = true,
           ValueFromPipelineByPropertyName = true,
           HelpMessage = "XDocument of the footer to add")
        ]
        [ValidateNotNullOrEmpty]
        public XDocument Footer
        {
            get
            {
                return footer;
            }
            set
            {
                footer = value;
            }
        }


        #endregion

        #region Cmdlet Overrides

        /// <summary>
        /// Entry point for PowerShell cmdlets
        /// </summary>
        protected override void ProcessRecord()
        {
            if (footerPath != null)
                footer = XDocument.Load(footerPath);
            if (footer == null)
            {
                WriteError(new ErrorRecord(new ArgumentException("No footer was specified."), "OpenXmlPowerTools", ErrorCategory.InvalidArgument, null));
                return;
            }
            foreach (var document in AllDocuments("Set-OpenXmlFooter"))
            {
                try
                {
                    if (!(document is WmlDocument))
                        throw new PowerToolsDocumentException("Not a wordprocessing document.");
                    OutputDocument(FooterAccessor.SetFooter((WmlDocument)document, footer, kind, Section - 1));
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
