// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS006Config.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS006Config type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis.Telegrams
{
    using System;

    using Gorba.Common.Configuration.Protran.Common;

    /// <summary>
    /// Container of all the information about a DS006 IBIS telegram.
    /// </summary>
    [Serializable]
    public class DS006Config : TelegramConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS006Config"/> class.
        /// </summary>
        public DS006Config()
        {
            this.InitialYear = 2014;
            this.OutputFormat = "dd.MM.yyyy";
        }

        /// <summary>
        /// Gets or sets the initial year.
        /// If DS006 is 5 digits long (i.e. DDMMY), this defines
        /// how to calculate the actual four-digit year.
        /// </summary>
        public int InitialYear { get; set; }

        /// <summary>
        /// Gets or sets the output format for the date.
        /// The default value is culture independent <code>dd.MM.yyyy</code>.
        /// The date is only formatted using this string
        /// if the incoming telegram has either a 5 or 6
        /// character payload.
        /// </summary>
        public string OutputFormat { get; set; }

        /// <summary>
        /// Gets or sets the usage of this telegram.
        /// </summary>
        public override GenericUsage UsedFor { get; set; }
    }
}
