// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TeamFoundationVersionControlState.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TeamFoundationVersionControlState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Tools.CompactFrameworkConversion.ConverterApp.XmlSolution
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Xml;

    /// <summary>
    /// The <c>TeamFoundationVersionControl</c> global section.
    /// </summary>
    internal class TeamFoundationVersionControlState : GlobalSectionState
    {
        private static readonly Regex PropertyRegex = new Regex(@"^([a-z]+)(\d*) = (.+)$", RegexOptions.IgnoreCase);

        private string lastIndexString;

        private int childIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamFoundationVersionControlState"/> class.
        /// </summary>
        public TeamFoundationVersionControlState()
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
                if (outputParent.Name == "SccProject")
                {
                    outputParent = outputParent.ParentNode;
                }

                return base.ProcessLine(line, ref outputParent);
            }

            var parent = outputParent as XmlElement;
            if (parent == null)
            {
                throw new NotSupportedException("Couldn't find parent element");
            }

            if (m.Groups[2].Length == 0)
            {
                if (m.Groups[1].Value != "SccNumberOfProjects")
                {
                    parent.SetAttribute(m.Groups[1].Value, m.Groups[3].Value);
                }

                return this;
            }

            if (this.lastIndexString != m.Groups[2].Value)
            {
                if (this.lastIndexString != null)
                {
                    outputParent = outputParent.ParentNode;
                }

                this.lastIndexString = m.Groups[2].Value;
                parent = this.AppendElement(outputParent, "SccProject");
                outputParent = parent;
            }

            parent.SetAttribute(m.Groups[1].Value, m.Groups[3].Value);

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
            output.WriteLine("\t\tSccNumberOfProjects = {0}", element.ChildNodes.Count);
            foreach (XmlAttribute attribute in element.Attributes)
            {
                output.WriteLine("\t\t{0} = {1}", attribute.Name, attribute.Value);
            }

            this.childIndex = 0;
            base.ProcessElement(output, element);
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
            foreach (XmlAttribute attribute in element.Attributes)
            {
                output.WriteLine("\t\t{0}{1} = {2}", attribute.Name, this.childIndex, attribute.Value);
            }

            this.childIndex++;
            return null;
        }
    }
}
