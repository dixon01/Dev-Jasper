// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS006AConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS006AConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis.Telegrams
{
    using System;

    using Gorba.Common.Configuration.Protran.Common;

    /// <summary>
    /// Container of all the information about a DS006a IBIS telegram.
    /// </summary>
    [Serializable]
    public class DS006AConfig : TelegramConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS006AConfig"/> class.
        /// </summary>
        public DS006AConfig()
        {
            this.OutputFormat = "dd.MM.yyyy";
        }

        /// <summary>
        /// Gets or sets the output format for the date.
        /// The default value is culture independent <code>dd.MM.yyyy</code>.
        /// The date is only formatted using this string
        /// if the incoming telegram has a valid 14 character payload.
        /// </summary>
        public string OutputFormat { get; set; }

        /// <summary>
        /// Gets or sets the usage of this telegram.
        /// </summary>
        public override GenericUsage UsedFor { get; set; }
    }
}
