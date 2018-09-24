// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShellController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ShellController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.Controllers
{
    using System.IO;

    using Gorba.Motion.Infomedia.Core;
    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.ViewModels;
    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.WpfCore;

    /// <summary>
    /// The shell controller.
    /// </summary>
    public class ShellController : IController
    {
        private readonly ComposerVisualizerShell shell;

        private readonly PresentationController presentationController;

        private readonly LayoutController layoutController;

        private readonly string presentationFile;

        private bool isFrozen;

        private LoggingController loggingController;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellController"/> class.
        /// </summary>
        /// <param name="shell">
        ///     The shell.
        /// </param>
        /// <param name="commandRegistry">
        ///     The command registry.
        /// </param>
        /// <param name="application">
        /// the composer application</param>
        public ShellController(
            ComposerVisualizerShell shell,
            ICommandRegistry commandRegistry,
            ComposerApplication application)
        {
            this.shell = shell;
            this.presentationFile = application.File;
            commandRegistry.RegisterCommand("Freeze", new RelayCommand(this.FreezeVisualization));
            this.presentationController = new PresentationController(application.PresentationManager, commandRegistry);
            this.layoutController = new LayoutController(application.PresentationManager);
            this.loggingController = new LoggingController();
        }

        /// <summary>
        /// Starts the controller
        /// </summary>
        public void Run()
        {
            this.presentationController.Run();
            this.layoutController.Run();
            this.loggingController.Run();
            this.shell.PresentationTreeViewModel = this.presentationController.PresentationTree;
            this.shell.LayoutViewModel = this.layoutController.LayoutsTabViewModel;
            var file = new FileInfo(this.presentationFile).Name;
            this.shell.CreateWindow(this.layoutController.LayoutsTab, file);
        }

        /// <summary>
        /// Stops the controller
        /// </summary>
        public void Stop()
        {
            this.presentationController.Stop();
            this.layoutController.Stop();
            this.loggingController.Stop();
        }

        private void FreezeVisualization()
        {
            this.isFrozen = !this.isFrozen;
            this.presentationController.FreezeVisualization(this.isFrozen);
            this.layoutController.FreezeVisualization(this.isFrozen);
        }
    }
}