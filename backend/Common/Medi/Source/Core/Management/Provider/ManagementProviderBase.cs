// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagementProviderBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Simple base implementation of .
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management.Provider
{
    using System.Collections.Generic;

    /// <summary>
    /// Simple base implementation of <see cref="IManagementProvider"/>.
    /// </summary>
    public abstract class ManagementProviderBase : IManagementProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementProviderBase"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="parent">
        /// The parent.
        /// </param>
        protected ManagementProviderBase(string name, IManagementProvider parent)
        {
            this.Name = name;
            this.Parent = parent;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        public IManagementProvider Parent { get; private set; }

        /// <summary>
        /// Gets the factory that created this node.
        /// </summary>
        public virtual IManagementProviderFactory Factory
        {
            get
            {
                return this.Parent.Factory;
            }
        }

        /// <summary>
        /// Gets all children. This implementation returns an
        /// empty enumerable.
        /// </summary>
        public virtual IEnumerable<IManagementProvider> Children
        {
            get
            {
                yield break;
            }
        }

        /// <summary>
        /// Get a child by its name.
        /// This implementation searches through all children
        /// returned by <see cref="Children"/> to see if one
        /// matches the given name.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// the child if found, otherwise null.
        /// </returns>
        public virtual IManagementProvider GetChild(string name)
        {
            foreach (var child in this.Children)
            {
                if (child.Name.Equals(name))
                {
                    return child;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a descendant for a given path.
        /// </summary>
        /// <param name="path">
        /// The path as an array of path elements.
        /// </param>
        /// <returns>
        /// The <see cref="IManagementProvider"/> or null if the descendant is not found.
        /// </returns>
        public virtual IManagementProvider GetDescendant(params string[] path)
        {
            IManagementProvider provider = this;
            foreach (var pathElement in path)
            {
                provider = provider.GetChild(pathElement);
                if (provider == null)
                {
                    return null;
                }
            }

            return provider;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
            var parent = this.Parent as IModifiableManagementProvider;
            if (parent != null)
            {
                parent.RemoveChild(this);
            }
        }
    }
}