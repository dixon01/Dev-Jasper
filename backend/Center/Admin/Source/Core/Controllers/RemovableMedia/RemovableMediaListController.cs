// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemovableMediaListController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RemovableMediaListController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.RemovableMedia
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.ViewModels;
    using Gorba.Center.Admin.Core.ViewModels.Stages;
    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Common.Update.Usb;

    using NLog;

    /// <summary>
    /// The controller that takes care of the insertion and removal of removable media (USB sticks).
    /// This controller creates <see cref="RemovableMediaController"/> objects to handle the single sticks.
    /// </summary>
    public class RemovableMediaListController : SynchronizableControllerBase, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IAdminShell shell;

        private readonly ICommandRegistry commandRegistry;

        private readonly UsbStickDetector detector;

        private readonly List<RemovableMediaController> controllers = new List<RemovableMediaController>();

        private IConnectionController connectionController;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemovableMediaListController"/> class.
        /// </summary>
        /// <param name="shell">
        /// The shell.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public RemovableMediaListController(IAdminShell shell, ICommandRegistry commandRegistry)
        {
            this.shell = shell;
            this.commandRegistry = commandRegistry;

            this.RegisterCommands();

            this.detector = new UsbStickDetector();
            this.detector.Inserted += this.DetectorOnChanged;
            this.detector.Removed += this.DetectorOnChanged;
        }

        /// <summary>
        /// Initializes this controller with the given <see cref="ConnectionController"/>.
        /// </summary>
        /// <param name="connController">
        /// The connection controller.
        /// </param>
        public void Initialize(IConnectionController connController)
        {
            this.connectionController = connController;

            Task.Run(() => this.UpdateDriveList());

            this.detector.Start();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.detector.Stop();
            foreach (var controller in this.controllers.ToArray())
            {
                controller.Dispose();
            }

            this.controllers.Clear();
        }

        private void RegisterCommands()
        {
            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.RemovableMedia.ImportFeedback,
                new RelayCommand<RemovableMediaStageViewModel>(this.ImportFeedback, this.CanImportFeedback));
            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.RemovableMedia.ExportUpdates,
                new RelayCommand<RemovableMediaStageViewModel>(this.ExportUpdates, this.CanExportUpdates));
            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.RemovableMedia.CancelOperation,
                new RelayCommand<RemovableMediaStageViewModel>(this.CancelOperation, this.CanCancelOperation));
        }

        private bool CanImportFeedback(RemovableMediaStageViewModel stage)
        {
            return stage != null && stage.HasFeedback;
        }

        private void ImportFeedback(RemovableMediaStageViewModel stage)
        {
            var controller = this.GetController(stage);
            if (controller == null)
            {
                return;
            }

            controller.ImportFeedback();
        }

        private bool CanExportUpdates(RemovableMediaStageViewModel stage)
        {
            return stage != null && stage.HasSelectedExportUnits;
        }

        private void ExportUpdates(RemovableMediaStageViewModel stage)
        {
            var controller = this.GetController(stage);
            if (controller == null)
            {
                return;
            }

            controller.ExportUpdates();
        }

        private bool CanCancelOperation(RemovableMediaStageViewModel stage)
        {
            var controller = this.GetController(stage);
            return controller != null && controller.CanCancelOperation;
        }

        private void CancelOperation(RemovableMediaStageViewModel stage)
        {
            var controller = this.GetController(stage);
            if (controller == null)
            {
                return;
            }

            controller.CancelOperation();
        }

        private RemovableMediaController GetController(RemovableMediaStageViewModel stage)
        {
            return stage == null ? null : this.controllers.FirstOrDefault(c => c.Stage == stage);
        }

        private void LoadDrives(IEnumerable<DriveInfo> drives)
        {
            var found = new List<RemovableMediaController>();
            foreach (var drive in drives.ToList())
            {
                var controller =
                    this.controllers.FirstOrDefault(
                        c => c.Drive.RootDirectory.FullName.Equals(drive.RootDirectory.FullName));
                if (controller == null)
                {
                    controller = new RemovableMediaController(
                        drive,
                        this.shell,
                        this.connectionController,
                        this.commandRegistry);
                    this.controllers.Add(controller);
                }

                found.Add(controller);
            }

            for (var i = this.controllers.Count - 1; i >= 0; i--)
            {
                var controller = this.controllers[i];
                if (found.Remove(controller))
                {
                    continue;
                }

                this.controllers.Remove(controller);
                controller.Dispose();
            }

            this.shell.Navigator.HasRemovableMedia = this.shell.Navigator.RemovableMedia.Any();
        }

        private void DetectorOnChanged(object s, EventArgs e)
        {
            this.UpdateDriveList();
        }

        private void UpdateDriveList()
        {
            var drives =
                DriveInfo.GetDrives()
                    .Where(d => d.DriveType == DriveType.Removable && d.IsReady);
            this.StartNew(() => this.LoadDrives(drives));
        }
    }
}
