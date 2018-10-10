// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteAppController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RemoteAppController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Controllers.App
{
    using System;
    using System.IO;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Diag.Core.Controllers.Gioom;
    using Gorba.Center.Diag.Core.Controllers.Log;
    using Gorba.Center.Diag.Core.Controllers.Unit;
    using Gorba.Center.Diag.Core.ViewModels.App;
    using Gorba.Center.Diag.Core.ViewModels.MediTree;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Management.Remote;
    using Gorba.Common.SystemManagement.Client;

    /// <summary>
    /// Controller for an application running on a unit.
    /// </summary>
    public class RemoteAppController : SynchronizableControllerBase, IDisposable
    {
        private readonly IRootMessageDispatcher messageDispatcher;

        private string executableName;

        private IApplicationStateObserver stateObserver;

        private IRemoteManagementProvider managementProvider;

        private RemoteSystemManagerClient systemManagerClient;

        private ApplicationInfo applicationInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteAppController"/> class.
        /// </summary>
        /// <param name="viewModel">
        /// The view model.
        /// </param>
        /// <param name="unitController">
        /// The unit controller to which this application belongs.
        /// </param>
        /// <param name="messageDispatcher">
        /// The message dispatcher used for sending messages to the unit.
        /// </param>
        public RemoteAppController(
            RemoteAppViewModel viewModel, IUnitController unitController, IRootMessageDispatcher messageDispatcher)
        {
            this.messageDispatcher = messageDispatcher;
            this.ViewModel = viewModel;
            this.UnitController = unitController;
            this.GioomPortsController = new GioomPortsController(this.ViewModel.GioomPorts);
            this.LoggingController = new ApplicationLoggingController(this.ViewModel, messageDispatcher);
        }

        /// <summary>
        /// Gets the view model associated to this controller.
        /// </summary>
        public RemoteAppViewModel ViewModel { get; private set; }

        /// <summary>
        /// Gets the unit controller owning this application controller.
        /// </summary>
        public IUnitController UnitController { get; private set; }

        /// <summary>
        /// Gets the GIOoM ports controller.
        /// </summary>
        public GioomPortsController GioomPortsController { get; private set; }

        /// <summary>
        /// Gets the application logging controller.
        /// </summary>
        public ApplicationLoggingController LoggingController { get; private set; }

        /// <summary>
        /// Verifies if the given <paramref name="appName"/> matches this application.
        /// This method checks the executable name as well as the Medi application address.
        /// </summary>
        /// <param name="appName">
        /// The application name to verify.
        /// </param>
        /// <returns>
        /// If this controller has the given application name.
        /// </returns>
        public bool HasApplicationName(string appName)
        {
            if (this.ViewModel.Address == null)
            {
                return appName.Equals(this.executableName, StringComparison.InvariantCultureIgnoreCase);
            }

            return appName.Equals(this.ViewModel.Address.Application, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Requests the remote application to re-launch.
        /// </summary>
        public void Relaunch()
        {
            if (this.systemManagerClient == null || this.applicationInfo == null)
            {
                return;
            }

            this.systemManagerClient.RelaunchApplication(this.applicationInfo, "Requested by icenter.diag user");
        }

        /// <summary>
        /// Requests the remote application to exit.
        /// </summary>
        public void Exit()
        {
            if (this.systemManagerClient == null || this.applicationInfo == null)
            {
                return;
            }

            this.systemManagerClient.ExitApplication(this.applicationInfo, "Requested by icenter.diag user");
        }

        /// <summary>
        /// Updates this controller and its view model from the given Medi address.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        public void UpdateFrom(MediAddress address)
        {
            this.ViewModel.Address = address;

            if (this.ViewModel.Name == null)
            {
                this.ViewModel.Name = address.Application;
            }

            if (this.ViewModel.ApplicationType == ApplicationType.Unknown)
            {
                this.ViewModel.ApplicationType = GetApplicationType(address.Application);
            }

            if (this.managementProvider != null)
            {
                return;
            }

            this.managementProvider = this.messageDispatcher.ManagementProviderFactory.CreateRemoteProvider(
                this.ViewModel.Address);
            this.managementProvider.BeginGetChildren(this.GotMediTreeRoots, null);
        }

        /// <summary>
        /// Updates this controller and its view model from the given application info.
        /// </summary>
        /// <param name="application">
        /// The application info.
        /// </param>
        /// <param name="client">
        /// The system management client used to communicate with
        /// System Management on the unit to which this app belongs.
        /// </param>
        public void UpdateFrom(ApplicationInfo application, RemoteSystemManagerClient client)
        {
            this.systemManagerClient = client;
            this.applicationInfo = application;
            this.ViewModel.Name = application.Name;
            this.executableName = Path.GetFileNameWithoutExtension(application.Path);

            var state = this.ViewModel.State;
            if (state == null)
            {
                state = new RemoteAppStateViewModel();
                this.ViewModel.State = state;
            }

            state.Version = application.Version;
            state.Path = application.Path;
            state.State = application.State;

            state.CpuUsage = string.Format("{0:0}%", application.CpuUsage * 100.0);
            state.RamUsage = string.Format("{0:0.#} MB", application.RamBytes / 1024.0 / 1024.0);
            state.RamUsageBytes = application.RamBytes;

            if (this.stateObserver != null)
            {
                return;
            }

            if (this.ViewModel.ApplicationType == ApplicationType.Unknown)
            {
                this.ViewModel.ApplicationType = GetApplicationType(this.executableName);
            }

            this.stateObserver = client.CreateApplicationStateObserver(application);
            this.stateObserver.StateChanged += this.StateObserverOnStateChanged;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (this.stateObserver != null)
            {
                this.stateObserver.StateChanged -= this.StateObserverOnStateChanged;
                this.stateObserver.Dispose();
            }

            if (this.managementProvider != null)
            {
                this.managementProvider.Dispose();
                this.managementProvider = null;
            }

            this.GioomPortsController.Dispose();
            this.LoggingController.Dispose();
        }

        private static ApplicationType GetApplicationType(string applicationName)
        {
            if (string.IsNullOrEmpty(applicationName))
            {
                return ApplicationType.Unknown;
            }

            if (applicationName.IndexOf("System", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return ApplicationType.SystemManager;
            }

            if (applicationName.IndexOf("Hardware", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return ApplicationType.HardwareManager;
            }

            if (applicationName.IndexOf("Update", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return ApplicationType.Update;
            }

            if (applicationName.IndexOf("Protran", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return ApplicationType.Protran;
            }

            if (applicationName.IndexOf("Composer", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return ApplicationType.Composer;
            }

            if (applicationName.IndexOf("AHDLC", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return ApplicationType.AhdlcRenderer;
            }

            if (applicationName.IndexOf("Audio", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return ApplicationType.AudioRenderer;
            }

            if (applicationName.IndexOf("Renderer", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                // all other are probably DX renderers
                return ApplicationType.DirectXRenderer;
            }

            return ApplicationType.Unknown;
        }

        private void UpdateState()
        {
            this.ViewModel.State.State = this.stateObserver.State;
        }

        private void GotMediTreeRoots(IAsyncResult ar)
        {
            var provider = this.managementProvider;
            if (provider == null)
            {
                return;
            }

            var children = provider.EndGetChildren(ar);
            var roots = children.OfType<IRemoteManagementProvider>().ToList();

            this.StartNew(
                () =>
                    {
                        this.ViewModel.MediTreeRoots.Clear();
                        roots.ForEach(n => this.ViewModel.MediTreeRoots.Add(new MediTreeNodeViewModel(n)));
                    });
        }

        private void StateObserverOnStateChanged(object sender, EventArgs eventArgs)
        {
            this.StartNew(this.UpdateState);
        }
    }
}
