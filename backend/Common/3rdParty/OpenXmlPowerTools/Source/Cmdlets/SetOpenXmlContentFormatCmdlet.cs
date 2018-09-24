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
    /// Set-OpenXmlContentFormat cmdlet
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "OpenXmlContentFormat", SupportsShouldProcess = true)]
    [OutputType("OpenXmlPowerToolsDocument")]
    public class SetOpenXmlContentFormat : PowerToolsModifierCmdlet
    {
        #region Parameters

        private string xpathInsertionPoint;
        private string xmlContent;

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
        /// Content parameter
        /// </summary>
        [Parameter(Position = 3,
            Mandatory = true,
            HelpMessage = "Xml to insert")]
        [ValidateNotNullOrEmpty]
        public string Content
        {
            get
            {
                return xmlContent;
            }
            set
            {
                xmlContent = value;
            }
        }

        #endregion

        #region Cmdlet Overrides

        protected override void ProcessRecord()
        {
            foreach (var document in AllDocuments("Set-OpenXmlContentFormat"))
            {
                try
                {
                    if (!(document is WmlDocument))
                        throw new PowerToolsDocumentException("Not a wordprocessing document.");
                    OutputDocument(ContentFormatAccessor.Insert((WmlDocument)document, xpathInsertionPoint, xmlContent));
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