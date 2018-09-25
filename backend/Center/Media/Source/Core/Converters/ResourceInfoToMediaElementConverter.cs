// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceInfoToMediaElementConverter.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceInfoToMediaElementConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Common.Update.ServiceModel.Resources;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// Converts a <see cref="ResourceInfoDataViewModel"/> object into its image.
    /// </summary>
    public class ResourceInfoToMediaElementConverter : IValueConverter
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
            var resourceInfo = value as ResourceInfoDataViewModel;
            if (resourceInfo == null)
            {
                Logger.Debug("This converter only accepts values of type 'ResourceInfoDataViewModel'");
                return null;
            }

            var controller = GetApplicationController();
            var isThumbnail = parameter is bool && (bool)parameter;
            switch (resourceInfo.Type)
            {
                case ResourceType.Image:
                case ResourceType.Video:
                case ResourceType.Symbol:
                    return Convert(isThumbnail, controller, resourceInfo);

                case ResourceType.Audio:
                    return GetAudioPlaceholder();

                case ResourceType.Font:
                    return GetFontTumbnail();

                case ResourceType.Csv:
                    return GetCsvPlaceholder();

                default:
                    throw new NotSupportedException("Only images and videos are supported by the converter");
            }
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

        private static object Convert(
            bool isThumbnail, IMediaApplicationController controller, ResourceInfoDataViewModel resourceInfo)
        {
            try
            {
                if (isThumbnail)
                {
                    IResource thumbnailResource;
                    return controller.ShellController.ResourceController.EnsurePreview(
                        resourceInfo,
                        out thumbnailResource)
                               ? GetImage(thumbnailResource)
                               : null;
                }

                var state = GetApplicationState();
                var resource = state.ProjectManager.GetResource(resourceInfo.Hash);
                return GetImage(resource);
            }
            catch (Exception e)
            {
                string resourceInfoMessage;
                if (resourceInfo != null)
                {
                    resourceInfoMessage = string.Format(
                        "Type: {0}, Hash: {1}",
                        resourceInfo.Type,
                        resourceInfo.Hash);
                }
                else
                {
                    resourceInfoMessage = string.Empty;
                }

                var message =
                    string.Format(
                        "Error trying to convert resource (IsThumbnail: {0}, {1}) to a media element.",
                        isThumbnail,
                        resourceInfoMessage);
                Logger.ErrorException(message, e);
                return null;
            }
        }

        private static IMediaApplicationState GetApplicationState()
        {
            return ServiceLocator.Current.GetInstance<IMediaApplicationState>();
        }

        private static object GetAudioPlaceholder()
        {
            const string ImagePath =
                "/Gorba.Center.Media.Core;component/Resources/Images/Icons/audiodata_dark_48x48.png";

            ImageSource audioPlaceholder = null;

            if (!string.IsNullOrEmpty(ImagePath))
            {
                audioPlaceholder = new BitmapImage(new Uri(ImagePath, UriKind.Relative));
            }

            return audioPlaceholder;
        }

        private static object GetFontTumbnail()
        {
            const string ImagePath =
                "/Gorba.Center.Media.Core;component/Resources/Images/Icons/fontplaceholder_48x48.png";

            ImageSource fontPlaceholder = null;

            if (!string.IsNullOrEmpty(ImagePath))
            {
                fontPlaceholder = new BitmapImage(new Uri(ImagePath, UriKind.Relative));
            }

            return fontPlaceholder;
        }

        private static object GetCsvPlaceholder()
        {
            const string ImagePath =
                "/Gorba.Center.Media.Core;component/Resources/Images/Icons/csvplaceholder_48x48.png";

            ImageSource fontPlaceholder = null;

            if (!string.IsNullOrEmpty(ImagePath))
            {
                fontPlaceholder = new BitmapImage(new Uri(ImagePath, UriKind.Relative));
            }

            return fontPlaceholder;
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

        private static IMediaApplicationController GetApplicationController()
        {
            return ServiceLocator.Current.GetInstance<IMediaApplicationController>();
        }
    }
}