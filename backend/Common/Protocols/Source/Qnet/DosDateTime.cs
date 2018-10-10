// --------------------------------------------------------------------------------------------------------------------
// <copyright file="dosDateTime.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the dosDateTime type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System;

    /// <summary>
    /// Defines the DosDateTime class that suppplies some helper methods :
    /// - to convert c# datetime to <see cref="DOSTimeStruct"/> and vice versa.
    /// - to convert a qnet timestamp (format hhmmss) into DOSTimeStruct. 
    /// </summary>
    public class DosDateTime
    {
        /// <summary>
        /// Converts a c# <see cref="DateTime"/> to <see cref="DOSTimeStruct"/>
        /// </summary>
        /// <param name="dateTime">
        /// The date Time.
        /// </param>
        /// <returns>
        /// The <see cref="DOSTimeStruct"/> calulated from the specified DateTime.
        /// </returns>
        public static DOSTimeStruct DatetimeToDosDateTime(DateTime dateTime)
        {
            DOSTimeStruct dosDateTimeStruct;

            // Calculates the dos Date
            short year = Convert.ToInt16(dateTime.Year - 2000);

            // bugtracker #221 : change with unsigned short
            dosDateTimeStruct.Date = Convert.ToUInt16((((year * 16) + dateTime.Month) * 32) + dateTime.Day);

            // Calculates the dos time
            dosDateTimeStruct.Time =
                Convert.ToUInt16((((dateTime.Hour * 64) + dateTime.Minute) * 32) + (dateTime.Second / 2));

            return dosDateTimeStruct;
        }

        /// <summary>
        /// Convert a timeStamp into dosDateTime format.
        /// </summary>
        /// <param name="timeStamp">time stamp to convert. TimeStamp format is "hhmmss" </param>
        /// <returns>Returns tDOSTime</returns>
        public static DOSTimeStruct TimestampToDosDateTime(int timeStamp)
        {
            DateTime currentDateTime = DateTime.Now;

            /* Convert message time */
            int hour = timeStamp / 10000;
            int min = (timeStamp - (hour * 10000)) / 100;
            int sec = timeStamp - (hour * 10000) - (min * 100);

            if ((hour < 24) && (min < 60) && (sec < 60))
            {
                // Add or substract one day to determe the date because there could be a small delay between the calculation and the emitter system date.    
                if ((hour == 23) && (currentDateTime.Hour == 0))
                {
                    // messsage time offset: -1 day
                    currentDateTime = currentDateTime.AddDays(-1);
                }
                else if ((hour == 0) && (currentDateTime.Hour == 23))
                {
                    // messsage time offset: +1 day
                    currentDateTime = currentDateTime.AddDays(1); 
                }

                currentDateTime = new DateTime(
                    currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, hour, min, sec);
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                    "timeStamp", timeStamp, "The time stamp must have the format in integer : HHMMSS.");
            } 

            return DatetimeToDosDateTime(currentDateTime);
        }

        /// <summary>
        /// Convert a DOSTimeStruct into c# DateTime format
        /// <remarks>
        /// 2 = precision of 2 secondes
        /// </remarks>
        /// </summary>
        /// <param name="dateTime">
        /// DOSDateTime to convert.
        /// </param>
        /// <returns>
        /// The DateTime 
        /// </returns>
        public static DateTime DosDateTimeToDateTime(DOSTimeStruct dateTime)
        {
            // unpack date
            int year = (dateTime.Date / 512) + 2000;
            int month = (dateTime.Date % 512) / 32;
            int day = dateTime.Date % 32;

            // Unpack time
            int hour = dateTime.Time / 2048;
            int min = (dateTime.Time % 2048) / 32;
            
            int sec = (dateTime.Time % 32) * 2;

            return new DateTime(year, month, day, hour, min, sec);
        }
    }
}
