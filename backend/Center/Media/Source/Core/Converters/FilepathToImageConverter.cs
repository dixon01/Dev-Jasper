// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilepathToImageConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FilepathToImageConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;

    using Gorba.Center.Media.Core.Models;
    using Gorba.Common.Update.ServiceModel;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// The File path To Image Converter
    /// </summary>
    public class FilepathToImageConverter : IValueConverter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
            try
            {
                using (var fs = new FileStream((string)value, FileMode.Open, FileAccess.Read))
                {
                    var image = new BitmapImage();

                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = fs;
                    image.EndInit();
                    image.Freeze();

                    return image;
                }
            }
            catch (Exception exception)
            {
                Logger.WarnException("Error converting path '" + value + "' to an image object", exception);
            }

            return null;
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

        private string GetFullFilename(object value)
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
                    model => Path.GetFileName(model.Filename) == filename);
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
    }
}
