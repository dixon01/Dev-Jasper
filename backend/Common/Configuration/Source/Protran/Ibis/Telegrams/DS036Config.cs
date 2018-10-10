// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS036Config.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS036Config type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis.Telegrams
{
    using System;
    using System.ComponentModel;

    using Gorba.Common.Configuration.Protran.Common;

    /// <summary>
    /// Container of all the information about a DS006 IBIS telegram.
    /// </summary>
    [Serializable]
    public class DS036Config : TelegramConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS036Config"/> class.
        /// </summary>
        public DS036Config()
        {
            this.AutoReset = true;
        }

        /// <summary>
        /// Gets or sets the usage of this telegram.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public override GenericUsage UsedFor { get; set; }

        /// <summary>
        /// Gets or sets the XML element called Answer.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public override Answer Answer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to automatically reset
        /// the configured cell (<see cref="TelegramConfig.UsedFor"/>) to
        /// an empty string after receiving a value.
        /// This is set to true by default since we only use this telegram
        /// to trigger audio output but don't want it to stay in the cache.
        /// </summary>
        public bool AutoReset { get; set; }
    }
}
