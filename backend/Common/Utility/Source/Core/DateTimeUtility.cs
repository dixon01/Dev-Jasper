// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeUtility.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Provides utility methods for date and time.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core
{
    using System;
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// Provides utility methods for date and time.
    /// </summary>
    public static class DateTimeUtility
    {
        #region Static Fields

        /// <summary>
        /// Origin date for the unix time.
        /// </summary>
        public static readonly DateTime Origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        /// <summary>The epoch.</summary>
        public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #endregion
        /// <summary>
        /// Converts from unix timestamp.
        /// </summary>
        /// <param name="timestamp">The timestamp.</param>
        /// <returns>The date time for the provided value.</returns>
        public static DateTime ConvertFromUnixTimestamp(uint timestamp)
        {
            return DateTimeUtility.Origin.AddSeconds(timestamp);
        }

        /// <summary>
        /// Converts to unix timestamp.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>The unix timestamp which represents the given date.</returns>
        public static uint ConvertToUnixTimestamp(DateTime date)
        {
            var diff = date - DateTimeUtility.Origin;
            return (uint)System.Math.Floor(diff.TotalSeconds);
        }

        #region Public Methods and Operators

        /// <summary>The epoch.</summary>
        /// <param name="dateTime">The date Time.</param>
        /// <returns>The <see cref="DateTime"/>.</returns>
        public static DateTime GetEpoch(this DateTime dateTime)
        {
            return Epoch;
        }

        /// <summary>The is in range.</summary>
        /// <param name="sourceDate">The source date.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool IsInRange(this DateTime sourceDate, DateTime startDate, DateTime endDate = default(DateTime))
        {
            return sourceDate >= startDate && sourceDate <= endDate;
        }

        /// <summary>The Current Date at Midnight.</summary>
        /// <param name="dateTime">The DateTime value.</param>
        /// <returns>The <see cref="DateTime"/>.</returns>
        public static DateTime Midnight(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0);
        }

        /// <summary>The to unix timestamp.</summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public static double ToUnixTimestamp(this DateTime dateTime)
        {
            return (dateTime - new DateTime(1970, 1, 1, dateTime.Hour, dateTime.Minute, dateTime.Second)).TotalSeconds;
        }

        /// <summary>The trim milliseconds.</summary>
        /// <param name="dateTime">The DateTime value.</param>
        /// <returns>The <see cref="DateTime"/>.</returns>
        public static DateTime TrimMilliseconds(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, 0);
        }

        /// <summary>Get a DateTime with the time part of the date time set to a time of 12:00:00:00 AM.</summary>
        /// <returns>The <see cref="DateTime"/>.</returns>
        /// <summary>The from unix time stamp.</summary>
        /// <param name="unixTimeStamp">The unix time stamp.</param>
        /// <returns>The DateTime</returns>
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch            
            var dateTime = Epoch.AddSeconds(unixTimeStamp); // .ToLocalTime();
            return dateTime;
        }

        /// <summary>The unix time to date time.</summary>
        /// <param name="text">The text.</param>
        /// <returns>The <see cref="DateTime"/>.</returns>
        public static DateTime UnixTimeToDateTime(string text)
        {
            double seconds = double.Parse(text, CultureInfo.InvariantCulture);
            return Epoch.AddSeconds(seconds);
        }

        /// <summary>Trim milliseconds from the timespan.</summary>
        /// <param name="timeSpan">The time span.</param>
        /// <returns>The <see cref="TimeSpan"/>.</returns>
        public static TimeSpan TrimMilliseconds(this TimeSpan timeSpan)
        {
            return new TimeSpan(timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, 0);
        }

        /// <summary>Trim seconds from the timespan.</summary>
        /// <param name="timeSpan">The time span.</param>
        /// <returns>The <see cref="TimeSpan"/>.</returns>
        public static TimeSpan TrimSeconds(this TimeSpan timeSpan)
        {
            return new TimeSpan(timeSpan.Hours, timeSpan.Minutes, 0, 0);
        }

        /// <summary>Text if the TimeSpan is midnight. Hours and Minutes are Zero</summary>
        /// <param name="timeSpan">The time span.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool IsMidnight(this TimeSpan timeSpan)
        {
            return timeSpan.Hours == 0 && timeSpan.Minutes == 0;
        }

        public static DateTime ChangeTime(this DateTime dateTime, int hours, int minutes, int seconds, int milliseconds)
        {
            return new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                hours,
                minutes,
                seconds,
                milliseconds,
                dateTime.Kind);
        }

        /// <summary>Test if time of day between a time span range.</summary>
        /// <param name="timeOfDay">The time.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool IsTimeOfDayBetween(TimeSpan timeOfDay, TimeSpan startTime, TimeSpan endTime)
        {
            Debug.WriteLine("IsTimeOfDayBetween() Enter timeOfDay={0}, startTime={1}, endTime={2}", timeOfDay, startTime, endTime);
            startTime = new TimeSpan(startTime.Hours, startTime.Minutes, 0);
            endTime = new TimeSpan(endTime.Hours, endTime.Minutes, 0);
            if (TimeSpan.Equals(startTime, endTime))
            {
                Debug.WriteLine("endTime {0} == startTime{1}", startTime, endTime);
                return true;
            }

            bool isTimeOfDayBetween;
            if (endTime < startTime)
            {
                Debug.WriteLine("True, endTime {0} < startTime {1}", endTime, startTime);
                isTimeOfDayBetween = timeOfDay <= endTime || timeOfDay >= startTime;
                Debug.WriteLineIf(
                    isTimeOfDayBetween,
                    string.Format(
                        "Result={0}, {1} >= endTime {2} && {3} <= startTime {4}",
                        isTimeOfDayBetween,
                        timeOfDay,
                        endTime,
                        timeOfDay,
                        startTime));
            }

            isTimeOfDayBetween = timeOfDay >= startTime && timeOfDay <= endTime;
            Debug.WriteLineIf(
                isTimeOfDayBetween,
                string.Format("Result={0}, {1} >= startTime {2} && {3} <= endTime {4}", isTimeOfDayBetween, timeOfDay, startTime, timeOfDay, endTime));
            return isTimeOfDayBetween;
        }

        #endregion
    }
}