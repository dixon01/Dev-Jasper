/***************************************************************************

Copyright (c) Microsoft Corporation 2011.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System;
using System.Management.Automation;
using System.Collections.Generic;
using System.Linq;

namespace OpenXmlPowerTools.Commands
{
    [Cmdlet(VerbsCommon.Add, "OpenXmlDigitalSignature", SupportsShouldProcess = true)]
    [OutputType("OpenXmlPowerToolsDocument")]
    public class AddOpenXmlDigitalSignatureCmdlet : PowerToolsModifierCmdlet
    {
        #region Parameters

        private string certificate;

        [Parameter(
            Position = 2,
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Digital certificate path")
        ]
        [ValidateNotNullOrEmpty]
        public string Certificate
        {
            get
            {
                return certificate;
            }
            set
            {
                certificate = value;
            }
        }

        #endregion

        #region Cmdlet Overrides
        protected override void ProcessRecord()
        {
            IEnumerable<string> certList = SessionState.Path.GetResolvedPSPathFromPSPath(certificate).Select(e => e.Path);
            foreach (var document in AllDocuments("Add-OpenXmlDigitalSignature"))
            {
                try
                {
                    OutputDocument(DigitalSignatureAccessor.Insert(document, certList));
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

