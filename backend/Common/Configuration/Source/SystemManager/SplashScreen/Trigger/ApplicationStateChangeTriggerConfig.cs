// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationStateChangeTriggerConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationStateChangeTriggerConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager.SplashScreen.Trigger
{
    using System;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.SystemManager;
    using Gorba.Common.Configuration.SystemManager.Application;
    using Gorba.Common.SystemManagement.ServiceModel;

    /// <summary>
    /// Trigger to show or hide the splash screen if the given application reaches the given state.
    /// </summary>
    [Serializable]
    public class ApplicationStateChangeTriggerConfig : SplashScreenTriggerConfigBase
    {
        /// <summary>
        /// Gets or sets the name of the application to observe.
        /// This has to match a <see cref="ApplicationConfigBase.Name"/>.
        /// </summary>
        [XmlAttribute]
        public string Application { get; set; }

        /// <summary>
        /// Gets or sets the state that has to be reached.
        /// </summary>
        [XmlAttribute]
        public ApplicationState State { get; set; }
    }
}