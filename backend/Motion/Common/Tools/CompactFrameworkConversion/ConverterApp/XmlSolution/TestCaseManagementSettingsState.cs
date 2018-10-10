// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestCaseManagementSettingsState.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TestCaseManagementSettingsState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Tools.CompactFrameworkConversion.ConverterApp.XmlSolution
{
    using System;
    using System.IO;
    using System.Xml;

    /// <summary>
    /// The <c>TestCaseManagementSettings</c> global section.
    /// </summary>
    internal class TestCaseManagementSettingsState : GlobalSectionState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseManagementSettingsState"/> class.
        /// </summary>
        public TestCaseManagementSettingsState()
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
            var parts = line.Split(new[] { " = " }, 2, StringSplitOptions.None);
            if (parts.Length != 2)
            {
                return base.ProcessLine(line, ref outputParent);
            }

            var solutionConfiguration = this.AppendElement(outputParent, "Property");
            solutionConfiguration.SetAttribute("name", parts[0]);
            solutionConfiguration.SetAttribute("value", parts[1]);

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
            if (element.Name != "Property")
            {
                return base.EnterChild(output, element);
            }

            output.WriteLine("\t\t{0} = {1}", element.GetAttribute("name"), element.GetAttribute("value"));
            return null;
        }
    }
}