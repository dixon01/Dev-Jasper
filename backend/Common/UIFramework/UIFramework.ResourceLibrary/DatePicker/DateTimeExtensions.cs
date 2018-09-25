namespace Luminator.UIFramework.ResourceLibrary.DatePicker
{
    using System;

    /// <summary>
    /// Taken from  David Veenemans codeproject article
    /// http://www.codeproject.com/KB/WPF/ExtendingWPFCalendar.aspx
    /// </summary>
    public static class DateTimeExtensions
    {
        #region DateTime Extensions Methods
        /// <summary>
        /// Determines whether a subject date is the same as a date passed in.
        /// </summary>
        /// <param name="subjectDate"> The subject date.</param>
        /// <param name="dateToCompare">The date passed in.</param>
        /// <returns>True if the two DateTime objects represent the same date, even if their time components differ; false otherwise.</returns>
        public static bool IsSameDateAs(this DateTime subjectDate, DateTime dateToCompare)
        {
            var dayIsSame = subjectDate.Day == dateToCompare.Day;
            var monthIsSame = subjectDate.Month == dateToCompare.Month;
            var yearIsSame = subjectDate.Year == dateToCompare.Year;
            return dayIsSame && monthIsSame && yearIsSame;
        }

        /// <summary>
        /// Determines whether a subject date is in the same month as a date passed in.
        /// </summary>
        /// <param name="subjectDate"> The subject date.</param>
        /// <param name="dateToCompare">The date passed in.</param>
        /// <returns>True if the two DateTime objects are in the same month; false otherwise.</returns>
        public static bool IsSameMonthAs(this DateTime subjectDate, DateTime dateToCompare)
        {
            var monthIsSame = subjectDate.Month == dateToCompare.Month;
            var yearIsSame = subjectDate.Year == dateToCompare.Year;
            return monthIsSame && yearIsSame;
        }
        #endregion
    }
}
