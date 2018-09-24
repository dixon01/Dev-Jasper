// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserVisiblePropertyAttribute.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The UserVisiblePropertyAttribute.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Views.Components.PropertyGrid
{
    using System;

    /// <summary>
    /// The UserVisibleProperty.
    /// </summary>
    public class UserVisiblePropertyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserVisiblePropertyAttribute"/> class.
        /// </summary>
        /// <param name="groupName">
        /// The group name
        /// </param>
        public UserVisiblePropertyAttribute(string groupName)
        {
            this.GroupName = groupName;
        }

        /// <summary>
        /// Gets or sets the group name
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets the group order index.
        /// </summary>
        public int GroupOrderIndex { get; set; }

        /// <summary>
        /// Gets or sets the field name.
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// Gets or sets the order index used to order the properties.
        /// </summary>
        public int OrderIndex { get; set; }

        /// <summary>
        /// Gets or sets the format string.
        /// This value is used, to display it correctly on the view.
        /// </summary>
        public string FormatString { get; set; }
    }
}