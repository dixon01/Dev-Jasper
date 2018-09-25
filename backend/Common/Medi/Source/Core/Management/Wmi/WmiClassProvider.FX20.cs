// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WmiClassProvider.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WmiClassProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management.Wmi
{
    using System;
    using System.Collections.Generic;
    using System.Management;

    using Gorba.Common.Medi.Core.Management.Provider;

    /// <summary>
    /// The provider for a single management object.
    /// </summary>
    internal partial class WmiClassProvider : ManagementTableProviderBase
    {
        private readonly ManagementClass clazz;

        /// <summary>
        /// Initializes a new instance of the <see cref="WmiClassProvider"/> class.
        /// </summary>
        /// <param name="clazz">
        /// The class to be represented.
        /// </param>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public WmiClassProvider(ManagementClass clazz, IManagementProvider parent)
            : base(clazz["__CLASS"].ToString(), parent)
        {
            this.clazz = clazz;
        }

        /// <summary>
        /// Gets all rows of this table.
        /// Each row is a list of properties where the property name is the name of the column.
        /// </summary>
        public override IEnumerable<List<ManagementProperty>> Rows
        {
            get
            {
                // TODO: set options
                var options = new EnumerationOptions();

                foreach (var m in this.clazz.GetInstances(options))
                {
                    var properties = new List<ManagementProperty>(m.Properties.Count);
                    foreach (var property in m.Properties)
                    {
                        properties.Add(CreateProperty(property));
                    }

                    properties.Sort((a, b) => StringComparer.InvariantCultureIgnoreCase.Compare(a.Name, b.Name));
                    yield return properties;
                }
            }
        }

        private static ManagementProperty<string> CreateProperty(PropertyData property)
        {
            return new ManagementProperty<string>(property.Name, Convert.ToString(property.Value), true);
        }
    }
}