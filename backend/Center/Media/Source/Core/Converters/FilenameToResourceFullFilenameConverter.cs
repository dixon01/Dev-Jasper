// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilenameToResourceFullFilenameConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FilenameToResourceFullFilenameConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows.Data;

    using Gorba.Center.Media.Core.Models;
    using Gorba.Common.Update.ServiceModel;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Converts the simple filename to the full filename getting the first match from the state
    /// CurrentProject.Resources.
    /// </summary>
    public class FilenameToResourceFullFilenameConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var filename = value as string;
            if (string.IsNullOrEmpty(filename))
            {
                return null;
            }

            var state = ServiceLocator.Current.GetInstance<IMediaApplicationState>();
            try
            {
                var resource =
                    state.CurrentProject.Resources.FirstOrDefault(
                    model => Path.GetFileName(model.Filename) == Path.GetFileName(filename));
                if (resource == null)
                {
                    return null;
                }

                return resource.Filename;
            }
            catch (UpdateException)
            {
            }

            return null;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
