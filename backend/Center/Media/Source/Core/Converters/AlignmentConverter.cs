// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AlignmentConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AlignmentConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using Gorba.Common.Configuration.Infomedia.Layout;

    using NLog.Targets;

    /// <summary>
    /// The AlignmentConverter
    /// </summary>
    public class AlignmentConverter : IValueConverter
    {
        /// <summary>
        /// not implemented
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = value;

            if (targetType == typeof(System.Windows.HorizontalAlignment))
            {
                switch ((HorizontalAlignment)value)
                {
                    case HorizontalAlignment.Left:
                        result = System.Windows.HorizontalAlignment.Left;
                        break;
                    case HorizontalAlignment.Center:
                        result = System.Windows.HorizontalAlignment.Center;
                        break;
                    case HorizontalAlignment.Right:
                        result = System.Windows.HorizontalAlignment.Right;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("value");
                }
            }
            else if (targetType == typeof(System.Windows.VerticalAlignment))
            {
                switch ((VerticalAlignment)value)
                {
                    case VerticalAlignment.Top:
                        result = System.Windows.VerticalAlignment.Top;
                        break;
                    case VerticalAlignment.Middle:
                        result = System.Windows.VerticalAlignment.Center;
                        break;
                    case VerticalAlignment.Baseline:
                        result = System.Windows.VerticalAlignment.Top;
                        break;
                    case VerticalAlignment.Bottom:
                        result = System.Windows.VerticalAlignment.Bottom;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("value");
                }
            }

            return result;
        }

        /// <summary>
        /// not implemented
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = value;

            if (targetType == typeof(HorizontalAlignment))
            {
                switch ((System.Windows.HorizontalAlignment)value)
                {
                    case System.Windows.HorizontalAlignment.Left:
                        result = HorizontalAlignment.Left;
                        break;
                    case System.Windows.HorizontalAlignment.Center:
                        result = HorizontalAlignment.Center;
                        break;
                    case System.Windows.HorizontalAlignment.Right:
                        result = HorizontalAlignment.Right;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("value");
                }
            }
            else if (targetType == typeof(VerticalAlignment))
            {
                switch ((System.Windows.VerticalAlignment)value)
                {
                    case System.Windows.VerticalAlignment.Top:
                        result = VerticalAlignment.Top;
                        break;
                    case System.Windows.VerticalAlignment.Center:
                        result = VerticalAlignment.Middle;
                        break;
                    case System.Windows.VerticalAlignment.Bottom:
                        result = VerticalAlignment.Bottom;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("value");
                }
            }

            return result;
        }
    }
}
