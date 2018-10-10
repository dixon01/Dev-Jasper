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
    /// Set-OpenXmlContentStyle cmdlet
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "OpenXmlContentStyle", SupportsShouldProcess = true)]
    [OutputType("OpenXmlPowerToolsDocument")]
    public class SetOpenXmlContentStyle : PowerToolsModifierCmdlet
    {
        #region Parameters

        private string xpathInsertionPoint;
        private string stylesSourcePath;
        private XDocument stylesSource;
        private string styleName;

        /// <summary>
        /// InsertionPoint parameter
        /// </summary>
        [Parameter(Position = 2,
            Mandatory = true,
            HelpMessage = "Insertion point location")]
        [ValidateNotNullOrEmpty]
        public string InsertionPoint
        {
            get
            {
                return xpathInsertionPoint;
            }
            set
            {
                xpathInsertionPoint = value;
            }
        }

        /// <summary>
        /// StyleName parameter
        /// </summary>
        [Parameter(Position = 3,
            Mandatory = true,
            HelpMessage = "Style name")]
        [ValidateNotNullOrEmpty]
        public string StyleName
        {
            get
            {
                return styleName;
            }
            set
            {
                styleName = value;
            }
        }

        /// <summary>
        /// StylesSourcePath parameter
        /// </summary>
        [Parameter(Position = 4,
            Mandatory = false,
            HelpMessage = "Style file path")]
        [ValidateNotNullOrEmpty]
        public string StylesSourcePath
        {
            get
            {
                return stylesSourcePath;
            }
            set
            {
                stylesSourcePath = SessionState.Path.GetResolvedPSPathFromPSPath(value).First().Path;
            }
        }

        /// <summary>
        /// StylesSource parameter
        /// </summary>
        [Parameter(Position = 5,
            Mandatory = false,
            ValueFromPipeline = true,
            HelpMessage = "Styles from XDocument")]
        [ValidateNotNullOrEmpty]
        public XDocument StylesSource
        {
            get
            {
                return stylesSource;
            }
            set
            {
                stylesSource = value;
            }
        }

        #endregion

        #region Cmdlet Overrides

        /// <summary>
        /// Entry point for PowerShell cmdlets
        /// </summary>
        protected override void ProcessRecord()
        {
            if (stylesSourcePath != null)
                stylesSource = XDocument.Load(stylesSourcePath);
            if (stylesSource == null)
            {
                WriteError(new ErrorRecord(new Exception("No styles source was specified."), "BadParameters", ErrorCategory.InvalidArgument, null));
                return;
            }
            foreach (var document in AllDocuments("Set-OpenXmlContentStyle"))
            {
                try
                {
                    if (!(document is WmlDocument))
                        throw new PowerToolsDocumentException("Not a wordprocessing document.");
                    OutputDocument(StyleAccessor.Insert((WmlDocument)document, xpathInsertionPoint, styleName, stylesSource));
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