// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComposerApplicationController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ComposerApplicationController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.Controllers
{
    using System;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;

    using Gorba.Common.SystemManagement.Host;
    using Gorba.Motion.Infomedia.Core;
    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.ViewModels;
    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.WpfCore;

    /// <summary>
    /// The composer application controller.
    /// </summary>
    public class ComposerApplicationController : IController
    {
        private readonly ICommandRegistry commandRegistry;

        private ApplicationHost<ComposerApplication> composerApplication;

        private bool running;

        private ShellController shellController;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComposerApplicationController"/> class.
        /// </summary>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public ComposerApplicationController(ICommandRegistry commandRegistry)
        {
            this.commandRegistry = commandRegistry;
            this.commandRegistry.RegisterCommand("Exit", new RelayCommand(this.Stop));
        }

        /// <summary>
        /// Starts the controller
        /// </summary>
        public void Run()
        {
            if (this.running)
            {
                return;
            }

            this.running = true;
            var runThread = new Thread(this.RunComposer) { IsBackground = false };
            runThread.Start();
        }

        /// <summary>
        /// Stops the controller
        /// </summary>
        public void Stop()
        {
            if (!this.running)
            {
                return;
            }

            this.running = false;

            this.composerApplication.Application.Exit("Visualizer application stopped");
            Application.Current.MainWindow.Close();
        }

        private void RunComposer()
        {
            this.composerApplication = new ApplicationHost<ComposerApplication>();
            this.composerApplication.Application.PresentationStarted += this.ApplicationOnPresentationStarted;
            this.composerApplication.Run("Composer");
        }

        private void ApplicationOnPresentationStarted(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(this.StartShellController));
        }

        private void StartShellController()
        {
            var shell = new ComposerVisualizerShell(this.commandRegistry);
            this.shellController = new ShellController(
                shell,
                this.commandRegistry,
                this.composerApplication.Application);
            this.shellController.Run();
        }
    }
}