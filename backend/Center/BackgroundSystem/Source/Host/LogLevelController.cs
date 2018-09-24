// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogLevelController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The WCF traces controller. Allows runtime change of trace level.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Host
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// The log level LazyController. Allows runtime change of trace level.
    /// </summary>
    public class LogLevelController
    {
        private static readonly Lazy<LogLevelController> LazyControllerInstance =
            new Lazy<LogLevelController>(() => new LogLevelController());

        private LogLevelController()
        {
        }

        /// <summary>
        /// The trace level LazyController.
        /// </summary>
        /// <param name="level">
        /// The level.
        /// </param>
        public delegate void TraceLevelController(SourceLevels level);

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static LogLevelController Instance
        {
            get
            {
                return LazyControllerInstance.Value;
            }
        }

        /// <summary>
        /// Gets or sets the level LazyController.
        /// </summary>
        public TraceLevelController LevelController { get; set; }
    }
}