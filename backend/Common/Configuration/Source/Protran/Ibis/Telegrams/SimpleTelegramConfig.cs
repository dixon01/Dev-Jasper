// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleTelegramConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SimpleTelegramConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis.Telegrams
{
    using System;
    using System.ComponentModel;

    using Gorba.Common.Configuration.Protran.Common;

    /// <summary>
    /// Container of all the information about an IBIS telegram.
    /// </summary>
    [Serializable]
    public class SimpleTelegramConfig : TelegramConfig
    {
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
    }
}