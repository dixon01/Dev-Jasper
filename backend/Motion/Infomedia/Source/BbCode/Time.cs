// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Time.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Time type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.BbCode
{
    using System;

    /// <summary>
    /// The [time=format] BBCode tag that will render as the current date/time
    /// formatted with the given format string.
    /// </summary>
    public class Time : BbLeafValueTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Time"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <param name="tagName">
        /// The tag name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        internal Time(BbBranch parent, string tagName, string value)
            : base(parent, tagName, value)
        {
        }

        /// <summary>
        /// Gets the date and time format.
        /// </summary>
        /// <seealso cref="DateTime.ToString(string)"/>
        public string TimeFormat
        {
            get
            {
                return this.Value;
            }
        }
    }
}