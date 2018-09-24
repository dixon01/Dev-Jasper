// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiagApplicationController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The DiagApplicationController.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Controllers
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.Wpf.Client;
    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Interaction.FileDialog;
    using Gorba.Center.Common.Wpf.Framework.Services;
    using Gorba.Center.Common.Wpf.Framework.ViewModels;

    using NLog;

    /// <summary>
    /// The application controller for icenter.diag.
    /// </summary>
    [Export(typeof(IDiagApplicationController))]
    [Export]
    public class DiagApplicationController : ClientApplicationControllerBase, IDiagApplicationController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private bool stateSaved;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagApplicationController"/> class.
        /// </summary>
        /// <param name="commandRegistry">the command registry</param>
        [ImportingConstructor]
        public DiagApplicationController(ICommandRegistry commandRegistry)
            : base(commandRegistry, "icenter.diag", DataScope.CenterDiag)
        {
            this.CommandRegistry.RegisterCommand(
                ClientCommandCompositionKeys.Application.Exit,
                new RelayCommand(this.Shutdown));
        }

        /// <summary>
        /// Gets or sets the shell controller.
        /// </summary>
        /// <value>
        /// The shell controller.
        /// </value>
        [Import]
        public IDiagShellController ShellController { get; set; }

        /// <summary>
        /// Gets the application icon shown on the login and tenant selection dialogs.
        /// </summary>
        protected override ImageSource ApplicationIcon
        {
            get
            {
                return
                    new BitmapImage(
                        new Uri(@"pack://application:,,,/Gorba.Center.Diag.Core;component/Resources/diag.ico"));
            }
        }

        /// <summary>
        /// Gets the data scopes that are allowed in this application.
        /// This list should be fixed and never change over the runtime of an application.
        /// It is used to determine which data scopes have an influence on the selectable tenants.
        /// </summary>
        protected override DataScope[] AllowedDataScopes
        {
            get
            {
                return new[] { DataScope.Unit };
            }
        }

        /// <summary>
        /// Gets a value indicating whether this application supports offline mode.
        /// </summary>
        protected override bool SupportsOfflineMode
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Runs the controller logic until completed or until the <see cref="ApplicationController.Shutdown"/>.
        /// </summary>
        public override void Run()
        {
            Logger.Info("Running the icenter.diag application");
            this.InitializeApplicationState();
            this.DeleteTempFiles();
            InteractionManager<OpenFileDialogInteraction>.SetCurrent(new OpenFileDialogInteractionManager());
            InteractionManager<SaveFileDialogInteraction>.SetCurrent(new SaveFileDialogInteractionManager());

            this.RunLoginAsync().ContinueWith(
                t =>
                    {
                        if (t.IsFaulted)
                        {
                            Logger.ErrorException(
                                "Couldn't run login",
                                t.Exception == null ? null : t.Exception.Flatten());
                            this.Shutdown();
                            return;
                        }

                        if (t.Result == null)
                        {
                            return;
                        }

                        t.Result.SetMainScreen(this.ConnectedApplicationState);
                        this.ShellController.WindowClosing += this.ShellControllerOnWindowClosing;
                        this.ShellController.WindowClosed += this.ShellControllerOnWindowClosed;
                        this.ShellController.Window.SetMainScreen(this.ConnectedApplicationState);
                        this.ShellController.Show();
                        t.Result.Close();
                    },
                TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// Requests the shutdown of this controller.
        /// </summary>
        public override void Shutdown()
        {
            this.cancellationTokenSource.Cancel();

            if (this.ShellController != null)
            {
                this.ShellController.WindowClosed -= this.ShellControllerOnWindowClosed;
                this.ShellController.Dispose();
            }

            // Application.Current null check required for unit tests
            if (Application.Current != null && Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.Close();
            }

            this.SaveState();
            base.Shutdown();

            if (Application.Current != null)
            {
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// The logout.
        /// </summary>
        public override void Logout()
        {
            base.Logout();

            this.ShellController.WindowClosing -= this.ShellControllerOnWindowClosing;
            this.ShellController.WindowClosed -= this.ShellControllerOnWindowClosed;
            this.ShellController.Close();

            this.Run();
        }

        private void ShellControllerOnWindowClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            if (cancelEventArgs.Cancel)
            {
                return;
            }

            if (Application.Current == null)
            {
                return;
            }

            this.SaveState();
            Application.Current.Shutdown();
        }

        private void SaveState()
        {
            if (this.stateSaved)
            {
                return;
            }

            ApplicationStateManager.Current.Save(
                "Diag",
                "DiagApplication",
                this.ShellController.Shell.DiagApplicationState);
            this.stateSaved = true;
        }

        private void ShellControllerOnWindowClosed(object sender, EventArgs eventArgs)
        {
            this.Shutdown();
        }

        private void InitializeApplicationState()
        {
            var state = this.ShellController.Shell.DiagApplicationState;
            state.Initialize(this.ShellController.Shell);
            this.InitializeApplicationStateOptions(state);
            this.stateSaved = false;
        }

        private void DeleteTempFiles()
        {
            var resourcesDir = Path.Combine(
               Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
               "Gorba\\Center\\Diag\\Resources");
            Logger.Info("Deleting temporary files in '{0}'", resourcesDir);
            var directoryInfo = new DirectoryInfo(resourcesDir);
            if (directoryInfo.Exists)
            {
                directoryInfo.Delete(true);
            }
        }
    }
}
