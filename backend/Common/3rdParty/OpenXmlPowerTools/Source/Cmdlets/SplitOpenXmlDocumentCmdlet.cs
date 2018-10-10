/***************************************************************************

Copyright (c) Microsoft Corporation 2011.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using System.Management.Automation;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;

namespace OpenXmlPowerTools.Commands
{
    /// <summary>
    /// Split-OpenXmlDocument cmdlet
    /// </summary>
    [Cmdlet(VerbsCommon.Split, "OpenXmlDocument", SupportsShouldProcess = true)]
    [OutputType("OpenXmlPowerToolsDocument")]
    public class SplitOpenXmlDocumentCmdlet : PowerToolsModifierCmdlet
    {
        private int NextNumber;

        #region Parameters

        [Parameter(Position = 2,
           Mandatory = true,
           HelpMessage = "Defines the prefix name of resulting documents")
        ]
        public string Prefix;

        #endregion

        #region Cmdlet Overrides

        protected override void ProcessRecord()
        {
            NextNumber = 1;
            foreach (var document in AllDocuments("Split-OpenXmlDocument"))
            {
                try
                {
                    if (!(document is WmlDocument))
                        throw new PowerToolsDocumentException("Not a wordprocessing document.");
                    foreach (WmlDocument item in DocumentBuilder.SplitOnSections((WmlDocument)document))
                    {
                        item.FileName = String.Format("{0}{1}.docx", Prefix, NextNumber++);
                        OutputDocument(item);
                    }
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
