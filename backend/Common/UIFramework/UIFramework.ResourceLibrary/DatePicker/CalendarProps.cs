namespace Luminator.UIFramework.ResourceLibrary.DatePicker
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Provides helper properties for working with Datepicker/Calendar controls
    /// </summary>
    public static class CalendarProps
    {
        #region BlackOutDatesTextLookup

        /// <summary>
        /// BlackOutDatesTextLookup : Stores dictionary to allow lookup of
        /// Calendar.BlackoutDates to reason for blackout dates string.
        /// </summary>
        public static readonly DependencyProperty BlackOutDatesTextLookupProperty =
            DependencyProperty.RegisterAttached("BlackOutDatesTextLookup", 
                typeof(Dictionary<CalendarDateRange, string>), typeof(CalendarProps),
                    new FrameworkPropertyMetadata(new Dictionary<CalendarDateRange, string>()));


        public static Dictionary<CalendarDateRange, string> GetBlackOutDatesTextLookup(DependencyObject d)
        {
            return (Dictionary<CalendarDateRange, string>)d.GetValue(BlackOutDatesTextLookupProperty);
        }


        public static void SetBlackOutDatesTextLookup(DependencyObject d, Dictionary<CalendarDateRange, string> value)
        {
            d.SetValue(BlackOutDatesTextLookupProperty, value);
        }

        #endregion
    }
}
