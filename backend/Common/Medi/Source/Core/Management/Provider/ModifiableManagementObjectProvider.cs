// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModifiableManagementObjectProvider.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   An <see cref="IModifiableManagementProvider" /> implementation
//   that allows to add and remove children and properties.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management.Provider
{
    using System.Collections.Generic;

    /// <summary>
    /// An <see cref="IModifiableManagementProvider"/> implementation
    /// that allows to add and remove children and properties.
    /// </summary>
    public class ModifiableManagementObjectProvider : ModifiableManagementProvider, IManagementObjectProvider
    {
        private readonly Dictionary<string, ManagementProperty> properties = new Dictionary<string, ManagementProperty>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifiableManagementObjectProvider"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public ModifiableManagementObjectProvider(string name, IManagementProvider parent)
            : base(name, parent)
        {
        }

        /// <summary>
        /// Gets all <see cref="ManagementProperty"/> objects for this node.
        /// </summary>
        public virtual IEnumerable<ManagementProperty> Properties
        {
            get
            {
                return this.properties.Values;
            }
        }

        /// <summary>
        /// Get a property by its name.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property to be found.
        /// </param>
        /// <returns>
        /// the property if found, otherwise null.
        /// </returns>
        public virtual ManagementProperty GetProperty(string propertyName)
        {
            ManagementProperty property;
            this.properties.TryGetValue(propertyName, out property);
            return property;
        }

        /// <summary>
        /// Adds a property to this node.
        /// </summary>
        /// <param name="property">
        /// The property.
        /// </param>
        public virtual void AddProperty(ManagementProperty property)
        {
            this.properties.Add(property.Name, property);
        }

        /// <summary>
        /// Clears all properties and children from this node.
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            this.properties.Clear();
        }
    }
}