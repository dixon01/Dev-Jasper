// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for App.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.NotificationObserver
{
    using System.Configuration;
    using System.Threading.Tasks;
    using System.Windows;

    using Gorba.Center.BackgroundSystem.Spikes.NotificationObserver.Controller;
    using Gorba.Center.BackgroundSystem.Spikes.NotificationObserver.ViewModel;
    using Gorba.Center.Common.ServiceBus;
    using Gorba.Center.Common.ServiceModel;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private ApplicationController applicationController;

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            var configuration = ConfigurationManager.AppSettings["CenterPortal"];
            var shell = new Shell(this.Dispatcher)
                            {
                                CenterPortalAddress = configuration
                            };
            DependencyResolver.Current.Register(shell);

            ServiceBusNotificationManagerUtility.ConfigureServiceBusNotificationManager();
            this.applicationController = new ApplicationController();
            DependencyResolver.Current.Register(this.applicationController);
            Task.Run(() => this.applicationController.RunAsync());
            var window = new MainWindow { DataContext = shell };
            window.Show();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Exit"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.Windows.ExitEventArgs"/> that contains the event data.</param>
        protected override void OnExit(ExitEventArgs e)
        {
            if (this.applicationController == null)
            {
                return;
            }

            this.applicationController.Dispose();
        }
    }
}