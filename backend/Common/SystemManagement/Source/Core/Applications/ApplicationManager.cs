// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationManager.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core.Applications
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.SystemManager.Application;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.SystemManagement.ServiceModel;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// The application manager that takes care of all <see cref="ApplicationControllerBase"/>s.
    /// </summary>
    public partial class ApplicationManager : IDisposable, IManageableTable
    {
        private static readonly Logger Logger = LogHelper.GetLogger<ApplicationManager>();

        private readonly List<ApplicationConfigBase> configs;

        private readonly List<ApplicationControllerBase> controllers = new List<ApplicationControllerBase>();

        private bool running;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationManager"/> class.
        /// </summary>
        /// <param name="configs">
        /// The applications to launch.
        /// </param>
        public ApplicationManager(List<ApplicationConfigBase> configs)
        {
            this.configs = configs;

            var myself = new SystemManagerApplicationController(this);
            this.controllers.Add(myself);

            Logger.Debug("Configuring all application controllers");
            foreach (var config in this.configs)
            {
                if (this.controllers.Find(c => c.Name == config.Name) != null)
                {
                    Logger.Error("Found duplicate application name, ignoring second one: {0}", config.Name);
                    continue;
                }

                var controller = ApplicationControllerBase.Create(config, this);
                this.controllers.Add(controller);
            }
        }

        /// <summary>
        /// Gets all controllers.
        /// </summary>
        public IEnumerable<ApplicationControllerBase> Controllers
        {
            get
            {
                return this.controllers;
            }
        }

        /// <summary>
        /// Starts this manager.
        /// </summary>
        public void Start()
        {
            if (this.running)
            {
                throw new NotSupportedException("Can't start ApplicationManager twice");
            }

            this.running = true;
            this.KillExistingProcesses();

            Logger.Debug("Starting all application controllers");

            foreach (var controller in this.controllers)
            {
                if (controller.Enabled)
                {
                    controller.Start();
                }
                else
                {
                    Logger.Info("{0} is not enabled, not starting it", controller.Name);
                }
            }
        }

        /// <summary>
        /// Stops this manager.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        /// <param name="explanation">
        /// The explanation.
        /// </param>
        public void Stop(ApplicationReason reason, string explanation)
        {
            if (!this.running)
            {
                return;
            }

            this.running = false;

            Logger.Debug("Stopping all controllers");
            foreach (var controller in this.controllers)
            {
                Logger.Trace("Stopping {0}", controller.Name);
                controller.Stop(reason, explanation);
            }

            Logger.Debug("Waiting for all controllers to be stopped");
            foreach (var controller in this.controllers)
            {
                if (controller is SystemManagerApplicationController)
                {
                    // we don't care about the System Manager's state (it is never in state Exited!)
                    continue;
                }

                Logger.Trace("Waiting for {0}", controller.Name);


                if (controller.State == ApplicationState.AwaitingLaunch || controller.State == ApplicationState.Unknown)
                {
                    // HACK - Workaround to exit application quickly for maintenance using LRWIN + 'X'
                }
                else
                {
                    for (int i = 0; i < 60; i++)
                    {
                        // TODO: how should this timeout be set?
                        if (controller.WaitForStopped(TimeSpan.FromSeconds(5)))
                        {
                            break;
                        }

                        Logger.Trace("Still waiting for {0} (state is {1})", controller.Name, controller.State);
                    }
                }
            }

            Logger.Debug("All controllers stopped");
        }

        /// <summary>
        /// Gets a controller for the given name.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The controller with the given name or null if not found.
        /// </returns>
        public ApplicationControllerBase GetController(string name)
        {
            foreach (var controller in this.controllers)
            {
                if (controller.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return controller;
                }
            }

            return null;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!this.running)
            {
                return;
            }

            this.running = false;

            Logger.Debug("Disposing all controllers");
            foreach (var controller in this.controllers)
            {
                Logger.Trace("Disposing {0}", controller.Name);
                controller.Dispose();
            }
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            foreach (var controller in this.Controllers)
            {
                yield return parent.Factory.CreateManagementProvider(controller.Name, parent, controller);
            }
        }

        IEnumerable<List<ManagementProperty>> IManageableTable.GetRows()
        {
            const double MegaBytes = 1024 * 1024;

            foreach (var controller in this.Controllers)
            {
                if (!controller.Enabled)
                {
                    continue;
                }

                var info = controller.CreateApplicationInfo();
                var ramUsage = string.Format("{0:0.00}MB", info.RamBytes / MegaBytes);
                var cpuUsage = string.Format("{0:0} %", info.CpuUsage * 100);
                yield return new List<ManagementProperty>
                                 {
                                     new ManagementProperty<string>("Name", info.Name, true),
                                     new ManagementProperty<string>("Version", info.Version, true),
                                     new ManagementProperty<string>("State", info.State.ToString(), true),
                                     new ManagementProperty<string>("RAM Usage", ramUsage, true),
                                     new ManagementProperty<string>("CPU Usage", cpuUsage, true)
                                 };
            }
        }

        private void KillExistingProcesses()
        {
            // I'd love to use LINQ here ;-)
            var toKill =
                this.configs.FindAll(c => c.Enabled && c is ProcessConfig && ((ProcessConfig)c).KillIfRunning)
                    .ConvertAll(c => (ProcessConfig)c);
            if (toKill.Count == 0)
            {
                return;
            }

            Logger.Debug("Killing all existing processes for {0} configs", toKill.Count);
            this.KillProcesses(toKill);
        }

        partial void KillProcesses(List<ProcessConfig> toKill);
    }
}