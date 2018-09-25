// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteAppStateViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RemoteAppStateViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.App
{
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Common.SystemManagement.ServiceModel;

    /// <summary>
    /// View model for the state of an application running on a remote unit.
    /// </summary>
    public class RemoteAppStateViewModel : ViewModelBase
    {
        private string version;

        private string path;

        private ApplicationState state;

        private string cpuUsage;

        private string ramUsage;

        private long ramUsageBytes;

        /// <summary>
        /// Gets or sets the application version.
        /// </summary>
        public string Version
        {
            get
            {
                return this.version;
            }

            set
            {
                this.SetProperty(ref this.version, value, () => this.Version);
            }
        }

        /// <summary>
        /// Gets or sets the full executable path.
        /// </summary>
        public string Path
        {
            get
            {
                return this.path;
            }

            set
            {
                this.SetProperty(ref this.path, value, () => this.Path);
            }
        }

        /// <summary>
        /// Gets or sets the application state.
        /// </summary>
        public ApplicationState State
        {
            get
            {
                return this.state;
            }

            set
            {
                this.SetProperty(ref this.state, value, () => this.State);
            }
        }

        /// <summary>
        /// Gets or sets the CPU usage.
        /// </summary>
        public string CpuUsage
        {
            get
            {
                return this.cpuUsage;
            }

            set
            {
                this.SetProperty(ref this.cpuUsage, value, () => this.CpuUsage);
            }
        }

        /// <summary>
        /// Gets or sets the RAM usage.
        /// </summary>
        public string RamUsage
        {
            get
            {
                return this.ramUsage;
            }

            set
            {
                this.SetProperty(ref this.ramUsage, value, () => this.RamUsage);
            }
        }

        /// <summary>
        /// Gets or sets the RAM usage in bytes.
        /// </summary>
        public long RamUsageBytes
        {
            get
            {
                return this.ramUsageBytes;
            }

            set
            {
                this.SetProperty(ref this.ramUsageBytes, value, () => this.RamUsageBytes);
            }
        }
    }
}