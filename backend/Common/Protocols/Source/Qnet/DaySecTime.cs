// -----------------------------------------------------------------------
// <copyright file="DaySecTime.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Helper class to calculate the number of secondes from midnight
    /// </summary>
    public class DaySecTime
    {
        public static uint DaySecFromDateTime(DateTime dateTime)
        {
            TimeSpan span = dateTime.TimeOfDay;
            return (uint)span.TotalSeconds;
        }
    }
}
