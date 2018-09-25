// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceFilenameToImageConverter.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceFilenameToImageConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;

    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Common.Formats.AlphaNT.Bitmaps;
    using Gorba.Common.Formats.AlphaNT.Common;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// Converts a (simple) filename of a resource to an image.
    /// </summary>
    public class ResourceFilenameToImageConverter : IValueConverter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly Dictionary<int, Brush> Brushes = new Dictionary<int, Brush>();
        private static readonly Dictionary<int, Pen> Pens = new Dictionary<int, Pen>();
        private static readonly Brush AmberBrush = new SolidBrush(Color.FromArgb(0xFF, 0xC2, 0x00));
        private static readonly Pen AmberPen = new Pen(AmberBrush);

        /// <summary>
        /// Cache for resource lookup.
        /// Key: resource filename (with extension)
        /// Value: resource hash
        /// </summary>
        private readonly IDictionary<string, string> resourceCache =
            new Dictionary<string, string>();

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
                Logger.Warn("This converter can only be used with objects of type ImageElementDataViewModel.");
                return null;
            }

            string hash;
            if (this.resourceCache.ContainsKey(filename))
            {
                hash = this.resourceCache[filename];
            }
            else
            {
                hash = FindHash(filename);
                if (string.IsNullOrWhiteSpace(hash))
                {
                    return null;
                }

                this.resourceCache.Add(filename, hash);
            }

            return CreateImage(hash, filename);
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

        private static BitmapSource CreateImage(string hash, string filename)
        {
            try
            {
                var resourceManager = ServiceLocator.Current.GetInstance<IResourceManager>();
                var localFilename = resourceManager.GetResourcePath(hash);
                if (filename.EndsWith(".egr", StringComparison.InvariantCultureIgnoreCase))
                {
                    var bitmap = new EgrBitmap(localFilename);
                    return CreateGorbaImage(bitmap);
                }

                if (filename.EndsWith(".egl", StringComparison.InvariantCultureIgnoreCase))
                {
                    var bitmap = new EglBitmap(localFilename);
                    return CreateGorbaImage(bitmap);
                }

                if (filename.EndsWith(".egf", StringComparison.InvariantCultureIgnoreCase))
                {
                    var bitmap = new EgfBitmap(localFilename);
                    return CreateGorbaImage(bitmap);
                }

                var state = ServiceLocator.Current.GetInstance<IMediaApplicationState>();
                var resource = state.ProjectManager.GetResource(hash);
                using (var fs = resource.OpenRead())
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
            catch (Exception e)
            {
                var message = string.Format(
                    "Error while trying to create the image with hash {0} from local system.",
                    hash);
                Logger.DebugException(message, e);
                return null;
            }
        }

        private static void GetBrushAndPen(IColor color, out Brush brush, out Pen pen)
        {
            var key = (color.R << 16) | (color.G << 8) | color.B;
            if (Brushes.TryGetValue(key, out brush))
            {
                pen = Pens[key];
                return;
            }

            brush = new SolidBrush(Color.FromArgb(color.R, color.G, color.B));
            pen = new Pen(brush);
            Brushes.Add(key, brush);
            Pens.Add(key, pen);
        }

        private static BitmapSource CreateGorbaImage(IBitmap bitmap)
        {
            const int SizeFactor = 3;
            var bmp = new Bitmap((bitmap.Width * SizeFactor) + 1, (bitmap.Height * SizeFactor) + 1);
            var hasColor = bitmap is EgfBitmap;
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Black);
                for (int x = 0; x < bitmap.Width; x++)
                {
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        var pixel = bitmap.GetPixel(x, y);
                        if (pixel == Colors.Transparent)
                        {
                            continue;
                        }

                        Brush brush;
                        Pen pen;
                        if (hasColor)
                        {
                            GetBrushAndPen(pixel, out brush, out pen);
                        }
                        else
                        {
                            if (pixel == Colors.Black)
                            {
                                continue;
                            }

                            brush = AmberBrush;
                            pen = AmberPen;
                        }

                        g.FillEllipse(
                            brush, (x * SizeFactor) + 1, (y * SizeFactor) + 1, SizeFactor - 2, SizeFactor - 2);
                        g.DrawEllipse(pen, (x * SizeFactor) + 1, (y * SizeFactor) + 1, SizeFactor - 2, SizeFactor - 2);
                    }
                }
            }

           return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
              bmp.GetHbitmap(),
              IntPtr.Zero,
              Int32Rect.Empty,
              BitmapSizeOptions.FromEmptyOptions());
        }

        private static string FindHash(string value)
        {
            var state = ServiceLocator.Current.GetInstance<IMediaApplicationState>();
            if (state.CurrentProject == null)
            {
                return null;
            }

            var filename = Path.GetFileName(value);
            var resource =
                state.CurrentProject.Resources.FirstOrDefault(model => Path.GetFileName(model.Filename) == filename);
            return resource == null ? null : resource.Hash;
        }
    }
}