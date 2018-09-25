// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModifiableManagementProvider.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ModifiableManagementProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management.Provider
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An <see cref="IModifiableManagementProvider"/> implementation
    /// that allows to add and remove children.
    /// </summary>
    public class ModifiableManagementProvider : IModifiableManagementProvider
    {
        private readonly Dictionary<string, IManagementProvider> children =
            new Dictionary<string, IManagementProvider>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifiableManagementProvider"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public ModifiableManagementProvider(string name, IManagementProvider parent)
        {
            this.Name = name;
            this.Parent = parent;
        }

        /// <summary>
        /// Gets the name of this node.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the parent of this node.
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
        /// Gets all children.
        /// </summary>
        public virtual IEnumerable<IManagementProvider> Children
        {
            get
            {
                return this.children.Values;
            }
        }

        /// <summary>
        /// Get a child by its name.
        /// </summary>
        /// <param name="childName">
        /// The name of the child to be found.
        /// </param>
        /// <returns>
        /// the child if found, otherwise null.
        /// </returns>
        public virtual IManagementProvider GetChild(string childName)
        {
            IManagementProvider child;
            this.children.TryGetValue(childName, out child);
            return child;
        }

        /// <summary>
        /// Add a child to this node.
        /// </summary>
        /// <param name="child">
        /// The child.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If the child has a different parent than this.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// A provider with the same name already exists in this provider.
        /// </exception>
        public virtual void AddChild(IManagementProvider child)
        {
            if (child.Parent != this)
            {
                throw new ArgumentException("Can't add a child that belongs to another parent");
            }

            this.children.Add(child.Name, child);
        }

        /// <summary>
        /// Removes a child from this node.
        /// </summary>
        /// <param name="child">
        /// The child.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If the child doesn't belong to this provider.
        /// </exception>
        public void RemoveChild(IManagementProvider child)
        {
            if (child.Parent != this)
            {
                throw new ArgumentException("Can't remove a child that belongs to another parent");
            }

            this.children.Remove(child.Name);
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
            return this.GetDescendant(false, path);
        }

        /// <summary>
        /// Gets a descendant for a given path.
        /// If the descendant or any intermediate node doesn't exist,
        /// and <see cref="createIfNecessary"/> is set to true, a new
        /// <see cref="IModifiableManagementProvider"/> is created.
        /// </summary>
        /// <param name="createIfNecessary">
        /// If this is set to true, a new provider will be created if necessary.
        /// </param>
        /// <param name="path">
        /// The path as an array of path elements.
        /// </param>
        /// <returns>
        /// The <see cref="IManagementProvider"/> if necessary it will be created.
        /// </returns>
        public virtual IManagementProvider GetDescendant(bool createIfNecessary, params string[] path)
        {
            IManagementProvider provider = this;
            foreach (var pathElement in path)
            {
                var parent = provider as IModifiableManagementProvider;
                provider = provider.GetChild(pathElement);
                if (provider != null)
                {
                    continue;
                }

                if (!createIfNecessary || parent == null)
                {
                    return null;
                }

                provider = new ModifiableManagementProvider(pathElement, parent);
                parent.AddChild(provider);
            }

            return provider;
        }

        /// <summary>
        /// Clears this provider.
        /// </summary>
        public virtual void Clear()
        {
            this.children.Clear();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
            var dispose = new List<IManagementProvider>(this.children.Values);
            foreach (var provider in dispose)
            {
                var disposable = provider as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            var parent = this.Parent as IModifiableManagementProvider;
            if (parent != null)
            {
                parent.RemoveChild(this);
            }
        }
    }
}
