// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS003AConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS003AConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis.Telegrams
{
    using System;

    using Gorba.Common.Configuration.Protran.Common;

    /// <summary>
    /// Container of all the information about a DS003A IBIS telegram.
    /// </summary>
    [Serializable]
    public class DS003AConfig : TelegramConfig
    {
        /// <summary>
        /// Gets or sets a value indicating the block length it should be split by.
        /// </summary>
        public int SplitByBlock { get; set; }

        /// <summary>
        /// Gets or sets the character the telegram should be split by.
        /// </summary>
        public string SplitByChar { get; set; }

        /// <summary>
        /// Gets or sets the control character which indicates the type of processing to be done for the telegram.
        /// </summary>
        public ControlChar ControlChar { get; set; }

        /// <summary>
        /// Gets or sets the usage of this telegram.
        /// </summary>
        public override GenericUsage UsedFor { get; set; }
    }
}
