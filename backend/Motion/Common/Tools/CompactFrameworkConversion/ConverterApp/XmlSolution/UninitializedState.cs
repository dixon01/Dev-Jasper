// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UninitializedState.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UninitializedState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Tools.CompactFrameworkConversion.ConverterApp.XmlSolution
{
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Xml;

    /// <summary>
    /// The initial state of the state machine.
    /// </summary>
    internal class UninitializedState : State
    {
        private static readonly Regex VersionRegex =
            new Regex("Microsoft Visual Studio Solution File, Format Version ([0-9.]+)");

        /// <summary>
        /// Processes a single line of input in the original Visual Studio solution file (<c>.sln</c>).
        /// </summary>
        /// <param name="line">
        /// The line read from the file.
        /// </param>
        /// <param name="outputParent">
        /// When this method is called, <see cref="outputParent"/> is set to the parent node
        /// that was previously constructed.
        /// This method should then update <see cref="outputParent"/> to be the parent for the
        /// next line in the file.
        /// </param>
        /// <returns>
        /// The new state to be used to process the next line in the file.
        /// </returns>
        public override State ProcessLine(string line, ref XmlNode outputParent)
        {
            var m = VersionRegex.Match(line);
            if (!m.Success)
            {
                return base.ProcessLine(line, ref outputParent);
            }

            var solution = this.AppendElement(outputParent, "Solution");
            solution.SetAttribute("version", m.Groups[1].Value);
            outputParent = solution;
            return new SolutionState();
        }

        /// <summary>
        /// Recursively processes a node of the XML solution structure
        /// and writes in the Visual Studio solution file format to the given stream.
        /// </summary>
        /// <param name="output">
        /// The output stream to write to.
        /// </param>
        /// <param name="node">
        /// The node to process.
        /// </param>
        public override void ProcessNode(StreamWriter output, XmlNode node)
        {
            if (node.NodeType != XmlNodeType.Document)
            {
                return;
            }

            var element = node.FirstChild as XmlElement;
            if (element == null || element.Name != "Solution")
            {
                return;
            }

            string version = element.GetAttribute("version");
            output.WriteLine("Microsoft Visual Studio Solution File, Format Version {0}", version);
            if (version == "8.00")
            {
                output.WriteLine("# Visual Studio 2003");
            }
            else if (version == "9.00")
            {
                output.WriteLine("# Visual Studio 2005");
            }
            else if (version == "10.00")
            {
                output.WriteLine("# Visual Studio 2008");
            }
            else if (version == "11.00")
            {
                output.WriteLine("# Visual Studio 2010");
            }

            var state = new SolutionState();
            state.ProcessNode(output, element);
        }
    }
}