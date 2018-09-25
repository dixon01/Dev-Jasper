/***************************************************************************

Copyright (c) Microsoft Corporation 2011.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System.IO;
using System.Collections.Generic;
using System.Management.Automation;
using System.Linq;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;

namespace OpenXmlPowerTools.Commands
{
    /// <summary>
    /// Export-OpenXmlWordprocessing cmdlet
    /// </summary>
    [Cmdlet("Export", "OpenXmlWordprocessing", SupportsShouldProcess = true)]
    [OutputType("OpenXmlPowerToolsDocument")]
    public class ExportOpenXmlWordprocessingCmdlet : PowerToolsCreateCmdlet
    {
        #region Parameters

        private string outputPath;
        private string[] text;
        private List<string> processedObjects = new List<string>();

        /// <summary>
        /// OutputPath parameter
        /// </summary>
        [Parameter(
            Position = 0,
            Mandatory = true,
            HelpMessage = "Path of file to store export results")
        ]
        [ValidateNotNullOrEmpty]
        public string OutputPath
        {
            get
            {
                return outputPath;
            }
            set
            {
                outputPath = Path.Combine(SessionState.Path.CurrentLocation.Path, value);
            }
        }

        [Parameter(
            Position = 1,
            ValueFromPipeline = true,
            Mandatory = true,
            HelpMessage = "Text to insert in the new wordprocessing document")
        ]
        [AllowEmptyString]
        public string[] Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
            }
        }

        #endregion

        #region Cmdlet Overrides
        
        /// <summary>
        /// Entry point for PowerShell cmdlets
        /// </summary>
        protected override void ProcessRecord()
        {
            if (text != null)
            {
                foreach (string item in text)
                    processedObjects.Add(item);
            }
        }
        protected override void EndProcessing()
        {
            if (PassThru || !File.Exists(outputPath) || ShouldProcess(outputPath, "Export-OpenXmlWordprocessing"))
            {
                using (OpenXmlMemoryStreamDocument streamDoc = OpenXmlMemoryStreamDocument.CreateWordprocessingDocument())
                {
                    using (WordprocessingDocument document = streamDoc.GetWordprocessingDocument())
                    {
                        if (processedObjects.Count > 0)
                            PowerToolsExtensions.SetContent(document, processedObjects.Select(e => new XElement(W.p, new XElement(W.r, new XElement(W.t, new XAttribute(XNamespace.Xml + "space", "preserve"), e)))));
                    }
                    OpenXmlPowerToolsDocument output = streamDoc.GetModifiedDocument();
                    output.FileName = outputPath;
                    OutputDocument(output);
                }
            }
        }
        #endregion
    }
}