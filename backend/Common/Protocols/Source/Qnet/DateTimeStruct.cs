// -----------------------------------------------------------------------
// <copyright file="DateTimeStruct.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Date structure that contains Time divided in Hour, Minute and second as short. 
    /// Len = 6 bytes. 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TimeStruct
    {
        /// <summary>
        /// Gets or sets the hour component of the time
        /// </summary>
        public short Hour;

        /// <summary>
        /// Gets or sets the minute component of the time
        /// </summary>
        public short Minute;

        /// <summary>
        /// Gets or sets the second component of the time
        /// </summary>
        public short Second;
    }

    /// <summary>
    /// Date structure that contains Date divided in Day, month and year as short. 
    /// Len = 6 bytes. 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DateStruct
    {
        /// <summary>
        /// Gets or sets the day component of the date
        /// </summary>
        public short Day;

        /// <summary>
        /// Gets or sets the month component of the date
        /// </summary>
        public short Month;

        /// <summary>
        /// Gets or sets the year component of the date
        /// </summary>
        public short Year;        
    }

    /// <summary>
    /// DateTime structure contained 2 parts Date and Time structure. It used in qnet messages
    /// Len = 12 bytes
    /// </summary>
    public struct DateTimeStruct
    {
        /// <summary>
        /// Gets the Date component of the DateTime structure.
        /// </summary>
        public DateStruct Date;

        /// <summary>
        /// Gets the time component of the DateTime structure.
        /// </summary>
        public TimeStruct Time;
    }
}
