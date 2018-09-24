// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityMessageTypeBrushConverter.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ActivityMessageTypeBrushConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    using Gorba.Center.Common.Wpf.Framework.ViewModels;

    /// <summary>
    /// Converts the <see cref="ActivityMessage.ActivityMessageType"/> to a <see cref="Color"/>.
    /// </summary>
    public class ActivityMessageTypeBrushConverter : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityMessageTypeBrushConverter"/> class.
        /// </summary>
        public ActivityMessageTypeBrushConverter()
        {
            this.DefaultBrush = new SolidColorBrush(Colors.Black);
            this.InfoBrush = new SolidColorBrush(Colors.Black);
            this.WarningBrush = new SolidColorBrush(Colors.Coral);
            this.ErrorBrush = new SolidColorBrush(Colors.Red);
        }

        /// <summary>
        /// Gets or sets the Warning color.
        /// </summary>
        /// <value>
        /// The Warning color.
        /// </value>
        public Brush WarningBrush { get; set; }

        /// <summary>
        /// Gets or sets the Info color.
        /// </summary>
        /// <value>
        /// The Info color.
        /// </value>
        public Brush InfoBrush { get; set; }

        /// <summary>
        /// Gets or sets the default color.
        /// </summary>
        /// <value>
        /// The default color.
        /// </value>
        public Brush DefaultBrush { get; set; }

        /// <summary>
        /// Gets or sets the Error Color.
        /// </summary>
        /// <value>
        /// The Error color.
        /// </value>
        public Brush ErrorBrush { get; set; }

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
            if (!(value is ActivityMessage.ActivityMessageType))
            {
                throw new ArgumentOutOfRangeException(
                    "value", value, "This converter must be used with objects of type ActivityMessageType only");
            }

            var activityMessageType = (ActivityMessage.ActivityMessageType)value;
            switch (activityMessageType)
            {
                case ActivityMessage.ActivityMessageType.None:
                    return this.DefaultBrush;
                case ActivityMessage.ActivityMessageType.Info:
                    return this.InfoBrush;
                case ActivityMessage.ActivityMessageType.Warning:
                    return this.WarningBrush;
                case ActivityMessage.ActivityMessageType.Error:
                    return this.ErrorBrush;
                default:
                    throw new ArgumentOutOfRangeException();
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
    }
}
