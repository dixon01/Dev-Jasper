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
    /// Cmdlet class for setting a cell style in a SpreadsheetML document
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "OpenXmlSpreadSheetCellStyle", SupportsShouldProcess = true)]
    [OutputType("OpenXmlPowerToolsDocument")]
    public class SetOpenXmlSpreadSheetCellStyleCmdlet : PowerToolsModifierCmdlet
    {
        #region Parameters
        private int fromRow;
        private int toRow;
        private short fromColumn;
        private short toColumn;
        private string cellStyle;
        private string worksheetName;

        /// <summary>
        /// Initial row for start setting the value
        /// </summary>
        [Parameter(Position = 2,
            Mandatory = true,
            HelpMessage = "Initial row for start setting the cell style")]
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
                    new System.Management.Automation.ParameterBindingException("Initial row must be greater than zero");
                }
            }
        }

        /// <summary>
        /// Initial row for start setting the value
        /// </summary>
        [Parameter(Position = 3,
            Mandatory = true,
            HelpMessage = "Final row for start setting the cell style")]
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
                    new System.Management.Automation.ParameterBindingException("Final row must be greater than zero");
                }
            }
        }

        /// <summary>
        /// Initial column for start setting the value
        /// </summary>
        [Parameter(Position = 4,
            Mandatory = true,
            HelpMessage = "Initial column for start setting the cell style")]
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
                    new System.Management.Automation.ParameterBindingException("Initial column must be greater than zero");
                }
            }
        }

        /// <summary>
        /// Final column for start setting the value
        /// </summary>
        [Parameter(Position = 5,
            Mandatory = true,
            HelpMessage = "Final column for start setting the cell style")]
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
                    new System.Management.Automation.ParameterBindingException("Final column must be greater than zero");
                }
            }
        }

        /// <summary>
        /// Cell Style Name
        /// </summary>
        [Parameter(Position = 6,
            Mandatory = true,
            HelpMessage = "Cell Style Name")]
        [ValidateNotNullOrEmpty]
        public string CellStyle
        {
            get
            {
                return cellStyle;
            }
            set
            {
                if (value.Trim().Length > 0)
                {
                    cellStyle = value;
                }
                else
                {
                    throw new ParameterBindingException("CellStyle cannot be empty");
                }
            }
        }

        /// <summary>
        /// Worksheet name to set the cell value
        /// </summary>
        [Parameter(Position = 7,
            Mandatory = true,
            HelpMessage = "Worksheet name to set the cell value")]
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
            foreach (var document in AllDocuments("Set-OpenXmlSpreadSheetCellStyle"))
            {
                try
                {
                    if (!(document is SmlDocument))
                        throw new PowerToolsDocumentException("Not a spreadsheet document.");
                    OutputDocument(WorksheetAccessor.SetCellStyle((SmlDocument)document, worksheetName, fromColumn, toColumn, fromRow, toRow, cellStyle));
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
