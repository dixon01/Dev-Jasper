// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileIconConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileIconConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    using Gorba.Common.Utility.Win32.Wrapper;

    using NLog;

    /// <summary>
    /// The FileIconConverter
    /// </summary>
    public class FileIconConverter : IValueConverter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly Dictionary<string, BitmapSource> IconCache =
            new Dictionary<string, BitmapSource>(StringComparer.InvariantCultureIgnoreCase);

        private static bool InvokeRequired
        {
            get { return Dispatcher.CurrentDispatcher != Application.Current.Dispatcher; }
        }

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
            var stringValue = value as string;
            if (stringValue == null)
            {
                return this.GetSimpleFileIcon();
            }

            try
            {
                BitmapSource result;

                var extension = Path.GetExtension(stringValue);
                if (IconCache.TryGetValue(extension, out result))
                {
                    return result;
                }

                var icon = ShellFileInfo.GetFileIcon(Path.GetExtension(stringValue), false, false);
                if (icon != null)
                {
                    result = CreateBitmapSourceFromBitmap(icon.ToBitmap());
                    IconCache[extension] = result;
                }
            }
            catch (Exception e)
            {
                Logger.Warn("Failed to convert '" + (string)value + "' to it's corresponding file icon.", e);
            }

            return this.GetSimpleFileIcon();
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

        private static BitmapSource CreateBitmapSourceFromBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException("bitmap");
            }

            if (Application.Current.Dispatcher == null)
            {
                return null; // Is it possible?
            }

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    // You need to specify the image format to fill the stream.
                    // I'm assuming it is PNG
                    bitmap.Save(memoryStream, ImageFormat.Png);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    // Make sure to create the bitmap in the UI thread
                    if (InvokeRequired)
                    {
                        return
                               (BitmapSource)Application.Current.Dispatcher.Invoke(
                                   new Func<Stream, BitmapSource>(CreateBitmapSourceFromBitmap),
                                   DispatcherPriority.Normal,
                                   memoryStream);
                    }

                    return CreateBitmapSourceFromBitmap(memoryStream);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static BitmapSource CreateBitmapSourceFromBitmap(Stream stream)
        {
            var bitmapDecoder = BitmapDecoder.Create(
                stream,
                BitmapCreateOptions.PreservePixelFormat,
                BitmapCacheOption.OnLoad);

            // This will disconnect the stream from the image completely...
            var writable = new WriteableBitmap(bitmapDecoder.Frames.Single());
            writable.Freeze();

            return writable;
        }

        private BitmapSource GetSimpleFileIcon()
        {
            var logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource =
                new Uri(
                    "pack://application:,,,/"
                    + "Gorba.Center.Diag.Core;component/Resources/Icons/document_light_16x16.png");
            logo.EndInit();

            return logo;
        }
    }
}
