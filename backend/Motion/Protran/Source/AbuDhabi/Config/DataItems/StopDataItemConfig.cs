// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StopDataItemConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StopDataItemConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Config.DataItems
{
    using System;
    using System.ComponentModel;

    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.Core;

    /// <summary>
    /// Base class for data item configurations that refer to stops that have to be shown on two lines.
    /// </summary>
    [Serializable]
    public abstract class StopDataItemConfig : DataItemConfig
    {
        /// <summary>
        /// Gets or sets the usage of this telegram for the first line in the stops list.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericUsage FirstLineUsedFor { get; set; }

        /// <summary>
        /// Gets or sets the usage of this telegram for the second line in the stops list.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericUsage SecondLineUsedFor { get; set; }
    }
}
