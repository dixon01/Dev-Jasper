// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelaunchLimitActionConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RelaunchLimitActionConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager.Limits
{
    using System;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.SystemManager.Application;

    /// <summary>
    /// Action performed if a given limit is reached: re-launch the given application.
    /// </summary>
    [Serializable]
    public class RelaunchLimitActionConfig : LimitActionConfigBase
    {
        /// <summary>
        /// Gets or sets the application name.
        /// This can be null if the action is related to an application; in this case
        /// the application triggering the action is used as the application to re-launch.
        /// Otherwise this has to match a <see cref="ApplicationConfigBase.Name"/>.
        /// </summary>
        [XmlAttribute]
        public string Application { get; set; }
    }
}