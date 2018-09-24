// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeManager.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DateTimeManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.SoapIbisPlus.Service
{
    using System;

    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Motion.Protran.Core.Utils;
    using Gorba.Motion.Protran.SoapIbisPlus.Config;

    /// <summary>
    /// Class that manages date and time updates and creates Ximple for it.
    /// </summary>
    internal class DateTimeManager : DataManagerBase
    {
        private readonly GenericUsageHandler dateUsage;
        private readonly GenericUsageHandler timeUsage;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeManager"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        public DateTimeManager(TimeSyncConfig config, Dictionary dictionary)
        {
            if (config == null || !config.Enabled)
            {
                return;
            }

            this.dateUsage = new GenericUsageHandler(config.DateUsedFor, dictionary);
            this.timeUsage = new GenericUsageHandler(config.TimeUsedFor, dictionary);
        }

        /// <summary>
        /// Sets the date time and sends Ximple data for it.
        /// </summary>
        /// <param name="dateTime">
        /// The date time.
        /// </param>
        public void SetDateTime(DateTime dateTime)
        {
            if (this.dateUsage == null)
            {
                return;
            }

            var ximple = new Ximple();
            this.dateUsage.AddCell(ximple, dateTime.ToString("dd.MM.yyyy"));
            this.timeUsage.AddCell(ximple, dateTime.ToString("HH:mm:ss"));
            this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
        }
    }
}