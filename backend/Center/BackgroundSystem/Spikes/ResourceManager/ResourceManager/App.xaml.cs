// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for App.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.ResourceManager
{
    using System.Configuration;
    using System.Windows;
    using System.Windows.Threading;

    using Gorba.Center.BackgroundSystem.Spikes.ResourceManager.Controllers;
    using Gorba.Center.BackgroundSystem.Spikes.ResourceManager.ViewModels;
    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.ServiceModel;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            var backgroundSystemConfiguration =
                BackgroundSystemConfigurationProvider.Current.GetConfiguration(
                    ConfigurationManager.AppSettings["Server"]);
            ChannelScopeFactoryUtility<IResourceService>.ConfigureAsFunctionalService(
               backgroundSystemConfiguration.FunctionalServices, "Resources");
            Dispatcher dispatcher = this.Dispatcher;
            DependencyResolver.Current.Register(dispatcher);

            var applicationController = new ApplicationController();
            DependencyResolver.Current.Register(applicationController);

            var shell = new Shell();
            DependencyResolver.Current.Register(shell);

            var window = new MainWindow
                              {
                                  DataContext = shell
                              };
            window.Show();
        }
    }
}