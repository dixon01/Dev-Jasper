// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlobalState.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GlobalState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Tools.CompactFrameworkConversion.ConverterApp.XmlSolution
{
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Xml;

    /// <summary>
    /// The state when inside of the <c>Global</c> part of the solution file.
    /// </summary>
    internal class GlobalState : State
    {
        private static readonly Regex SectionRegex = new Regex(@"^GlobalSection\(([a-z]+)\)", RegexOptions.IgnoreCase);

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
            var m = SectionRegex.Match(line);
            if (m.Success)
            {
                outputParent = this.AppendElement(outputParent, m.Groups[1].Value);
                return GlobalSectionState.Create(m.Groups[1].Value);
            }

            if (line.Equals("EndGlobal"))
            {
                outputParent = outputParent.ParentNode;
                return new SolutionState();
            }

            return base.ProcessLine(line, ref outputParent);
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
            output.WriteLine("EndGlobal");
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
            var newState = GlobalSectionState.Create(element.Name);
            output.WriteLine("\tGlobalSection({0}) = {1}", element.Name, newState.Type);
            return newState;
        }
    }
}