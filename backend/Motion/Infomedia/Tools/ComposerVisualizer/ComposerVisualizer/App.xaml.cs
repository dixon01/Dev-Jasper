// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for App.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer
{
    using System.Windows;

    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.Controllers;
    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.WpfCore;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private ComposerApplicationController applicationController;

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            var commandRegistry = new CommandRegistry();
            this.applicationController = new ComposerApplicationController(commandRegistry);
            this.applicationController.Run();
        }
    }
}
