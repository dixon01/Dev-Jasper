// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HourIntervalFormatter.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HourIntervalFormatter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Views.Stages
{
    using System;

    using Telerik.Windows.Controls.TimeBar;

    /// <summary>
    /// Interval formatter that provides 24 hour time values (instead of 12 hour).
    /// </summary>
    public class HourIntervalFormatter : IIntervalFormatterProvider
    {
        /// <summary>
        /// Returns a collection of formatters used to convert <see cref="DateTime"/> objects to specific strings.
        /// </summary>
        /// <param name="interval">
        /// The <see cref="IntervalBase"/> that this formatter provider is associated with.
        /// </param>
        /// <remarks>
        /// The collection is used when <see cref="IntervalBase.CurrentIntervalSpan"/> equals 1
        /// otherwise, the collection returned by <see cref="GetIntervalSpanFormatters"/> is used.
        /// </remarks>
        /// <returns>
        /// A collection of formatters.
        /// </returns>
        public Func<DateTime, string>[] GetFormatters(IntervalBase interval)
        {
            return new Func<DateTime, string>[] { date => date.ToString("HH") };
        }

        /// <summary>
        /// Gets a collection of formatters used to convert <see cref="T:System.DateTime"/> objects to specific strings.
        /// </summary>
        /// <param name="interval">
        /// The <see cref="IntervalBase"/> that this formatter provider is associated with.
        /// </param>
        /// <remarks>
        /// The collection is used when <see cref="IntervalBase.CurrentIntervalSpan"/> is different than 1;
        /// otherwise, the collection returned by <see cref="GetFormatters"/> is used.
        /// </remarks>
        /// <returns>
        /// A collection of formatters.
        /// </returns>
        public Func<DateTime, string>[] GetIntervalSpanFormatters(IntervalBase interval)
        {
            return new Func<DateTime, string>[] { date => date.ToString("HH") };
        }
    }
}
