// --------------------------------------------------------------------------------------------------------------------
// <copyright file="State.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the State type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Tools.CompactFrameworkConversion.ConverterApp.XmlSolution
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml;

    using NLog;

    /// <summary>
    /// Base class for all XML solution state machine states.
    /// </summary>
    internal abstract class State
    {
        /// <summary>
        /// The NLog logger.
        /// </summary>
        protected readonly Logger Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="State"/> class.
        /// </summary>
        protected State()
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName);
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
        public virtual State ProcessLine(string line, ref XmlNode outputParent)
        {
            this.Logger.Warn("Unused line in {0}: {1}", this.GetType().Name, line);
            return this;
        }

        /// <summary>
        /// Recursively processes a node of the XML solution structure
        /// and writes in the Visual Studio solution file format to the given stream.
        /// </summary>
        /// <param name="output">
        /// The output stream to write to.
        /// </param>
        /// <param name="node">
        /// The node to process.
        /// </param>
        public virtual void ProcessNode(StreamWriter output, XmlNode node)
        {
            var element = node as XmlElement;
            if (element != null)
            {
                this.ProcessElement(output, element);
            }
        }

        /// <summary>
        /// Recursively processes an element of the XML solution structure
        /// and writes in the Visual Studio solution file format to the given stream.
        /// This is a convenience method directly called by <see cref="ProcessNode"/>.
        /// Overriding methods should always call the base class implementation.
        /// </summary>
        /// <param name="output">
        /// The output stream to write to.
        /// </param>
        /// <param name="element">
        /// The element.
        /// </param>
        protected virtual void ProcessElement(StreamWriter output, XmlElement element)
        {
            foreach (var childElement in element.ChildNodes.OfType<XmlElement>())
            {
                var state = this.EnterChild(output, childElement);
                if (state != null)
                {
                    state.ProcessElement(output, childElement);
                }
            }
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
        protected virtual State EnterChild(StreamWriter output, XmlElement element)
        {
            throw new NotSupportedException(this.GetType().Name + " didn't expect child " + element.Name);
        }

        /// <summary>
        /// Appends an empty XML element to the given node.
        /// </summary>
        /// <param name="parent">
        /// The parent node.
        /// </param>
        /// <param name="name">
        /// The name of the element.
        /// </param>
        /// <returns>
        /// The new <see cref="XmlElement"/>.
        /// </returns>
        protected XmlElement AppendElement(XmlNode parent, string name)
        {
            var doc = parent.OwnerDocument ?? (parent as XmlDocument);
            if (doc == null)
            {
                throw new NotSupportedException("Can't create an element if owner document is not defined");
            }

            var element = doc.CreateElement(name);
            parent.AppendChild(element);
            return element;
        }
    }
}