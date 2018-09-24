// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlobalSectionState.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GlobalSectionState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Tools.CompactFrameworkConversion.ConverterApp.XmlSolution
{
    using System;
    using System.IO;
    using System.Xml;

    /// <summary>
    /// The state when inside one of the <c>GlobalSection</c>s of the solution file.
    /// </summary>
    internal abstract class GlobalSectionState : State
    {
        private readonly string type;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalSectionState"/> class.
        /// </summary>
        /// <param name="type">
        /// The type (<c>preSolution</c> or <c>postSolution</c>).
        /// </param>
        protected GlobalSectionState(string type)
        {
            this.type = type;
        }

        /// <summary>
        /// Gets the type of this section (<c>preSolution</c> or <c>postSolution</c>).
        /// </summary>
        public string Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Creates a new <see cref="GlobalSectionState"/> implementation for the given section name.
        /// </summary>
        /// <param name="name">
        /// The section name.
        /// </param>
        /// <returns>
        /// The new <see cref="GlobalSectionState"/>.
        /// </returns>
        public static GlobalSectionState Create(string name)
        {
            switch (name)
            {
                case "SolutionConfigurationPlatforms":
                    return new SolutionConfigurationPlatformsState();
                case "ProjectConfigurationPlatforms":
                    return new ProjectConfigurationPlatformsState();
                case "SolutionProperties":
                    return new SolutionPropertiesState();
                case "NestedProjects":
                    return new NestedProjectsState();
                case "ExtensibilityGlobals":
                    return new ExtensibilityGlobalsState();
                case "TeamFoundationVersionControl":
                    return new TeamFoundationVersionControlState();
                case "TestCaseManagementSettings":
                    return new TestCaseManagementSettingsState();
                default:
                    throw new NotSupportedException("Unknown global section " + name);
            }
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
            if (line.Equals("EndGlobalSection"))
            {
                outputParent = outputParent.ParentNode;
                return new GlobalState();
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
            output.WriteLine("\tEndGlobalSection");
        }
    }
}