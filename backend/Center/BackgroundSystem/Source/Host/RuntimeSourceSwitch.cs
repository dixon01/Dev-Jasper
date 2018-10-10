// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeSourceSwitch.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The runtime source switch.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Host
{
    using System.Diagnostics;

    using NLog;

    /// <summary>
    /// The runtime source switch.
    /// </summary>
    public class RuntimeSourceSwitch : SourceSwitch
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeSourceSwitch"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public RuntimeSourceSwitch(string name)
            : base("System.ServiceModel")
        {
            this.Level = SourceLevels.Off;
            LogLevelController.Instance.LevelController = this.WcfTracesLevelController;
        }

        /// <summary>
        /// The WCF traces level controller.
        /// </summary>
        /// <param name="level">
        /// The level.
        /// </param>
        private void WcfTracesLevelController(SourceLevels level)
        {
            Logger.Trace("Setting SourceLevel to '{0}'", level);
            this.Level = level;
        }
    }
}