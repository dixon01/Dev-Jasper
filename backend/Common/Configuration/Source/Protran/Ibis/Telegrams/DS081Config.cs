// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS081Config.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS081Config type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis.Telegrams
{
    using System;
    using System.ComponentModel;

    using Gorba.Common.Configuration.Protran.Common;

    /// <summary>
    /// Configuration for the DS081 telegram (door closed)
    /// </summary>
    [Serializable]
    public class DS081Config : TelegramConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS081Config"/> class.
        /// </summary>
        public DS081Config()
        {
            this.Value = "0";
        }

        /// <summary>
        /// Gets or sets the value to be set when the DS081 telegram is received.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the usage of this telegram.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public override GenericUsage UsedFor { get; set; }
    }
}