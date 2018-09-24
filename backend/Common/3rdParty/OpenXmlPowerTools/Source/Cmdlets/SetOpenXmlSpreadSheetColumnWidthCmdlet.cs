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
    /// Class for setting the width for a range of columns in a worksheet in a SpreadsheetML document
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "OpenXmlSpreadSheetColumnWidth", SupportsShouldProcess = true)]
    [OutputType("OpenXmlPowerToolsDocument")]
    public class SetOpenXmlSpreadSheetColumnWidthCmdlet : PowerToolsModifierCmdlet
    {
        #region Parameters

        private short fromColumn;
        private short toColumn;
        private int width;
        private string worksheetName;

        /// <summary>
        /// Row index for setting value
        /// </summary>
        [Parameter(Position = 2,
            Mandatory = true,
            HelpMessage = "Initial Column for Setting Width")]
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
        /// Column index for setting value
        /// </summary>
        [Parameter(Position = 3,
            Mandatory = true,
            HelpMessage = "Final Column for setting width")]
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
                    new System.Management.Automation.ParameterBindingException("Final Column must be greater than zero");
                }
            }
        }

        /// <summary>
        /// Cell Style Name
        /// </summary>
        [Parameter(Position = 4,
            Mandatory = true,
            HelpMessage = "Column Width")]
        [ValidateNotNullOrEmpty]
        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }

        /// <summary>
        /// Worksheet name to set the cell value
        /// </summary>
        [Parameter(Position = 5,
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

        #region  Cmdlet Overrides

        protected override void ProcessRecord()
        {
            foreach (var document in AllDocuments("Set-OpenXmlSpreadSheetColumnWidth"))
            {
                try
                {
                    if (!(document is SmlDocument))
                        throw new PowerToolsDocumentException("Not a spreadsheet document.");
                    OutputDocument(WorksheetAccessor.SetColumnWidth((SmlDocument)document, worksheetName, fromColumn, toColumn, width));
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
