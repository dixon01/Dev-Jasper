// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandsRunner.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CommandsRunner type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Agent
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;
    using Gorba.Motion.Update.Core.Utility;

    using NLog;

    /// <summary>
    /// Class that runs all commands defined in a <see cref="RunCommands"/> object.
    /// </summary>
    public class CommandsRunner
    {
        private const string TempDirectoryPath = @"RunCommands\";

        private static readonly Logger Logger = LogHelper.GetLogger<CommandsRunner>();

        private readonly RunCommands commands;

        private readonly Dictionary<ResourceId, ResourceInfo> resources = new Dictionary<ResourceId, ResourceInfo>();

        private readonly string tempDirectory;

        private readonly IResourceService resourceService;

        private readonly IWritableFileSystem fileSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandsRunner"/> class.
        /// </summary>
        /// <param name="commands">
        /// The pre or post installation commands to run.
        /// </param>
        public CommandsRunner(RunCommands commands)
        {
            this.commands = commands;
            this.tempDirectory = PathManager.Instance.CreatePath(FileType.Data, TempDirectoryPath);
            this.resourceService = MessageDispatcher.Instance.GetService<IResourceService>();
            this.fileSystem = (IWritableFileSystem)FileSystemManager.Local;
        }

        /// <summary>
        /// Event that is fired when the <see cref="RunCommands"/> object provided in the
        /// constructor has changed (i.e. an item was removed from the tree).
        /// This happens whenever a command was successfully run and thus the item is
        /// removed from the tree.
        /// </summary>
        public event EventHandler CommandsUpdated;

        /// <summary>
        /// Runs all commands using the given list of resources.
        /// </summary>
        /// <param name="resourceList">
        /// The resource list.
        /// </param>
        public void Run(IList<ResourceInfo> resourceList)
        {
            var fileUtility = new FileUtility(this.fileSystem);
            this.resources.Clear();
            foreach (var resource in resourceList)
            {
                this.resources[resource.Id] = resource;
            }

            fileUtility.DeleteDirectory(this.tempDirectory);

            Logger.Debug("Copying resources");
            this.CopyResources(this.commands.Items, this.tempDirectory);

            Logger.Debug("Executing commands");
            this.ExecuteCommands(this.commands.Items, this.tempDirectory);

            fileUtility.DeleteDirectory(this.tempDirectory);
        }

        /// <summary>
        /// Raises the <see cref="CommandsUpdated"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseCommandsUpdated(EventArgs e)
        {
            var handler = this.CommandsUpdated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void CopyResources(IEnumerable<FileSystemUpdate> items, string path)
        {
            this.fileSystem.CreateDirectory(path);

            Logger.Trace("Copying resources to '{0}'", path);
            foreach (var item in items)
            {
                if (item is RunApplication)
                {
                    continue;
                }

                var subPath = Path.Combine(path, item.Name);

                var folder = item as FolderUpdate;
                if (folder != null)
                {
                    this.CopyResources(folder.Items, subPath);
                    continue;
                }

                var file = item as FileUpdate; // this also includes ExecutableFile
                if (file != null)
                {
                    Logger.Trace("Copying resource {0} to '{1}'", file.Hash, subPath);
                    var resource = this.resources[new ResourceId(file.Hash)];
                    this.resourceService.ExportResource(resource, subPath);
                }
            }
        }

        private void ExecuteCommands(List<FileSystemUpdate> items, string path)
        {
            Logger.Trace("Executing commands from '{0}'", path);
            foreach (var item in items.ToArray())
            {
                var run = item as RunApplication;
                if (run != null)
                {
                    this.RunProcess(run.Name, run.Arguments, path);
                    items.Remove(run);
                    this.RaiseCommandsUpdated(EventArgs.Empty);
                    continue;
                }

                var subPath = Path.Combine(path, item.Name);

                var folder = item as FolderUpdate;
                if (folder != null)
                {
                    this.ExecuteCommands(folder.Items, subPath);
                    continue;
                }

                var executable = item as ExecutableFile;
                if (executable != null)
                {
                    this.RunProcess(subPath, executable.Arguments, path);
                    items.Remove(executable);
                    this.RaiseCommandsUpdated(EventArgs.Empty);
                }
            }
        }

        private void RunProcess(string path, string args, string workingDirectory)
        {
            Logger.Info("Running \"'{0}' {1}\" in \"{2}\"", path, args, workingDirectory);
            var startInfo = new ProcessStartInfo(path, args ?? string.Empty) { WorkingDirectory = workingDirectory };
            var process = new Process { StartInfo = startInfo };
            process.Start();
            process.WaitForExit();

            Logger.Debug("Process '{0}' has exited with {1}", path, process.ExitCode);
        }
    }
}
