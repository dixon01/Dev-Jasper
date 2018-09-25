// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolutionItemsState.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SolutionItemsState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Tools.CompactFrameworkConversion.ConverterApp.XmlSolution
{
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Xml;

    /// <summary>
    /// The <c>ProjectSection(SolutionItems)</c> state.
    /// </summary>
    internal class SolutionItemsState : State
    {
        private static readonly Regex ItemRegex = new Regex(@"([^=]+) = ([^=]+)", RegexOptions.IgnoreCase);

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
            if (line.Equals("EndProjectSection"))
            {
                outputParent = outputParent.ParentNode; // </SolutionItems>

                return new ProjectState();
            }

            var m = ItemRegex.Match(line);
            if (!m.Success)
            {
                return base.ProcessLine(line, ref outputParent);
            }

            var solutionItem = this.AppendElement(outputParent, "SolutionItem");
            solutionItem.SetAttribute("name", m.Groups[1].Value);
            solutionItem.SetAttribute("path", m.Groups[2].Value);

            return this;
        }

        /// <summary>
        /// Recursively processes an element of the XML solution structure
        /// and writes in the Visual Studio solution file format to the given stream.
        /// This is a convenience method directly called by <see cref="State.ProcessNode"/>.
        /// Overriding methods should always call the base class implementation.
        /// </summary>
        /// <param name="output">
        /// The output stream to write to.
        /// </param>
        /// <param name="element">
        /// The element.
        /// </param>
        protected override void ProcessElement(StreamWriter output, XmlElement element)
        {
            base.ProcessElement(output, element);
            output.WriteLine("\tEndProjectSection");
        }

        /// <summary>
        /// This method is called when a child element is processed.
        /// </summary>
        /// <param name="output">
        /// The output stream to write to.
        /// </param>
        /// <param name="element">
        /// The child element element.
        /// </param>
        /// <returns>
        /// The new state to be used to process the child element.
        /// This can be null which means the child won't be processed.
        /// </returns>
        protected override State EnterChild(StreamWriter output, XmlElement element)
        {
            if (element.Name == "SolutionItem")
            {
                output.WriteLine("\t\t{0} = {1}", element.GetAttribute("name"), element.GetAttribute("path"));
                return null;
            }

            return base.EnterChild(output, element);
        }
    }
}