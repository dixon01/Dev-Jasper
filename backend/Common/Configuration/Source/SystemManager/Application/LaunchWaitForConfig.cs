// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LaunchWaitForConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LaunchWaitForConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager.Application
{
    using System;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.SystemManager;
    using Gorba.Common.SystemManagement.ServiceModel;

    /// <summary>
    /// Object to reference an application and its state for
    /// <see cref="ApplicationConfigBase.LaunchWaitFor"/>.
    /// </summary>
    [Serializable]
    public class LaunchWaitForConfig
    {
        /// <summary>
        /// Gets or sets the name of the application to wait for.
        /// This must match <see cref="ApplicationConfigBase.Name"/>.
        /// </summary>
        [XmlAttribute]
        public string Application { get; set; }

        /// <summary>
        /// Gets or sets the state that the other application has to reach
        /// so this application can launch. This is usually <see cref="ApplicationState.Running"/>.
        /// </summary>
        [XmlAttribute]
        public ApplicationState State { get; set; }
    }
}