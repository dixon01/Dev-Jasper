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
    /// Set-OpenXmlCustomXmlData cmdlet	
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "OpenXmlCustomXmlData", SupportsShouldProcess = true)]
    [OutputType("OpenXmlPowerToolsDocument")]
    public class SetOpenXmlCustomXmlDataCmdlet : PowerToolsModifierCmdlet
    {
        #region Parameters

        private string xmlPath;
        private string partName;
        private XDocument customData;

        /// <summary>
        /// Specify the CustomXmlPart parameter
        /// </summary>
        [Parameter(Position = 2,
            Mandatory = false,
            HelpMessage = "Custom Xml part path")]
        [ValidateNotNullOrEmpty]
        public string PartPath
        {
            get
            {
                return xmlPath;
            }
            set
            {
                var xmlPartFileNames = SessionState.Path.GetResolvedPSPathFromPSPath(value);
                if (xmlPartFileNames.Count() == 1)
                {
                    xmlPath = xmlPartFileNames.First().Path;
                }
                else if (xmlPartFileNames.Count() > 1)
                {
                    throw new Exception("Too many xmlParts specified.");
                }

            }
        }

        /// <summary>
        /// Specify the CustomXmlPart parameter
        /// </summary>
        [Parameter(Position = 3,
            Mandatory = false,
            HelpMessage = "Name for the new custom part")]
        [ValidateNotNullOrEmpty]
        public string PartName
        {
            get
            {
                return partName;
            }
            set
            {
                partName = value;
            }
        }

        /// <summary>
        /// Specify the CustomXmlPart parameter
        /// </summary>
        [Parameter(Position = 4,
            Mandatory = false,
            ValueFromPipeline = true,
            HelpMessage = "Custom Xml document")]
        [ValidateNotNullOrEmpty]
        public XDocument Part
        {
            get
            {
                return customData;
            }
            set
            {
                customData = value;
            }
        }

        #endregion

        #region Cmdlet Overrides

        protected override void ProcessRecord()
        {
            if (xmlPath != null)
            {
                customData = XDocument.Load(xmlPath);
                if (partName == null)
                    partName = System.IO.Path.GetFileName(xmlPath);
            }
            foreach (var document in AllDocuments("Set-OpenXmlCustomXmlData"))
            {
                try
                {
                    if (!(document is WmlDocument))
                        throw new PowerToolsDocumentException("Not a wordprocessing document.");
                    OutputDocument(CustomXmlAccessor.SetDocument((WmlDocument)document, customData, partName));
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