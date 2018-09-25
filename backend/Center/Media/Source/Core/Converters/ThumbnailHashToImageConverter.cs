// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThumbnailHashToImageConverter.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ThumbnailHashToImageConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;

    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.Properties;
    using Gorba.Common.Update.ServiceModel.Resources;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// Converts a thumbnail hash of a resource to an image.
    /// </summary>
    public class ThumbnailHashToImageConverter : IValueConverter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
            var hash = value as string;
            if (hash == null)
            {
                Logger.Debug("This converter only accepts values of type string");
                return null;
            }

            if (hash == Settings.Default.UriVideoThumbnailHash)
            {
                return null;
            }

            IResource thumbnailResource;
            var controller = GetApplicationController();
            return controller.ShellController.ResourceController.GetVideoThumbnail(hash, out thumbnailResource)
                       ? GetImage(thumbnailResource)
                       : null;
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

        private static IMediaApplicationController GetApplicationController()
        {
            return ServiceLocator.Current.GetInstance<IMediaApplicationController>();
        }

        private static BitmapImage GetImage(IResource imageResource)
        {
            using (var fs = imageResource.OpenRead())
            {
                return GetBitmapImage(fs);
            }
        }

        private static BitmapImage GetBitmapImage(Stream fs)
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
}
