// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CultureInfoConverter.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CultureInfoConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using NLog;

    /// <summary>
    /// Converts culture identifiers in their display string.
    /// </summary>
    public class CultureInfoConverter : IValueConverter
    {
        private const string DefaultEmptyReplacement = "-";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="CultureInfoConverter"/> class.
        /// </summary>
        public CultureInfoConverter()
        {
            this.EmptyReplacement = DefaultEmptyReplacement;
        }

        /// <summary>
        /// Gets or sets the empty replacement.
        /// </summary>
        /// <value>
        /// The empty replacement.
        /// </value>
        public string EmptyReplacement { get; set; }

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
            var s = value as string;
            if (string.IsNullOrWhiteSpace(s))
            {
                return this.EmptyReplacement;
            }

            try
            {
                var cultureInfo = CultureInfo.GetCultureInfo(s);
                return cultureInfo.DisplayName;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error converting culture '{0}'", s));
            }

            return this.EmptyReplacement;
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
