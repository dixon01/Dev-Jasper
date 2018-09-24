// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensibilityGlobalsState.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ExtensibilityGlobalsState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Tools.CompactFrameworkConversion.ConverterApp.XmlSolution
{
    using System.Text.RegularExpressions;
    using System.Xml;

    /// <summary>
    /// The <c>ExtensibilityGlobals</c> global section.
    /// </summary>
    internal class ExtensibilityGlobalsState : GlobalSectionState
    {
        private static readonly Regex PropertyRegex = new Regex(
            @"^([a-z0-9-]+) = ([a-z0-9-]+)$", RegexOptions.IgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensibilityGlobalsState"/> class.
        /// </summary>
        public ExtensibilityGlobalsState()
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
            var m = PropertyRegex.Match(line);
            if (!m.Success)
            {
                return base.ProcessLine(line, ref outputParent);
            }

            var extensibilityGlobal = this.AppendElement(outputParent, "ExtensibilityGlobals");
            extensibilityGlobal.SetAttribute("name", m.Groups[1].Value);
            extensibilityGlobal.SetAttribute("value", m.Groups[2].Value);

            return this;
        }
    }
}