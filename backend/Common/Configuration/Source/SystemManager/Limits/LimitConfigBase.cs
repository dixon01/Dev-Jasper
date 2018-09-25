// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LimitConfigBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LimitConfigBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager.Limits
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Base class for all resource limit configurations.
    /// </summary>
    [Serializable]
    public abstract class LimitConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LimitConfigBase"/> class.
        /// </summary>
        protected LimitConfigBase()
        {
            this.Enabled = true;
            this.Actions = new List<LimitActionConfigBase>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this resource limit observation is enabled.
        /// Default value is true.
        /// </summary>
        [XmlAttribute]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the actions to perform if the limit is reached.
        /// One action at the time is executed and then it is verified if the limit is still reached.
        /// </summary>
        [XmlIgnore]
        public virtual List<LimitActionConfigBase> Actions { get; set; }
    }
}