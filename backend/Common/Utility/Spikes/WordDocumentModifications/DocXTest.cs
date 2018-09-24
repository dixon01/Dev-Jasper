// -----------------------------------------------------------------------
// <copyright file="DocXTest.cs" company="HP">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace WordDocumentModifications
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;

    using Novacode;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class DocXTest
    {
        public static void Test(string fileName)
        {
            using (var document = DocX.Load("Test.docx"))
            {
                var p = document.InsertParagraph();
                p.Append("I am ")
                 .Append("bold")
                 .Bold()
                 .Append(" and I am ")
                 .Append("italic")
                 .Italic()
                 .Append(".")
                 .AppendLine("I am ")
                 .Append("Arial Black")
                 .Font(new FontFamily("Arial Black"))
                 .Append(" and I am not.")
                 .AppendLine("I am ")
                 .Append("BLUE")
                 .Color(Color.Blue)
                 .Append(" and I am")
                 .Append("Red")
                 .Color(Color.Red)
                 .Append(".");

                // Save this document.
                document.Save();

            }
        }
    }
}
