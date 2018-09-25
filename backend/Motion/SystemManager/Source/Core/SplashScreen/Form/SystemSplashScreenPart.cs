// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemSplashScreenPart.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemSplashScreenPart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.SplashScreen.Form
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Gorba.Common.Configuration.SystemManager.SplashScreen.Items;
    using Gorba.Common.SystemManagement.Core;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Core;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Splash screen part that shows information about the system.
    /// </summary>
    public class SystemSplashScreenPart : TextSplashScreenPartBase
    {
        private const double MegaBytes = 1024 * 1024;

        private readonly List<StringGetter> lines = new List<StringGetter>();

        private readonly SystemManagementControllerBase controller;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemSplashScreenPart"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="manager">
        /// The splash screen manager.
        /// </param>
        public SystemSplashScreenPart(SystemSplashScreenItem config, SplashScreenManager manager)
        {
            this.controller = ServiceLocator.Current.GetInstance<SystemManagementControllerBase>();

            if (config.MachineName)
            {
                this.lines.Add(() => "System Name: " + ApplicationHelper.MachineName);
            }

            if (config.Serial)
            {
                this.lines.Add(() => "Identifier: " + (manager.HardwareAbstraction.SerialNumber ?? "n/a"));
            }

            if (config.Ram)
            {
                this.lines.Add(this.GetRamString);
            }

            if (config.Cpu)
            {
                this.lines.Add(this.GetCpuString);
            }

            if (config.Uptime)
            {
                this.lines.Add(this.GetUptimeString);
            }
        }

        private delegate string StringGetter();

        /// <summary>
        /// Calculates the string to be shown on the splash screen.
        /// This string can contain multiple lines.
        /// </summary>
        /// <returns>
        /// The string to be shown on the splash screen.
        /// </returns>
        protected override string GetDisplayString()
        {
            var sb = new StringBuilder();
            foreach (var line in this.lines)
            {
                this.AppendLine(sb, line());
            }

            return sb.ToString();
        }

        private string GetRamString()
        {
            var available = this.controller.SystemResources.AvailableRam;
            var total = this.controller.SystemResources.TotalRam;
            var used = total - available;
            if (total == 0)
            {
                return string.Empty;
            }

            return string.Format(
                "Used RAM: {0:0.0} MB / {1:0.0} MB ({2:0}%)", used / MegaBytes, total / MegaBytes, 100 * used / total);
        }

        private string GetCpuString()
        {
            return string.Format("CPU: {0:0}%", this.controller.SystemResources.CpuUsage * 100);
        }

        private string GetUptimeString()
        {
            var uptime = TimeSpan.FromMilliseconds(TimeProvider.Current.TickCount);
            return string.Format(
                "Uptime: {0:##00}:{1:00}:{2:00}", (int)uptime.TotalHours, uptime.Minutes, uptime.Seconds);
        }
    }
}