// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FontFaceToFontFamilyConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FontFaceToFontFamilyConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows.Data;
    using System.Windows.Media;

    using Gorba.Center.Media.Core.Models;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The FontFaceToFontFamilyConverter
    /// </summary>
    public class FontFaceToFontFamilyConverter : IValueConverter
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
            var fontface = value as string;
            var fontFamily = new FontFamily();
            if (fontface != null)
            {
                var state = ServiceLocator.Current.GetInstance<IMediaApplicationState>();
                var font = state.CurrentProject == null
                               ? null
                               : state.CurrentProject.Resources.FirstOrDefault(r => r.Facename == fontface);
                if (font != null)
                {
                    var tempPath = Path.GetTempPath();
                    var filename = Path.GetFileName(font.Filename);
                    if (filename == null)
                    {
                        return fontFamily;
                    }

                    var tempFontFile = Path.Combine(tempPath, filename);
                    if (!File.Exists(tempFontFile))
                    {
                        var fontResource = state.ProjectManager.GetResource(font.Hash);
                        if (fontResource == null)
                        {
                            return fontFamily;
                        }

                        using (var stream = fontResource.OpenRead())
                        {
                            using (var file = File.Create(tempFontFile))
                            {
                                stream.CopyTo(file);
                            }
                        }
                    }

                    var uri = new Uri(tempPath);
                    var fontFamilyName = string.Format("{0}/#{1}", uri, fontface);
                    return new FontFamily(fontFamilyName);
                }

                return fontface;
            }

            return fontFamily;
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
