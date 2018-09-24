// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlSolutionConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the XmlSolutionConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Tools.CompactFrameworkConversion.ConverterApp.XmlSolution
{
    using System.IO;
    using System.Xml;

    /// <summary>
    /// Converter that reads <c>.sln</c> files and generates an XML structure (and vice-versa).
    /// </summary>
    public static class XmlSolutionConverter
    {
        /// <summary>
        /// Creates an <see cref="XmlDocument"/> from the given solution file.
        /// </summary>
        /// <param name="solutionFileName">
        /// The solution file name.
        /// </param>
        /// <returns>
        /// The new <see cref="XmlDocument"/>.
        /// </returns>
        public static XmlDocument CreateXml(string solutionFileName)
        {
            var document = new XmlDocument();
            using (var reader = File.OpenText(solutionFileName))
            {
                State state = new UninitializedState();
                XmlNode node = document;

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.Length == 0)
                    {
                        continue;
                    }

                    state = state.ProcessLine(line, ref node);
                }

                if (!(state is SolutionState))
                {
                    throw new EndOfStreamException("Unexpected end of file: " + state.GetType().Name);
                }
            }

            return document;
        }

        /// <summary>
        /// Converts an <see cref="XmlDocument"/> to a stream.
        /// </summary>
        /// <param name="document">
        /// The XML document.
        /// </param>
        /// <param name="output">
        /// The output stream to write to.
        /// </param>
        public static void CreateSolution(XmlDocument document, Stream output)
        {
            output.WriteByte(0xEF);
            output.WriteByte(0xBB);
            output.WriteByte(0xBF);
            output.Flush();

            using (var writer = new StreamWriter(output))
            {
                writer.WriteLine();

                var state = new UninitializedState();
                state.ProcessNode(writer, document);
            }
        }
    }
}
