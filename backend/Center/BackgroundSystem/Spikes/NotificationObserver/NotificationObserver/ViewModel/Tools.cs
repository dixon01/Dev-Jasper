// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Tools.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Tools type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.NotificationObserver.ViewModel
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;

    using Gorba.Center.BackgroundSystem.Spikes.NotificationObserver.Controller;
    using Gorba.Center.Common.ServiceBus;
    using Gorba.Center.Common.ServiceModel;

    using Microsoft.ServiceBus;

    /// <summary>
    /// The tools view model.
    /// </summary>
    public class Tools : ViewModelBase
    {
        private DateTime lastServerClearLocalTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tools"/> class.
        /// </summary>
        /// <param name="shell">
        /// The shell.
        /// </param>
        public Tools(Shell shell)
        {
            this.Shell = shell;
            this.ClearServerCommand = new AsyncCommand(this.ClearServer, this.CanClearServer);
        }

        /// <summary>
        /// Gets the local time for the last clear request.
        /// </summary>
        public DateTime LastServerClearLocalTime
        {
            get
            {
                return this.lastServerClearLocalTime;
            }

            private set
            {
                if (this.lastServerClearLocalTime == value)
                {
                    return;
                }

                this.lastServerClearLocalTime = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets the shell.
        /// </summary>
        public Shell Shell { get; private set; }

        /// <summary>
        /// Gets the clear server command.
        /// </summary>
        public ICommand ClearServerCommand { get; private set; }

        private void ClearServer()
        {
            const string Message = "Applications connected to this server will stop working until restarted. Are you"
                                   + " over 18 and do you still want to continue?";
            var result = MessageBox.Show(Message, "Are you sure?", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            this.LastServerClearLocalTime = DateTime.Now;
            this.Shell.Clear();
            var task = Task.Run(() => this.ClearServerAsync());
            task.ContinueWith(this.OnTaskCompleted);
        }

        private async Task ClearServerAsync()
        {
            this.Shell.Status = "Clearing server";
            var configuration = BackgroundSystemConfigurationProvider.Current.GetConfiguration();
            var connectionStringBuilder = new ServiceBusConnectionStringBuilder(
                configuration.ServiceBusConnectionString);
            var connectionString = connectionStringBuilder.SetServiceBusCredentials();
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            var topics = await namespaceManager.GetTopicsAsync();
            foreach (var topicDescription in topics)
            {
                var subscriptions = (await namespaceManager.GetSubscriptionsAsync(topicDescription.Path)).ToList();
                var subscriptionsToDelete =
                    subscriptions.Where(description => !description.Name.Contains("BackgroundSystem")).ToList();
                foreach (var subscriptionDescription in subscriptionsToDelete)
                {
                    await namespaceManager.DeleteSubscriptionAsync(topicDescription.Path, subscriptionDescription.Name);
                }
            }

            this.Shell.Status = "Server cleared. Recreating subscriptions for observer";
            var applicationController = DependencyResolver.Current.Get<ApplicationController>();
            await applicationController.RunAsync();
        }

        private void OnTaskCompleted(Task task)
        {
            if (task.Exception == null)
            {
                this.Shell.Status = "Server cleared. Recreating subscriptions";
                return;
            }

            var exception = task.Exception.Flatten();
            this.Shell.Status = "Error while clearing server: " + exception.Message;
        }

        private bool CanClearServer()
        {
            return true;
        }
    }
}