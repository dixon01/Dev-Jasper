// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationsSplashScreenPart.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationsSplashScreenPart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.SplashScreen.Form
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;

    using Gorba.Common.Configuration.SystemManager.SplashScreen.Items;
    using Gorba.Common.SystemManagement.Client;
    using Gorba.Common.SystemManagement.Core.Applications;
    using Gorba.Common.SystemManagement.ServiceModel;
    using Gorba.Common.Utility.Core;

    using Microsoft.Practices.ServiceLocation;

    using Math = System.Math;

    /// <summary>
    /// Splash screen part showing information about running applications.
    /// </summary>
    public class ApplicationsSplashScreenPart : SplashScreenPartBase
    {
        private const string RamFormat = "RAM: {0:0.0} MB";
        private const string CpuFormat = "CPU: {0:0}%";
        private const string UptimeFormat = "{0:00}:{1:00}:{2:00}";

        private const string StringMeasureSpacer = "WM";

        private readonly ApplicationsSplashScreenItem config;

        private readonly List<ApplicationInfoRow> rows = new List<ApplicationInfoRow>();

        private readonly List<ApplicationControllerBase> controllers = new List<ApplicationControllerBase>();

        private Font font;

        private SolidBrush brush;

        private int nameWidth;
        private int versionWidth;
        private int stateWidth;
        private int ramWidth;
        private int cpuWidth;
        private int uptimeWidth;
        private int launchWidth;
        private int exitWidth;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationsSplashScreenPart"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public ApplicationsSplashScreenPart(ApplicationsSplashScreenItem config)
        {
            this.config = config;
            var appManager = ServiceLocator.Current.GetInstance<ApplicationManager>();
            foreach (var controller in appManager.Controllers)
            {
                if (!controller.Enabled)
                {
                    continue;
                }

                if (config.State)
                {
                    controller.StateChanged += this.ControllerOnStateChanged;
                }

                this.controllers.Add(controller);
            }

            if (config.Visibility.Count > 0)
            {
                var shows =
                    config.Visibility.FindAll(v => v is ApplicationsSplashScreenShow);
                if (shows.Count > 0)
                {
                    this.controllers.RemoveAll(c => shows.Find(v => v.Application == c.Name) == null);
                }
                else
                {
                    var hides = config.Visibility.FindAll(v => v is ApplicationsSplashScreenHide);
                    this.controllers.RemoveAll(c => hides.Find(v => v.Application == c.Name) != null);
                }
            }

            foreach (var controller in this.controllers)
            {
                this.rows.Add(new ApplicationInfoRow(controller));
            }
        }

        private delegate string StringGetter(ApplicationInfoRow row);

        /// <summary>
        /// Gets or sets the foreground color.
        /// </summary>
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }

            set
            {
                base.ForeColor = value;
                this.brush = new SolidBrush(value);
            }
        }

        /// <summary>
        /// Paints this part.
        /// </summary>
        /// <param name="g">
        /// The graphics object.
        /// </param>
        /// <param name="rect">
        /// The rectangle into which we should paint.
        /// </param>
        public override void Paint(Graphics g, Rectangle rect)
        {
            var spaceLeft = rect.Width - this.nameWidth - this.versionWidth - this.stateWidth - this.ramWidth
                            - this.cpuWidth - this.uptimeWidth;

            var launch = this.launchWidth;
            var exit = this.exitWidth;

            if (spaceLeft < launch + exit && (this.config.LaunchReason || this.config.ExitReason))
            {
                launch = spaceLeft * this.launchWidth / (this.launchWidth + this.exitWidth);
                exit = spaceLeft * this.exitWidth / (this.launchWidth + this.exitWidth);
            }

            var x = rect.X + ((spaceLeft - launch - exit) / 2);
            var y = rect.Y;

            x = this.DrawString(g, r => r.Name, x, y, this.nameWidth, true);
            x = this.DrawString(g, r => r.Version, x, y, this.versionWidth, this.config.Version);
            x = this.DrawString(g, r => r.State, x, y, this.stateWidth, this.config.State);
            x = this.DrawString(g, r => r.Ram, x, y, this.ramWidth, this.config.Ram);
            x = this.DrawString(g, r => r.Cpu, x, y, this.cpuWidth, this.config.Cpu);
            x = this.DrawString(g, r => r.Uptime, x, y, this.uptimeWidth, this.config.Uptime);
            x = this.DrawString(g, r => r.LaunchReason, x, y, launch, this.config.LaunchReason);
            this.DrawString(g, r => r.ExitReason, x, y, exit, this.config.ExitReason);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            base.Dispose();

            foreach (var controller in this.controllers)
            {
                controller.StateChanged -= this.ControllerOnStateChanged;
            }
        }

        /// <summary>
        /// Implementation of the scaling.
        /// </summary>
        /// <param name="factor">
        /// The factor (0.0 .. 1.0).
        /// </param>
        /// <param name="graphics">
        /// The graphics to calculate the scaling.
        /// </param>
        /// <returns>
        /// The calculated height of this part with the used scaling factor.
        /// </returns>
        protected override Size DoScale(double factor, Graphics graphics)
        {
            if (this.font != null)
            {
                this.font.Dispose();
            }

            this.font = new Font(FontFamily.GenericSansSerif, (float)(20 * factor), FontStyle.Bold);

            var measureString = new StringBuilder();
            foreach (var row in this.rows)
            {
                this.AppendLine(measureString.Append(row.Name), StringMeasureSpacer);
            }

            var size = graphics.MeasureString(measureString.ToString(), this.font);

            this.nameWidth = (int)Math.Ceiling(size.Width);
            if (this.config.Version)
            {
                this.versionWidth = this.GetStringWidth("99.99.9999.9999", graphics);
            }

            if (this.config.State)
            {
                this.stateWidth = this.GetStringWidth(ApplicationState.AwaitingLaunch.ToString(), graphics);
            }

            if (this.config.Ram)
            {
                this.ramWidth = this.GetStringWidth(string.Format(RamFormat, 9999), graphics);
            }

            if (this.config.Cpu)
            {
                this.cpuWidth = this.GetStringWidth(string.Format(CpuFormat, 100), graphics);
            }

            if (this.config.Uptime)
            {
                this.uptimeWidth = this.GetStringWidth(string.Format(UptimeFormat, 99, 99, 99), graphics);
            }

            if (this.config.LaunchReason)
            {
                measureString.Length = 0;
                foreach (var row in this.rows)
                {
                    this.AppendLine(measureString, row.LaunchReason);
                }

                this.launchWidth = this.GetStringWidth(measureString.ToString(), graphics);
            }

            if (this.config.ExitReason)
            {
                measureString.Length = 0;
                foreach (var row in this.rows)
                {
                    this.AppendLine(measureString, row.ExitReason);
                }

                this.exitWidth = this.GetStringWidth(measureString.ToString(), graphics);
            }

            return new Size
                       {
                           Height = (int)Math.Ceiling(size.Height),
                           Width = this.nameWidth + this.versionWidth + this.stateWidth + this.ramWidth
                            + this.cpuWidth + this.uptimeWidth + this.launchWidth + this.exitWidth
                       };
        }

        private int DrawString(Graphics graphics, StringGetter getter, int x, int y, int width, bool draw)
        {
            if (!draw)
            {
                return x;
            }

            var sb = new StringBuilder();
            foreach (var row in this.rows)
            {
                this.AppendLine(sb, getter(row));
            }

            graphics.DrawString(
                sb.ToString(),
                this.font,
                this.brush,
                new RectangleF(x, y, width, this.Size.Height),
                SplashScreenPartBase.DefaultStringFormat);
            return x + width;
        }

        private int GetStringWidth(string value, Graphics graphics)
        {
            return (int)Math.Ceiling(graphics.MeasureString(StringMeasureSpacer + value, this.font).Width);
        }

        private void ControllerOnStateChanged(object sender, EventArgs eventArgs)
        {
            this.RaiseContentChanged(eventArgs);
        }

        private class ApplicationInfoRow
        {
            private const double MegaBytes = 1024 * 1024;

            private readonly ApplicationControllerBase controller;

            private DateTime lastInfoUpdateUtc;
            private ApplicationInfo lastApplicationInfo;

            public ApplicationInfoRow(ApplicationControllerBase controller)
            {
                this.controller = controller;
                this.lastInfoUpdateUtc = DateTime.MinValue;
            }

            public string Name
            {
                get
                {
                    return this.controller.Name;
                }
            }

            public string Version
            {
                get
                {
                    return this.GetInfo().Version;
                }
            }

            public string State
            {
                get
                {
                    return this.controller.State.ToString();
                }
            }

            public string Ram
            {
                get
                {
                    return string.Format(RamFormat, this.GetInfo().RamBytes / MegaBytes);
                }
            }

            public string Cpu
            {
                get
                {
                    return string.Format(CpuFormat, this.GetInfo().CpuUsage * 100);
                }
            }

            public string Uptime
            {
                get
                {
                    var uptime = this.GetInfo().Uptime;
                    if (uptime < TimeSpan.Zero)
                    {
                        return string.Empty;
                    }

                    return string.Format(UptimeFormat, uptime.Hours, uptime.Minutes, uptime.Seconds);
                }
            }

            public string LaunchReason
            {
                get
                {
                    var reason = this.GetInfo().LastLaunchReason;
                    return reason == null ? "n/a" : "L: " + reason.Reason;
                }
            }

            public string ExitReason
            {
                get
                {
                    var reason = this.GetInfo().LastExitReason;
                    return reason == null ? "n/a" : "E: " + reason.Reason.ToString();
                }
            }

            private ApplicationInfo GetInfo()
            {
                var now = TimeProvider.Current.UtcNow;
                if ((now - this.lastInfoUpdateUtc) >= TimeSpan.FromSeconds(2))
                {
                    this.lastApplicationInfo = this.controller.CreateApplicationInfo();
                    this.lastInfoUpdateUtc = now;
                }

                return this.lastApplicationInfo;
            }
        }
    }
}