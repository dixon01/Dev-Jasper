/***************************************************************************

Copyright (c) Microsoft Corporation 2011.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using System.Xml;

namespace OpenXmlPowerTools
{
    /// <summary>
    /// Provides access to worksheet operations
    /// </summary>
    public class WorksheetAccessor
    {
        private static XNamespace ns;
        private static XNamespace relationshipsns;

        static WorksheetAccessor()
        {
            ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
            relationshipsns = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
        }

        /// <summary>
        /// Returns a worksheet located at a specific index
        /// </summary>
        /// <param name="worksheetIndex">Index for the worksheet to be returned</param>
        /// <returns></returns>
        public static WorksheetPart Get(SpreadsheetDocument document, int worksheetIndex)
        {
            return document.WorkbookPart.WorksheetParts.ElementAt(worksheetIndex);
        }

        /// <summary>
        /// Returns the worksheet corresponding to the specified name
        /// </summary>
        /// <param name="worksheetName">Name for the worksheet to be returned</param>
        /// <returns></returns>
        public static WorksheetPart Get(SpreadsheetDocument document, string worksheetName)
        {
            XDocument workbook = document.WorkbookPart.GetXDocument();
            XElement worksheetXelement = workbook.Root.Element(ns + "sheets")
                                        .Elements(ns + "sheet")
                                        .Where(s => s.Attribute("name").Value.ToLower().Equals(worksheetName.ToLower())).FirstOrDefault();
            return (WorksheetPart)document.WorkbookPart
                                                .GetPartById(worksheetXelement.Attribute(relationshipsns + "id").Value);

            //return parentDocument.Document.WorkbookPart.WorksheetParts.FirstOrDefault(worksheet => worksheet.Uri.OriginalString.Split(new string[]{"/"},StringSplitOptions.RemoveEmptyEntries).Last<string>().ToLower().Equals(worksheetName.ToLower()+".xml"));

        }

        /// <summary>
        /// Adds a given worksheet to the document
        /// </summary>
        /// <param name="worksheet">Worksheet document to add</param>
        /// <returns>Worksheet part just added</returns>
        public static WorksheetPart Add(SpreadsheetDocument doc, XDocument worksheet)
        {
            // Associates base content to a new worksheet part
            WorkbookPart workbook = doc.WorkbookPart;
            WorksheetPart worksheetPart = workbook.AddNewPart<WorksheetPart>();
            worksheetPart.PutXDocument(worksheet);

            // Associates the worksheet part to the workbook part
            XDocument document = doc.WorkbookPart.GetXDocument();
            int sheetId =
                document.Root
                .Element(ns + "sheets")
                .Elements(ns + "sheet")
                .Count() + 1;

            int worksheetCount =
                document.Root
                .Element(ns + "sheets")
                .Elements(ns + "sheet")
                .Where(
                    t =>
                        t.Attribute("name").Value.StartsWith("sheet", StringComparison.OrdinalIgnoreCase)
                )
                .Count() + 1;

            // Adds content to workbook document to reference worksheet document
            document.Root
                .Element(ns + "sheets")
                .Add(
                    new XElement(ns + "sheet",
                        new XAttribute("name", string.Format("sheet{0}", worksheetCount)),
                        new XAttribute("sheetId", sheetId),
                        new XAttribute(relationshipsns + "id", workbook.GetIdOfPart(worksheetPart))
                    )
                );
            doc.WorkbookPart.PutXDocument();
            return worksheetPart;
        }

        /// <summary>
        /// Creates element structure needed to describe an empty worksheet
        /// </summary>
        /// <returns>Document with contents for an empty worksheet</returns>
        private static XDocument CreateEmptyWorksheet()
        {
            XDocument document =
                new XDocument(
                    new XElement(ns + "worksheet",
                        new XAttribute("xmlns", ns),
                        new XAttribute(XNamespace.Xmlns + "r", relationshipsns),
                        new XElement(ns + "sheetData")
                    )
                );
            return document;
        }

        /// <summary>
        /// Adds a value to a cell inside a worksheet document
        /// </summary>
        /// <param name="worksheet">document to add values</param>
        /// <param name="row">Row</param>
        /// <param name="column">Column</param>
        /// <param name="value">Value to add</param>
        private static void AddValue(XDocument worksheet, int row, int column, string value)
        {
            //Set the cell reference
            string cellReference = GetColumnId(column) + row.ToString();
            double numericValue;
            //Determining if value for cell is text or numeric
            bool valueIsNumeric = double.TryParse(value, out numericValue);

            //Creating the new cell element (markup)
            XElement newCellXElement = valueIsNumeric ?
                    new XElement(ns + "c",
                        new XAttribute("r", cellReference),
                        new XElement(ns + "v", numericValue)
                    )
                :
                    new XElement(ns + "c",
                        new XAttribute("r", cellReference),
                        new XAttribute("t", "inlineStr"),
                        new XElement(ns + "is",
                            new XElement(ns + "t", value)
                        )
                    );

            // Find the row containing the cell to add the value to
            XName rowName = "r";
            XElement rowElement =
                worksheet.Root
                    .Element(ns + "sheetData")
                    .Elements(ns + "row")
                    .Where(
                        t => t.Attribute(rowName).Value == row.ToString()
                    )
                    .FirstOrDefault();

            if (rowElement == null)
            {
                //row element does not exist
                //create a new one
                rowElement = CreateEmptyRow(row);

                //row elements must appear in order inside sheetData element
                if (worksheet.Root
                 .Element(ns + "sheetData").HasElements)
                {   //if there are more rows already defined at sheetData element
                    //find the row with the inmediate higher index for the row containing the cell to set the value to
                    XElement rowAfterElement = FindRowAfter(worksheet, row);
                    //if there is a row with an inmediate higher index already defined at sheetData
                    if (rowAfterElement != null)
                    {
                        //add the new row before the row with an inmediate higher index
                        rowAfterElement.AddBeforeSelf(rowElement);
                    }
                    else
                    {   //this row is going to be the one with the highest index (add it as the last element for sheetData)
                        worksheet.Root.Element(ns + "sheetData").Elements(ns + "row").Last().AddAfterSelf(rowElement);
                    }
                }
                else
                {   //there are no other rows already defined at sheetData
                    //Add a new row elemento to sheetData
                    worksheet
                        .Root
                        .Element(ns + "sheetData")
                        .Add(
                            rowElement //= CreateEmptyRow(row)
                        );
                }

                //Add the new cell to the row Element
                rowElement.Add(newCellXElement);
            }
            else
            {
                //row containing the cell to set the value to is already defined at sheetData
                //look if cell already exist at that row
                XElement currentCellXElement = rowElement
                    .Elements(ns + "c")
                    .Where(
                        t => t.Attribute("r").Value == cellReference
                    ).FirstOrDefault();

                if (currentCellXElement == null)
                {   //cell element does not exist at row indicated as parameter
                    //find the inmediate right column for the cell to set the value to
                    XElement columnAfterXElement = FindColumAfter(worksheet, row, column);
                    if (columnAfterXElement != null)
                    {
                        //Insert the new cell before the inmediate right column
                        columnAfterXElement.AddBeforeSelf(newCellXElement);
                    }
                    else
                    {   //There is no inmediate right cell 
                        //Add the new cell as the last element for the row
                        rowElement.Add(newCellXElement);
                    }
                }
                else
                {
                    //cell alreay exist
                    //replace the current cell with that with the new value
                    currentCellXElement.ReplaceWith(newCellXElement);
                }
            }

        }

        /// <summary>
        /// Finds the row with the inmediate higher index for a specific row
        /// </summary>
        /// <param name="worksheet">Worksheet for finding the row</param>
        /// <param name="row">Row index to look for inmediate higher row</param>
        /// <returns></returns>
        private static XElement FindRowAfter(XDocument worksheet, int row)
        {
            XElement rowAfterXElement = worksheet.Root
            .Element(ns + "sheetData")
            .Elements(ns + "row").FirstOrDefault(r => System.Convert.ToInt32(r.Attribute("r").Value) > row);

            return rowAfterXElement;
        }

        private static XElement FindColumAfter(XDocument worksheet, int row, int column)
        {
            XElement columnAfterXElement = worksheet.Root
                .Element(ns + "sheetData")
                .Elements(ns + "row").FirstOrDefault(r => System.Convert.ToInt32(r.Attribute("r").Value) == row)
                .Elements(ns + "c").FirstOrDefault(c =>
                    GetColumnNumberFromCellReference(c.Attribute("r").Value, row) > GetColumnNumberFromCellReference(GetColumnId(column) + row, row));

            return columnAfterXElement;
        }

        /// <summary>
        /// Returns the column number from the cell reference received as parameter
        /// </summary>
        /// <param name="cellReference">Cell reference to obtain the column number from</param>
        /// <param name="row">Row containing the cell to obtain the column number from</param>
        /// <returns></returns>
        private static int GetColumnNumberFromCellReference(string cellReference, int row)
        {
            int columnNumber = 0;
            //Removing row number from cell reference
            string columnReference = cellReference.Remove(cellReference.Length - row.ToString().Length);

            int charPosition = 1;
            int charValue = 0;
            foreach (char c in columnReference)
            {
                //Getting the Unicode value for the current char in cell reference
                charValue = System.Convert.ToInt32(c);
                if (charPosition < columnReference.Length)
                {   //we have not reached the last character in cell reference
                    //we need to multiply the charValue (from 0 to 25) by 26 and add the result of powering 26 to current char position in cell reference
                    //65 is the Unicode value for "A" letter
                    //26 is the number of letters in English alphabet
                    columnNumber += (((charValue - 65) * 26) + (System.Convert.ToInt32(Math.Pow(26, charPosition++))));
                }
                else
                {   //This is the last character in cell reference
                    //we substract 64 instead of 65 because we want to get a one-based index for last character (instead of a zero-based index for previous characters)
                    columnNumber += (charValue - 64);
                }
            }
            return columnNumber;
        }

        /// <summary>
        /// Creates tags to describe an empty row
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private static XElement CreateEmptyRow(int row)
        {
            XElement rowElement =
                new XElement(ns + "row",
                    new XAttribute("r", row.ToString())
                );
            return rowElement;
        }

        /// <summary>
        /// Gets the column Id for a given column number
        /// </summary>
        /// <param name="columnNumber">Column number</param>
        /// <returns>Column Id</returns>
        public static string GetColumnId(int columnNumber)
        {
            int alfa = (int)'Z' - (int)'A' + 1;
            if (columnNumber <= alfa)
                return ((char)((int)'A' + columnNumber - 1)).ToString();
            else
                return
                    GetColumnId(
                        (int)((columnNumber - 1) / alfa)
                    ) +
                    (
                        (char)(
                            (int)'A' + (int)((columnNumber - 1) % alfa)
                        )
                    ).ToString();
        }


        /// <summary>
        /// Creates a worksheet document and inserts data into it
        /// </summary>
        /// <param name="headerList">List of values that will act as the header</param>
        /// <param name="valueTable">Values for worksheet content</param>
        /// <param name="headerRow">Header row</param>
        /// <returns></returns>
        internal static WorksheetPart Create(SpreadsheetDocument document, List<string> headerList, string[][] valueTable, int headerRow)
        {
            XDocument xDocument = CreateEmptyWorksheet();
            for (int i = 0; i < headerList.Count; i++)
            {
                AddValue(xDocument, headerRow, i + 1, headerList[i]);
            }
            int rows = valueTable.GetLength(0);
            int cols = valueTable[0].GetLength(0);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    AddValue(xDocument, i + headerRow + 1, j + 1, valueTable[i][j]);
                }
            }
            WorksheetPart part = Add(document, xDocument);
            return part;
        }

        /// <summary>
        /// Get value for a cell in a worksheet
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <remarks>Author:Johann Granados Company: Staff DotNet Creation Date: 8/30/2008</remarks>
        /// <returns></returns>
        public static string GetValue(SpreadsheetDocument document, WorksheetPart worksheet, short column, int row)
        {
            XDocument worksheetXDocument = XDocument.Load(new XmlTextReader(worksheet.GetStream()));
            XElement cellValueXElement = GetCell(worksheetXDocument, column, row);

            if (cellValueXElement != null)
            {
                if (cellValueXElement.Element(ns + "v") != null)
                {
                    return GetSharedString(document, System.Convert.ToInt32(cellValueXElement.Value));
                }
                else
                {
                    return cellValueXElement.Element(ns + "is").Element(ns + "t").Value;
                }
            }
            else
            {
                return string.Empty;
            }

        }
        private static string GetSharedString(SpreadsheetDocument document, int index)
        {
            XDocument sharedStringsXDocument = XDocument.Load(new XmlTextReader(document.WorkbookPart.SharedStringTablePart.GetStream()));
            return sharedStringsXDocument.Root.Elements().ElementAt<XElement>(index).Value;
        }

        /// <summary>
        /// Set the value for a specific cell
        /// </summary>
        /// <param name="worksheet">Worksheet part containing the cell to be affected</param>
        /// <param name="fromColumn">Initial column for setting the value</param>
        /// <param name="fromRow">Initial row for setting the value</param>
        /// <param name="toColumn">Final column for setting the value</param>
        /// <param name="toRow">Final row for setting the value</param>
        /// <param name="value">Cell value</param>
        public static OpenXmlPowerToolsDocument SetCellValue(SmlDocument doc, string worksheetName, int fromRow, int toRow, int fromColumn, int toColumn, string value)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(doc))
            {
                using (SpreadsheetDocument document = streamDoc.GetSpreadsheetDocument())
                {
                    WorksheetPart worksheet = Get(document, worksheetName);
                    XDocument worksheetXDocument = XDocument.Load(new XmlTextReader(worksheet.GetStream()));
                    for (int row = fromRow; row <= toRow; row++)
                    {
                        for (int col = fromColumn; col <= toColumn; col++)
                        {
                            AddValue(worksheetXDocument, row, col, value);
                        }
                    }

                    XmlWriter worksheetWriter = XmlTextWriter.Create(worksheet.GetStream(System.IO.FileMode.Create));
                    worksheetXDocument.WriteTo(worksheetWriter);
                    worksheetWriter.Flush();
                    worksheetWriter.Close();
                }
                return streamDoc.GetModifiedDocument();
            }
        }

        /// <summary>
        /// Apply a cell style to a specific cell
        /// </summary>
        /// <param name="worksheet">worksheet containing the cell to be affected</param>
        /// <param name="fromColumn">Starting Cell Column</param>
        /// <param name="toColumn">Ending Cell Column</param>
        /// <param name="fromRow">Starting Cell Row</param>
        /// <param name="toRow">Ending Cell Row</param>
        /// <param name="cellStyle">Cell Style</param>
        public static OpenXmlPowerToolsDocument SetCellStyle(SmlDocument doc, string worksheetName, short fromColumn, short toColumn, int fromRow, int toRow, string cellStyle)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(doc))
            {
                using (SpreadsheetDocument document = streamDoc.GetSpreadsheetDocument())
                {
                    WorksheetPart worksheet = Get(document, worksheetName);
                    XDocument worksheetXDocument = XDocument.Load(new XmlTextReader(worksheet.GetStream()));

                    for (int row = fromRow; row <= toRow; row++)
                    {
                        for (short col = fromColumn; col <= toColumn; col++)
                        {
                            XElement cellXelement = GetCell(worksheetXDocument, col, row);
                            cellXelement.SetAttributeValue("s", SpreadSheetStyleAccessor.GetCellStyleIndex(document, cellStyle));
                        }
                    }

                    XmlWriter worksheetWriter = XmlTextWriter.Create(worksheet.GetStream(System.IO.FileMode.Create));
                    worksheetXDocument.WriteTo(worksheetWriter);
                    worksheetWriter.Flush();
                    worksheetWriter.Close();
                }
                return streamDoc.GetModifiedDocument();
            }
        }

        /// <summary>
        /// Set the width for a range of columns
        /// </summary>
        /// <param name="worksheet">Worksheet containing the columns to be affected</param>
        /// <param name="fromColumn">Initial column to affect</param>
        /// <param name="toColumn">Final column to affect</param>
        /// <param name="width">Column width</param>
        public static OpenXmlPowerToolsDocument SetColumnWidth(SmlDocument doc, string worksheetName, short fromColumn, short toColumn, int width)
        {
            using (OpenXmlMemoryStreamDocument streamDoc = new OpenXmlMemoryStreamDocument(doc))
            {
                using (SpreadsheetDocument document = streamDoc.GetSpreadsheetDocument())
                {
                    WorksheetPart worksheet = Get(document, worksheetName);
                    //Get the worksheet markup
                    XDocument worksheetXDocument = XDocument.Load(new XmlTextReader(worksheet.GetStream()));
                    //Look for worksheet cols element
                    XElement colsXElement = worksheetXDocument.Root.Element(ns + "cols");
                    if (colsXElement == null)
                    {
                        //cols elements does not exist
                        //create a new one
                        colsXElement = new XElement(ns + "cols");
                        //create a new col element (for setting the width) 
                        //the col element could span more than one column -span is controlled by min (initial column) and max (final column) attributes
                        colsXElement.Add(new XElement(ns + "col",
                                                                 new XAttribute("min", fromColumn.ToString()),
                                                                 new XAttribute("max", toColumn.ToString()),
                                                                 new XAttribute("width", width.ToString()),
                                                                 new XAttribute("customWidth", "1")));

                        //cols element must be added before worksheet sheetData element
                        worksheetXDocument.Root.Element(ns + "sheetData").AddBeforeSelf(colsXElement);
                    }
                    else
                    {
                        //look for a col element for the column range indicated for fromColumn and toColumn
                        XElement colXElement = colsXElement.Elements(ns + "col")
                                        .Where(c => (System.Convert.ToInt32(c.Attribute("min").Value) == fromColumn) && (System.Convert.ToInt32(c.Attribute("max").Value) == toColumn)).FirstOrDefault();
                        if (colXElement != null)
                        {
                            //col element does exist
                            //change its width value
                            colXElement.SetAttributeValue("width", width);
                        }
                        else
                        {
                            //col element does not exist
                            //create a new one
                            colsXElement.Add(new XElement(ns + "col",
                                                                 new XAttribute("min", fromColumn.ToString()),
                                                                 new XAttribute("max", toColumn.ToString()),
                                                                 new XAttribute("width", width.ToString()),
                                                                 new XAttribute("customWidth", "1")));
                        }
                    }
                    //Update the worksheet part markup at worksheet part stream
                    XmlWriter worksheetWriter = XmlTextWriter.Create(worksheet.GetStream(System.IO.FileMode.Create));
                    worksheetXDocument.WriteTo(worksheetWriter);
                    worksheetWriter.Flush();
                    worksheetWriter.Close();
                }
                return streamDoc.GetModifiedDocument();
            }
        }

        /// <summary>
        /// Return a XElement representing the markup for a specific cell in a SpreadsheetML document
        /// </summary>
        /// <param name="worksheetXDocument">Worksheet markup</param>
        /// <param name="column">Cell column</param>
        /// <param name="row">Cell row</param>
        /// <returns></returns>
        private static XElement GetCell(XDocument worksheetXDocument, short column, int row)
        {
            return worksheetXDocument.Root
                   .Element(ns + "sheetData")
                   .Elements(ns + "row")
                   .Where(
                           r => r.Attribute("r").Value.Equals(row.ToString())).FirstOrDefault<XElement>()
                   .Elements(ns + "c").Where(c => c.Attribute("r").Value.Equals(GetColumnId(column) + row.ToString())).FirstOrDefault<XElement>();
        }
    }
}
