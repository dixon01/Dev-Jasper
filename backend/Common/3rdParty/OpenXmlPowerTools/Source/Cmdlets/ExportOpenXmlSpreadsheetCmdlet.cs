/***************************************************************************

Copyright (c) Microsoft Corporation 2011.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using DocumentFormat.OpenXml.Packaging;

namespace OpenXmlPowerTools.Commands
{
    [Cmdlet(VerbsData.Export, "OpenXmlSpreadsheet", SupportsShouldProcess = true)]
    [OutputType("OpenXmlPowerToolsDocument")]
    public class ExportOpenXmlSpreadsheetCmdlet : PowerToolsCreateCmdlet
    {
        #region Parameters

        private PSObject[] pipeObjects;
        private Collection<PSObject> processedObjects = new Collection<PSObject>();
        private List<string> columnsToChart;
        string headerColumn;
        string outputPath;
        bool displayChart;
        private int initialRow = 1;
        ChartType chartType;

        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = false,
            HelpMessage = "Path of file in which to store results")
        ]
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

        /// <summary>
        /// InputObject parameter
        /// </summary>
        [Parameter(
            ValueFromPipeline = true,
            HelpMessage = "Objects passed by pipe to be included in spreadsheet")
        ]
        public PSObject[] InputObject
        {
            get
            {
                return pipeObjects;
            }
            set
            {
                pipeObjects = value;
            }
        }

        /// <summary>
        /// Chart parameter
        /// </summary>
        [Parameter(
            Mandatory = false,
            ValueFromPipeline = false,
            ParameterSetName = "charting",
            HelpMessage = "Whether generate a chart from loaded data or not")
        ]
        public SwitchParameter Chart
        {
            get
            {
                return displayChart;
            }
            set
            {
                displayChart = value;
            }
        }

        /// <summary>
        /// ChartType parameter
        /// </summary>
        [Parameter(
            Mandatory = false,
            ValueFromPipeline = false,
            ParameterSetName = "charting",
            HelpMessage = "Type of chart to be generated")
        ]
        public ChartType ChartType
        {
            get
            {
                return chartType;
            }
            set
            {
                chartType = value;
            }
        }

        /// <summary>
        /// ColumnsToChart parameter
        /// </summary>
        [Parameter(
            Mandatory = false,
            ValueFromPipeline = false,
            ParameterSetName = "charting",
            HelpMessage = "Columns from data to be used as series in chart")
        ]
        public List<string> ColumnsToChart
        {
            get
            {
                return columnsToChart;
            }
            set
            {
                columnsToChart = value;
            }
        }

        /// <summary>
        /// HeaderColumn parameter
        /// </summary>
        [Parameter(
            Mandatory = false,
            ValueFromPipeline = false,
            ParameterSetName = "charting",
            HelpMessage = "Column from data to be used as category in chart")
        ]
        public string HeaderColumn
        {
            get
            {
                return headerColumn;
            }
            set
            {
                headerColumn = value;
            }
        }

        [Parameter(
            Mandatory = false,
            ValueFromPipeline = false,
            HelpMessage = "Header Row")
        ]
        [ValidateNotNullOrEmpty]
        public int InitialRow
        {
            get
            {
                return initialRow;
            }
            set
            {
                initialRow = value;
            }
        }

        #endregion

        #region Cmdlet Overrides

        protected override void ProcessRecord()
        {
            if (pipeObjects != null)
            {
                foreach (PSObject pipeObject in pipeObjects)
                    processedObjects.Add(pipeObject);
            }
        }

        protected override void EndProcessing()
        {
            if (!File.Exists(outputPath) || ShouldProcess(outputPath, "Export-OpenXmlSpreadsheet"))
            {
                using (OpenXmlMemoryStreamDocument streamDoc = OpenXmlMemoryStreamDocument.CreateSpreadsheetDocument())
                {
                    using (SpreadsheetDocument document = streamDoc.GetSpreadsheetDocument())
                    {
                        if (processedObjects.Count > 0)
                        {
                            List<string> headerList = new List<string>();
                            foreach (PSPropertyInfo propertyInfo in processedObjects[0].Properties)
                            {
                                headerList.Add(propertyInfo.Name.ToUpper());
                            }

                            // Stores into a matrix all properties of objects passed as parameter
                            int rowLength = headerList.Count;
                            int rowCount = processedObjects.Count;
                            string[][] valueMatrix = new string[rowCount][];

                            int currentRow = 0, currentColumn = 0;
                            foreach (PSObject obj in processedObjects)
                            {
                                currentColumn = 0;
                                valueMatrix[currentRow] = new string[rowLength];
                                foreach (PSPropertyInfo propertyInfo in obj.Properties)
                                {
                                    try
                                    {
                                        if (propertyInfo.Value != null)
                                        {
                                            valueMatrix[currentRow][currentColumn] = propertyInfo.Value.ToString();
                                        }
                                    }
                                    // Suppress errors on properties that cannot be read, but write the information to debug output.
                                    catch (GetValueInvocationException e)
                                    {
                                        WriteDebug(string.Format(CultureInfo.InvariantCulture, "Exception ({0}) at Object {1}, property {2}", e.Message, currentRow, currentColumn));
                                    }
                                    currentColumn++;
                                }
                                currentRow++;
                            }
                            if (displayChart)
                                SpreadsheetDocumentManager.Create(document, headerList, valueMatrix, chartType, headerColumn, columnsToChart, initialRow);
                            else
                                SpreadsheetDocumentManager.Create(document, headerList, valueMatrix, initialRow);
                        }
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