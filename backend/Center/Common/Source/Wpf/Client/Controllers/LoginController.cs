// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The login controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Deployment.Application;
    using System.Diagnostics;
    using System.Net.Sockets;
    using System.ServiceModel;
    using System.ServiceModel.Security;
    using System.Threading.Tasks;
    using System.Windows;

    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.Faults;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.ServiceModel.Security;
    using Gorba.Center.Common.Wpf.Client.ViewModels;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Startup;
    using Gorba.Center.Common.Wpf.Framework.ViewModels;
    using Gorba.Center.Common.Wpf.Framework.ViewModels.Dialogs;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// The login controller.
    /// </summary>
    internal class LoginController : DialogControllerBase, ILoginController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IConnectionController connectionController;

        private readonly IPermissionController permissionController;

        private readonly DataScope applicationDataScope;

        private readonly IList<DataScope> allowedDataScopes;

        private User selectedUser;

        private DialogResultBase dialogResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginController"/> class.
        /// </summary>
        /// <param name="commandRegistry">
        /// The commandRegistry of the application.
        /// </param>
        /// <param name="connectionController">
        /// The connection Controller.
        /// </param>
        /// <param name="permissionController">
        /// The permission controller.
        /// </param>
        /// <param name="window">
        /// The window.
        /// </param>
        /// <param name="applicationDataScope">
        /// The data scope that represents this application.
        /// This is used to figure out if the user has the right to use this application for a given tenant.
        /// </param>
        /// <param name="allowedDataScopes">
        /// Only permissions with allowed data scope are used, never null.
        /// </param>
        public LoginController(
            ICommandRegistry commandRegistry,
            IConnectionController connectionController,
            IPermissionController permissionController,
            ILoginViewModel window,
            DataScope applicationDataScope,
            IList<DataScope> allowedDataScopes)
            : base(window)
        {
            this.connectionController = connectionController;
            this.permissionController = permissionController;
            this.applicationDataScope = applicationDataScope;
            this.allowedDataScopes = allowedDataScopes;

            commandRegistry.RegisterCommand(
                ClientCommandCompositionKeys.Login,
                new RelayCommand(this.DoLogin, this.CanDoLogin));

            commandRegistry.RegisterCommand(
                ClientCommandCompositionKeys.OfflineMode,
                new RelayCommand(this.OpenOfflineMode));
        }

        /// <summary>
        /// Gets the login.
        /// </summary>
        public ILoginViewModel Login
        {
            get
            {
                return this.Dialog as ILoginViewModel;
            }
        }

        /// <summary>
        /// Shows the dialog and returns only when the newly opened dialog is closed.
        /// </summary>
        /// <returns>
        /// The result of the dialog.
        /// </returns>
        public override DialogResultBase Run()
        {
            this.Login.IsBusy = false;
            this.SetInitialActivityMessage();

            this.dialogResult = new EmptyDialogResult();

            if (this.CanDoLogin(null))
            {
                // automatic re-login if all is available
                Logger.Info("Logging in with known password");
                this.DoLogin();
            }

            this.Login.Closing += this.LoginOnClosing;

            this.Login.SetMainScreen(this.Login.ApplicationState);
            this.Login.ShowDialog();
            this.Login.Closing -= this.LoginOnClosing;
            return this.dialogResult;
        }

        private void LoginOnClosing(object sender, EventArgs eventArgs)
        {
            this.Login.ApplicationState.DisplayContext.MainScreen = ScreenIdentifier.GetFrom((Window)this.Login.Dialog);
        }

        private void SetInitialActivityMessage()
        {
            var viewModel = this.Dialog as ILoginViewModel;
            if (viewModel != null && viewModel.HadNoTenants)
            {
                this.SetActivityMessage(ActivityMessage.ActivityMessageType.Error, Strings.Login_NoTenants);
            }
        }

        private void OpenOfflineMode()
        {
            this.selectedUser = null;
            this.dialogResult = new SuccessDialogResult();
            this.Login.ApplicationState.IsOfflineMode = true;
            this.Dialog.DialogResult = true;
        }

        private bool CanDoLogin(object parameter)
        {
            return !this.Login.IsBusy
                   && this.Login.IsServerUriValid
                   && !string.IsNullOrWhiteSpace(this.Login.Username)
                   && !string.IsNullOrWhiteSpace(this.Login.Password);
        }

        private async void DoLogin()
        {
            this.CloseConnectionController();
            this.selectedUser = null;
            this.Login.IsBusy = true;
            var currentErrorMessage = Strings.Login_Error;
            try
            {
                currentErrorMessage = Strings.Login_UpdateError;
                if (await this.CheckForUpdatesAsync().ConfigureAwait(false))
                {
                    return;
                }

                var dispatcher = ServiceLocator.Current.GetInstance<IDispatcher>();
                dispatcher.Dispatch(
                    () =>
                    this.SetActivityMessage(ActivityMessage.ActivityMessageType.Info, Strings.Login_Authenticating));
                currentErrorMessage = Strings.Login_NoConnection;
                var user = await this.ConfigureCredentialsAsync().ConfigureAwait(false);
                currentErrorMessage = Strings.Login_NoServices;
                dispatcher.Dispatch(
                    () =>
                    this.SetActivityMessage(ActivityMessage.ActivityMessageType.Info, Strings.Login_FetchingTenants));

                var stopWatch = Stopwatch.StartNew();
                await
                    this.permissionController.LoadPermissionsAsync(
                        user,
                        this.applicationDataScope,
                        this.allowedDataScopes);
                stopWatch.Stop();
                Logger.Trace("Permissions loaded in {0} ms", stopWatch.ElapsedMilliseconds);
                this.selectedUser = user;

                dispatcher.Dispatch(
                    () =>
                    {
                        this.dialogResult = new SuccessDialogResult();
                        this.UpdateApplicationState();
                        this.Dialog.DialogResult = true;
                    });
                return;
            }
            catch (MessageSecurityException ex)
            {
                var inner = ex.InnerException as FaultException;
                if (inner == null)
                {
                    currentErrorMessage = Strings.Login_UserNotAuthenticated;
                    Logger.Warn(ex, "Couldn't login. Inner exception is null");
                }
                else if (string.Equals(inner.Code.Name, FaultCodes.MaintenanceMode))
                {
                    currentErrorMessage = string.Format(Strings.Login_SystemInMaintenance, inner.Message);
                    Logger.Warn(ex, "Couldn't login, system is in maintenance mode");
                }
                else if (string.Equals(inner.Code.Name, FaultCodes.SystemNotAvailable))
                {
                    currentErrorMessage = string.Format(Strings.Login_SystemNotAvailable);
                    Logger.Warn(ex, "Couldn't login, system is not available");
                }
                else
                {
                    currentErrorMessage = Strings.Login_UserNotAuthenticated;
                    Logger.Warn(ex, "Couldn't login");
                }
            }
            catch (SocketException ex)
            {
                Logger.Error(ex, "Couldn't resolve host");
                currentErrorMessage = Strings.Login_ServerNotFound;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error during login validation");
            }

            this.SetActivityMessage(ActivityMessage.ActivityMessageType.Error, currentErrorMessage);
            this.Login.IsBusy = false;
        }

        private async Task<bool> CheckForUpdatesAsync()
        {
            if (!ApplicationDeployment.IsNetworkDeployed)
            {
                Logger.Info("Application was not started as ClickOnce");
                return false;
            }

            var deployment = ApplicationDeployment.CurrentDeployment;
            if (deployment == null)
            {
                Logger.Warn("Couldn't find current application deployment");
                return false;
            }

            this.SetActivityMessage(ActivityMessage.ActivityMessageType.Info, Strings.Login_CheckingUpdates);
            var info = await Task.Run(() => deployment.CheckForDetailedUpdate());
            if (!info.UpdateAvailable)
            {
                Logger.Info("No ClickOnce update found");
                return false;
            }

            Logger.Info("Found ClickOnce update: {0} ({1}K)", info.AvailableVersion, info.UpdateSizeBytes / 1024);
            this.dialogResult = new ShouldUpdateDialogResult(info, this.Login.Password);

            var dispatcher = ServiceLocator.Current.GetInstance<IDispatcher>();
            dispatcher.Dispatch(() => this.Dialog.DialogResult = true);
            return true;
        }

        private async Task<User> ConfigureCredentialsAsync()
        {
            var userCredentials = new UserCredentials
                                      {
                                          HashedPassword = this.Login.Password,
                                          Username = this.Login.Username
                                      };
            return await this.connectionController.ConfigureAsync(this.Login.InputServer, userCredentials);
        }

        private void SetActivityMessage(ActivityMessage.ActivityMessageType type, string message)
        {
            this.Login.ActivityMessage.Message = message;
            this.Login.ActivityMessage.Type = type;
        }

        private void UpdateApplicationState()
        {
            var state = this.Login.ApplicationState;
            var serverUri = this.Login.InputServer;
            state.IsOfflineMode = false;

            if (state.RecentServers == null)
            {
                state.RecentServers = new ObservableCollection<string>();
            }
            else if (state.RecentServers.Contains(serverUri))
            {
                state.RecentServers.Remove(serverUri);
            }

            state.RecentServers.Insert(0, serverUri);
            state.LastServer = serverUri;

            if (this.selectedUser == null)
            {
                throw new Exception("No selected user after login.");
            }

            state.UsedDomainLogin = !string.IsNullOrEmpty(this.selectedUser.Domain);
        }

        private void CloseConnectionController()
        {
            var controller = this.connectionController as ConnectionController;
            if (controller != null)
            {
                controller.Close();
            }
        }
    }
}
