// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiagShellController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DiagShellController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media.Imaging;

    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
    using Gorba.Center.Common.ServiceModel.Filters.Units;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Core.Collections;
    using Gorba.Center.Common.Wpf.Framework;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.ViewModels;
    using Gorba.Center.Common.Wpf.Framework.ViewModels.Options;
    using Gorba.Center.Diag.Core.Communication;
    using Gorba.Center.Diag.Core.Controllers.App;
    using Gorba.Center.Diag.Core.Controllers.Unit;
    using Gorba.Center.Diag.Core.Interaction;
    using Gorba.Center.Diag.Core.Models;
    using Gorba.Center.Diag.Core.Resources;
    using Gorba.Center.Diag.Core.ViewModels;
    using Gorba.Center.Diag.Core.ViewModels.App;
    using Gorba.Center.Diag.Core.ViewModels.FileSystem;
    using Gorba.Center.Diag.Core.ViewModels.MediTree;
    using Gorba.Center.Diag.Core.ViewModels.Unit;
    using Gorba.Common.Medi.Core.Management.Remote;
    using Gorba.Common.Protocols.Udcp;
    using Gorba.Common.Protocols.Udcp.Fields;
    using Gorba.Common.SystemManagement.ServiceModel;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Files;

    using NLog;

    using Version = System.Version;

    /// <summary>
    /// The shell controller for icenter.diag.
    /// </summary>
    [Export(typeof(IDiagShellController)), PartCreationPolicy(CreationPolicy.Shared)]
    public class DiagShellController : WindowControllerBase, IDiagShellController, IWeakEventListener
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ICommandRegistry commandRegistry;

        private readonly TaskFactory taskFactory;

        private readonly UdcpManager udcpManager = new UdcpManager();

        private readonly List<IUnitController> unitControllers = new List<IUnitController>();

        private AboutScreenPrompt aboutScreenPrompt;

        private IUnitChangeTrackingManager unitChangeTrackingManager;

        private OptionsController optionsController;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagShellController"/> class.
        /// </summary>
        /// <param name="shell">The shell.</param>
        /// <param name="commandRegistry">the command registry</param>
        [ImportingConstructor]
        public DiagShellController(IDiagShell shell, ICommandRegistry commandRegistry)
            : base(shell)
        {
            this.commandRegistry = commandRegistry;

            this.taskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());

            PropertyChangedEventManager.AddListener(shell, this, "ActiveStage");
            PropertyChangedEventManager.AddListener(shell, this, "IsAutoRefresh");

            this.RegisterCommands();

            this.Shell.Closing += this.ShellOnClosing;
            this.Shell.Closed += this.ShellOnClosed;
            this.Shell.Created += this.ShellOnCreated;

            this.Shell.AllUnits.CollectionChanged += this.AllUnitsOnCollectionChanged;
            this.Shell.AllUnits.ItemPropertyChanged += this.AllUnitsOnItemPropertyChanged;

            this.udcpManager.GetInformationResponseReceived += this.UdcpManagerOnGetInformationResponseReceived;
            this.optionsController = new OptionsController(this.commandRegistry, "Diag", "DiagApplication");
        }

        IShellViewModel IShellController.Shell
        {
            get
            {
                return this.Shell;
            }
        }

        /// <summary>
        /// Gets the shell.
        /// </summary>
        /// <value>
        /// The shell.
        /// </value>
        public IDiagShell Shell
        {
            get
            {
                return this.Window as IDiagShell;
            }
        }

        /// <summary>
        /// Gets the options controller.
        /// </summary>
        public OptionsController OptionsController
        {
            get
            {
                return this.optionsController;
            }
        }

        /// <summary>
        /// Gets the parent controller.
        /// </summary>
        [Import]
        public IDiagApplicationController ApplicationController { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the application is in offline mode.
        /// </summary>
        public bool IsOfflineMode
        {
            get
            {
                return this.Shell.DiagApplicationState.IsOfflineMode;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current user can interact with a Unit.
        /// </summary>
        public bool UserCanInteract { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.DisposeControllers();
        }

        /// <summary>
        /// Receives events from the centralized event manager.
        /// </summary>
        /// <param name="managerType">
        /// The type of the <see cref="T:System.Windows.WeakEventManager"/> calling this method.</param>
        /// <param name="sender">Object that originated the event.</param>
        /// <param name="e">Event data.</param>
        /// <returns>
        /// true if the listener handled the event.
        /// It is considered an error by the <see cref="T:System.Windows.WeakEventManager"/> handling in WPF
        /// to register a listener for an event that the listener does not handle.
        /// Regardless, the method should return false if it receives an event that it does not recognize or handle.
        /// </returns>
        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            var args = (PropertyChangedEventArgs)e;
            return this.OnPropertyChanged(args);
        }

        private void RegisterCommands()
        {
            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.UI.ShowFileMenu,
                new RelayCommand(this.ShowFileMenu));
            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.UI.ShowUnitMenu,
                new RelayCommand(this.ShowUnitMenu));
            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.UI.ShowApplicationMenu,
                new RelayCommand(this.ShowApplicationMenu));
            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.UI.ShowViewMenu,
                new RelayCommand(this.ShowViewMenu));
            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.UI.Reset, new RelayCommand(this.ResetView));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Unit.RefreshAllUnits, new RelayCommand(this.RefreshAllUnits));
            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Unit.RequestAdd, new RelayCommand(this.RequestAdd, this.CanRequestAdd));
            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Unit.Announce,
                new RelayCommand<UnitViewModelBase>(this.AnnounceUnit, this.CanAnnounceUnit));
            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Unit.Connect,
                new RelayCommand<UnitViewModelBase>(this.ConnectUnit, this.CanConnectUnit));
            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Unit.ToggleConnect,
                new RelayCommand<UnitViewModelBase>(this.ToggleConnectUnit, this.CanToggleConnectUnit));
            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Unit.ToggleFavorite,
                new RelayCommand<UnitViewModelBase>(this.ToggleFavoriteUnit, this.CanToggleFavoriteUnit));
            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Unit.Disconnect,
                new RelayCommand<UnitViewModelBase>(this.DisconnectUnit, this.CanDisconnectUnit));
            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Unit.EditIpSettings,
                new RelayCommand<UnitViewModelBase>(this.EditIpSettingsUnit, this.CanEditIpSettingsUnit));
            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Unit.LoadFileSystemFolder,
                new RelayCommand<FolderViewModel>(this.LoadFileSystemFolder, this.CanLoadFileSystemFolder));
            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Unit.OpenRemoteFile,
                new RelayCommand<FileViewModel>(this.OpenRemoteFile, this.CanOpenRemoteFile));
            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Unit.DownloadRemoteFile,
                new RelayCommand<FileViewModel>(this.DownloadRemoteFile, this.CanDownloadRemoteFile));
            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Unit.CancelRemoteFileDownload,
                new RelayCommand<UnitViewModelBase>(this.CancelRemoteFileDownload, this.CanCancelRemoteFileDownload));
            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Unit.Reboot,
                new RelayCommand<UnitViewModelBase>(this.RebootUnit, this.CanRebootUnit));
            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Unit.Remove,
                new RelayCommand<UnitViewModelBase>(this.RemoveUnit, this.CanRemoveUnit));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.UnitApplication.Launch,
                new RelayCommand<RemoteAppViewModel>(this.LaunchApplicationOnUnit, this.CanLaunchApplicationOnUnit));
            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.UnitApplication.Relaunch,
                new RelayCommand<RemoteAppViewModel>(
                    this.RelaunchApplicationOnUnit,
                    this.CanRelaunchApplicationOnUnit));
            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.UnitApplication.End,
                new RelayCommand<RemoteAppViewModel>(this.EndApplicationOnUnit, this.CanEndApplicationOnUnit));
            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.UnitApplication.LoadMediTreeChildren,
                new RelayCommand<MediTreeNodeViewModel>(this.LoadMediTreeChildren, this.CanLoadMediTreeChildren));
            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.UI.ShowAboutScreen,
                new RelayCommand(this.ShowAboutScreen));
            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.UI.ShowOptionsDialog,
                new RelayCommand(this.ShowOptionsDialog));
        }

        private void ShowOptionsDialog()
        {
            Logger.Debug("Showing options dialog");

            var optionsPrompt = new OptionsPrompt(this.commandRegistry, this.Shell.DiagApplicationState.Options);
            var category = optionsPrompt.Categories.First();
            category.Title = FrameworkStrings.OptionsDialog_GeneralTitle;
            category.TitleTooltip = FrameworkStrings.OptionsDialog_GeneralTooltip;
            var generalIcon = new BitmapImage();
            generalIcon.BeginInit();
            generalIcon.UriSource =
                new Uri(
                    "pack://application:,,,/Gorba.Center.Common.Wpf.Views;component/Icons/application-gear_32x32.png");
            generalIcon.EndInit();
            category.CategoryIconSource = generalIcon;
            var group = category.Groups.First();
            group.Label = FrameworkStrings.OptionsDialog_LanguageLabel;
            group.GroupLabelTooltip = FrameworkStrings.OptionsDialog_LanguageLabelTooltip;
            ((LanguageOptionGroupViewModel)group).RestartInformation =
                FrameworkStrings.OptionsDialog_LanguageRestartInformation;
            optionsPrompt.SelectedCategory = optionsPrompt.Categories.First();
            InteractionManager<OptionsPrompt>.Current.Raise(optionsPrompt);
        }

        private bool CanRequestAdd(object obj)
        {
            return this.IsOfflineMode;
        }

        private void ShowAboutScreen()
        {
            Logger.Debug("Request to show the about screen");
            if (this.aboutScreenPrompt == null)
            {
                var applicationIcon = new BitmapImage();
                applicationIcon.BeginInit();
                applicationIcon.UriSource =
                    new Uri("pack://application:,,,/Gorba.Center.Diag.Core;component/Resources/diag_196x196.png");
                applicationIcon.EndInit();
                this.aboutScreenPrompt = new AboutScreenPrompt { ApplicationIconSource = applicationIcon };
            }

            InteractionManager<AboutScreenPrompt>.Current.Raise(this.aboutScreenPrompt);
        }

        private void ShowFileMenu()
        {
            Logger.Debug("Request to show the main menu.");
            InteractionManager<MainMenuPrompt>.Current.Raise(new MainMenuPrompt(this.Shell, this.commandRegistry));
        }

        private void ShowUnitMenu()
        {
            Logger.Debug("Request to show the unit menu.");
            InteractionManager<UnitMenuPrompt>.Current.Raise(new UnitMenuPrompt(this.Shell, this.commandRegistry));
        }

        private void ShowApplicationMenu()
        {
            Logger.Debug("Request to show the application menu.");
            InteractionManager<ApplicationMenuPrompt>.Current.Raise(
                new ApplicationMenuPrompt(this.Shell, this.commandRegistry));
        }

        private void ShowViewMenu()
        {
            Logger.Debug("Request to show the view menu.");
            InteractionManager<ViewMenuPrompt>.Current.Raise(new ViewMenuPrompt(this.Shell, this.commandRegistry));
        }

        private void ResetView()
        {
            Logger.Debug("Request to Reset the View.");
        }

        private bool CanLaunchApplicationOnUnit(RemoteAppViewModel application)
        {
            if (application == null || application.State == null || !this.UserCanInteract)
            {
                return false;
            }

            switch (application.State.State)
            {
                case ApplicationState.Unknown:
                case ApplicationState.Exited:
                    return true;
                default:
                    return false;
            }
        }

        private void LaunchApplicationOnUnit(RemoteAppViewModel application)
        {
            var controller = this.GetController(application);
            if (controller == null)
            {
                return;
            }

            Logger.Debug(
                "Request to launch Application {0} On Unit {1}",
                application.Name,
                controller.UnitController.ViewModel.DisplayName);
            controller.Relaunch();
        }

        private bool CanRelaunchApplicationOnUnit(RemoteAppViewModel application)
        {
            return this.UserCanInteract && application != null && application.State != null
                   && application.State.State == ApplicationState.Running
                   && application.ApplicationType != ApplicationType.SystemManager; // SM can't be relaunched
        }

        private void RelaunchApplicationOnUnit(RemoteAppViewModel application)
        {
            var controller = this.GetController(application);
            if (controller == null)
            {
                return;
            }

            var message = string.Format(
                DiagStrings.UnitAction_ReallyRelaunchApplicationMessage,
                application.Name,
                application.Unit.DisplayName);
            InteractionManager<UnitActionConfirmationPrompt>.Current.Raise(
                new UnitActionConfirmationPrompt { Message = message },
                prompt =>
                    {
                        if (prompt.Confirmed)
                        {
                            Logger.Debug(
                                "Request to relaunch application {0} on unit {1}",
                                application.Name,
                                application.Unit.DisplayName);
                            controller.Relaunch();
                        }
                    });
        }

        private bool CanEndApplicationOnUnit(RemoteAppViewModel application)
        {
            if (application == null || application.State == null || !this.UserCanInteract)
            {
                return false;
            }

            switch (application.State.State)
            {
                case ApplicationState.AwaitingLaunch:
                case ApplicationState.Launching:
                case ApplicationState.Starting:
                case ApplicationState.Running:
                    return true;
                default:
                    return false;
            }
        }

        private void EndApplicationOnUnit(RemoteAppViewModel application)
        {
            var controller = this.GetController(application);
            if (controller == null)
            {
                return;
            }

            var message = string.Format(
                DiagStrings.UnitAction_ReallyEndApplicationMessage,
                application.Name,
                application.Unit.DisplayName);
            InteractionManager<UnitActionConfirmationPrompt>.Current.Raise(
                new UnitActionConfirmationPrompt { Message = message },
                prompt =>
                    {
                        if (prompt.Confirmed)
                        {
                            Logger.Debug(
                                "Request to end application {0} on unit {1}",
                                application.Name,
                                application.Unit.DisplayName);
                            controller.Exit();
                        }
                    });
        }

        private bool CanLoadMediTreeChildren(MediTreeNodeViewModel node)
        {
            return node != null;
        }

        private void LoadMediTreeChildren(MediTreeNodeViewModel node)
        {
            node.Children.Clear();

            node.IsLoading = true;
            node.Provider.BeginReload(this.ReloadedMediTreeNode, node);
        }

        private void ReloadedMediTreeNode(IAsyncResult ar)
        {
            var node = (MediTreeNodeViewModel)ar.AsyncState;
            List<IRemoteManagementProvider> children = null;
            try
            {
                node.Provider.EndReload(ar);
                children = node.Provider.Children.OfType<IRemoteManagementProvider>().ToList();
            }
            catch (Exception ex)
            {
                Logger.WarnException("Couldn't load children of " + node.Name, ex);
            }

            this.taskFactory.StartNew(
                () =>
                {
                    if (children == null || children.Count == 0)
                    {
                        // ugly, but otherwise Telerik thinks it is still loading
                        node.Children.Add(MediTreeNodeViewModel.Dummy);
                        node.Children.Remove(MediTreeNodeViewModel.Dummy);
                    }
                    else
                    {
                        children.ForEach(c => node.Children.Add(new MediTreeNodeViewModel(c)));
                    }

                    node.Info = NodeInfoViewModelBase.Create(node.Provider);
                    node.IsLoading = false;
                });
        }

        private void RefreshAllUnits()
        {
            this.udcpManager.SendGetInformationRequest();
        }

        private void RequestAdd()
        {
            InteractionManager<AddUnitPromptNotification>.Current.Raise(
                new AddUnitPromptNotification(),
                n =>
                {
                    if (n.Confirmed)
                    {
                        this.AddUnit(n.UnitAddress);
                    }
                });
        }

        private bool CanAnnounceUnit(UnitViewModelBase unit)
        {
            var motion = unit as MotionUnitViewModel;
            return motion != null && motion.HasUdcpInformation && this.UserCanInteract;
        }

        private void AnnounceUnit(UnitViewModelBase unit)
        {
            var motion = unit as MotionUnitViewModel;
            if (motion == null)
            {
                return;
            }

            Logger.Debug("Request to Announce the Unit {0}", unit.DisplayName);
            this.udcpManager.SendAnnounceRequest(motion.UdcpAddress);
        }

        private bool CanToggleFavoriteUnit(UnitViewModelBase unit)
        {
            return unit != null;
        }

        private void ToggleFavoriteUnit(UnitViewModelBase unit)
        {
            Logger.Debug("Request to toggle the Units favorite state {0}", unit.DisplayName);
            unit.IsFavorite = !unit.IsFavorite;
        }

        private void UpdateFavoriteUnits()
        {
            if (this.IsOfflineMode)
            {
                this.Shell.DiagApplicationState.FavoriteUnits =
                    this.Shell.AllUnits.OfType<MotionUnitViewModel>()
                        .Where(u => u.IsFavorite && u.IpAddress != null)
                        .Select(u => new UnitFavorite { Name = u.Name, Address = u.IpAddress.ToString() });
            }
        }

        private bool CanToggleConnectUnit(UnitViewModelBase unit)
        {
            if (unit == null)
            {
                return false;
            }

            switch (unit.ConnectionState)
            {
                case ConnectionState.Disconnected:
                    return this.CanConnectUnit(unit);
                case ConnectionState.Connecting:
                case ConnectionState.Connected:
                    return this.CanDisconnectUnit(unit);
                default:
                    return false;
            }
        }

        private void ToggleConnectUnit(UnitViewModelBase unit)
        {
            switch (unit.ConnectionState)
            {
                case ConnectionState.Disconnected:
                    this.ConnectUnit(unit);
                    break;
                case ConnectionState.Connecting:
                case ConnectionState.Connected:
                    this.DisconnectUnit(unit);
                    break;
            }
        }

        private bool CanConnectUnit(UnitViewModelBase unit)
        {
            return unit != null && unit.ConnectionState == ConnectionState.Disconnected;
        }

        private void ConnectUnit(UnitViewModelBase unit)
        {
            Logger.Debug("Request to Connect to the Unit {0}", unit.DisplayName);
            var controller = this.GetController(unit);
            if (controller == null)
            {
                return;
            }

            controller.Connect();
        }

        private bool CanDisconnectUnit(UnitViewModelBase unit)
        {
            return unit != null && unit.ConnectionState != ConnectionState.Disconnected;
        }

        private void DisconnectUnit(UnitViewModelBase unit)
        {
            Logger.Debug("Request to Disconnect the Unit {0}", unit.DisplayName);
            var controller = this.GetController(unit);
            if (controller == null)
            {
                return;
            }

            controller.Disconnect();
        }

        private bool CanEditIpSettingsUnit(UnitViewModelBase unit)
        {
            var motion = unit as MotionUnitViewModel;
            return motion != null && motion.HasUdcpInformation && this.UserCanInteract;
        }

        private void EditIpSettingsUnit(UnitViewModelBase unit)
        {
            var motion = unit as MotionUnitViewModel;
            if (motion == null)
            {
                return;
            }

            Logger.Debug("Request to Edit IP Settings of the Unit {0}", motion.DisplayName);

            // DHCP setting is only available from 1.5.1508.9178 on
            Version version;
            var dhcpEnabled = ParserUtil.TryParse(motion.SoftwareVersion, out version)
                              && version >= new Version(1, 5, 1508, 9178);

            var notification = new EditIpSettingsPromptNotification
                                   {
                                       Unit = motion,
                                       UseDhcp = motion.DhcpEnabled,
                                       IpAddress = Convert.ToString(motion.IpAddress),
                                       NetworkMask = Convert.ToString(motion.NetworkMask),
                                       GatewayAddress = Convert.ToString(motion.GatewayAddress),
                                       IsDhcpEnabled = dhcpEnabled
                                   };
            InteractionManager<EditIpSettingsPromptNotification>.Current.Raise(
                notification,
                n =>
                {
                    if (n.Confirmed)
                    {
                        this.UpdateIpSettings(motion, notification);
                    }
                });
        }

        private bool CanLoadFileSystemFolder(FolderViewModel folder)
        {
            return folder != null;
        }

        private void LoadFileSystemFolder(FolderViewModel folder)
        {
            folder.Children.Clear();

            folder.IsLoading = true;
            Task.Run(() => this.LoadFileSystemFolderAsync(folder));
        }

        private void LoadFileSystemFolderAsync(FolderViewModel folder)
        {
            IDirectoryInfo[] directories = null;
            IFileInfo[] files = null;
            try
            {
                directories = folder.Directory.GetDirectories();
                files = folder.Directory.GetFiles();
            }
            catch (Exception ex)
            {
                Logger.WarnException("Couldn't load items of " + folder.Path, ex);
            }

            this.taskFactory.StartNew(
                () =>
                    {
                        if (directories == null || directories.Length == 0)
                        {
                            // ugly, but otherwise Telerik thinks it is still loading
                            folder.Children.Add(FolderViewModel.Dummy);
                            folder.Children.Remove(FolderViewModel.Dummy);
                        }
                        else
                        {
                            foreach (var directory in directories)
                            {
                                folder.Children.Add(new FolderViewModel(directory, folder.Unit));
                            }
                        }

                        if (files != null)
                        {
                            foreach (var file in files)
                            {
                                folder.Children.Add(new FileViewModel(file, folder.Unit, this.commandRegistry));
                            }
                        }

                        folder.IsLoading = false;
                    });
        }

        private bool CanOpenRemoteFile(FileViewModel file)
        {
            if (file == null || file.Name == null || file.File.Size == 0)
            {
                return false;
            }

            var controller = this.GetController(file.Unit);
            return controller != null && controller.CanOpenRemoteFile(file);
        }

        private void OpenRemoteFile(FileViewModel file)
        {
            var controller = this.GetController(file.Unit);
            if (controller != null)
            {
                controller.OpenRemoteFile(file);
            }
        }

        private bool CanDownloadRemoteFile(FileViewModel file)
        {
            return file != null && file.Name != null && file.File.Size > 0;
        }

        private void DownloadRemoteFile(FileViewModel file)
        {
            var controller = this.GetController(file.Unit);
            if (controller != null)
            {
                controller.DownloadRemoteFile(file);
            }
        }

        private void CancelRemoteFileDownload(UnitViewModelBase unit)
        {
            var controller = this.GetController(unit);
            if (controller != null)
            {
                controller.CancelRemoteFileDownload();
            }
        }

        private bool CanCancelRemoteFileDownload(UnitViewModelBase unit)
        {
            return unit != null && unit.FileSystemIsDownloading;
        }

        private void UpdateIpSettings(MotionUnitViewModel unit, EditIpSettingsPromptNotification notification)
        {
            IPAddress ipAddress = null;
            IPAddress networkMask = null;
            IPAddress gateway = null;
            if (!notification.UseDhcp)
            {
                if (!IPAddress.TryParse(notification.IpAddress, out ipAddress))
                {
                    Logger.Warn(
                        "Couldn't update IP settings because IP address is invalid: {0}",
                        notification.IpAddress);
                    return;
                }

                if (!IPAddress.TryParse(notification.NetworkMask, out networkMask))
                {
                    Logger.Warn(
                        "Couldn't update IP settings because network mask is invalid: {0}",
                        notification.NetworkMask);
                    return;
                }

                // gateway can be empty
                IPAddress.TryParse(notification.GatewayAddress, out gateway);
            }

            this.udcpManager.SendSetConfigurationRequest(
                unit.UdcpAddress,
                notification.UseDhcp,
                ipAddress,
                networkMask,
                gateway);
        }

        private bool CanRebootUnit(UnitViewModelBase unit)
        {
            var motion = unit as MotionUnitViewModel;
            return motion != null && (motion.HasUdcpInformation || motion.Applications.Any(a => a.State != null))
                   && this.UserCanInteract;
        }

        private void RebootUnit(UnitViewModelBase unit)
        {
            Logger.Debug("Request to reboot the unit {0}", unit.DisplayName);
            var controller = this.GetController(unit) as MotionUnitController;
            if (controller == null)
            {
                return;
            }

            var message = string.Format(DiagStrings.UnitAction_ReallyRebootMessage, unit.DisplayName);
            InteractionManager<UnitActionConfirmationPrompt>.Current.Raise(
                new UnitActionConfirmationPrompt { Message = message },
                prompt =>
                    {
                        if (prompt.Confirmed)
                        {
                            this.DoRebootUnit(unit, controller);
                        }
                    });
        }

        private async void DoRebootUnit(UnitViewModelBase unit, MotionUnitController controller)
        {
            if (controller.ViewModel.HasUdcpInformation)
            {
                this.udcpManager.SendRebootRequest(controller.ViewModel.UdcpAddress);
            }
            else
            {
                controller.RebootUnit();
                await Task.Delay(TimeSpan.FromSeconds(0.5));
            }

            this.DisconnectUnit(unit);
        }

        private bool CanRemoveUnit(UnitViewModelBase unit)
        {
            var motion = unit as MotionUnitViewModel;
            return motion != null && !motion.HasUdcpInformation && this.IsOfflineMode;
        }

        private void RemoveUnit(UnitReadableModel unit)
        {
            var viewModel =
                this.Shell.AllUnits.FirstOrDefault(
                    u => u.Name.Equals(unit.Name, StringComparison.InvariantCultureIgnoreCase));
            if (viewModel != null)
            {
                this.RemoveUnit(viewModel);
            }
        }

        private void RemoveUnit(UnitViewModelBase unit)
        {
            Logger.Debug("Request to remove the Unit {0}", unit.DisplayName);
            if (this.CanDisconnectUnit(unit))
            {
                // disconnect from the unit before we remove it from the list
                this.DisconnectUnit(unit);
            }

            this.Shell.AllUnits.Remove(unit);
            var controller = this.GetController(unit);
            if (controller == null)
            {
                return;
            }

            this.unitControllers.Remove(controller);
            controller.Dispose();
        }

        private void StartAutoRefresh()
        {
            Logger.Debug("Starting auto-refresh");
            try
            {
                this.udcpManager.Start();
            }
            catch (Exception ex)
            {
                Logger.WarnException("Couldn't start UDCP manager", ex);
            }
        }

        private void StopAutoRefresh()
        {
            Logger.Debug("Stopping auto-refresh");
            this.udcpManager.Stop();
        }

        private async void LoadUnitsFromServer()
        {
            try
            {
                this.Shell.IsBusy = true;
                var query = UnitQuery.Create().WithTenant(this.Shell.DiagApplicationState.CurrentTenant.ToDto());
                var units = await this.unitChangeTrackingManager.QueryAsync(query);
                foreach (var unit in units)
                {
                    this.AddUnit(unit);
                }

                Logger.Debug("Units loaded from server");
            }
            catch (Exception ex)
            {
                Logger.WarnException("Couldn't load units from server", ex);
            }

            this.Shell.IsBusy = false;
        }

        private UnitViewModelBase AddUnit(string address)
        {
            Logger.Debug("Request to add the Unit '{0}'", address);
            IPAddress ip;
            if (!IPAddress.TryParse(address, out ip))
            {
                // TODO: should we notify the user that the IP address is not valid?
                Logger.Warn("Couldn't add the Unit '{0}', not a valid IP address", address);
                return null;
            }

            if (this.Shell.AllUnits.OfType<MotionUnitViewModel>().Any(u => ip.Equals(u.IpAddress)))
            {
                // TODO: should we notify the user that the unit was already there?
                Logger.Warn("Couldn't add the Unit '{0}', it aready exists", ip);
                return null;
            }

            var unit = new MotionUnitViewModel(this.Shell) { IpAddress = ip, ConnectionMode = ConnectionMode.Local };
            this.AddUnit(unit);
            return unit;
        }

        private UnitViewModelBase AddUnit(UdcpResponse response)
        {
            var udcpAddress = response.Header.UnitAddress;
            var match = new Predicate<MotionUnitViewModel>(vm => udcpAddress.Equals(vm.UdcpAddress));

            var ipAddressField = response.GetField<IpAddressField>();
            if (ipAddressField != null)
            {
                var origMatch = match;
                match = vm => origMatch(vm) || (vm.UdcpAddress == null && ipAddressField.Value.Equals(vm.IpAddress));
            }

            var unitNameField = response.GetField<UnitNameField>();
            if (unitNameField != null && !this.IsOfflineMode)
            {
                var origMatch = match;
                match =
                    vm =>
                    origMatch(vm) || unitNameField.Value.Equals(vm.Name, StringComparison.InvariantCultureIgnoreCase);
            }

            var controller = this.unitControllers.OfType<MotionUnitController>().FirstOrDefault(
                c => match(c.ViewModel));

            if (controller == null)
            {
                if (!this.IsOfflineMode)
                {
                    Logger.Debug("Not adding unit for UDCP address {0}, not found in list", udcpAddress);
                    return null;
                }

                Logger.Debug("Adding unit for UDCP address {0}", udcpAddress);
                controller = this.AddUnit(new MotionUnitViewModel(this.Shell));
            }

            controller.UpdateFrom(response);
            return controller.ViewModel;
        }

        private void AddUnit(UnitReadableModel unit)
        {
            var controller =
                this.unitControllers.OfType<MotionUnitController>().FirstOrDefault(
                    c => c.ViewModel.Name.Equals(unit.Name, StringComparison.InvariantCultureIgnoreCase));
            if (controller == null)
            {
                var viewModel = new MotionUnitViewModel(this.Shell) { Name = unit.Name };
                controller = this.AddUnit(viewModel);
            }

            controller.UpdateFrom(unit);
        }

        private MotionUnitController AddUnit(MotionUnitViewModel unitViewModel)
        {
            var controller = new MotionUnitController(unitViewModel, this.commandRegistry);
            this.unitControllers.Add(controller);
            this.Shell.AllUnits.Add(unitViewModel);
            return controller;
        }

        private void UpdateDescription(UnitViewModelBase unit)
        {
            if (unit == null || unit.Description != null || string.IsNullOrEmpty(unit.Name))
            {
                return;
            }

            string description;
            var unitDescriptions = this.Shell.DiagApplicationState.UnitDescriptions;
            if (unitDescriptions != null && unitDescriptions.TryGetValue(unit.Name, out description))
            {
                unit.Description = description;
            }
        }

        private RemoteAppController GetController(RemoteAppViewModel application)
        {
            if (application == null)
            {
                return null;
            }

            return
                this.unitControllers.SelectMany(u => u.ApplicationControllers.Where(a => a.ViewModel == application))
                    .FirstOrDefault();
        }

        private IUnitController GetController(UnitViewModelBase unit)
        {
            if (unit == null)
            {
                return null;
            }

            return this.unitControllers.FirstOrDefault(u => u.ViewModel == unit);
        }

        private void DisposeControllers()
        {
            this.udcpManager.Stop();
            if (this.unitChangeTrackingManager == null)
            {
                return;
            }

            this.unitChangeTrackingManager.Added -= this.UnitChangeTrackingManagerOnAdded;
            this.unitChangeTrackingManager.Removed -= this.UnitChangeTrackingManagerOnRemoved;
            this.unitChangeTrackingManager = null;
        }

        private bool OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "IsAutoRefresh")
            {
                if (this.Shell.IsAutoRefresh)
                {
                    this.StartAutoRefresh();
                }
                else
                {
                    this.StopAutoRefresh();
                }
            }

            return true;
        }

        private void ApplicationStateOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsOfflineMode")
            {
                if (this.IsOfflineMode)
                {
                    this.StartAutoRefresh();
                }

                return;
            }

            if (e.PropertyName != "CurrentTenant")
            {
                return;
            }

            this.Shell.IsBusy = true;

            var units = this.Shell.AllUnits.ToList();
            this.Shell.AllUnits.Clear();
            foreach (var unit in units)
            {
                this.RemoveUnit(unit);
            }

            if (this.Shell.DiagApplicationState.CurrentTenant != null && this.unitChangeTrackingManager != null)
            {
                this.LoadUnitsFromServer();
            }
            else
            {
                this.Shell.IsBusy = false;
                Logger.Debug(
                    "Couldn't load units from server. Either CurrentTenant or the UnitChangeTrackingManager is null");
            }
        }

        private void ShellOnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            this.RaiseWindowClosing(cancelEventArgs);
        }

        private void ShellOnClosed(object sender, EventArgs eventArgs)
        {
            this.RaiseWindowClosed();
            this.DisposeControllers();
        }

        private void ShellOnCreated(object sender, EventArgs eventArgs)
        {
            this.Shell.ShowTileView = this.IsOfflineMode;

            if (this.Shell.IsAutoRefresh)
            {
                this.StartAutoRefresh();
            }

            if (!this.IsOfflineMode)
            {
                this.UserCanInteract = this.ApplicationController.PermissionController.HasPermission(
                    Permission.Interact, DataScope.Unit);
                this.unitChangeTrackingManager =
                    this.ApplicationController.ConnectionController.UnitChangeTrackingManager;

                this.LoadUnitsFromServer();

                this.Shell.DiagApplicationState.PropertyChanged += this.ApplicationStateOnPropertyChanged;

                this.unitChangeTrackingManager.Added += this.UnitChangeTrackingManagerOnAdded;
                this.unitChangeTrackingManager.Removed += this.UnitChangeTrackingManagerOnRemoved;

                // don't load favorites if we are in online mode
                return;
            }

            this.UserCanInteract = true;
            foreach (var favorite in this.Shell.DiagApplicationState.FavoriteUnits.Where(u => u.Address != null))
            {
                var unit = this.AddUnit(favorite.Address);
                if (unit == null)
                {
                    continue;
                }

                unit.Name = favorite.Name;
                unit.IsFavorite = true;
            }
        }

        private async void UnitChangeTrackingManagerOnAdded(object sender, ReadableModelEventArgs<UnitReadableModel> e)
        {
            try
            {
                await e.Model.LoadReferencePropertiesAsync();
                if (!e.Model.Tenant.Equals(this.Shell.DiagApplicationState.CurrentTenant))
                {
                    return;
                }

                await this.taskFactory.StartNew(() => this.AddUnit(e.Model));
            }
            catch (Exception ex)
            {
                Logger.WarnException("Couldn't add unit from ChangeTracking", ex);
            }
        }

        private void UnitChangeTrackingManagerOnRemoved(object sender, ReadableModelEventArgs<UnitReadableModel> e)
        {
            this.taskFactory.StartNew(() => this.RemoveUnit(e.Model));
        }

        private void UdcpManagerOnGetInformationResponseReceived(object sender, UdcpDatagramEventArgs<UdcpResponse> e)
        {
            this.taskFactory.StartNew(() => this.AddUnit(e.Datagram));
        }

        private void AllUnitsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (var unit in e.NewItems.OfType<UnitViewModelBase>())
                {
                    this.UpdateDescription(unit);
                }
            }

            this.UpdateFavoriteUnits();
        }

        private void AllUnitsOnItemPropertyChanged(object sender, ItemPropertyChangedEventArgs<UnitViewModelBase> e)
        {
            switch (e.PropertyName)
            {
                case "ConnectionState":
                    if (e.Item.ConnectionState == ConnectionState.Disconnected)
                    {
                        this.Shell.RemoveUnitTab(e.Item);
                    }
                    else
                    {
                        this.Shell.CreateUnitTab(e.Item);
                    }

                    break;
                case "Name":
                    this.UpdateDescription(e.Item as MotionUnitViewModel);
                    break;
                case "Description":
                    if (!string.IsNullOrEmpty(e.Item.Name))
                    {
                        this.Shell.DiagApplicationState.UnitDescriptions[e.Item.Name] = e.Item.Description;
                    }

                    break;
                case "IsFavorite":
                    this.UpdateFavoriteUnits();
                    break;
            }
        }
    }
}