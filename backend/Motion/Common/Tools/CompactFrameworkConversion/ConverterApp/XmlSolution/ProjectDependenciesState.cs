// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectDependenciesState.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectDependenciesState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Tools.CompactFrameworkConversion.ConverterApp.XmlSolution
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Xml;

    /// <summary>
    /// The project dependencies state.
    /// </summary>
    internal class ProjectDependenciesState : State
    {
        private static readonly Regex DependencyRegex = new Regex(
            @"(\{[a-z0-9-]+\}) = (\{[a-z0-9-]+\})", RegexOptions.IgnoreCase);

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
            var m = DependencyRegex.Match(line);
            if (m.Success)
            {
                var projectDependency = this.AppendElement(outputParent, "ProjectDependency");
                projectDependency.SetAttribute("guid", m.Groups[1].Value);

                return this;
            }

            if (line.Equals("EndProjectSection"))
            {
                outputParent = outputParent.ParentNode; // </ProjectDependency>

                return new ProjectState();
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
            throw new NotImplementedException();
        }
    }
}