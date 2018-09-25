// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationInfo.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Client
{
    using System;
    using System.Diagnostics;

    using Gorba.Common.SystemManagement.ServiceModel;
    using Gorba.Common.Utility.Core;

    /// <summary>
    /// Information about an application.
    /// </summary>
    public class ApplicationInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationInfo"/> class.
        /// </summary>
        /// <param name="id">
        /// The application id.
        /// </param>
        internal ApplicationInfo(string id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationInfo"/> class
        /// from the given message object.
        /// </summary>
        /// <param name="appInfo">
        /// The <see cref="Motion.SystemManager.ServiceModel.Messages.ApplicationInfo"/> object.
        /// </param>
        internal ApplicationInfo(Motion.SystemManager.ServiceModel.Messages.ApplicationInfo appInfo)
        {
            this.Id = appInfo.Id;
            this.Name = appInfo.Name;
            this.Version = appInfo.Version;
            this.Path = appInfo.Path;
            this.State = MessageConverter.Convert(appInfo.State);
            this.StartTimeUtc = appInfo.StartTimeUtc;
            this.RamBytes = appInfo.RamBytes;
            this.CpuUsage = appInfo.CpuUsage;
            this.WindowState = appInfo.WindowState;
            this.HasFocus = appInfo.HasFocus;

            if (appInfo.LastLaunchReason != null)
            {
                this.LastLaunchReason = new ApplicationReasonInfo(appInfo.LastLaunchReason);
            }

            if (appInfo.LastExitReason != null)
            {
                this.LastExitReason = new ApplicationReasonInfo(appInfo.LastExitReason);
            }
        }

        /// <summary>
        /// Gets the name of the application given in the System Manager config.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the version of the application.
        /// </summary>
        public string Version { get; internal set; }

        /// <summary>
        /// Gets the path of the application (the path to the DLL or EXE).
        /// </summary>
        public string Path { get; internal set; }

        /// <summary>
        /// Gets the state of this application.
        /// </summary>
        public ApplicationState State { get; internal set; }

        /// <summary>
        /// Gets the uptime (how long has the application been running).
        /// </summary>
        public TimeSpan Uptime
        {
            get
            {
                return TimeProvider.Current.UtcNow - this.StartTimeUtc;
            }
        }

        /// <summary>
        /// Gets the amount of RAM used in bytes.
        /// </summary>
        public long RamBytes { get; internal set; }

        /// <summary>
        /// Gets the CPU usage in percent.
        /// </summary>
        public double CpuUsage { get; internal set; }

        /// <summary>
        /// Gets the window state of the main window.
        /// </summary>
        public ProcessWindowStyle WindowState { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the main window has the focus.
        /// </summary>
        public bool HasFocus { get; internal set; }

        /// <summary>
        /// Gets the reason of the last application launch.
        /// </summary>
        public ApplicationReasonInfo LastLaunchReason { get; internal set; }

        /// <summary>
        /// Gets the reason of the last application exit.
        /// </summary>
        public ApplicationReasonInfo LastExitReason { get; internal set; }

        /// <summary>
        /// Gets the application id.
        /// </summary>
        internal string Id { get; private set; }

        /// <summary>
        /// Gets or sets the start time of the application in UTC.
        /// </summary>
        internal DateTime StartTimeUtc { get; set; }

        /// <summary>
        /// Converts this object to an <see cref="Motion.SystemManager.ServiceModel.Messages.ApplicationInfo"/>
        /// message object.
        /// </summary>
        /// <returns>
        /// The <see cref="Motion.SystemManager.ServiceModel.Messages.ApplicationInfo"/> object.
        /// </returns>
        internal Motion.SystemManager.ServiceModel.Messages.ApplicationInfo ToMessage()
        {
            return new Motion.SystemManager.ServiceModel.Messages.ApplicationInfo
                       {
                           Id = this.Id,
                           Name = this.Name,
                           Version = this.Version,
                           Path = this.Path,
                           State = MessageConverter.Convert(this.State),
                           RamBytes = this.RamBytes,
                           CpuUsage = this.CpuUsage,
                           WindowState = this.WindowState,
                           HasFocus = this.HasFocus,
                           LastLaunchReason = this.LastLaunchReason != null ? this.LastLaunchReason.ToMessage() : null,
                           LastExitReason = this.LastExitReason != null ? this.LastExitReason.ToMessage() : null,
                           StartTimeUtc = this.StartTimeUtc
                       };
        }
    }
}