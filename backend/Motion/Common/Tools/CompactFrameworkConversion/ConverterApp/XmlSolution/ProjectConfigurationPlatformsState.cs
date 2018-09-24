// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectConfigurationPlatformsState.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectConfigurationPlatformsState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Tools.CompactFrameworkConversion.ConverterApp.XmlSolution
{
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Xml;

    /// <summary>
    /// The <c>ProjectConfigurationPlatforms</c> global section.
    /// </summary>
    internal class ProjectConfigurationPlatformsState : GlobalSectionState
    {
        private static readonly Regex PlatformRegex =
            new Regex(
                @"^(\{[a-z0-9-]+\})\.([a-z0-9]+\|[a-z0-9 ]+)\.([a-z0-9.]+) = ([a-z0-9]+\|[a-z0-9 ]+)$",
                RegexOptions.IgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectConfigurationPlatformsState"/> class.
        /// </summary>
        public ProjectConfigurationPlatformsState()
            : base("postSolution")
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
            var m = PlatformRegex.Match(line);
            if (m.Success)
            {
                var projectConfiguration = this.AppendElement(outputParent, "ProjectConfiguration");
                projectConfiguration.SetAttribute("guid", m.Groups[1].Value);
                projectConfiguration.SetAttribute("type", m.Groups[2].Value);
                projectConfiguration.SetAttribute("extension", m.Groups[3].Value);
                projectConfiguration.SetAttribute("value", m.Groups[4].Value);

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
            if (element.Name != "ProjectConfiguration")
            {
                return base.EnterChild(output, element);
            }

            output.WriteLine(
                "\t\t{0}.{1}.{2} = {3}",
                element.GetAttribute("guid"),
                element.GetAttribute("type"),
                element.GetAttribute("extension"),
                element.GetAttribute("value"));
            return null;
        }
    }
}