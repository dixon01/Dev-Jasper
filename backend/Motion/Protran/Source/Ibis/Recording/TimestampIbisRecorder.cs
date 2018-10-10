// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimestampIbisRecorder.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TimestampIbisRecorder type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Recording
{
    using System;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// This class gives the timestamps basing on the Protran IBIS format.
    /// </summary>
    public class TimestampIbisRecorder : IbisRecorder
    {
        /// <summary>
        /// The object tasked to concatenate the pieces of string
        /// for a well formatted string.
        /// </summary>
        private readonly StringBuilder formatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimestampIbisRecorder"/> class.
        /// </summary>
        /// <param name="configContext">
        /// The config context.
        /// </param>
        public TimestampIbisRecorder(IIbisConfigContext configContext)
            : base(configContext)
        {
            this.formatter = new StringBuilder();
        }

        /// <summary>
        /// Converts the incoming text to a text formatted
        /// as specified by the Protran format.
        /// </summary>
        /// <param name="text">The text to which add the format timestamp.</param>
        /// <returns>The string with the timestamp formatted.</returns>
        protected override string FormatText(string text)
        {
            // inside the file, I've to write some text.
            // but how should be formatted the text ?
            // this recorder allows two types of format:
            // 1) [Timestamp] text  (where Timestamp ==> yyyy/MM/dd HH:mm:ss.fff)
            // 2) Gismo format
            this.formatter.Length = 0;

            // it's valid the first formatting style.
            DateTime now = DateTime.Now;
            this.formatter.AppendFormat(CultureInfo.InvariantCulture, "{0:yyyy/MM/dd HH:mm:ss.fff}", now);

            this.formatter.Append(' ');
            this.formatter.Append(text);
            return this.formatter.ToString();
        }
    }
}