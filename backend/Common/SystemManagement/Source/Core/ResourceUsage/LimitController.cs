// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LimitController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LimitController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core.ResourceUsage
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Gorba.Common.Configuration.SystemManager.Limits;
    using Gorba.Common.Protocols.Alarming;
    using Gorba.Common.SystemManagement.Core.Applications;
    using Gorba.Common.Utility.Core;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// Controller that performs actions defined in a <see cref="LimitConfigBase"/>.
    /// </summary>
    public class LimitController
    {
        private static readonly Logger Logger = LogHelper.GetLogger<LimitController>();

        private readonly LimitConfigBase config;

        private readonly ApplicationRelaunchAttribute relaunchReason;

        private readonly ApplicationControllerBase controller;

        private int actionIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="LimitController"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="relaunchReason">
        /// The reason why an application would be re-launched.
        /// </param>
        /// <param name="controller">
        /// The controller.
        /// </param>
        public LimitController(
            LimitConfigBase config, ApplicationRelaunchAttribute relaunchReason, ApplicationControllerBase controller)
        {
            this.config = config;
            this.relaunchReason = relaunchReason;
            this.controller = controller;
        }

        /// <summary>
        /// Executes next action as defined in the config.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        /// <param name="wrapAround">
        /// If true, when the last action is reached, it will execute
        /// the first again (and then continue with the next, ...).
        /// If false, when the last action is reached, it will execute every time the last.
        /// </param>
        public void ExecuteNextAction(string reason, bool wrapAround)
        {
            LimitActionConfigBase action;
            lock (this)
            {
                if (this.config.Actions.Count == 0)
                {
                    return;
                }

                if (this.actionIndex >= this.config.Actions.Count)
                {
                    this.actionIndex = wrapAround ? 0 : this.config.Actions.Count - 1;
                }

                action = this.config.Actions[this.actionIndex];
                this.actionIndex++;
            }

            this.PerformAction(action, reason);
        }

        private void PerformAction(LimitActionConfigBase action, string reason)
        {
            var relaunch = action as RelaunchLimitActionConfig;
            if (relaunch != null)
            {
                this.Relaunch(relaunch, reason);
                return;
            }

            var purge = action as PurgeLimitActionConfig;
            if (purge != null)
            {
                this.Purge(purge, reason);
                return;
            }

            var reboot = action as RebootLimitActionConfig;
            if (reboot != null)
            {
                this.Reboot(reboot, reason);
                return;
            }

            throw new ArgumentException("Don't know what to do with " + action.GetType().Name);
        }

        private void Relaunch(RelaunchLimitActionConfig relaunch, string reason)
        {
            string name;
            if (!string.IsNullOrEmpty(relaunch.Application))
            {
                name = relaunch.Application;
            }
            else if (this.controller != null)
            {
                name = this.controller.Name;
            }
            else
            {
                Logger.Warn("Couldn't perform Relaunch action because application was not defined");
                return;
            }

            var relaunchCtrl = ServiceLocator.Current.GetInstance<ApplicationManager>().GetController(name);
            if (relaunchCtrl != null)
            {
                Logger.Debug("Performing relaunch of {0}", name);
                relaunchCtrl.RequestRelaunch(this.relaunchReason, reason);
            }
            else
            {
                Logger.Warn("Couldn't perform Relaunch action because application was not found: {0}", name);
            }
        }

        private void Purge(PurgeLimitActionConfig purge, string reason)
        {
            try
            {
                var dir = Path.GetDirectoryName(purge.Path);
                var filePattern = Path.GetFileName(purge.Path);

                if (dir == null || !Directory.Exists(dir))
                {
                    Logger.Warn("Couldn't find directory for {0}", purge.Path);
                    return;
                }

                var files = new List<string>();
                if (string.IsNullOrEmpty(filePattern))
                {
                    this.AddAllFiles(dir, files);
                }
                else if (filePattern.IndexOf('*') >= 0)
                {
                    files.AddRange(Directory.GetFiles(dir, filePattern));
                }
                else
                {
                    var path = Path.Combine(dir, filePattern);
                    if (Directory.Exists(path))
                    {
                        this.AddAllFiles(path, files);
                    }
                    else if (File.Exists(path))
                    {
                        files.Add(path);
                    }
                }

                Logger.Debug("Deleting {0} files from {1} ({2})", files.Count, purge.Path, reason);
                foreach (var file in files)
                {
                    Logger.Trace("Deleting {0}", file);
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, "Couldn't delete File {0}", file);
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't purge {0}", purge.Path);
            }
        }

        private void AddAllFiles(string dir, List<string> files)
        {
            files.AddRange(Directory.GetFiles(dir));
            foreach (var subDir in Directory.GetDirectories(dir))
            {
                this.AddAllFiles(subDir, files);
            }
        }

        private void Reboot(RebootLimitActionConfig reboot, string reason)
        {
            ServiceLocator.Current.GetInstance<SystemManagementControllerBase>().RequestReboot(reason);
        }
    }
}