// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PortTreeNodeBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PortTreeNodeBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Tools.ClientGUI
{
    using System;

    /// <summary>
    /// A node of the <see cref="PortsTreeModel"/>.
    /// </summary>
    public abstract class PortTreeNodeBase : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PortTreeNodeBase"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        protected PortTreeNodeBase(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public abstract void Dispose();
    }
}