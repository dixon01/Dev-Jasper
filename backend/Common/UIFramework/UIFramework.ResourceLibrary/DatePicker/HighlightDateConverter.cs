namespace Luminator.UIFramework.ResourceLibrary.DatePicker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Markup;

    /// <summary>
    /// Gets a tool tip for a date passed in. Could also return null
    /// 
    /// Inspired by David Veenemans codeproject article
    /// http://www.codeproject.com/KB/WPF/ExtendingWPFCalendar.aspx
    /// </summary>
        public class HighlightDateConverter : MarkupExtension, IMultiValueConverter
        {
        #region MarkupExtension Overrides
        private static HighlightDateConverter converter = null;
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (converter == null)
            {
                converter = new HighlightDateConverter();
            }
            return converter;
        }
        #endregion

        #region IMultiValueConverter Members

        /// <summary>
        /// Gets a tool tip for a date passed in. Could also return null
        /// </summary>
        /// The 'values' array parameter has the following elements:
        /// 
        /// • values[0] = Binding #1: The date to be looked up. This should be set up as a pathless binding; 
        ///   the Calendar control will provide the date.
        /// 
        /// • values[1] = Binding #2: A binding reference to the Calendar control that is invoking this converter.
        /// </remarks>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Exit if values not set
            if ((values[0] == null) || (values[1] == null)) return null;

            // Get values passed in
            DateTime targetDate = (DateTime)values[0];
            DatePickerEx dp = (DatePickerEx)values[1];

            var range = dp.BlackoutDates.Where(x => x.Start.IsSameDateAs(targetDate));
            if (range.Count() > 0)
            {
                Dictionary<CalendarDateRange, string> blackOutDatesTextLookup =
                    (Dictionary<CalendarDateRange, string>)dp.GetValue(CalendarProps.BlackOutDatesTextLookupProperty);

                return blackOutDatesTextLookup[range.First()];
            }
            else
                return null;

        }

        /// <summary>
        /// Not used.
        /// </summary>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return new object[0];
        }

        #endregion
        }
}
