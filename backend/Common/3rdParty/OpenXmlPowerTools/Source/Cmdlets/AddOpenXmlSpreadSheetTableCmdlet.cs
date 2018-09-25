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
    /// Cmdlet for adding a new table to a worksheet in a SpreadsheetML document
    /// </summary>
    [Cmdlet(VerbsCommon.Add, "OpenXmlSpreadSheetTable", SupportsShouldProcess = true)]
    [OutputType("OpenXmlPowerToolsDocument")]
    public class AddOpenXmlSpreadSheetTableCmdlet : PowerToolsModifierCmdlet
    {
        #region Parameters

        private string tableStyle = string.Empty;
        private SwitchParameter hasHeaders =false; 
        private short fromColumn;
        private short toColumn;
        private int fromRow;
        private int toRow;
        private string worksheetName;

        [Parameter(Position = 2,
            Mandatory = true,
            HelpMessage = "Table Style Name")]
        [ValidateNotNullOrEmpty]
        public string TableStyle
        {
            get
            {
                return tableStyle;
            }
            set
            {
                tableStyle = value;
            }
        }

        /// <summary>
        /// Use Headers parameter
        /// </summary>
        [Parameter(
            Mandatory = false,
            HelpMessage = "Has Headers?")]
        [ValidateNotNullOrEmpty]
        public SwitchParameter HasHeaders
        {
            get
            {
            return hasHeaders;
            }
            set
            {
                hasHeaders = value;
            }
        }

        /// <summary>
        /// Initial table column
        /// </summary>
        [Parameter(Position = 3,
            Mandatory = true,
            HelpMessage = "Initial table column")]
        [ValidateNotNullOrEmpty]
        public short FromColumn
        {
            get
            {
                return fromColumn;
            }
            set
            {
                if (value > 0)
                {
                    fromColumn = value;
                }
                else
                {
                    throw new System.Management.Automation.ParameterBindingException("Initial Table Column must be greater than 0");
                }
            }
        }

        /// <summary>
        /// Final table column
        /// </summary>
        [Parameter(Position = 4,
            Mandatory = true,
            HelpMessage = "Final table column")]
        [ValidateNotNullOrEmpty]
        public short ToColumn
        {
            get
            {
                return toColumn;
            }
            set
            {
                if (value > 0)
                {
                    toColumn = value;
                }
                else
                {
                    throw new System.Management.Automation.ParameterBindingException("Final table column must be greater than 0");
                }
            }
        }

        /// <summary>
        /// Initial table row
        /// </summary>
        [Parameter(Position = 5,
            Mandatory = true,
            HelpMessage = "Initial table row")]
        [ValidateNotNullOrEmpty]
        public int FromRow
        {
            get
            {
                return fromRow;
            }
            set
            {
                if (value > 0)
                {
                    fromRow = value;
                }
                else
                {
                    throw new System.Management.Automation.ParameterBindingException("Initial Table Row must be greater than 0");
                }
            }
        }

        /// <summary>
        /// Final table row
        /// </summary>
        [Parameter(Position = 6,
            Mandatory = true,
            HelpMessage = "Final table row")]
        [ValidateNotNullOrEmpty]
        public int ToRow
        {
            get
            {
                return toRow;
            }
            set
            {
                if (value > 0)
                {
                    toRow = value;
                }
                else
                {
                    throw new System.Management.Automation.ParameterBindingException("Final table row must be greater than 0");
                }
            }
        }

        /// <summary>
        /// Index for worksheet to add the table to
        /// </summary>
        [Parameter(Position = 7,
            Mandatory = true,
            HelpMessage = "Worksheet name to add the table to")]
        [ValidateNotNullOrEmpty]
        public string WorksheetName
        {
            get
            {
                return worksheetName;
            }
            set
            {
                worksheetName = value;
            }
        }

        #endregion

        #region Cmdlet Overrides
        protected override void ProcessRecord()
        {
            foreach (var document in AllDocuments("Add-OpenXmlSpreadSheetTable"))
            {
                try
                {
                    if (!(document is SmlDocument))
                        throw new PowerToolsDocumentException("Not a spreadsheet document.");
                    OutputDocument(SpreadSheetTableAccessor.Add((SmlDocument)document, worksheetName, tableStyle, hasHeaders, fromColumn, toColumn, fromRow, toRow));
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
