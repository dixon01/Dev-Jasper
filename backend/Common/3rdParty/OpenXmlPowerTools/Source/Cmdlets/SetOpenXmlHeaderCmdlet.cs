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
    /// Set the header files of a word document
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "OpenXmlHeader", SupportsShouldProcess = true)]
    [OutputType("OpenXmlPowerToolsDocument")]
    public class SetOpenXmlHeaderCmdlet : PowerToolsModifierCmdlet
    {

        #region Parameters

        private string headerPath;
        private XDocument header;
        private HeaderType kind;

        [Parameter(Position = 2,
           Mandatory = false,
           HelpMessage = "The path of the header to add in the document")
        ]
        [ValidateNotNullOrEmpty]
        public string HeaderPath
        {
            get
            {
                return headerPath;
            }
            set
            {
                headerPath = SessionState.Path.GetResolvedPSPathFromPSPath(value).First().Path;
            }
        }

        [Parameter(Position = 3,
           Mandatory = false,
           HelpMessage = "Specify the kind of the header to extract")
        ]
        public HeaderType HeaderType
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
            HelpMessage = "XDocument of the header to add")
        ]
        [ValidateNotNullOrEmpty]
        public XDocument Header
        {
            get
            {
                return header;
            }
            set
            {
                header = value;
            }
        }

        #endregion

        #region Cmdlet Overrides
        
        /// <summary>
        /// Entry point for PowerShell cmdlets
        /// </summary>
        protected override void ProcessRecord()
        {
            if (headerPath != null)
                header = XDocument.Load(headerPath);
            if (header == null)
            {
                WriteError(new ErrorRecord(new ArgumentException("No footer was specified."), "OpenXmlPowerTools", ErrorCategory.InvalidArgument, null));
                return;
            }
            foreach (var document in AllDocuments("Set-OpenXmlHeader"))
            {
                try
                {
                    if (!(document is WmlDocument))
                        throw new PowerToolsDocumentException("Not a wordprocessing document.");
                    OutputDocument(HeaderAccessor.SetHeader((WmlDocument)document, header, kind, Section - 1));
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
