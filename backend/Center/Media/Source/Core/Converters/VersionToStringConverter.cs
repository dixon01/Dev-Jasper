// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionToStringConverter.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VersionToStringConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;

    /// <summary>
    /// The VersionToStringConverter
    /// </summary>
    public class VersionToStringConverter : IValueConverter
    {
        /// <summary>
        /// Returns the formatted DocumentVersionReadableModel version string.
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var version = value as DocumentVersionReadableModel;

            if (version == null)
            {
                return null;
            }

            return string.Format("{0}.{1}", version.Major, version.Minor);
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
            throw new NotImplementedException();
        }
    }
}
