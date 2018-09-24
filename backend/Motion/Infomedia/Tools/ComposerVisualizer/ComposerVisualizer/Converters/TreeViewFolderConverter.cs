// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TreeViewFolderConverter.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TreeViewFolderConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.Converters
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels;

    /// <summary>
    /// Converter that organizes several collections into child collections that are put into containers.
    /// </summary>
    public class TreeViewFolderConverter : IMultiValueConverter
    {
        /// <summary>
        /// Merges several collections into child collections that are put into folder containers.
        /// </summary>
        /// <param name="values">The collections to merge.</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">A comma separated string with the names of the sub folders.</param>
        /// <param name="culture">the culture</param>
        /// <returns>A collection with the <paramref name="values"/> as child collections.</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var folder = string.Empty;
            var parameterString = parameter as string;
            if (parameterString != null)
            {
                folder = parameterString;
            }

            var folders = folder.Split(',').Select(f => f.Trim()).ToList();

            while (values.Length > folders.Count)
            {
                folders.Add(string.Empty);
            }

            var items = new List<object>();

            for (var i = 0; i < values.Length; i++)
            {
                var childs = values[i] as IEnumerable ?? new List<object> { values[i] };

                var folderName = folders[i];

                if (folderName != string.Empty)
                {
                    var folderItem = new TreeViewFolderItem { Name = folderName, Items = childs };
                    items.Add(folderItem);
                }
                else
                {
                    items.AddRange(childs.Cast<object>());
                }
            }

            return items;
        }

        /// <summary>
        /// not implemented
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetTypes">the target types</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
