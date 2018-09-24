// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemovableMediaController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RemovableMediaController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.RemovableMedia
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels;
    using Gorba.Center.Admin.Core.ViewModels.Navigator;
    using Gorba.Center.Admin.Core.ViewModels.RemovableMedia;
    using Gorba.Center.Admin.Core.ViewModels.Stages;
    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel.Filters.Units;
    using Gorba.Center.Common.ServiceModel.Filters.Update;
    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Center.Common.Wpf.Client.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Update.Providers;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Repository;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Update.ServiceModel.Utility;
    using Gorba.Common.Update.Usb;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    using UpdateCommandMsg = Gorba.Common.Update.ServiceModel.Messages.UpdateCommand;

    /// <summary>
    /// The controller for a single removable media (USB stick).
    /// </summary>
    public class RemovableMediaController : SynchronizableControllerBase, IUpdateContext, IDisposable
    {
        private const double DownloadPercentage = 0.5;
        private const string UpdateFolderRoot = @"Gorba\Update";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IAdminShell shell;

        private readonly IConnectionController connectionController;

        private readonly RemovableMediaViewModel navigation;

        private readonly string repositoryBasePath;

        private readonly IAdminApplicationState applicationState;

        private readonly ServerResourceProvider resourceProvider;

        private ProgressMonitor currentProgressMonitor;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemovableMediaController"/> class.
        /// </summary>
        /// <param name="drive">
        /// The drive.
        /// </param>
        /// <param name="shell">
        /// The application shell.
        /// </param>
        /// <param name="connectionController">
        /// The connection controller.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public RemovableMediaController(
            DriveInfo drive,
            IAdminShell shell,
            IConnectionController connectionController,
            ICommandRegistry commandRegistry)
        {
            this.shell = shell;
            this.connectionController = connectionController;
            this.Drive = drive;

            this.repositoryBasePath = Path.Combine(drive.RootDirectory.FullName, UpdateFolderRoot);

            this.applicationState = ServiceLocator.Current.GetInstance<IAdminApplicationState>();
            this.applicationState.PropertyChanged += this.ApplicationStateOnPropertyChanged;

            this.resourceProvider = new ServerResourceProvider(this.connectionController);

            this.Name = string.Format("{0} ({1})", drive.VolumeLabel, drive.Name.TrimEnd('\\'));

            this.navigation = new RemovableMediaViewModel(this.Name);
            this.Stage = new RemovableMediaStageViewModel(this.Name, commandRegistry);

            this.shell.Navigator.RemovableMedia.Add(this.navigation);
            this.shell.RemovableMediaStages.Add(this.Stage);

            Task.Run(() => this.InitializeAsync());
        }

        /// <summary>
        /// Gets the name of the drive this controller is responsible for.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the stage managed by this controller.
        /// </summary>
        public RemovableMediaStageViewModel Stage { get; private set; }

        /// <summary>
        /// Gets a value indicating whether there is currently any operation running that can be cancelled.
        /// </summary>
        public bool CanCancelOperation
        {
            get
            {
                return this.currentProgressMonitor != null;
            }
        }

        /// <summary>
        /// Gets the drive.
        /// </summary>
        public DriveInfo Drive { get; private set; }

        IResourceProvider IUpdateContext.ResourceProvider
        {
            get
            {
                return this.resourceProvider;
            }
        }

        string IUpdateContext.TemporaryDirectory
        {
            get
            {
                return Path.GetTempPath();
            }
        }

        IEnumerable<IUpdateSink> IUpdateContext.Sinks
        {
            get
            {
                return new IUpdateSink[0];
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (this.connectionController.IsConfigured)
            {
                this.connectionController.UpdateGroupChangeTrackingManager.Added -= this.UpdateGroupsOnAdded;
                this.connectionController.UpdateGroupChangeTrackingManager.Removed -= this.UpdateGroupsOnRemoved;
            }

            this.applicationState.PropertyChanged -= this.ApplicationStateOnPropertyChanged;

            this.shell.Navigator.RemovableMedia.Remove(this.navigation);
            this.shell.RemovableMediaStages.Remove(this.Stage);

            if (this.shell.CurrentStage == this.Stage)
            {
                // select the home stage if the currently selected stick is removed
                this.shell.CurrentStage = this.shell.HomeStage;
            }
        }

        /// <summary>
        /// Imports feedback from the removable media.
        /// </summary>
        public void ImportFeedback()
        {
            Task.Run(() => this.DoImportFeedback());
        }

        /// <summary>
        /// Exports updates to the removable media.
        /// </summary>
        public void ExportUpdates()
        {
            Task.Run(() => this.DoExportUpdates());
        }

        /// <summary>
        /// Cancels an ongoing operation.
        /// </summary>
        public void CancelOperation()
        {
            var progressMonitor = this.currentProgressMonitor;
            if (progressMonitor != null)
            {
                progressMonitor.Cancel();
            }
        }

        IProgressMonitor IUpdateContext.CreateProgressMonitor(UpdateStage stage, bool showVisualization)
        {
            throw new NotSupportedException();
        }

        private async void InitializeAsync()
        {
            if (this.applicationState.CurrentTenant != null)
            {
                try
                {
                    this.connectionController.UpdateGroupChangeTrackingManager.Added -= this.UpdateGroupsOnAdded;
                    this.connectionController.UpdateGroupChangeTrackingManager.Removed -= this.UpdateGroupsOnRemoved;

                    var updateGroups =
                        await
                        this.connectionController.UpdateGroupChangeTrackingManager.QueryAsync(
                            UpdateGroupQuery.Create().WithTenant(this.applicationState.CurrentTenant.ToDto()));

                    foreach (var updateGroup in updateGroups)
                    {
                        var @group = updateGroup;
                        await @group.LoadNavigationPropertiesAsync();
                        this.StartNew(() => this.Stage.UpdateGroups.Add(new UpdateGroupSelectionViewModel(@group)));
                    }

                    this.connectionController.UpdateGroupChangeTrackingManager.Added += this.UpdateGroupsOnAdded;
                    this.connectionController.UpdateGroupChangeTrackingManager.Removed += this.UpdateGroupsOnRemoved;
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Couldn't get update groups from server");
                }
            }

            RepositoryVersionConfig currentConfig;
            try
            {
                var repositoryConfigPath = Path.Combine(
                    this.repositoryBasePath, RepositoryConfig.RepositoryXmlFileName);
                if (!File.Exists(repositoryConfigPath))
                {
                    return;
                }

                var configurator = new Configurator(repositoryConfigPath, RepositoryConfig.Schema);
                var config = configurator.Deserialize<RepositoryConfig>();
                currentConfig = config.GetCurrentConfig();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't get repository config from " + this.repositoryBasePath);
                return;
            }

            var feedbackPath = Path.Combine(this.repositoryBasePath, currentConfig.FeedbackDirectory);
            try
            {
                var feedbackDir = new DirectoryInfo(feedbackPath);
                var files =
                    feedbackDir.GetFiles(
                        "*" + FileDefinitions.LogFileExtension,
                        SearchOption.AllDirectories)
                        .Concat(
                            feedbackDir.GetFiles(
                                "*" + FileDefinitions.UpdateStateInfoExtension,
                                SearchOption.AllDirectories));
                this.SetFeedbackUnits(
                    files.Where(f => f.Directory != null)
                        .Select(f => f.Directory.Name)
                        .Distinct(StringComparer.CurrentCultureIgnoreCase));
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't check for feedback from " + this.repositoryBasePath);
            }
        }

        private async void UpdateGroupsOnAdded(
            object s,
            ReadableModelEventArgs<UpdateGroupReadableModel> e)
        {
            await e.Model.LoadNavigationPropertiesAsync();
            if (e.Model.Tenant.Id == this.applicationState.CurrentTenant.Id)
            {
                this.StartNew(() => this.Stage.UpdateGroups.Add(new UpdateGroupSelectionViewModel(e.Model)));
            }
        }

        private void UpdateGroupsOnRemoved(
            object s,
            ReadableModelEventArgs<UpdateGroupReadableModel> e)
        {
            this.StartNew(
                () =>
                this.Stage.UpdateGroups.Remove(this.Stage.UpdateGroups.FirstOrDefault(g => g.Model.Id == e.Model.Id)));
        }

        private void DoImportFeedback()
        {
            try
            {
                var progressMonitor = new ProgressMonitor(this);
                this.currentProgressMonitor = progressMonitor;

                this.SetProgress(0, AdminStrings.RemovableMedia_Importing);
                var provider = new UsbUpdateProvider();
                var config = new UsbUpdateProviderConfig
                                 {
                                     Name = this.Name,
                                     RepositoryBasePath = this.repositoryBasePath,
                                     ShowVisualization = true
                                 };

                provider.Configure(config, this);
                provider.FeedbackReceived += (sender, e) =>
                    {
                        try
                        {
                            using (
                                var updateService = this.connectionController.CreateChannelScope<IUpdateService>())
                            {
                                if (e.ReceivedUpdateStates != null && e.ReceivedUpdateStates.Length > 0)
                                {
                                    updateService.Channel.AddFeedbacksAsync(e.ReceivedUpdateStates).Wait();
                                }

                                if (e.ReceivedLogFiles != null && e.ReceivedLogFiles.Length > 0)
                                {
                                    for (int i = 0; i < e.ReceivedLogFiles.Length; i++)
                                    {
                                        var logFile = e.ReceivedLogFiles[i];
                                        this.SetProgress((1.0 + i) / (e.ReceivedLogFiles.Length + 1), logFile.FileName);
                                        this.UploadLogFileAsync(logFile, updateService).Wait();
                                    }
                                }
                            }
                        }
                        catch
                        {
                            progressMonitor.Cancel();
                            throw;
                        }
                    };

                provider.CheckForFeedback(progressMonitor);

                this.currentProgressMonitor = null;
                if (!progressMonitor.IsCancelled)
                {
                    this.SetFeedbackUnits(new string[0]);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Couldn't import feedback");
                var prompt = new ConnectionExceptionPrompt(
                    ex,
                    AdminStrings.ServerError_ImportFeedback_Message,
                    AdminStrings.ServerError_ImportFeedback_Title);
                this.StartNew(() => InteractionManager<ConnectionExceptionPrompt>.Current.Raise(prompt));
            }
            finally
            {
                this.SetProgress(double.NaN, null);
            }
        }

        private async Task UploadLogFileAsync(IReceivedLogFile logFile, ChannelScope<IUpdateService> updateService)
        {
            var unitId =
                (await
                 this.connectionController.UnitChangeTrackingManager.QueryAsync(
                     UnitQuery.Create().WithName(logFile.UnitName))).Select(u => u.Id).FirstOrDefault();

            using (var input = logFile.OpenRead())
            {
                var request = new LogFileUploadRequest
                                  {
                                      UnitId = unitId,
                                      Filename = logFile.FileName,
                                      Content = input
                                  };
                await updateService.Channel.UploadLogFileAsync(request);
            }
        }

        private async void DoExportUpdates()
        {
            var selectedUnits =
                this.Stage.UpdateGroups.Where(g => !g.IsChecked.HasValue || g.IsChecked.Value)
                    .SelectMany(g => g.Units.Where(u => u.IsChecked)).Select(u => u.Model).ToList();

            if (selectedUnits.Count == 0)
            {
                return;
            }

            try
            {
                var progressMonitor = new ProgressMonitor(this, DownloadPercentage);
                this.currentProgressMonitor = progressMonitor;

                this.SetProgress(0, AdminStrings.RemovableMedia_Exporting);

                var allCommands = await this.CreateUpdateCommandsAsync(selectedUnits);

                if (allCommands.Count == 0)
                {
                    return;
                }

                var provider = new UsbUpdateProvider();
                var config = new UsbUpdateProviderConfig
                                 {
                                     Name = this.Name,
                                     RepositoryBasePath = this.repositoryBasePath,
                                     ShowVisualization = true
                                 };

                provider.Configure(config, this);

                provider.HandleCommands(allCommands, progressMonitor);

                this.StartNew(() => this.Stage.UpdateGroups.ForEach(g => g.IsChecked = false));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Couldn't export updates");
                var prompt = new ConnectionExceptionPrompt(
                    ex,
                    AdminStrings.ServerError_ExportUpdates_Message,
                    AdminStrings.ServerError_ExportUpdates_Title);
                this.StartNew(() => InteractionManager<ConnectionExceptionPrompt>.Current.Raise(prompt));
            }
            finally
            {
                this.SetProgress(double.NaN, null);
            }
        }

        private async Task<List<UpdateCommandMsg>> CreateUpdateCommandsAsync(IList<UnitReadableModel> selectedUnits)
        {
            var allCommands = new List<UpdateCommandMsg>();
            using (var updateCommands = this.connectionController.CreateChannelScope<IUpdateCommandDataService>())
            {
                for (int i = 0; i < selectedUnits.Count; i++)
                {
                    var unit = selectedUnits[i].ToDto();

                    this.SetProgress(DownloadPercentage * i / selectedUnits.Count, unit.Name);

                    var commands =
                        await
                        updateCommands.Channel.QueryAsync(
                            UpdateCommandQuery.Create().WithUnit(unit).IncludeCommand().OrderByUpdateIndexDescending());
                    var lastActivateTime = DateTime.MaxValue;
                    foreach (var command in commands.TakeWhile(c => !c.WasTransferred))
                    {
                        var commandMsg = (UpdateCommandMsg)command.Command.Deserialize();
                        if (commandMsg.ActivateTime >= lastActivateTime)
                        {
                            break;
                        }

                        lastActivateTime = commandMsg.ActivateTime;
                        allCommands.Add(commandMsg);
                    }
                }
            }

            return allCommands;
        }

        private void SetFeedbackUnits(IEnumerable<string> unitNames)
        {
            this.StartNew(
                () =>
                {
                    this.Stage.FeedbackUnits.Clear();
                    foreach (var unitName in unitNames)
                    {
                        this.Stage.FeedbackUnits.Add(unitName);
                    }

                    this.navigation.HasFeedback = this.Stage.HasFeedback;
                });
        }

        private void SetProgress(double value, string message)
        {
            this.StartNew(
                () =>
                    {
                        if (!double.IsNaN(value))
                        {
                            this.Stage.BusyProgress = (int)(100 * value);
                            this.Stage.BusyMessage = message;
                            this.Stage.IsBusy = true;
                            return;
                        }

                        this.Stage.IsBusy = false;
                        this.Stage.BusyMessage = null;
                        this.Stage.BusyProgress = 0;
                        if (!string.IsNullOrEmpty(message))
                        {
                            MessageBox.Show(
                                message,
                                AdminStrings.ServerError_ExportUpdates_Title,
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        }
                    });
        }

        private void ApplicationStateOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentTenant")
            {
                this.Stage.UpdateGroups.Clear();
                Task.Run(() => this.InitializeAsync());
            }
        }

        private class ProgressMonitor : IProgressMonitor
        {
            private readonly RemovableMediaController controller;

            private readonly double start;
            private readonly double end;

            public ProgressMonitor(RemovableMediaController controller, double start = 0, double end = 1)
            {
                this.controller = controller;
                this.start = start;
                this.end = end;
            }

            public bool IsCancelled { get; private set; }

            public void Start()
            {
            }

            public void Cancel()
            {
                this.IsCancelled = true;
            }

            public void Progress(double value, string note)
            {
                this.controller.SetProgress(this.ToLocalProgress(value), note);
            }

            public IPartProgressMonitor CreatePart(double startValue, double endValue)
            {
                return new PartProgressMonitor(this, this.ToLocalProgress(startValue), this.ToLocalProgress(endValue));
            }

            public void Complete(string errorMessage, string successMessage)
            {
                this.controller.SetProgress(double.NaN, errorMessage);
            }

            private double ToLocalProgress(double value)
            {
                var current = this.start + (value * (this.end - this.start));
                current = Math.Max(this.start, Math.Min(this.end, current));
                return current;
            }
        }
    }
}