// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphicalElementType2ImageConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GraphicalElementType2ImageConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using Gorba.Center.Media.Core.DataViewModels.Layout;

    /// <summary>
    /// The GraphicalElementType2ImageConverter
    /// </summary>
    public class GraphicalElementType2ImageConverter : IValueConverter
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
            string imagePath = null;

            if (value is StaticTextElementDataViewModel)
            {
                imagePath = "/Gorba.Center.Media.Core;component/Resources/Images/Icons/text_grey_12x12.png";
            }
            else if (value is DynamicTextElementDataViewModel)
            {
                imagePath = "/Gorba.Center.Media.Core;component/Resources/Images/Icons/dynamictext_dark_12x12.png";
            }
            else if (value is VideoElementDataViewModel)
            {
                imagePath = "/Gorba.Center.Media.Core;component/Resources/Images/Icons/video_grey_12x12.png";
            }
            else if (value is ImageElementDataViewModel)
            {
                imagePath = "/Gorba.Center.Media.Core;component/Resources/Images/Icons/image_grey_12x12.png";
            }
            else if (value is FrameElementDataViewModel)
            {
                imagePath = "/Gorba.Center.Media.Core;component/Resources/Images/Icons/frame_grey_12x12.png";
            }
            else if (value is AnalogClockElementDataViewModel)
            {
                imagePath = "/Gorba.Center.Media.Core;component/Resources/Images/Icons/analogclock_grey_12x12.png";
            }
            else if (value is ImageListElementDataViewModel)
            {
                imagePath = "/Gorba.Center.Media.Core;component/Resources/Images/Icons/imagelist_grey_12x12.png";
            }

            ImageSource result = null;

            if (!string.IsNullOrEmpty(imagePath))
            {
                result = new BitmapImage(new Uri(imagePath, UriKind.Relative));
            }

            return result;
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
