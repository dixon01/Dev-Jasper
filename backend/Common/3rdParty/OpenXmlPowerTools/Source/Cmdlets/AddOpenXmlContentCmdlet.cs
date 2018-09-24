/***************************************************************************

Copyright (c) Microsoft Corporation 2011.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System;
using System.Management.Automation;

namespace OpenXmlPowerTools.Commands
{
    [Cmdlet(VerbsCommon.Add, "OpenXmlContent", SupportsShouldProcess = true)]
    [OutputType("OpenXmlPowerToolsDocument")]
    public class AddOpenXmlContentCmdlet : PowerToolsModifierCmdlet
    {
        #region Parameters

        string[] xmlPartPaths;
        string xpathInsertionPoint;
        string xmlContent;

        [Parameter(Position = 2, Mandatory = true, HelpMessage = "Custom Xml part path")]
        [ValidateNotNullOrEmpty]
        public string[] PartPath
        {
            get
            {
                return xmlPartPaths;
            }
            set
            {
                xmlPartPaths = value;
            }
        }

        [Parameter(Position = 3, Mandatory = true, HelpMessage = "Insertion point location")]
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

        [Parameter(Position = 4, Mandatory = true, HelpMessage = "Xml to insert")]
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
            foreach (var document in AllDocuments("Add-OpenXmlContent"))
            {
                try
                {
                    OutputDocument(PowerToolsExtensions.InsertXml(document, xmlPartPaths, xpathInsertionPoint, xmlContent));
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
