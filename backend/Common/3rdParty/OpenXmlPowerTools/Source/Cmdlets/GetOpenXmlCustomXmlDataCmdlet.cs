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
    /// Get-OpenXmlCustomXmlData cmdlet
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "OpenXmlCustomXmlData")]
    [OutputType("XDocument")]
    public class GetOpenXmlCustomXmlData : PowerToolsReadOnlyCmdlet
    {
        #region Parameters

        string xmlPartName;

        /// <summary>
        /// Part parameter
        /// </summary>
        [Parameter(
            Position = 1,
            Mandatory = true,
            HelpMessage = "Custom Xml part name")
        ]
        [ValidateNotNullOrEmpty]
        public string Part
        {
            get
            {
                return xmlPartName;
            }
            set
            {
                xmlPartName = value;
            }
        }

        #endregion

        #region Cmdlet Overrides
        
        protected override void ProcessRecord()
        {
            foreach (var document in AllDocuments("Get-OpenXmlCustomXmlData"))
            {
                try
                {
                    if (!(document is WmlDocument))
                        throw new PowerToolsDocumentException("Not a wordprocessing document.");
                    WriteObject(CustomXmlAccessor.Find((WmlDocument)document, xmlPartName), true);
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