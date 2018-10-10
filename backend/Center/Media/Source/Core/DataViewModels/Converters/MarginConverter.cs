// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MarginConverter.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MarginConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// The MarginConverter
    /// </summary>
    public class MarginConverter : IMultiValueConverter
    {
        /// <summary>
        /// returns a margin
        /// </summary>
        /// <param name="values">the values</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            object result = null;

            if (values.Length == 3
                && values[0] is int
                && values[1] is int)
            {
                var x = (int)values[0];
                var y = (int)values[1];
                var factor = 1.0;

                if (values[2] is double)
                {
                    factor = (double)values[2] / 100;
                }

                result = new Thickness { Left = x * factor, Top = y * factor, Bottom = 0, Right = 0 };
            }

            if (values.Length == 5
                && values[0] is int
                && values[1] is int
                && values[3] is int
                && values[4] is int)
            {
                var posx = (int)values[0];
                var posy = (int)values[1];

                var diameter = (int)values[3] * 2;
                var ledDotSpace = (int)values[4];
                var dotWidth = diameter + ledDotSpace;

                var x = posx * dotWidth;
                var y = posy * dotWidth;
                x -= ledDotSpace / 2;
                y -= ledDotSpace / 2;

                var factor = 1.0;

                if (values[2] is double)
                {
                    factor = (double)values[2] / 100;
                }

                result = new Thickness
                {
                    Left   = x * factor,
                    Top    = y * factor,
                    Bottom = 0,
                    Right  = 0
                };
            }

            return result;
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
