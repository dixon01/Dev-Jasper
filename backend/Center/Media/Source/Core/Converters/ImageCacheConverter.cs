// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageCacheConverter.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImageCacheConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Net.Cache;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;

    using NLog;

    /// <summary>
    /// Prepares images for UIs to use without being locked.
    /// </summary>
    public class ImageCacheConverter : IValueConverter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Loads the image into the memory and unlocks the file itself.
        /// </summary>
        /// <param name="value">
        /// The path to the image.
        /// </param>
        /// <param name="targetType">
        /// The target type. (not used)
        /// </param>
        /// <param name="parameter">
        /// The parameter. (not used)
        /// </param>
        /// <param name="culture">
        /// The culture. (not used)
        /// </param>
        /// <returns>
        /// The converted image.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var path = (string)value;

            // load the image, specify CacheOption so the file is not locked
            var image = new BitmapImage();
            try
            {
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.None;
                image.UriCachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                image.UriSource = new Uri(path);
                image.EndInit();
            }
            catch (Exception e)
            {
                Logger.DebugException("Loading the image failed due to:", e);
                return null;
            }

            return image;
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
