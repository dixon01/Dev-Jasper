// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateProgressController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateProgressController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.Controllers
{
    using System.ComponentModel;
    using System.Deployment.Application;

    using Gorba.Center.Common.Wpf.Client.ViewModels;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.ViewModels;
    using Gorba.Center.Common.Wpf.Framework.ViewModels.Dialogs;

    using NLog;

    /// <summary>
    /// The update progress controller.
    /// </summary>
    internal class UpdateProgressController : DialogControllerBase, IUpdateProgressController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private ApplicationDeployment applicationDeployment;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateProgressController"/> class.
        /// </summary>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        /// <param name="window">
        /// The window view model.
        /// </param>
        public UpdateProgressController(
            ICommandRegistry commandRegistry,
            UpdateProgressViewModel window)
            : base(window)
        {
            commandRegistry.RegisterCommand(
                ClientCommandCompositionKeys.CancelUpdate,
                new RelayCommand(this.CancelUpdate));
        }

        /// <summary>
        /// Gets the update progress view model of this controller.
        /// </summary>
        public UpdateProgressViewModel UpdateProgress
        {
            get
            {
                return this.Dialog as UpdateProgressViewModel;
            }
        }

        /// <summary>
        /// Shows the dialog and returns only when the newly opened dialog is closed.
        /// </summary>
        /// <returns>The result of the dialog.</returns>
        public override DialogResultBase Run()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                this.applicationDeployment = ApplicationDeployment.CurrentDeployment;
            }

            if (this.applicationDeployment == null)
            {
                Logger.Warn("Couldn't find application deployment");
                return new EmptyDialogResult();
            }

            if (this.applicationDeployment.UpdateLocation != null)
            {
                this.UpdateProgress.UpdateSource = this.applicationDeployment.UpdateLocation.Host;
                this.UpdateProgress.UpdateSourceUri = this.applicationDeployment.UpdateLocation.ToString();
            }

            this.applicationDeployment.UpdateProgressChanged += this.ApplicationDeploymentOnUpdateProgressChanged;
            this.applicationDeployment.UpdateCompleted += this.ApplicationDeploymentOnUpdateCompleted;

            Logger.Info("Starting ClickOnce update");
            this.applicationDeployment.UpdateAsync();

            var dialog = this.UpdateProgress.ShowDialog();
            if (dialog.HasValue && dialog.Value)
            {
                return new SuccessDialogResult();
            }

            this.CancelUpdate();
            return new EmptyDialogResult();
        }

        private void CancelUpdate()
        {
            if (this.Dialog.Dialog == null)
            {
                return;
            }

            this.applicationDeployment.UpdateAsyncCancel();
            this.Dialog.DialogResult = false;
        }

        private void ApplicationDeploymentOnUpdateProgressChanged(object sender, DeploymentProgressChangedEventArgs e)
        {
            Logger.Trace(
                "ClickOnce update: {0}K of {1}K ({2}%)",
                e.BytesCompleted / 1024,
                e.BytesTotal / 1024,
                e.ProgressPercentage);

            this.UpdateProgress.ProgressValue = e.ProgressPercentage;
            if (this.UpdateProgress.ActivityMessage != null
                && this.UpdateProgress.ActivityMessage.Type > ActivityMessage.ActivityMessageType.Info)
            {
                return;
            }

            var message = new ActivityMessage
                              {
                                  Type = ActivityMessage.ActivityMessageType.Info,
                                  Message =
                                      string.Format(
                                          Strings.UpdateProgress_Message,
                                          e.BytesCompleted / 1024.0 / 1024.0,
                                          e.BytesTotal / 1024.0 / 1024.0,
                                          e.ProgressPercentage)
                              };
            this.UpdateProgress.ActivityMessage = message;
        }

        private void ApplicationDeploymentOnUpdateCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Error == null)
            {
                Logger.Info("Update completed successfully");
                this.Dialog.DialogResult = true;
                return;
            }

            Logger.Warn("Update completed with error; Cancelled=" + e.Cancelled, e.Error);

            var message = new ActivityMessage { Type = ActivityMessage.ActivityMessageType.Error };
            message.Message = e.Error != null ? e.Error.Message : "Update Cancelled";
            this.UpdateProgress.ActivityMessage = message;
        }
    }
}
