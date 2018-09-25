// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeItemHandler.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TimeItemHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.SoapIbisPlus.Service
{
    using System;
    using System.Globalization;

    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Motion.Protran.Core.Utils;
    using Gorba.Motion.Protran.SoapIbisPlus.Config;

    /// <summary>
    /// The handler for <see cref="TimeItemConfig"/>.
    /// </summary>
    internal class TimeItemHandler
    {
        private static readonly DateTime Zero = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);

        private readonly GenericUsageHandler relativeUsageHandler;

        private readonly GenericUsageHandler absoluteUsageHandler;

        private readonly string absoluteTimeFormat;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeItemHandler"/> class.
        /// This constructor should only be called from the <see cref="ItemHandlerFactory"/>.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        public TimeItemHandler(TimeItemConfig config, Dictionary dictionary)
        {
            if (config == null || !config.Enabled)
            {
                return;
            }

            this.relativeUsageHandler = new GenericUsageHandler(config.UsedForRelativeTime, dictionary);
            this.absoluteUsageHandler = new GenericUsageHandler(config.UsedForAbsoluteTime, dictionary);
            this.absoluteTimeFormat = config.UsedForAbsoluteTime.TimeFormat;
        }

        /// <summary>
        /// Adds one or more <see cref="XimpleCell"/>s to the given <see cref="Ximple"/> object.
        /// </summary>
        /// <param name="ximple">
        /// The ximple object to which the cell should be added.
        /// </param>
        /// <param name="value">
        /// The time value (Unix timestamp).
        /// </param>
        /// <param name="now">
        /// The current local time.
        /// </param>
        /// <param name="rowIndex">
        /// The row index (default is 0).
        /// </param>
        public void AddCells(Ximple ximple, int value, DateTime now, int rowIndex = 0)
        {
            if (this.relativeUsageHandler == null)
            {
                return;
            }

            if (value == 0)
            {
                this.relativeUsageHandler.AddCell(ximple, string.Empty, rowIndex);
                this.absoluteUsageHandler.AddCell(ximple, string.Empty, rowIndex);
                return;
            }

            var time = Zero + TimeSpan.FromSeconds(value);
            this.relativeUsageHandler.AddCell(ximple, this.GetRelativeString(time - now), rowIndex);
            this.absoluteUsageHandler.AddCell(ximple, this.GetAbsoluteString(value), rowIndex);
        }

        private string GetRelativeString(TimeSpan value)
        {
            return ((int)value.TotalMinutes).ToString(CultureInfo.InvariantCulture);
        }

        private string GetAbsoluteString(int value)
        {
            var time = Zero + TimeSpan.FromSeconds(value);
            return time.ToString(this.absoluteTimeFormat);
        }
    }
}