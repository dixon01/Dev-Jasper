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
    /// Set-OpenXmlWatermark cmdlet
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "OpenXmlWatermark", SupportsShouldProcess = true)]
    [OutputType("OpenXmlPowerToolsDocument")]
    public class SetOpenXmlWatermarkCmdlet : PowerToolsModifierCmdlet
    {
        #region Parameters

        private bool diagonalOrientation;
        private string watermarkText;

        /// <summary>
        /// WatermarkText parameter
        /// </summary>
        [Parameter(Position = 2,
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "Text to show in the watermark")]
        [ValidateNotNullOrEmpty]
        public string WatermarkText
        {
            get
            {
                return watermarkText;
            }
            set
            {
                watermarkText = value;
            }
        }

        /// <summary>
        /// DiagonalOrientation parameter
        /// </summary>
        [Parameter(
            Mandatory = false,
            HelpMessage = "Specifies diagonal orientation for watermark")]
        [ValidateNotNullOrEmpty]
        public SwitchParameter DiagonalOrientation
        {
            get
            {
                return diagonalOrientation;
            }
            set
            {
                diagonalOrientation = value;
            }
        }

        #endregion

        #region Cmdlet Overrides

        protected override void ProcessRecord()
        {
            foreach (var document in AllDocuments("Set-OpenXmlWatermark"))
            {
                try
                {
                    if (!(document is WmlDocument))
                        throw new PowerToolsDocumentException("Not a wordprocessing document.");
                    OutputDocument(WatermarkAccessor.InsertWatermark((WmlDocument)document, watermarkText, diagonalOrientation));
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