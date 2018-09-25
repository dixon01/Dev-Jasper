// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HelpLinkConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The HelpLinkConverter.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// The HelpLinkConverter
    /// </summary>
    public class HelpLinkConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets the Help text prefix
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// returns a localized help text by constructing the media string name
        /// </summary>
        /// <param name="value">string to identify the desired help text</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var helpText = string.Empty;

            if (value != null)
            {
                var selectedItem = value.ToString();
                helpText = this.Prefix + selectedItem;
            }

            return helpText;
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