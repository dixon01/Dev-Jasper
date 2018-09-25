// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitToTileGroupConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitToTileGroupConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using Gorba.Center.Diag.Core.ViewModels.Unit;

    using Telerik.Windows.Controls;

    /// <summary>
    /// The UnitToTileGroupConverter
    /// </summary>
    public class UnitToTileGroupConverter : IMultiValueConverter
    {
        /// <summary>
        /// Gets or sets the group for favorite units
        /// </summary>
        public TileGroup FavoriteGroup { get; set; }

        /// <summary>
        /// Gets or sets the group for connected units
        /// </summary>
        public TileGroup ConnectedGroup { get; set; }

        /// <summary>
        /// Gets or sets the group for disconnected units
        /// </summary>
        public TileGroup DisconnectedGroup { get; set; }

        /// <summary>
        /// determines which group to put the unit in
        /// </summary>
        /// <param name="values">the values</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
            {
                return null;
            }

            object result;
            var isFavorite = (bool)values[0];
            var connectionState = (ConnectionState)values[1];

            if (isFavorite)
            {
                result = this.FavoriteGroup;
            }
            else
            {
                result = connectionState == ConnectionState.Disconnected
                             ? this.DisconnectedGroup
                             : this.ConnectedGroup;
            }

            return result;
        }

        /// <summary>
        /// determines unit states from the group
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetTypes">the target types</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            var result = new object[2];
            result[0] = Binding.DoNothing;
            result[1] = Binding.DoNothing;
            return result;
        }
    }
}
