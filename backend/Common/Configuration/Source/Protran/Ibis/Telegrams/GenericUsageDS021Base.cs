// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericUsageDS021Base.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GenericUsageDS021Base type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis.Telegrams
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.Core;

    /// <summary>
    /// The generic usage for <see cref="DS021ConfigBase"/> and subclasses
    /// which allow for the block to be defined manually.
    /// </summary>
    [Serializable]
    public class GenericUsageDS021Base : GenericUsage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericUsageDS021Base"/> class.
        /// </summary>
        public GenericUsageDS021Base()
        {
            this.FromBlock = -1;
        }

        /// <summary>
        /// Gets or sets the block number to use when filling the generic cell.
        /// </summary>
        [DefaultValue(-1)]
        [XmlAttribute("FromBlock")]
        public int FromBlock { get; set; }
    }
}