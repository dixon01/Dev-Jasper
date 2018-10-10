// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolutionPropertiesState.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SolutionPropertiesState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Tools.CompactFrameworkConversion.ConverterApp.XmlSolution
{
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Xml;

    /// <summary>
    /// The <c>GlobalSection(SolutionProperties)</c> state.
    /// </summary>
    internal class SolutionPropertiesState : GlobalSectionState
    {
        private static readonly Regex PropertyRegex = new Regex(
            @"^([a-z0-9-]+) = ([a-z0-9-]+)$", RegexOptions.IgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionPropertiesState"/> class.
        /// </summary>
        public SolutionPropertiesState()
            : base("preSolution")
        {
        }

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
            var m = PropertyRegex.Match(line);
            if (!m.Success)
            {
                return base.ProcessLine(line, ref outputParent);
            }

            var solutionProperty = this.AppendElement(outputParent, "SolutionProperty");
            solutionProperty.SetAttribute("name", m.Groups[1].Value);
            solutionProperty.SetAttribute("value", m.Groups[2].Value);

            return this;
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
            if (element.Name != "SolutionProperty")
            {
                return base.EnterChild(output, element);
            }

            output.WriteLine("\t\t{0} = {1}", element.GetAttribute("name"), element.GetAttribute("value"));
            return null;
        }
    }
}