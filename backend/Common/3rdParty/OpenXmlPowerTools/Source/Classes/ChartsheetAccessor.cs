/***************************************************************************

Copyright (c) Microsoft Corporation 2011.

This code is licensed using the Microsoft Public License (Ms-PL).  The text of the license can be found here:

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx

***************************************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;

namespace OpenXmlPowerTools
{
    /// <summary>
    /// Chart types available
    /// </summary>
    public enum ChartType
    {
        /// <summary>
        /// Bar
        /// </summary>
        Bar,
        /// <summary>
        /// Column
        /// </summary>
        Column,
        /// <summary>
        /// Line
        /// </summary>
        Line,
        /// <summary>
        /// Area
        /// </summary>
        Area,
        /// <summary>
        /// Pie
        /// </summary>
        Pie
    }

    /// <summary>
    /// Provides access to chartsheet operations
    /// </summary>
    public class ChartsheetAccessor
    {
        private const int defaultAnchorPosX = 0;
        private const int defaultAnchorPosY = 0;
        private const int defaultAnchorExtX = 8673523;
        private const int defaultAnchorExtY = 6306705;
        private const string defaultLegendPosition = "r";

        private static XNamespace ns;
        private static XNamespace relationshipsns;
        private static XNamespace sdrns;
        private static XNamespace drawingns;
        private static XNamespace chartns;

        static ChartsheetAccessor()
        {
            ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
            relationshipsns = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
            sdrns = "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing";
            drawingns = "http://schemas.openxmlformats.org/drawingml/2006/main";
            chartns =   "http://schemas.openxmlformats.org/drawingml/2006/chart";
        }

        /// <summary>
        /// Creates a chartsheet part from given data
        /// </summary>
        /// <param name="chartType">Type of chart to generate</param>
        /// <param name="values">Values to represent in the chart</param>
        /// <param name="headerReference">Columns to be used as series</param>
        /// <param name="categoryReference">Column to be used as category</param>
        /// <returns>Chartsheet part with contents related</returns>
        public static ChartsheetPart Create(SpreadsheetDocument parentDocument, ChartType chartType, List<string> values, List<string> headerReference, string categoryReference)
        {
            //Creates base content and associates it to a new chartsheet part
            WorkbookPart workbook = parentDocument.WorkbookPart;
            ChartsheetPart chartsheetPart = workbook.AddNewPart<ChartsheetPart>();
            XDocument chartsheetDocument = CreateEmptyChartsheet();
            chartsheetPart.PutXDocument(chartsheetDocument);

            //Creates a base drawings part and associates it to the chartsheet part
            DrawingsPart drawingsPart = chartsheetPart.AddNewPart<DrawingsPart>();
            XDocument drawingDocument = CreateEmptyDrawing();
            drawingsPart.PutXDocument(drawingDocument);
            
            //Adds content to chartsheet document to reference drawing document
            chartsheetDocument
                .Element(ns + "chartsheet")
                .Add(
                    new XElement(ns + "drawing",
                        new XAttribute(relationshipsns + "id", chartsheetPart.GetIdOfPart(drawingsPart))
                )
            );

            //creates the chart part and associates it to the drawings part
            ChartPart chartPart = drawingsPart.AddNewPart<ChartPart>();
            XDocument chartDocument = CreateChart(chartType, values, headerReference, categoryReference);
            chartPart.PutXDocument(chartDocument);

            //Adds content to drawing document to reference chart document
            drawingDocument
                .Descendants(drawingns + "graphicData")
                .First()
                .Add(
                    new XAttribute("uri", chartns),
                    new XElement(chartns + "chart",
                        new XAttribute(XNamespace.Xmlns + "c", chartns),
                        new XAttribute(XNamespace.Xmlns + "r", relationshipsns),
                        new XAttribute(relationshipsns + "id", drawingsPart.GetIdOfPart(chartPart))
                    )
                );

            //Associates the chartsheet part to the workbook part
            XDocument document = parentDocument.WorkbookPart.GetXDocument();

            int sheetId = document.Root.Element(ns + "sheets").Elements(ns + "sheet").Count() + 1;

            int chartsheetCount =
                document.Root
                .Element(ns + "sheets")
                .Elements(ns + "sheet")
                .Where(
                    t =>
                        t.Attribute("name").Value.StartsWith("chart")
                )
                .Count() + 1;

            //Adds content to workbook document to reference chartsheet document
            document.Root
                .Element(ns + "sheets")
                .Add(
                    new XElement(ns + "sheet",
                        new XAttribute("name", string.Format("chart{0}", chartsheetCount)),
                        new XAttribute("sheetId", sheetId),
                        new XAttribute(relationshipsns + "id", workbook.GetIdOfPart(chartsheetPart))
                    )
                );

            chartsheetPart.PutXDocument();
            drawingsPart.PutXDocument();
            parentDocument.WorkbookPart.PutXDocument();

            return chartsheetPart;
        }

        /// <summary>
        /// Creates element structure needed to describe an empty worksheet
        /// </summary>
        /// <returns>Document with contents for an empty worksheet</returns>
        private static XDocument CreateEmptyChartsheet()
        {
            XDocument document =
                new XDocument(
                    new XElement(ns + "chartsheet",
                        new XAttribute("xmlns", ns),
                        new XAttribute(XNamespace.Xmlns + "r", relationshipsns),
                        new XElement(ns + "sheetViews",
                            new XElement(ns + "sheetView",
                                new XAttribute("workbookViewId", 0)
                            )
                        )
                    )
                );
            return document;
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
                    new XAttribute("r", row)
                );
            return rowElement;
        }

        /// <summary>
        /// Creates element structure needed to describe an empty drawing
        /// </summary>
        private static XDocument CreateEmptyDrawing()
        {
            return new XDocument(
                new XElement(sdrns + "wsDr",
                    new XAttribute(XNamespace.Xmlns + "xdr", sdrns),
                    new XAttribute(XNamespace.Xmlns + "a", drawingns),
                    new XElement(sdrns + "absoluteAnchor",
                        new XElement(sdrns + "pos",
                            new XAttribute("x", defaultAnchorPosX),
                            new XAttribute("y", defaultAnchorPosY)
                        ),
                        new XElement(sdrns + "ext",
                            new XAttribute("cx", defaultAnchorExtX),
                            new XAttribute("cy", defaultAnchorExtY)
                        ),
                        new XElement(sdrns + "graphicFrame",
                            new XAttribute("macro", string.Empty),
                            new XElement(sdrns + "nvGraphicFramePr",
                                new XElement(sdrns + "cNvPr",
                                    new XAttribute("id", 2),
                                    new XAttribute("name", "Chart 1")
                                ),
                                new XElement(sdrns + "cNvGraphicFramePr",
                                    new XElement(drawingns + "graphicFrameLocks",
                                        new XAttribute("noGrp", "1")
                                    )
                                )
                            ),
                            new XElement(sdrns + "xfrm",
                                new XElement(drawingns + "off",
                                    new XAttribute("x", 0),
                                    new XAttribute("y", 0)
                                ),
                                new XElement(drawingns + "ext",
                                    new XAttribute("cx", 0),
                                    new XAttribute("cy", 0)
                                )
                            ),
                            new XElement(drawingns + "graphic",
                                new XElement(drawingns + "graphicData")
                            )
                        ),
                        new XElement(sdrns + "clientData")
                    )
                )
            );
        }

        /// <summary>
        /// Creates element structure needed to describe an empty chart
        /// </summary>
        private static XDocument CreateEmptyChart()
        {
            return new XDocument(
                new XElement(chartns + "chartSpace",
                    new XAttribute(XNamespace.Xmlns + "c", chartns),
                    new XAttribute(XNamespace.Xmlns + "a", drawingns),
                    new XAttribute(XNamespace.Xmlns + "r", relationshipsns),
                    new XElement(chartns + "chart",
                        new XElement(chartns + "title",
                            new XElement(chartns + "layout")
                        ),
                        new XElement(chartns + "plotArea",
                            new XElement(chartns + "layout")
                        ),
                        new XElement(chartns + "legend",
                            new XElement(chartns + "legendPos",
                                new XAttribute("val", defaultLegendPosition)
                            ),
                            new XElement(chartns + "layout")
                        )
                    )
                )
            );
        }

        /// <summary>
        /// Creates element structure needed to describe a chart with data related
        /// </summary>
        private static XDocument CreateChart(ChartType chartType, IEnumerable<string> seriesReferences, IEnumerable<string> seriesTitles, string category)
        {
            XDocument chartDocument = CreateEmptyChart();
            XElement chartElement =
                chartDocument
                .Element(chartns + "chartSpace")
                .Element(chartns + "chart")
                .Element(chartns + "plotArea");
            string categoryAxisId = "28819";
            string valueAxisId = "28818";

            //Chooses the right chart type
            switch (chartType)
            {
                case ChartType.Bar:
                    chartElement.Add(
                        CreateBarChart(seriesReferences, seriesTitles, category, categoryAxisId, valueAxisId),
                        CreateCategoryAxis(categoryAxisId, valueAxisId),
                        CreateValueAxis(valueAxisId, categoryAxisId)
                    );
                    break;
                case ChartType.Column:
                    chartElement.Add(
                        CreateColumnChart(seriesReferences, seriesTitles, category, categoryAxisId, valueAxisId),
                        CreateCategoryAxis(categoryAxisId, valueAxisId),
                        CreateValueAxis(valueAxisId, categoryAxisId)
                    );
                    break;
                case ChartType.Line:
                    chartElement.Add(
                        CreateLineChart(seriesReferences, seriesTitles, category, categoryAxisId, valueAxisId),
                        CreateCategoryAxis(categoryAxisId, valueAxisId),
                        CreateValueAxis(valueAxisId, categoryAxisId)
                    );
                    break;
                case ChartType.Area:
                    chartElement.Add(
                        CreateAreaChart(seriesReferences, seriesTitles, category, categoryAxisId, valueAxisId),
                        CreateCategoryAxis(categoryAxisId, valueAxisId),
                        CreateValueAxis(valueAxisId, categoryAxisId)
                    );
                    break;
                case ChartType.Pie:
                    chartElement.Add(
                        CreatePieChart(seriesReferences, seriesTitles, category, categoryAxisId, valueAxisId)
                    );
                    break;
            }
            return chartDocument;
        }

        /// <summary>
        /// Creates element structure needed to describe a column chart with data related
        /// </summary>
        private static XElement CreateColumnChart(IEnumerable<string> seriesReferences, IEnumerable<string> seriesTitles, string category, string categoryAxisId, string valueAxisId)
        {
            return new XElement(chartns + "barChart",
                new XElement(chartns + "barDir",
                    new XAttribute("val", "col")
                ),
                new XElement(chartns + "grouping",
                    new XAttribute("val", "clustered")
                ),
                CreateSeries(seriesReferences, seriesTitles, category),
                seriesReferences.Count() < 1 ?
                    new XElement(chartns + "gapWidth",
                        new XAttribute("val", 100)
                    )
                    : null,
                new XElement(chartns + "axId",
                    new XAttribute("val", categoryAxisId)
                ),
                new XElement(chartns + "axId",
                    new XAttribute("val", valueAxisId)
                )
            );
        }

        /// <summary>
        /// Creates element structure needed to describe a bar chart with data related
        /// </summary>
        private static XElement CreateBarChart(IEnumerable<string> seriesReferences, IEnumerable<string> seriesTitles, string category, string categoryAxisId, string valueAxisId)
        {
            return new XElement(chartns + "barChart",
                new XElement(chartns + "barDir",
                    new XAttribute("val", "bar")
                ),
                new XElement(chartns + "grouping",
                    new XAttribute("val", "clustered")
                ),
                CreateSeries(seriesReferences, seriesTitles, category),
                seriesReferences.Count() < 1 ?
                    new XElement(chartns + "gapWidth",
                        new XAttribute("val", 100)
                    )
                    : null,
                new XElement(chartns + "axId",
                    new XAttribute("val", categoryAxisId)
                ),
                new XElement(chartns + "axId",
                    new XAttribute("val", valueAxisId)
                )
            );
        }

        /// <summary>
        /// Creates element structure needed to describe a line chart with data related
        /// </summary>
        private static XElement CreateLineChart(IEnumerable<string> seriesReferences, IEnumerable<string> seriesTitles, string category, string categoryAxisId, string valueAxisId)
        {
            return new XElement(chartns + "lineChart",
                new XElement(chartns + "grouping",
                    new XAttribute("val", "standard")
                ),
                CreateSeries(seriesReferences, seriesTitles, category),
                new XElement(chartns + "marker",
                    new XAttribute("val", 1)
                ),
                new XElement(chartns + "axId",
                    new XAttribute("val", categoryAxisId)
                ),
                new XElement(chartns + "axId",
                    new XAttribute("val", valueAxisId)
                )
            );
        }

        /// <summary>
        /// Creates element structure needed to describe an area chart with data related
        /// </summary>
        private static XElement CreateAreaChart(IEnumerable<string> seriesReferences, IEnumerable<string> seriesTitles, string category, string categoryAxisId, string valueAxisId)
        {
            return new XElement(chartns + "areaChart",
                new XElement(chartns + "grouping",
                    new XAttribute("val", "stacked")
                ),
                CreateSeries(seriesReferences, seriesTitles, category),
                new XElement(chartns + "axId",
                    new XAttribute("val", categoryAxisId)
                ),
                new XElement(chartns + "axId",
                    new XAttribute("val", valueAxisId)
                )
            );
        }

        /// <summary>
        /// Creates element structure needed to describe a pie chart with data related
        /// </summary>
        private static XElement CreatePieChart(IEnumerable<string> seriesReferences, IEnumerable<string> seriesTitles, string category, string categoryAxisId, string valueAxisId)
        {
            return new XElement(chartns + "pieChart",
                new XElement(chartns + "varyColors",
                    new XAttribute("val", 1)
                ),
                CreateSeries(seriesReferences, seriesTitles, category),
                new XElement(chartns + "firstSliceAng",
                    new XAttribute("val", 0)
                )
            );
        }

        /// <summary>
        /// Creates element structure needed to describe a data series
        /// </summary>
        private static IEnumerable<XElement> CreateSeries(IEnumerable<string> seriesReference, IEnumerable<string> seriesTitles, string category)
        {
            List<XElement> seriesList = new List<XElement>();
            int numSeries = 0;
            foreach (var series in seriesReference)
            {
                seriesList.Add(
                    new XElement(chartns + "ser",
                        new XElement(chartns + "idx",
                            new XAttribute("val", numSeries)
                        ),
                        new XElement(chartns + "order",
                            new XAttribute("val", numSeries)
                        ),
                        new XElement(chartns + "tx",
                            new XElement(chartns + "strRef",
                                new XElement(chartns + "f", seriesTitles.ElementAt(numSeries))
                            )
                        ),
                        new XElement(chartns + "cat",
                            new XElement(chartns + "strRef",
                                new XElement(chartns + "f", category)
                            )
                        ),
                        new XElement(chartns + "val",
                            new XElement(chartns + "numRef",
                                new XElement(chartns + "f", series)
                            )
                        )
                    )
                );
                numSeries++;
            }
            return seriesList;
        }

        /// <summary>
        /// Creates element structure needed to describe a category axis
        /// </summary>
        private static XElement CreateCategoryAxis(string categoryAxisId, string valueAxisId)
        {
            return new XElement(chartns + "catAx",
                new XElement(chartns + "axId",
                    new XAttribute("val", categoryAxisId)
                ),
                new XElement(chartns + "scaling",
                    new XElement(chartns + "orientation",
                        new XAttribute("val", "minMax")
                    )
                ),
                new XElement(chartns + "axPos",
                    new XAttribute("val", "b")
                ),
                new XElement(chartns + "tickLblPos",
                    new XAttribute("val", "nextTo")
                ),
                new XElement(chartns + "crossAx",
                    new XAttribute("val", valueAxisId)
                ),
                new XElement(chartns + "crosses",
                    new XAttribute("val", "autoZero")
                ),
                new XElement(chartns + "auto",
                    new XAttribute("val", 1)
                ),
                new XElement(chartns + "lblAlgn",
                    new XAttribute("val", "ctr")
                ),
                new XElement(chartns + "lblOffset",
                    new XAttribute("val", 100)
                )
            );
        }

        /// <summary>
        /// Creates element structure needed to describe a value axis
        /// </summary>
        private static XElement CreateValueAxis(string valueAxisId, string categoryAxisId)
        {
            return new XElement(chartns + "valAx",
                new XElement(chartns + "axId",
                    new XAttribute("val", valueAxisId)
                ),
                new XElement(chartns + "scaling",
                    new XElement(chartns + "orientation",
                        new XAttribute("val", "minMax")
                    )
                ),
                new XElement(chartns + "axPos",
                    new XAttribute("val", "l")
                ),
                new XElement(chartns + "majorGridlines"),
                new XElement(chartns + "numFmt",
                    new XAttribute("formatCode", "General"),
                    new XAttribute("sourceLinked", "1")
                ),
                new XElement(chartns + "tickLblPos",
                    new XAttribute("val", "nextTo")
                ),
                new XElement(chartns + "crossAx",
                    new XAttribute("val", categoryAxisId)
                ),
                new XElement(chartns + "crosses",
                    new XAttribute("val", "autoZero")
                ),
                new XElement(chartns + "crossBetween",
                    new XAttribute("val", "between")
                )
            );
        }
    }
}
