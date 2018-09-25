// -----------------------------------------------------------------------
// <copyright file="OpenXmlTest.cs" company="HP">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace WordDocumentModifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using DocumentFormat.OpenXml.Packaging;
    using DocumentFormat.OpenXml.Wordprocessing;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class OpenXmlTest
    {
        public static void Test(string filepath)
        {
            // Open a WordprocessingDocument for editing using the filepath.
            using (WordprocessingDocument wordprocessingDocument = WordprocessingDocument.Open(filepath, true))
            {

                // Assign a reference to the existing document body.
                Body body = wordprocessingDocument.MainDocumentPart.Document.Body;

                // Add new text.
                Paragraph para = body.AppendChild(new Paragraph());
                Run run = para.AppendChild(new Run());
                run.AppendChild(new Text("This is my sample text"));
                run.PrependChild(new RunProperties(new Bold()));

                // Close the handle explicitly.
                wordprocessingDocument.Close();
            }
        }
    }
}
