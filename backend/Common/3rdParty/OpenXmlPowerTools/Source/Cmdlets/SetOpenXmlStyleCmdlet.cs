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
    /// Set the style.xml of a word document
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "OpenXmlStyle", SupportsShouldProcess = true)]
    [OutputType("OpenXmlPowerToolsDocument")]
    public class SetOpenXmlStyleCmdlet : PowerToolsModifierCmdlet
    {
        #region Parameters

        private string styleFile;
        private XDocument stylesDocument;

        /// <summary>
        /// StylePath parameter
        /// </summary>
        [Parameter(Position = 2,
           Mandatory = false,
           HelpMessage = "New Style.xml path")
        ]
        [ValidateNotNullOrEmpty]
        public string StylePath
        {
            get
            {
                return styleFile;
            }
            set
            {
                styleFile = SessionState.Path.GetResolvedPSPathFromPSPath(value).First().Path;
            }
        }

        /// <summary>
        /// Style parameter
        /// </summary>
        [Parameter(
           Mandatory = false,
           ValueFromPipeline = true,
           HelpMessage = "New styles document")
        ]
        [ValidateNotNullOrEmpty]
        public XDocument Style
        {
            get
            {
                return stylesDocument;
            }
            set
            {
                stylesDocument = value;
            }
        }

        #endregion

        #region Cmdlet Overrides

        protected override void ProcessRecord()
        {
            try
            {
                if (styleFile != null)
                    stylesDocument = XDocument.Load(styleFile);
                if (stylesDocument == null)
                {
                    WriteError(new ErrorRecord(new ArgumentException("No styles document was specified."), "OpenXmlPowerTools", ErrorCategory.InvalidArgument, null));
                }
                else
                {
                    foreach (var document in AllDocuments("Set-OpenXmlStyle"))
                    {
                        try
                        {
                            if (!(document is WmlDocument))
                                throw new PowerToolsDocumentException("Not a wordprocessing document.");
                            OutputDocument(StyleAccessor.SetStylePart((WmlDocument)document, stylesDocument));
                        }
                        catch (Exception e)
                        {
                            WriteError(PowerToolsExceptionHandling.GetExceptionErrorRecord(e, document));
                        }
                    }
                }
            }
            catch (ItemNotFoundException e)
            {
                WriteError(new ErrorRecord(e, "FileNotFound", ErrorCategory.OpenError, null));
            }
            catch (InvalidOperationException e)
            {
                WriteError(new ErrorRecord(e, "InvalidOperation", ErrorCategory.InvalidOperation, null));
            }
            catch (ArgumentException e)
            {
                WriteError(new ErrorRecord(e, "InvalidArgument", ErrorCategory.InvalidArgument, null));
            }
            catch (Exception e)
            {
                WriteError(new ErrorRecord(e, "General", ErrorCategory.NotSpecified, null));
            }
        }

        #endregion
    }
}
