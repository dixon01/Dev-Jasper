// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NestedProjectsState.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NestedProjectsState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Tools.CompactFrameworkConversion.ConverterApp.XmlSolution
{
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Xml;

    /// <summary>
    /// The <c>NestedProject</c> global section.
    /// </summary>
    internal class NestedProjectsState : GlobalSectionState
    {
        private static readonly Regex ProjectRegex = new Regex(
            @"^(\{[a-z0-9-]+\}) = (\{[a-z0-9-]+\})$", RegexOptions.IgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="NestedProjectsState"/> class.
        /// </summary>
        public NestedProjectsState()
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
            var m = ProjectRegex.Match(line);
            if (!m.Success)
            {
                return base.ProcessLine(line, ref outputParent);
            }

            var nestedProject = this.AppendElement(outputParent, "NestedProject");
            nestedProject.SetAttribute("guid", m.Groups[1].Value);
            nestedProject.SetAttribute("parent", m.Groups[2].Value);

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
            if (element.Name != "NestedProject")
            {
                return base.EnterChild(output, element);
            }

            output.WriteLine("\t\t{0} = {1}", element.GetAttribute("guid"), element.GetAttribute("parent"));
            return null;
        }
    }
}