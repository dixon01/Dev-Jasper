// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolutionState.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SolutionState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Tools.CompactFrameworkConversion.ConverterApp.XmlSolution
{
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Xml;

    /// <summary>
    /// The state when the solution header was parsed (after <see cref="UninitializedState"/>).
    /// </summary>
    internal class SolutionState : State
    {
        private static readonly Regex ProjectRegex =
            new Regex(
                @"Project\(""(\{[a-z0-9-]+\})""\) = ""([^\""]+)"", ""([^\""]+)"", ""(\{[a-z0-9-]+\})""",
                RegexOptions.IgnoreCase);

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
            if (m.Success)
            {
                var project = this.AppendElement(outputParent, "Project");
                project.SetAttribute("version", m.Groups[1].Value);
                project.SetAttribute("guid", m.Groups[4].Value);
                project.SetAttribute("type", m.Groups[1].Value);
                project.SetAttribute("name", m.Groups[2].Value);
                project.SetAttribute("file", m.Groups[3].Value);
                outputParent = project;

                return new ProjectState();
            }

            if (line.Equals("Global"))
            {
                outputParent = this.AppendElement(outputParent, "Global");
                return new GlobalState();
            }

            if (line[0] == '#')
            {
                return this;
            }

            return base.ProcessLine(line, ref outputParent);
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
            if (element.Name == "Project")
            {
                output.WriteLine(
                    "Project(\"{0}\") = \"{1}\", \"{2}\", \"{3}\"",
                    element.GetAttribute("type"),
                    element.GetAttribute("name"),
                    element.GetAttribute("file"),
                    element.GetAttribute("guid"));
                if (element.ChildNodes.Count == 0)
                {
                    output.WriteLine("EndProject");
                    return null;
                }

                return new ProjectState();
            }

            if (element.Name == "Global")
            {
                output.WriteLine("Global");
                return new GlobalState();
            }

            return base.EnterChild(output, element);
        }
    }
}