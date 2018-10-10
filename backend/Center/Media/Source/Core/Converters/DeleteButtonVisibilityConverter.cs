// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteButtonVisibilityConverter.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;

    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Project;

    using NLog;

    /// <summary>
    /// The delete button visibility converter.
    /// </summary>
    public class DeleteButtonVisibilityConverter : IMultiValueConverter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Converts the values into a <see cref="Visibility"/>.
        /// </summary>
        /// <param name="values">
        /// values[0], values[1]: MediaProjectDataViewModel, values[2]: boolean
        /// </param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>
        /// <see cref="Visibility.Visible"/> if values[0] and [1] are not equal and value[2] is <c>true</c>;
        /// <see cref="Visibility.Collapsed"/> otherwise.
        /// </returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
            {
                return Visibility.Collapsed;
            }

            var list = values.ToList();
            if (list.Count < 3)
            {
                return Visibility.Collapsed;
            }

            try
            {
                var first = values[0] as MediaConfigurationDataViewModel;
                var second = values[1] as MediaConfigurationDataViewModel;
                var isMouseOver = (bool)values[2];
                if (first == null || second == null)
                {
                    return isMouseOver ? Visibility.Visible : Visibility.Collapsed;
                }

                var result = !first.Name.Equals(second.Name, StringComparison.InvariantCultureIgnoreCase)
                    && isMouseOver;
                return result ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception exception)
            {
                Logger.DebugException("Error while casting converter values.", exception);
                return Visibility.Collapsed;
            }
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
