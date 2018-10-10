/***************************************************************************

Copyright (c) Microsoft Corporation 2011.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Collections.Generic;

namespace OpenXmlPowerTools.Commands
{
    [Cmdlet(VerbsData.Merge, "OpenXmlDocumentComment", SupportsShouldProcess = true)]
    public class MergeOpenXmlDocumentCommentCmdlet : PowerToolsReadOnlyCmdlet
    {
        private string m_OutputPath = "";

        private WmlDocument current;

        #region Parameters

        [Parameter(
            Position = 1,
            Mandatory = true,
            HelpMessage = "Path of file for output document")
        ]
        public string OutputPath
        {
            get
            {
                return m_OutputPath;
            }
            set
            {
                m_OutputPath = System.IO.Path.Combine(SessionState.Path.CurrentLocation.Path, value);
            }
        }

        [Parameter(
            Mandatory = false,
            HelpMessage = "Use this switch to pipe out the processed document.")
        ]
        public SwitchParameter PassThru;

        #endregion

        #region Cmdlet Overrides

        protected override void ProcessRecord()
        {
            foreach (var document in AllDocuments("Merge-OpenXmlDocumentCommentCmdlet"))
            {
                try
                {
                    if (!(document is WmlDocument))
                        throw new PowerToolsDocumentException("Not a wordprocessing document.");
                    if (current == null)
                        current = (WmlDocument)document;
                    else
                        current = CommentMerger.MergeComments(current, (WmlDocument)document);
                }
                catch (Exception e)
                {
                    WriteError(PowerToolsExceptionHandling.GetExceptionErrorRecord(e, document));
                }
            }
        }

        protected override void EndProcessing()
        {
            try
            {
                if (m_OutputPath != null)
                {
                    if (!File.Exists(m_OutputPath) || ShouldProcess(m_OutputPath, "Merge-OpenXmlDocumentCommentCmdlet"))
                        current.SaveAs(m_OutputPath);
                }
                if (PassThru)
                    WriteObject(current, true);
            }
            catch (Exception e)
            {
                WriteError(PowerToolsExceptionHandling.GetExceptionErrorRecord(e, null));
            }
        }

        #endregion
    }
}
