/***************************************************************************

Copyright (c) Microsoft Corporation 2008.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System;
using System.Management.Automation;
using DocumentFormat.OpenXml.Packaging;

namespace OpenXmlPowerTools.Commands
{
    /// <summary>
    /// Add-OpenXmlIndex cmdlet
    /// </summary>
    [Cmdlet(VerbsCommon.Add, "OpenXmlDocumentIndex", SupportsShouldProcess = true)]
    [OutputType("OpenXmlPowerToolsDocument")]
    public class AddOpenXmlDocumentIndexCmdlet : PowerToolsModifierCmdlet
    {
        #region Parameters

        string stylesSourcePath = "";
        bool addDefaultStyles = false;

        [Parameter(
            Position = 2,
            Mandatory = false,
            HelpMessage = "Path of the styles file used to get the styles of the index")
        ]
        [ValidateNotNullOrEmpty]
        public string StylesSourcePath
        {
            get
            {
                return stylesSourcePath;
            }
            set
            {
                stylesSourcePath = value;
            }
        }

        [Parameter(
            Mandatory = false,
            HelpMessage = "Specifies if the styles used in the index must be added to the document")
        ]
        [ValidateNotNullOrEmpty]
        public SwitchParameter AddDefaultStyles
        {
            get
            {
                return addDefaultStyles;
            }
            set
            {
                addDefaultStyles = value;
            }
        }


        #endregion

        #region Cmdlet Overrides
        
        protected override void ProcessRecord()
        {
            foreach (var document in AllDocuments("Add-OpenXmlDocumentIndex"))
            {
                try
                {
                    using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(document))
                    {
                        using (WordprocessingDocument doc = streamDoc.GetWordprocessingDocument())
                        {
                            IndexAccessor.Generate(doc);
                            StyleAccessor.CreateIndexStyles(doc, stylesSourcePath, addDefaultStyles);
                        }
                        OutputDocument(streamDoc.GetModifiedDocument());
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
