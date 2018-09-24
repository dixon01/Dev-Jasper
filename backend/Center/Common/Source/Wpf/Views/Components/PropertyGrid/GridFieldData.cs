// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GridFieldData.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The GridFieldData.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Views.Components.PropertyGrid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Gorba.Center.Common.Wpf.Framework.DataViewModels;

    /// <summary>
    /// Represents a field in the property grid
    /// </summary>
    public class GridFieldData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridFieldData"/> class.
        /// </summary>
        /// <param name="source">
        /// The source
        /// </param>
        /// <param name="property">
        /// The property info
        /// </param>
        /// <param name="groupName">
        /// The group name
        /// </param>
        /// <param name="header">
        /// The optional header.
        /// </param>
        /// <param name="tooltip">
        /// The tooltip. If null the header will be used as tooltip.
        /// </param>
        /// <param name="orderIndex">
        /// The property order Index.
        /// </param>
        /// <param name="groupOrderIndex">
        /// The group Order Index.
        /// </param>
        /// <param name="multiSelectFields">
        /// The multi selection fields.
        /// </param>
        /// <param name="formatstring">
        /// Format of the string
        /// </param>
        public GridFieldData(
            object source,
            PropertyInfo property,
            string groupName,
            string header,
            string tooltip,
            int orderIndex,
            int groupOrderIndex,
            IEnumerable<string> multiSelectFields,
            string formatstring = null)
        {
            this.FieldName = property.Name;
            this.GroupName = groupName;

            if (!string.IsNullOrEmpty(header))
            {
                this.Header = header;
            }
            else
            {
                this.Header = property.Name;
            }

            if (!string.IsNullOrEmpty(formatstring))
            {
                this.Formatstring = formatstring;
            }
            else
            {
                this.Formatstring = string.Empty;
            }

            if (!string.IsNullOrEmpty(tooltip))
            {
                this.Tooltip = tooltip;
            }
            else
            {
                this.Tooltip = this.Header;
            }

            var valueObject = property.GetValue(source, null);
            this.Data = valueObject as IDataValue;
            if (this.Data == null)
            {
                this.Reference = valueObject;
            }

            this.OrderIndex = orderIndex;
            this.GroupOrderIndex = groupOrderIndex;

            if (multiSelectFields != null)
            {
                if (multiSelectFields.Any(f => f.Equals(this.FieldName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    this.IsMultiSelect = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the format string.
        /// </summary>
        public string Formatstring { get; set; }

        /// <summary>
        /// Gets or sets the data
        /// </summary>
        public IDataValue Data { get; set; }

        /// <summary>
        /// Gets or sets the Reference
        /// </summary>
        public object Reference { get; set; }

        /// <summary>
        /// Gets or sets the fieldname
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the group name
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets the header of a grid field.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets the tooltip.
        /// </summary>
        public string Tooltip { get; set; }

        /// <summary>
        /// Gets or sets the order index of the property.
        /// </summary>
        public int OrderIndex { get; set; }

        /// <summary>
        /// Gets or sets the group order index.
        /// </summary>
        public int GroupOrderIndex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the given object is multi select.
        /// </summary>
        public bool IsMultiSelect { get; set; }

        /// <summary>
        /// Reflects the value of the property defined by FieldName from the given object.
        /// </summary>
        /// <typeparam name="T"> the type of the field.
        /// </typeparam>
        /// <param name="element">
        /// the object
        /// </param>
        /// <returns>
        /// the value of the property defined by FieldName
        /// </returns>
        public object GetValueForProperty<T>(T element)
        {
            return element.GetType().GetProperty(this.FieldName).GetValue(element, new object[0]);
        }
    }
}