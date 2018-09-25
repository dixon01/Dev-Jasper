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

namespace OpenXmlPowerTools
{
    /// <summary>
    /// Specify the entire source document	
    /// </summary>
    public class DocumentSource
    {
        /// <summary>
        /// Full or relative path name for the file
        /// </summary>
        public string SourceFile;
        /// <summary>
        /// Starting paragraph number (1 is the first paragraph)
        /// </summary>
        public int Start;
        /// <summary>
        /// Number of paragraphs
        /// </summary>
        public int Count;
        /// <summary>
        /// True, if you want to keep the section at the end of the document
        /// </summary>
        public bool KeepSection;

        /// <summary>
        /// Specify the entire source document	
        /// </summary>
        public DocumentSource(string source)
        {
            SourceFile = source;
            Start = -1;
            Count = -1;
        }
        /// <summary>
        /// Specify from "start" to the end of the document	
        /// </summary>
        public DocumentSource(string source, int start)
        {
            SourceFile = source;
            Start = start;
            Count = -1;
        }
        /// <summary>
        /// Specify from "start" and include "count" number of paragraphs
        /// </summary>
        public DocumentSource(string source, int start, int count)
        {
            SourceFile = source;
            Start = start;
            Count = count;
        }
    }
}

namespace OpenXmlPowerTools.Commands
{
    /// <summary>
    /// Transform-OpenXmlToHtml cmdlet	
    /// </summary>
    [Cmdlet(VerbsData.Merge, "OpenXmlDocument", SupportsShouldProcess = true)]
    public class MergeOpenXmlDocumentCmdlet : PowerToolsReadOnlyCmdlet
    {
        private DocumentSource[] m_Sources;
        private string m_OutputPath = "";
        private int m_Start = 1;
        private int m_Count = -1;
        private bool m_KeepSections = false;

        private List<Source> buildSources = new List<Source>();

        #region Parameters

        /// <summary>
        /// OutputPath parameter
        /// </summary>
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

        /// <summary>
        /// Start parameter
        /// </summary>
        [Parameter(
            Mandatory = false,
            HelpMessage = "Starting paragraph number to extract")
        ]
        public int Start
        {
            get
            {
                return m_Start;
            }
            set
            {
                m_Start = value;
            }
        }

        /// <summary>
        /// Count parameter
        /// </summary>
        [Parameter(
            Mandatory = false,
            HelpMessage = "Number of paragraphs to extract")
        ]
        public int Count
        {
            get
            {
                return m_Count;
            }
            set
            {
                m_Count = value;
            }
        }

        /// <summary>
        /// Sources parameter
        /// </summary>
        [Parameter(
            Mandatory = false,
            HelpMessage = "Array of sources to extract")
        ]
        public DocumentSource[] Sources
        {
            get
            {
                return m_Sources;
            }
            set
            {
                m_Sources = value;
            }
        }

        /// <summary>
        /// KeepSections parameter
        /// </summary>
        [Parameter(
            Mandatory = false,
            HelpMessage = "Keep a section break between each merged document.")
        ]
        [ValidateNotNullOrEmpty]
        public SwitchParameter KeepSections
        {
            get
            {
                return m_KeepSections;
            }
            set
            {
                m_KeepSections = value;
            }
        }

        [Parameter(
            Mandatory = false,
            HelpMessage = "Use this switch to pipe out the processed documents.")
        ]
        [ValidateNotNullOrEmpty]
        public SwitchParameter PassThru;

        #endregion

        #region Cmdlet Overrides

        protected override void ProcessRecord()
        {
            foreach (var document in AllDocuments("Merge-OpenXmlDocumentCmdlet"))
            {
                try
                {
                    if (!(document is WmlDocument))
                        throw new PowerToolsDocumentException("Not a wordprocessing document.");
                    if (m_Count != -1)
                        buildSources.Add(new Source((WmlDocument)document, m_Start - 1, m_Count, m_KeepSections));
                    else
                        buildSources.Add(new Source((WmlDocument)document, m_Start - 1, m_KeepSections));
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
                if (m_Sources != null)
                {
                    foreach (DocumentSource source in m_Sources)
                    {
                        Collection<PathInfo> fileList = SessionState.Path.GetResolvedPSPathFromPSPath(source.SourceFile);
                        foreach (var file in fileList)
                        {
                            OpenXmlPowerToolsDocument document = OpenXmlPowerToolsDocument.FromFileName(file.Path);
                            try
                            {
                                if (!(document is WmlDocument))
                                    throw new PowerToolsDocumentException("Not a wordprocessing document.");
                                if (source.Count != -1)
                                    buildSources.Add(new Source((WmlDocument)document, source.Start - 1, source.Count, source.KeepSection));
                                else if (source.Start != -1)
                                    buildSources.Add(new Source((WmlDocument)document, source.Start - 1, source.KeepSection));
                                else
                                    buildSources.Add(new Source((WmlDocument)document, source.KeepSection));
                            }
                            catch (Exception e)
                            {
                                WriteError(PowerToolsExceptionHandling.GetExceptionErrorRecord(e, document));
                            }
                        }
                    }
                }
                WmlDocument result = DocumentBuilder.BuildDocument(buildSources);
                if (m_OutputPath != null)
                {
                    if (!File.Exists(m_OutputPath) || ShouldProcess(m_OutputPath, "Merge-OpenXmlDocumentCmdlet"))
                        result.SaveAs(m_OutputPath);
                }
                if (PassThru)
                    WriteObject(result, true);
            }
            catch (Exception e)
            {
                WriteError(PowerToolsExceptionHandling.GetExceptionErrorRecord(e, null));
            }
        }

        #endregion
    }
}
