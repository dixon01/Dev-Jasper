// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DateTimeExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Utils
{
    using System;

    /// <summary>
    /// Defines extension methods for date and time.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Specifies the given source date as UTC.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>The date specified as UTC.</returns>
        public static DateTime SpecifyUtc(this DateTime source)
        {
            var result = DateTime.SpecifyKind(source, DateTimeKind.Utc);

            return result;
        }

        /// <summary>
        /// Specifies the given source date as UTC.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>The date specified as UTC.</returns>
        public static DateTime? SpecifyUtc(this DateTime? source)
        {
            DateTime? result = null;

            if (source.HasValue)
            {
                result = SpecifyUtc(source.Value);
            }

            return result;
        }

        /// <summary>
        /// Specifies the given source date as UTC.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="timezone">
        /// The time zone.
        /// </param>
        /// <returns>
        /// The date specified as UTC.
        /// </returns>
        public static DateTime ConvertToUtc(this DateTime source, TimeZoneInfo timezone)
        {
            var localDt = DateTime.SpecifyKind(source, DateTimeKind.Local);
            var result = TimeZoneInfo.ConvertTimeToUtc(localDt, timezone);

            return result;
        }

        /// <summary>
        /// Specifies the given source date as UTC.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="timezone">
        /// The time zone.
        /// </param>
        /// <returns>
        /// The date specified as UTC.
        /// </returns>
        public static DateTime? ConvertToUtc(this DateTime? source, TimeZoneInfo timezone)
        {
            DateTime? result = null;

            if (source.HasValue)
            {
                result = source.Value.ConvertToUtc(timezone);
            }

            return result;
        }

        /// <summary>
        /// Converts a date to a local time in the given time zone.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="timezone">
        /// The time zone.
        /// </param>
        /// <returns>
        /// The local date converted to the given time zone
        /// </returns>
        public static DateTime ConvertFromUtc(this DateTime source, TimeZoneInfo timezone)
        {
            var result = TimeZoneInfo.ConvertTimeFromUtc(source, timezone);

            return result;
        }

        /// <summary>
        /// Converts a date to a local time in the given time zone.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="timezone">
        /// The time zone.
        /// </param>
        /// <returns>
        /// The local date converted to the given time zone
        /// </returns>
        public static DateTime? ConvertFromUtc(this DateTime? source, TimeZoneInfo timezone)
        {
            DateTime? result = null;

            if (source.HasValue)
            {
                result = source.Value.ConvertFromUtc(timezone);
            }

            return result;
        }

        /// <summary>
        /// Truncates the precision specified with <paramref name="timeSpan"/>
        /// of a <see cref="DateTime"/> object.
        /// </summary>
        /// <param name="source">
        /// The source date.
        /// </param>
        /// <param name="timeSpan">
        /// The time span to remove from the <paramref name="source"/>.
        /// </param>
        /// <returns>
        /// The updated <see cref="DateTime"/>.
        /// </returns>
        public static DateTime Truncate(this DateTime source, TimeSpan timeSpan)
        {
            return source.AddTicks(-(source.Ticks % timeSpan.Ticks));
        }
    }
}
