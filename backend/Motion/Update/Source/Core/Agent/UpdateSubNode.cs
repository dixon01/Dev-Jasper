// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateSubNode.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Agent
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The sub folder or file to be updated.
    /// </summary>
    [XmlInclude(typeof(UpdateFile))]
    [XmlInclude(typeof(UpdateFolder))]
    public abstract class UpdateSubNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateSubNode"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent folder.
        /// </param>
        protected UpdateSubNode(UpdateFolder parent)
        {
            this.Parent = parent;
        }

        /// <summary>
        /// Gets the parent folder of this node.
        /// </summary>
        [XmlIgnore]
        public UpdateFolder Parent { get; private set; }

        /// <summary>
        /// Gets or sets the name of the folder or file to be updated.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of action to be performed for the folder or file.
        /// </summary>
        [XmlAttribute]
        public ActionType Action { get; set; }

        /// <summary>
        /// Tries to find the parent that satisfies the <see cref="predicate"/>.
        /// </summary>
        /// <param name="predicate">
        /// The predicate to test every folder up in the hierarchy with.
        /// </param>
        /// <returns>
        /// The <see cref="UpdateFolder"/> that matches the predicate or null if it wasn't found.
        /// </returns>
        public UpdateFolder FindParent(Predicate<UpdateFolder> predicate)
        {
            var parent = this.Parent;
            while (parent != null && !predicate(parent))
            {
                parent = parent.Parent;
            }

            return parent;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
