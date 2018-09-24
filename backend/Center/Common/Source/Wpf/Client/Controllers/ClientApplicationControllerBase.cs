// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientApplicationControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The client application controller. It contains all methods and controllers to access the BackgroundSystem
//   including the Login.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.Controllers
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Meta;
    using Gorba.Center.Common.ServiceModel.Settings;
    using Gorba.Center.Common.Wpf.Client.Interaction;
    using Gorba.Center.Common.Wpf.Client.ViewModels;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.ViewModels;
    using Gorba.Center.Common.Wpf.Framework.ViewModels.Dialogs;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Core;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// The client application controller. It contains all methods and controllers to access the BackgroundSystem
    /// including the Login.
    /// </summary>
    public abstract class ClientApplicationControllerBase : ApplicationController, IClientApplicationController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private readonly string applicationTitle;

        private readonly DataScope applicationDataScope;

        private LoginViewModel loginViewModel;

        private TenantSelectionViewModel tenantSelectionViewModel;

        private bool hadNoTenants;

        private SystemConfigReadableModel systemConfig;

        private UpdateProgressViewModel updateProgressViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientApplicationControllerBase"/> class.
        /// </summary>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        /// <param name="applicationTitle">
        /// The application title.
        /// </param>
        /// <param name="applicationDataScope">
        /// The data scope that represents this application.
        /// This is used to figure out if the user has the right to use this application for a given tenant.
        /// </param>
        protected ClientApplicationControllerBase(
            ICommandRegistry commandRegistry, string applicationTitle, DataScope applicationDataScope)
        {
            this.CommandRegistry = commandRegistry;
            this.applicationTitle = applicationTitle;
            this.applicationDataScope = applicationDataScope;

            this.ConnectionController = new ConnectionController();
            this.PermissionController = new PermissionController(this.ConnectionController);

            commandRegistry.RegisterCommand(ClientCommandCompositionKeys.Logout, new RelayCommand(this.Logout));
            commandRegistry.RegisterCommand(
                ClientCommandCompositionKeys.ChangePassword,
                new RelayCommand<string>(this.ChangePassword));
        }

        /// <summary>
        /// Gets the command registry.
        /// </summary>
        public ICommandRegistry CommandRegistry { get; private set; }

        /// <summary>
        /// Gets the cancellation token source.
        /// </summary>
        public CancellationTokenSource CancellationTokenSource
        {
            get
            {
                return this.cancellationTokenSource;
            }
        }

        /// <summary>
        /// Gets the connected application state.
        /// </summary>
        public IConnectedApplicationState ConnectedApplicationState
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IConnectedApplicationState>();
            }
        }

        /// <summary>
        /// Gets the login controller.
        /// </summary>
        public ILoginController LoginController { get; private set; }

        /// <summary>
        /// Gets the tenant selection controller.
        /// </summary>
        public ITenantSelectionController TenantSelectionController { get; private set; }

        /// <summary>
        /// Gets the update progress controller.
        /// </summary>
        public IUpdateProgressController UpdateProgressController { get; private set; }

        /// <summary>
        /// Gets or sets the connection controller.
        /// </summary>
        public IConnectionController ConnectionController { get; protected set; }

        /// <summary>
        /// Gets the permission controller.
        /// </summary>
        public IPermissionController PermissionController { get; private set; }

        /// <summary>
        /// Gets the application icon shown on the login and tenant selection dialogs.
        /// </summary>
        protected abstract ImageSource ApplicationIcon { get; }

        /// <summary>
        /// Gets the data scopes that are allowed in this application.
        /// This list should be fixed and never change over the runtime of an application.
        /// It is used to determine which data scopes have an influence on the selectable tenants.
        /// </summary>
        protected abstract DataScope[] AllowedDataScopes { get; }

        /// <summary>
        /// Gets a value indicating whether this application supports offline mode.
        /// </summary>
        protected virtual bool SupportsOfflineMode
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// The logout.
        /// </summary>
        public virtual void Logout()
        {
            this.CloseConnectionController();
            this.ConnectedApplicationState.CurrentUser = null;
            this.ConnectedApplicationState.CurrentTenant = null;
        }

        /// <summary>
        /// Requests the shutdown of this controller.
        /// </summary>
        public override void Shutdown()
        {
            this.CloseConnectionController();
        }

        /// <summary>
        /// Shows the login window.
        /// </summary>
        /// <returns>
        /// <c>true</c>, if the login was successful; <c>false</c> otherwise.
        /// </returns>
        protected async Task<LoadingWindowViewModel> RunLoginAsync()
        {
            this.hadNoTenants = false;

            while (true)
            {
                this.CloseConnectionController();
                this.InitializeLoginController();
                var loadingViewModel = new LoadingWindowViewModel(
                    new LoadingWindowFactory(),
                    this.ApplicationIcon,
                    Strings.LoadingWindow_BusyContent);
                var result = this.LoginController.Run();
                var shouldUpdate = result as ShouldUpdateDialogResult;
                if (shouldUpdate != null)
                {
                    this.RunUpdate(shouldUpdate.Password);
                    return null;
                }

                if (!(result is SuccessDialogResult))
                {
                    if (this.cancellationTokenSource.IsCancellationRequested)
                    {
                        this.RaiseShutdownCompleted();
                        return null;
                    }

                    this.Shutdown();
                    this.RaiseShutdownCompleted();
                    return null;
                }

                if (this.ConnectedApplicationState.IsOfflineMode)
                {
                    return loadingViewModel;
                }

                if (this.ConnectedApplicationState.AuthorizedTenants.Count == 0)
                {
                    MessageBox.Show(
                        Strings.Login_NoTenantsFound,
                        Strings.Login_NoTenantsFoundTitle,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    this.hadNoTenants = true;
                    Logger.Debug("No tenant for user.");
                    continue;
                }

                this.hadNoTenants = false;
                if (this.ConnectedApplicationState.AuthorizedTenants.Count == 1)
                {
                    Logger.Debug("Tenant selection skipped. Setting the selected tenant to the owner.");
                    this.ConnectedApplicationState.CurrentTenant =
                        this.ConnectedApplicationState.AuthorizedTenants.First();
                }
                else
                {
                    this.InitializeTenantSelectionController();
                    var dialogResult = this.TenantSelectionController.Run() as TenantSelectionDialogResult;
                    if (dialogResult == null || dialogResult.SelectedTenant == null)
                    {
                        continue;
                    }

                    this.ConnectedApplicationState.CurrentTenant = dialogResult.SelectedTenant;
                }

                loadingViewModel.Show();
                await this.LoadSystemConfigAsync();
                return loadingViewModel;
            }
        }

        private void RunUpdate(string password)
        {
            this.InitializeUpdateProgressController();
            var result = this.UpdateProgressController.Run();

            if (this.cancellationTokenSource.IsCancellationRequested)
            {
                this.RaiseShutdownCompleted();
                return;
            }

            if (result is SuccessDialogResult)
            {
                var encrypted = SecurityUtility.Encrypt(
                    password,
                    Environment.MachineName + this.loginViewModel.Username);
                this.ConnectedApplicationState.UserName = this.loginViewModel.Username;
                this.ConnectedApplicationState.LastServer = this.loginViewModel.InputServer;
                this.ConnectedApplicationState.Authentication = encrypted;
                System.Windows.Forms.Application.Restart();
            }

            this.Shutdown();
            this.RaiseShutdownCompleted();
        }

        private async Task LoadSystemConfigAsync()
        {
            this.systemConfig =
                (await this.ConnectionController.SystemConfigChangeTrackingManager.QueryAsync()).Single();
            this.systemConfig.PropertyChanged += this.SystemConfigOnPropertyChanged;
            await this.systemConfig.LoadXmlPropertiesAsync();
            var settings = (BackgroundSystemSettings)this.systemConfig.Settings.Deserialize();
            this.ConnectedApplicationState.MaintenanceMode = settings.MaintenanceMode;
        }

        private async void SystemConfigOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                var wasEnabled = this.ConnectedApplicationState.MaintenanceMode.IsEnabled;

                await this.systemConfig.LoadXmlPropertiesAsync();
                var settings = (BackgroundSystemSettings)this.systemConfig.Settings.Deserialize();
                this.ConnectedApplicationState.MaintenanceMode = settings.MaintenanceMode;

                if (settings.MaintenanceMode.IsEnabled == wasEnabled)
                {
                    return;
                }

                if (settings.MaintenanceMode.IsEnabled
                    && this.ConnectedApplicationState.CurrentUser != null
                    && !this.ConnectedApplicationState.CurrentUser.Username.Equals(CommonNames.AdminUsername))
                {
                    MessageBox.Show(
                        string.Format(Strings.MaintenanceModeActivated_MessageFormat, settings.MaintenanceMode.Reason),
                        Strings.MaintenanceModeActivated_Title,
                        MessageBoxButton.OK,
                        MessageBoxImage.Exclamation);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Could not notify user about maintenance mode change.");
            }
        }

        private async void ChangePassword(string newPassword)
        {
            try
            {
                var user = await this.ConnectionController.ChangePasswordAsync(newPassword);
                this.ConnectedApplicationState.CurrentUser = user;
            }
            catch (Exception ex)
            {
                Logger.Error("Couldn't change password", ex);
                var prompt = new ConnectionExceptionPrompt(
                    ex,
                    Strings.ChangePassword_Error_NotChanged,
                    Strings.ChangePassword_Error_NotChanged_Title);
                InteractionManager<ConnectionExceptionPrompt>.Current.Raise(prompt);
            }
        }

        private void InitializeLoginController()
        {
            if (this.LoginController == null)
            {
                this.LoginController = new LoginController(
                    this.CommandRegistry,
                    this.ConnectionController,
                    this.PermissionController,
                    null,
                    this.applicationDataScope,
                    this.AllowedDataScopes);
            }

            if (this.loginViewModel == null)
            {
                var offlineCommand = this.SupportsOfflineMode
                                         ? this.CommandRegistry.GetCommand(ClientCommandCompositionKeys.OfflineMode)
                                         : null;
                this.loginViewModel = new LoginViewModel(
                    new LoginFactory(),
                    this.CommandRegistry.GetCommand(ClientCommandCompositionKeys.Application.Exit),
                    this.CommandRegistry.GetCommand(ClientCommandCompositionKeys.Login),
                    this.ConnectedApplicationState.RecentServers,
                    offlineCommand);
                this.SetupStartupDialogViewModel(this.loginViewModel);

                if (!string.IsNullOrEmpty(this.ConnectedApplicationState.Authentication))
                {
                    try
                    {
                        var password = SecurityUtility.Decrypt(
                            this.ConnectedApplicationState.Authentication,
                            Environment.MachineName + this.ConnectedApplicationState.UserName);
                        this.loginViewModel.Password = password;
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, "Couldn't do automatic re-login");
                    }

                    this.ConnectedApplicationState.Authentication = null;
                }
            }
            else
            {
                this.loginViewModel.IsBusy = false;
                this.loginViewModel.ActivityMessage = new ActivityMessage();
            }

            this.loginViewModel.Username = this.ConnectedApplicationState.UserName;
            this.loginViewModel.InputServer = this.ConnectedApplicationState.LastServer;

            this.LoginController.Dialog = this.loginViewModel;
            this.loginViewModel.HadNoTenants = this.hadNoTenants;
        }

        private void SetupStartupDialogViewModel(IStartupDialogViewModel viewModel)
        {
            viewModel.ApplicationTitle = this.applicationTitle;
            viewModel.WindowTitle = this.applicationTitle;
            viewModel.ApplicationIcon = this.ApplicationIcon;
            viewModel.ApplicationVersion = ApplicationHelper.GetApplicationFileVersion();
        }

        private void InitializeTenantSelectionController()
        {
            if (this.TenantSelectionController == null)
            {
                this.TenantSelectionController = new TenantSelectionController(this.CommandRegistry, null);
            }

            if (this.tenantSelectionViewModel == null)
            {
                this.tenantSelectionViewModel = new TenantSelectionViewModel(
                    new TenantSelectionFactory(),
                    this.CommandRegistry)
                {
                    SelectedTenant = this.ConnectedApplicationState.CurrentTenant
                };
                this.SetupStartupDialogViewModel(this.tenantSelectionViewModel);
            }
            else
            {
                this.tenantSelectionViewModel.IsBusy = false;
            }

            this.TenantSelectionController.Dialog = this.tenantSelectionViewModel;
        }

        private void InitializeUpdateProgressController()
        {
            if (this.UpdateProgressController == null)
            {
                this.UpdateProgressController = new UpdateProgressController(this.CommandRegistry, null);
            }

            if (this.updateProgressViewModel == null)
            {
                this.updateProgressViewModel = new UpdateProgressViewModel(
                    new UpdateProgressFactory(),
                    this.CommandRegistry);
                this.SetupStartupDialogViewModel(this.updateProgressViewModel);
            }
            else
            {
                this.updateProgressViewModel.ProgressValue = 0;
            }

            this.UpdateProgressController.Dialog = this.updateProgressViewModel;
        }

        private void CloseConnectionController()
        {
            var controller = this.ConnectionController as ConnectionController;
            if (controller != null)
            {
                controller.Close();
            }
        }
    }
}
